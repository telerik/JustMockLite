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
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Telerik.JustMock.Core.MatcherTree;
using Telerik.JustMock.Core.Recording;

namespace Telerik.JustMock.Core
{
	internal static partial class MockingUtil
	{
		public static readonly object[] NoObjects = { };

		public const BindingFlags AllMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
		public const BindingFlags Default = (BindingFlags)0;

		public static bool IsExtensionMethod(this MethodBase method)
		{
			return method.IsStatic && method.GetCustomAttributes(typeof(ExtensionAttribute), false).Any();
		}

		public static EventInfo GetEventFromAddOrRemove(this MethodBase addOrRemoveMethod)
		{
			return addOrRemoveMethod.DeclaringType
									.GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
									.FirstOrDefault(evt => evt.GetAddMethod(true) == addOrRemoveMethod || evt.GetRemoveMethod(true) == addOrRemoveMethod);
		}

		public static PropertyInfo GetPropertyFromGetOrSet(this MethodBase getOrSetMethod)
		{
			return getOrSetMethod.DeclaringType
								 .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
								 .FirstOrDefault(prop => prop.GetGetMethod(true) == getOrSetMethod || prop.GetSetMethod(true) == getOrSetMethod);
		}

		public static EventInfo ParseAddHandlerAction(this MocksRepository repo, Action addHandlerAction, out object instance)
		{
			EventInfo theEvent = null;
			object instanceVar = null;

			var recorder = new DelegatingRecorder();
			recorder.Record += invocation =>
			{
				var candidateEvent = invocation.Method.GetEventFromAddOrRemove();

				if (candidateEvent != null)
				{
					theEvent = candidateEvent;
					instanceVar = MocksRepository.GetMockMixin(invocation.Instance, theEvent.DeclaringType)
						?? invocation.Instance;
				}
			};

			using (repo.StartRecording(recorder, true))
			{
				addHandlerAction();
			}

			instance = instanceVar;
			return theEvent;
		}

		public static EventInfo ParseAddHandlerAction<TMock>(this MocksRepository repo, TMock mock, Action<TMock> addHandlerAction)
		{
			object instance;
			repo.EnableInterception(typeof(TMock));
			return repo.ParseAddHandlerAction(() => addHandlerAction(mock), out instance);
		}

		public static object GetDefaultValue(this Type t)
		{
			Debug.Assert(t != typeof(void));
			return
				t.IsPointer ? IntPtr.Zero
				: t.IsValueType && t != typeof(void) ? MockingUtil.CreateInstance(t)
				: null;
		}

		public static Type GetImplementationOfGenericInterface(this Type type, Type genericIntf)
		{
			var interfaces = type.GetInterfaces().ToList();
			if (type.IsInterface)
				interfaces.Add(type);
			return interfaces.FirstOrDefault(intf => intf.IsGenericType && intf.GetGenericTypeDefinition() == genericIntf);
		}

		public static bool ArgumentsMatchSignature(this MethodInfo method, object[] args)
		{
			var parameters = method.GetParameters();
			if (parameters.Length != args.Length)
				return false;

			var generics = new Dictionary<Type, Type>();
			var genericArgs = method.GetGenericArguments();
			var argTypes = GetTypesFromArguments(args);

			if (method.ContainsGenericParameters)
			{
				for (int i = 0; i < parameters.Length; ++i)
				{
					if (!GetGenericsTypesFromActualType(parameters[i].ParameterType, argTypes[i], generics))
						return false;
				}

				if (GetRealMethodInfoFromGeneric(method, args) == null)
					return false;
			}
			else
			{
				for (int i = 0; i < parameters.Length; ++i)
				{
					if (!parameters[i].ParameterType.IsAssignableFrom(argTypes[i]))
						return false;
				}
			}

			return true;
		}

		public static Type[] GetTypesFromArguments(object[] args)
		{
			var result = new Type[args.Length];

			for (int i = 0; i < args.Length; ++i)
			{
				var arg = args[i];
				if (arg == null)
					throw new ArgumentException("Cannot use 'null' as an argument to find a non-public member. Use ArgExpr.IsNull<T>() instead.");

				var methodCall = arg as MethodCallExpression;
				if (methodCall != null)
				{
					var matcherMethod = methodCall.Method;
					if (Attribute.IsDefined(matcherMethod, typeof(OutArgAttribute)))
					{
						var argType = matcherMethod.ReturnType;
						result[i] = argType.MakeByRefType();
						continue;
					}

					if (Attribute.IsDefined(matcherMethod, typeof(ArgMatcherAttribute)))
					{
						var argType = matcherMethod.ReturnType;
						result[i] = argType;
						continue;
					}
				}
				var fieldExpr = arg as MemberExpression;
				if (fieldExpr != null && Attribute.IsDefined(fieldExpr.Member, typeof(RefArgAttribute)))
				{
					var innerExprValue = (fieldExpr.Expression as MethodCallExpression).Arguments[0].EvaluateExpression();
					result[i] = GetTypesFromArguments(new[] { innerExprValue })[0].MakeByRefType();
					continue;
				}

				result[i] = arg.GetType();
			}

			return result;
		}

		public static bool GetGenericsTypesFromActualType(Type type, Type actualType, Dictionary<Type, Type> generics)
		{
			if (type == actualType)
				return true;

			if (type.IsGenericParameter)
			{
				generics[type] = actualType;
				return true;
			}

			if (type.ContainsGenericParameters &&
				actualType.IsGenericType &&
				type.GetGenericTypeDefinition() == actualType.GetGenericTypeDefinition())
			{
				var typeGenerics = type.GetGenericArguments();
				var actualTypeGenerics = actualType.GetGenericArguments();
				if (typeGenerics.Length != actualTypeGenerics.Length)
					return false;

				for (int i = 0; i < typeGenerics.Length; i++)
				{
					if (!GetGenericsTypesFromActualType(typeGenerics[i], actualTypeGenerics[i], generics))
						return false;
				}

				return true;
			}

			return false;
		}

		public static Type GetReturnType(this MethodBase method)
		{
			var asInfo = method as MethodInfo;
			return asInfo != null ? asInfo.ReturnType : typeof(void);
		}

		public static object CreateObject(this Type type, object[] args)
		{
			args = args ?? NoObjects;

			var constructorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (constructorInfos.Length == 0 || (type.IsValueType && args.Length == 0))
			{
				if (args.Length > 0)
					throw new MockException("Type has no non-default constructors.");

				return type.GetDefaultValue();
			}

			object state;
			var ctor = (ConstructorInfo)MockingUtil.BindToMethod(MockingUtil.Default,
				constructorInfos, ref args, null, null, null, out state);

			var ctorParameters = ctor.GetParameters();
			for (int i = 0; i < ctorParameters.Length; ++i)
			{
				var paramType = ctorParameters[i].ParameterType;
                if (paramType.IsValueType && args[i] == null)
                    args[i] = paramType.GetDefaultValue();
                else if (args[i] != null && !paramType.IsAssignableFrom(args[i].GetType()))
                {
                        args[i] = Convert.ChangeType(args[i], paramType, System.Globalization.CultureInfo.CurrentCulture);
                }
			}

#if !PORTABLE
			var newCall = MockingUtil.CreateDynamicMethod<Func<object[], object>>(il =>
				{
					il.UnpackArgArray(OpCodes.Ldarg_0, ctor);
					il.PushArgArray(ctor);
					il.Emit(OpCodes.Newobj, ctor);
					if (type.IsValueType)
						il.Emit(OpCodes.Box, type);
					il.Emit(OpCodes.Ret);
				});

			return ProfilerInterceptor.GuardExternal(() =>
				{
					try
					{
						return newCall(args);
					}
					catch (MemberAccessException ex)
					{
						GC.KeepAlive(ex);
						return MockingUtil.CreateInstance(type, args);
					}
				});
#else
			return MockingUtil.CreateInstance(type, args);
#endif
		}

		public static object GetUninitializedObject(Type type)
		{
#if COREFX
			return ProfilerInterceptor.GetUninitializedObjectImpl(type);
#else
			if (typeof(ContextBoundObject).IsAssignableFrom(type))
				throw new MockException("Cannot mock constructors of ContextBoundObject descendants.");
			if (type == typeof(string))
				return string.Empty;
			if (type.IsPointer)
				return IntPtr.Zero;
			return FormatterServices.GetUninitializedObject(type);
#endif
		}

		public static object TryGetUninitializedObject(Type type)
		{
#if COREFX
			return ProfilerInterceptor.GetUninitializedObjectImpl(type);
#else
			if (typeof(ContextBoundObject).IsAssignableFrom(type)
				|| type.IsAbstract || type.IsInterface)
				return null;
			if (type == typeof(string))
				return string.Empty;
			return FormatterServices.GetUninitializedObject(type);
#endif
		}

		internal static Type GetTypeFrom(string fullName)
		{
#if !PORTABLE
			var type = AppDomain.CurrentDomain
								.GetAssemblies()
								.SelectMany(asm => asm.GetLoadableTypes())
								.FirstOrDefault(t => t.FullName == fullName);
			if (type == null)
				throw new ArgumentException(String.Format("Type '{0}' not found", fullName), "fullName");
			return type;
#else
			return Type.GetType(fullName, throwOnError: false);
#endif
		}

		public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
		{
			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				return e.Types.Where(t => t != null);
			}
		}

		public static MethodInfo GetRealMethodInfoFromGeneric(MethodInfo method, object[] args)
		{
			try
			{
				var generics = new Dictionary<Type, Type>();
				var parameters = method.GetParameters();
				var parsedArgs = MockingUtil.GetTypesFromArguments(args);
				for (int i = 0; i < args.Length; i++)
				{
					MockingUtil.GetGenericsTypesFromActualType(parameters[i].ParameterType, parsedArgs[i], generics);
				}

				var genericArgs = method.GetGenericArguments().Select(x => generics[x]).ToArray();
				return method.MakeGenericMethod(genericArgs);
			}
			catch
			{
				return null;
			}
		}

		public static IEnumerable<MethodBase> GetInheritanceChain(this MethodBase methodBase)
		{
			yield return methodBase;

			var method = methodBase as MethodInfo;
			if (method != null && !method.IsStatic && method.DeclaringType != null && !method.DeclaringType.IsInterface)
			{
				foreach (var currentInterface in method.DeclaringType.GetInterfaces())
				{
					var interfaceMap = method.ReflectedType.GetInterfaceMap(currentInterface);
					var index = interfaceMap.TargetMethods.IndexOf(t => t == method);
					if (index != -1)
						yield return interfaceMap.InterfaceMethods[index];
				}

				var genericMethodArgs = method.IsGenericMethod ? method.GetGenericArguments() : null;
				method = method.IsGenericMethod ? method.GetGenericMethodDefinition() : method;
				var baseDefinition = method.GetBaseDefinition();
				while (baseDefinition != method)
				{
					do
					{
						var baseType = method.ReflectedType.BaseType;
						method = baseType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
										 .Single(m => m.GetBaseDefinition() == baseDefinition);
					}
					while (method.ReflectedType != method.DeclaringType);

					var baseMethod = genericMethodArgs != null ? method.MakeGenericMethod(genericMethodArgs) : method;
					yield return baseMethod;
				}
			}
		}

		public static IEnumerable<Type> GetInheritanceChain(this Type type)
		{
			while (type != null)
			{
				yield return type;
				type = type.BaseType;
			}
		}

		public static MethodInfo GetConcreteImplementer(MethodInfo method, Type implementerType)
		{
			var targetMethod = method;

			if (method.DeclaringType.IsInterface)
			{
				var baseMethod = method.IsGenericMethod ? method.GetGenericMethodDefinition() : method;
				var intfMap = implementerType.GetInterfaceMap(method.DeclaringType);
				var intfMethodIdx = intfMap.InterfaceMethods.IndexOf(m => m == baseMethod);
				targetMethod = intfMap.TargetMethods[intfMethodIdx];
			}
			else if (method.IsVirtual)
			{
				var baseMethod = method.GetBaseDefinition();

				var typesToSearch = implementerType.GetInheritanceChain()
					.TakeWhile(t => t != baseMethod.DeclaringType);

				targetMethod = typesToSearch.Select(candidateType =>
					candidateType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
						.FirstOrDefault(m => m.GetBaseDefinition() == baseMethod))
					.FirstOrDefault(m => m != null)
					?? baseMethod;
			}

			if (method.IsGenericMethod && targetMethod.IsGenericMethodDefinition)
				targetMethod = targetMethod.MakeGenericMethod(method.GetGenericArguments());

			return targetMethod;
		}

		public static int IndexOf<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
		{
			int i = 0;
			foreach (var t in enumerable)
			{
				if (predicate(t))
					return i;
				i++;
			}
			return -1;
		}

		public static bool IsCompilerGenerated(this MemberInfo member)
		{
			return member.Name.Contains("<")
				|| Attribute.IsDefined(member, typeof(CompilerGeneratedAttribute))
				|| (member.DeclaringType != null && member.DeclaringType.IsCompilerGenerated());
		}

		public static bool IsValueType(this Type type)
		{
			return type.IsValueType || type == typeof(ValueType) || type == typeof(Enum);
		}

		public static bool IsProxy(this Type type)
		{
			return type.GetInterfaces().Any(intf => intf == typeof(IMockMixin));
		}

		public static Type GetUnproxiedType(object instance)
		{
			var type = instance.GetType();
			return type.IsProxy() ? ((IMockMixin)instance).DeclaringType : type;
		}

		public static bool IsImplementedBy(this MethodInfo interfaceMethod, MethodBase implMethod)
		{
			var type = implMethod.DeclaringType;
			if (type == null || !type.GetInterfaces().Contains(interfaceMethod.DeclaringType))
				return false;

			var mapping = type.GetInterfaceMap(interfaceMethod.DeclaringType);
			var idx = Array.IndexOf(mapping.InterfaceMethods, interfaceMethod);
			return implMethod == mapping.TargetMethods[idx];
		}

		public static string GetAssemblyName(this Assembly assembly)
		{
#if COREFX
			return new AssemblyName(assembly.FullName).Name;
#else
			return assembly.GetName().Name;
#endif
		}

		public static AssemblyName GetStrongAssemblyName(string name, byte[] keyPair)
		{
#if !COREFX
			var assemblyName = new AssemblyName(name);
			if (keyPair != null)
				assemblyName.KeyPair = new StrongNameKeyPair(keyPair);
#elif SILVERLIGHT
			var assemblyName = keyPair == null || !ProfilerInterceptor.IsProfilerAttached
				? new AssemblyName(name)
				: (AssemblyName) ProfilerInterceptor.CreateStrongNameAssemblyNameImpl(name, keyPair);
#else
			var assemblyName = new AssemblyName(name);
#endif
			return assemblyName;
		}

		public static bool SafeEquals(object a, object b)
		{
			//Don't call Equals in a GuardExternal block!
			//Equals may call other methods, which may go into the repository recursively,
			//which may recursively call to Equals when searching the trees and will
			//result in a stack overflow. Equals is a bitch and cannot be intercepted safely.
			//return ProfilerInterceptor.GuardExternal(() => Object.Equals(a, b));
			return Object.Equals(a, b);
		}

		private enum ComMethodKind
		{
			Method,
			PropGet,
			PropPut,
		}

		private struct ComMethodKey
		{
			public readonly int DispId;
			public readonly ComMethodKind Kind;

			public ComMethodKey(int dispId, ComMethodKind kind)
			{
				this.DispId = dispId;
				this.Kind = kind;
			}

			#region Equality

			public override int GetHashCode()
			{
				unchecked
				{
					int result = 17;
					result = result * 23 + this.DispId.GetHashCode();
					result = result * 23 + this.Kind.GetHashCode();
					return result;
				}
			}

			public bool Equals(ComMethodKey value)
			{
				return this.DispId == value.DispId &&
					   this.Kind.Equals(value.Kind);
			}

			public override bool Equals(object obj)
			{
				if (obj == null || !(obj is ComMethodKey))
				{
					return false;
				}
				return this.Equals((ComMethodKey)obj);
			}

			#endregion
		}

		private static ComMethodKey? GetMethodDispId(MethodInfo method)
		{
			var id = GetDispId(method);

			var kind = ComMethodKind.Method;
			var prop = GetPropertyFromGetOrSet(method);
			if (prop != null)
			{
				kind = prop.GetGetMethod(true) == method ? ComMethodKind.PropGet : ComMethodKind.PropPut;
				if (id == null)
					id = GetDispId(prop);
			}

			return id != null ? (ComMethodKey?)new ComMethodKey(id.Value, kind) : null;
		}

		private static readonly Dictionary<MethodInfo, MethodInfo> normalizedComInterfaceMethodCache = new Dictionary<MethodInfo, MethodInfo>();
		public static MethodInfo NormalizeComInterfaceMethod(this MethodInfo comMethod)
		{
			if (comMethod.DeclaringType == null || !comMethod.DeclaringType.IsInterface)
				return comMethod;

			lock (normalizedComInterfaceMethodCache)
			{
				MethodInfo result;
				if (normalizedComInterfaceMethodCache.TryGetValue(comMethod, out result))
					return result;
			}

			var dispId = GetMethodDispId(comMethod);
			if (dispId == null)
				return comMethod;

			var normalMethod = comMethod;
			var intf = comMethod.DeclaringType;
			while (true)
			{
				var baseIntf = intf.GetInterfaces().FirstOrDefault();
				if (baseIntf == null)
					break;

				var baseDeclaration = baseIntf.GetMethods().FirstOrDefault(mi => GetMethodDispId(mi).Equals(dispId));
				if (baseDeclaration != null)
					normalMethod = baseDeclaration;

				intf = baseIntf;
			}

			lock (normalizedComInterfaceMethodCache)
				normalizedComInterfaceMethodCache[comMethod] = normalMethod;
			return normalMethod;
		}

		public static IEnumerable<MethodInfo> GetAllMethods(this Type type)
		{
			return type.GetInheritanceChain().SelectMany(t => t.GetMethods(AllMembers | BindingFlags.DeclaredOnly));
		}

		public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
		{
			return type.GetInheritanceChain().SelectMany(t => t.GetProperties(AllMembers | BindingFlags.DeclaredOnly));
		}

		public static IEnumerable<FieldInfo> GetAllFields(this Type type)
		{
			return type.GetInheritanceChain().SelectMany(t => t.GetFields(AllMembers | BindingFlags.DeclaredOnly));
		}

		public static object CreateInstance(Type type, params object[] args)
		{
			if (ProfilerInterceptor.IsProfilerAttached)
				return ProfilerInterceptor.CreateInstanceWithArgsImpl(type, args);
			else
				return Activator.CreateInstance(type, args);
		}

		public static bool IsExtern(this MethodBase method)
		{
#if !PORTABLE
			return (method.GetMethodImplementationFlags() & MethodImplAttributes.InternalCall) == MethodImplAttributes.InternalCall;
#else
			return false;
#endif
		}

		public static string Join(this string separator, IEnumerable<object> objects)
		{
			return String.Join(separator, objects.Select(o => o != null ? o.ToString() : null).ToArray());
		}

		public static string Join(this string separator, IEnumerable<string> objects)
		{
			return String.Join(separator, objects.ToArray());
		}

		public static string EscapeFormatString(this string formatStr)
		{
			return formatStr.Replace("{", "{{").Replace("}", "}}");
		}

		public static void RaiseEventThruReflection(object instance, EventInfo evt, object[] args)
		{
			MethodInfo raise;

			if ((raise = evt.GetRaiseMethod(true)) != null)
			{
				if (!raise.IsStatic && instance == null)
					throw new MockException("Unable to deduce the instance on which to raise the event");

				//TODO: don't call reflection methods in GuardExternal when the profiler is working
				ProfilerInterceptor.GuardExternal(() => SecuredReflectionMethods.Invoke(raise, instance, args));
			}
			else
			{
				BindingFlags all = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
				var field = evt.DeclaringType.GetField(evt.Name, all) // C# event
					?? evt.DeclaringType.GetField(evt.Name + "Event", all); //VB event

				if (field != null && field.FieldType == evt.EventHandlerType)
				{
					if (!field.IsStatic && instance == null)
						throw new MockException("Unable to deduce the instance on which to raise the event");

					var handler = (Delegate)SecuredReflectionMethods.GetField(field, instance);
					if (ProfilerInterceptor.IsProfilerAttached)
					{
						var invoker = MockingUtil.MakeFuncCaller(handler);
						ProfilerInterceptor.GuardExternal(() => invoker(args, handler));
					}
					else
					{
						var invokeMethod = field.FieldType.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
						ProfilerInterceptor.GuardExternal(() => SecuredReflectionMethods.Invoke(invokeMethod, handler, args));
					}
				}
			}
		}

		public static string GetShortCSharpName(this Type type)
		{
			if (type.IsGenericType)
			{
				return String.Format("{0}<{1}>", StripNamespacePortion(type.Name),
					String.Join(", ", type.GetGenericArguments().Select(GetShortCSharpName).ToArray()));
			}
			else
			{
				string name = type.ToString();
				switch (name)
				{
					case "System.Boolean": return "bool";
					case "System.Byte": return "byte";
					case "System.SByte": return "sbyte";
					case "System.Char": return "char";
					case "System.Int16": return "short";
					case "System.UInt16": return "ushort";
					case "System.Int32": return "int";
					case "System.UInt32": return "uint";
					case "System.Int64": return "long";
					case "System.UInt64": return "ulong";
					case "System.Single": return "float";
					case "System.Double": return "double";
					case "System.Decimal": return "decimal";
					case "System.String": return "string";
					case "System.Object": return "object";
				}
				return StripNamespacePortion(name);
			}
		}

		public static string GetShortVisualBasicName(this Type type)
		{
			if (type.IsGenericType)
			{
				return String.Format("{0}(Of {1})", StripNamespacePortion(type.Name),
					String.Join(", ", type.GetGenericArguments().Select(GetShortVisualBasicName).ToArray()));
			}
			else
			{
				string name = type.ToString();
				switch (name)
				{
					case "System.Boolean": return "Boolean";
					case "System.Byte": return "Byte";
					case "System.SByte": return "SByte";
					case "System.Char": return "Char";
					case "System.Int16": return "Short";
					case "System.UInt16": return "UShort";
					case "System.Int32": return "Integer";
					case "System.UInt32": return "UInteger";
					case "System.Int64": return "Long";
					case "System.UInt64": return "ULong";
					case "System.Single": return "Single";
					case "System.Double": return "Double";
					case "System.Decimal": return "Decimal";
					case "System.String": return "String";
					case "System.DateTime": return "Date";
					case "System.Object": return "Object";
				}
				return StripNamespacePortion(name);
			}
		}

		private static string StripNamespacePortion(string typeName)
		{
			var backtick = typeName.LastIndexOf('`');
			if (backtick != -1)
				typeName = typeName.Substring(0, backtick);

			var dot = typeName.LastIndexOf('.');
			return dot != -1 ? typeName.Substring(dot + 1) : typeName;
		}

		public static bool IsInheritable(this MethodBase method)
		{
			return method.IsVirtual && !method.IsFinal;
		}

		public static Task<T> TaskFromResult<T>(T value)
		{
			var tcs = new TaskCompletionSource<T>();
			tcs.SetResult(value);
			return tcs.Task;
		}

		public static bool StringEqual(string a, string b, bool ignoreCase)
		{
			return String.Equals(a, b, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		public static MethodBase TrySpecializeGenericMethod(MethodBase methodBase, Type[] argTypes)
		{
			var method = methodBase as MethodInfo;
			if (method == null || !method.IsGenericMethodDefinition)
				return null;

			var parameters = method.GetParameters();
			if (parameters.Length < argTypes.Length)
				return null;

			var typeArgs = method.GetGenericArguments();
			for (int i = 0; i < argTypes.Length; ++i)
			{
				var argType = argTypes[i];
				if (argType == null)
					continue;

				var paramType = parameters[i].ParameterType;
				ApplyTypeSubstitutions(paramType, argType, typeArgs);
			}

			if (typeArgs.Any(t => t.ContainsGenericParameters))
				return null;

			try
			{
				return method.MakeGenericMethod(typeArgs);
			}
			catch
			{
				return null;
			}
		}

		private static void ApplyTypeSubstitutions(Type paramType, Type argType, Type[] typeArgs)
		{
			if (paramType.IsGenericParameter)
			{
				var i = typeArgs.IndexOf(t => t == paramType);
				if (i != -1)
				{
					typeArgs[i] = argType;
				}
			}
			else if (!paramType.ContainsGenericParameters)
			{
				return;
			}
			else if (paramType.IsArray || paramType.IsByRef)
			{
				var argElementType = (paramType.IsArray && argType.IsArray) || (paramType.IsByRef && argType.IsByRef) ? argType.GetElementType() : argType;
				ApplyTypeSubstitutions(paramType.GetElementType(), argElementType, typeArgs);
			}
			else if (paramType.IsGenericType && argType.IsGenericType && paramType.GetGenericTypeDefinition() == argType.GetGenericTypeDefinition())
			{
				var paramTypeArgs = paramType.GetGenericArguments();
				var argTypeArgs = argType.GetGenericArguments();
				if (paramTypeArgs.Length != argTypeArgs.Length)
					return;

				for (int i = 0; i < argTypeArgs.Length; ++i)
				{
					ApplyTypeSubstitutions(paramTypeArgs[i], argTypeArgs[i], typeArgs);
				}
			}
			else
			{
				foreach (var intf in argType.GetInterfaces())
				{
					ApplyTypeSubstitutions(paramType, intf, typeArgs);
				}
				if (argType.BaseType != null)
				{
					ApplyTypeSubstitutions(paramType, argType.BaseType, typeArgs);
				}
			}
		}

		public static Type[] TryGetTypeArgumentsFromBinder(InvokeMemberBinder binder)
		{
			if (SecuredReflection.IsAvailable)
			{
				var csharpInvoke = binder.GetType().GetInterfaces()
					.FirstOrDefault(intf => intf.FullName == "Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder");
				if (csharpInvoke != null)
				{
					var typeArgs = (ICollection<Type>)SecuredReflectionMethods.GetProperty(csharpInvoke.GetProperty("TypeArguments"), binder, null);
					if (typeArgs != null && typeArgs.Count > 0)
						return typeArgs.ToArray();
				}
			}
			return null;
		}

		public static MethodBase TryApplyTypeArguments(MethodBase method, Type[] typeArguments)
		{
			var methodInfo = method as MethodInfo;
			if (methodInfo == null || !method.IsGenericMethodDefinition || method.GetGenericArguments().Length != typeArguments.Length)
				return null;
			try
			{
				return methodInfo.MakeGenericMethod(typeArguments.ToArray());
			}
			catch (ArgumentException)
			{
			}
			return null;
		}

#if !COREFX
		[DllImport("user32.dll")]
		private static extern bool IsImmersiveProcess(IntPtr hProcess);
		private static bool? isMetro;
		public static bool IsMetro()
		{
			if (isMetro == null)
			{
				try
				{
					isMetro = IsImmersiveProcess(Process.GetCurrentProcess().Handle);
				}
				catch
				{
					isMetro = false;
				}
			}
			return isMetro.Value;
		}
#else
		public static bool IsMetro()
		{
			return false;
		}
#endif
	}
}
