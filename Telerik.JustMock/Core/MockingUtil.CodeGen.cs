/*
 JustMock Lite
 Copyright © 2010-2015 Progress Software Corporation

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using Telerik.JustMock.Core.Castle.DynamicProxy;

namespace Telerik.JustMock.Core
{
	internal static partial class MockingUtil
	{
		public static readonly Type[] EmptyTypes = Type.EmptyTypes;

		/// <summary>
		/// Create a delegate to a function that takes a object[] as a parameter and a delegate,
		/// unpacks the array and calls the delegate by substituting the array members for its parameters.
		/// It mimics what MethodInfo.Invoke does, but in JustMock it's a bad idea to call reflection
		/// methods within a GuardExternal block! GuardExternal blocks should do *minimum* amount of work.
		/// </summary>
		public static Func<object[], Delegate, object> MakeFuncCaller(Delegate delg)
		{
			return (Func<object[], Delegate, object>)MakeDelegateInvoker(delg, false);
		}

		private static Delegate MakeDelegateInvoker(Delegate delg, bool discardReturnValue)
		{
			var invokeMethod = delg.GetType().GetMethod("Invoke");
			var retType = discardReturnValue ? typeof(void) : invokeMethod.ReturnType;

			var proc = CreateDynamicMethod(typeof(Func<object[], Delegate, object>),
				il =>
				{
					var locals = il.UnpackArgArray(OpCodes.Ldarg_0, invokeMethod);

					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Castclass, delg.GetType());
					il.PushArgArray(invokeMethod);

					il.Emit(OpCodes.Callvirt, invokeMethod);

					if (retType != typeof(void) && retType.IsValueType)
						il.Emit(OpCodes.Box, retType);
					if (retType == typeof(void))
						il.Emit(OpCodes.Ldnull);

					il.PackArgArray(OpCodes.Ldarg_0, invokeMethod, locals);

					il.Emit(OpCodes.Ret);
				});

			return proc;
		}

		private static LocalBuilder[] UnpackArgArray(this ILGenerator il, OpCode ldArray, MethodBase method)
		{
			var parameters = method.GetParameters();
			var argCount = parameters.Length;
			var locals = parameters.Select(p => il.DeclareLocal(p.ParameterType.IsByRef ? p.ParameterType.GetElementType() : p.ParameterType)).ToArray();
			for (int i = 0; i < argCount; ++i)
			{
				il.Emit(ldArray);
				il.Emit(OpCodes.Ldc_I4, i);
				il.Emit(OpCodes.Ldelem_Ref);
				il.Emit(locals[i].LocalType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, locals[i].LocalType);
				il.Emit(OpCodes.Stloc, i);
			}
			return locals;
		}

		private static void PushArgArray(this ILGenerator il, MethodBase method)
		{
			var parameters = method.GetParameters();
			var argCount = parameters.Length;

			for (int i = 0; i < argCount; ++i)
			{
				il.Emit(parameters[i].ParameterType.IsByRef ? OpCodes.Ldloca : OpCodes.Ldloc, i);
			}
		}

		private static void PackArgArray(this ILGenerator il, OpCode ldArray, MethodBase method, LocalBuilder[] locals)
		{
			var parameters = method.GetParameters();
			var argCount = parameters.Length;

			for (int i = 0; i < argCount; ++i)
			{
				if (!parameters[i].ParameterType.IsByRef)
					continue;

				il.Emit(ldArray);
				il.Emit(OpCodes.Ldc_I4, i);
				il.Emit(OpCodes.Ldloc, i);
				if (locals[i].LocalType.IsValueType)
					il.Emit(OpCodes.Box, locals[i].LocalType);
				il.Emit(OpCodes.Stelem_Ref);
			}
		}

		public static TDelegateType CreateDynamicMethod<TDelegateType>(Action<ILGenerator> ilGen)
		{
			return (TDelegateType)(object)CreateDynamicMethod(typeof(TDelegateType), ilGen);
		}

		public static Delegate CreateDynamicMethod(Type delegateType, Action<ILGenerator> ilGen)
		{
			var invokeMethod = delegateType.GetMethod("Invoke");
			var returnType = invokeMethod.ReturnType;
			var parameterTypes = invokeMethod.GetParameters().Select(p => p.ParameterType).ToArray();

#if SILVERLIGHT
			var method = CreateDynamicMethodWithVisibilityChecks(returnType, parameterTypes, ilGen);
			return Delegate.CreateDelegate(delegateType, method);
#else
			// If you try to call the DynamicMethod constructor specifying a ByRef type
			// as the return value it throws NotSupportedException with the following message:
			// "The return Type contains some invalid type(i.e. null, ByRef)". In order to make
			// it working we are applying the following simple workaround:
			//   1. Pass a non-ref type to constructor and create dynamic method instance
			//   2. Using reflection change the value of private field 'm_returnType' of the
			//      newly created dynamic method to be ByRef type
			Type dynamicReturnType = (returnType.IsByRef) ? returnType.GetElementType() : returnType;

			var method = new DynamicMethod("DynamicMethod_" + Guid.NewGuid().ToString("N"), dynamicReturnType, parameterTypes, true);

			if (returnType.IsByRef)
			{
				method.GetType().GetField("m_returnType", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.SetValue(method, returnType);
			}

			ilGen(method.GetILGenerator());
			return method.CreateDelegate(delegateType);
#endif
		}

		private static ModuleBuilder moduleBuilder;

		public static ModuleBuilder ModuleBuilder
		{
			get
			{
				if (moduleBuilder == null)
					moduleBuilder = new ModuleScope().ObtainDynamicModuleWithStrongName();
				return moduleBuilder;
			}
		}

		public static MethodInfo CreateDynamicMethodWithVisibilityChecks(Type returnType, Type[] parameterTypes, Action<ILGenerator> ilGen)
		{
			var type = ModuleBuilder.DefineType("DynamicType_" + Guid.NewGuid().ToString("N"), TypeAttributes.Public);
			var methodBuilder = type.DefineMethod("Proc", MethodAttributes.Public | MethodAttributes.Static,
				returnType, parameterTypes);

			ilGen(methodBuilder.GetILGenerator());

			var constructedType = type.CreateType();
			return constructedType.GetMethod("Proc");
		}

		public static MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref Object[] args,
			ParameterModifier[] modifiers, CultureInfo culture, string[] names, out Object state)
		{
			return Type.DefaultBinder.BindToMethod(bindingAttr, match, ref args, modifiers, culture, names, out state);
		}

		private static int? GetDispId(MemberInfo member)
		{
			var attr = (DispIdAttribute)Attribute.GetCustomAttribute(member, typeof(DispIdAttribute));
			return attr != null ? (int?)attr.Value : null;
		}

		public static ProfilerInterceptor.RefReturn<TReturn> CreateDynamicMethodInvoker<TReturn>(object target, MethodInfo method, object[] args)
		{
			if (args.Length != method.GetParameters().Length)
			{
				throw new MockException(
					String.Format("Number of the supplied arguments does not match to the expected one in the method signature:" +
						" supplied '{0}', expected '{1}'", args.Length, method.GetParameters().Length));
			}

			ProfilerInterceptor.RefReturn<TReturn> @delegate =
				MockingUtil.CreateDynamicMethod<ProfilerInterceptor.RefReturn<TReturn>>(
					il =>
					{
						// store arguments as local variables
						il.UnpackArgArray(OpCodes.Ldarg_1, method);

						// push object reference to the stack in case if instance method
						if (target != null)
						{
							il.Emit(OpCodes.Ldarg_0);
						}

						// push stored arguments back to the stack
						il.PushArgArray(method);

						il.Emit((target != null) ? OpCodes.Callvirt : OpCodes.Call, method as MethodInfo);
						il.Emit(OpCodes.Ret);
					});

			return @delegate;
		}
	}
}
