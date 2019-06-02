/*
 JustMock Lite
 Copyright © 2018 Progress Software Corporation

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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Telerik.JustMock.Core
{
	internal static partial class MockingUtil
	{
		internal static bool CanCall(MethodBase method, bool hasInstance)
		{
			return method.IsStatic || hasInstance;
		}

		internal static PropertyInfo ResolveProperty(Type type, string name, bool ignoreCase, object[] indexArgs, bool hasInstance, object setterValue = null, bool getter = true)
		{
			var candidates = type.GetAllProperties().Where(prop => MockingUtil.StringEqual(prop.Name, name, ignoreCase)).ToArray();
			if (candidates.Length == 1)
				return candidates[0];

			if (!getter)
			{
				Array.Resize(ref indexArgs, indexArgs.Length + 1);
				indexArgs[indexArgs.Length - 1] = setterValue;
			}

			var propMethods = candidates
				.Select(prop => getter ? prop.GetGetMethod(true) : prop.GetSetMethod(true))
				.Where(m => m != null && CanCall(m, hasInstance))
				.ToArray();

			indexArgs = indexArgs ?? MockingUtil.NoObjects;
			object state;
			var foundGetter = MockingUtil.BindToMethod(MockingUtil.AllMembers, propMethods, ref indexArgs, null, null, null, out state);
			return candidates.First(prop => (getter ? prop.GetGetMethod(true) : prop.GetSetMethod(true)) == foundGetter);
		}

		internal static bool ReturnTypeMatches(Type returnType, MethodInfo method)
		{
			return returnType == null
				|| method.ReturnType == returnType
				|| (method.ReturnType.IsPointer && returnType == typeof(IntPtr));
		}

        internal static MethodInfo GetGenericMethodInstanceByName(Type type, Type returnType, string memberName, Type[] typeArguments, ref object[] args)
        {
            if (type.IsProxy())
                type = type.BaseType;


            object[] arguments = args;

            var allGenericMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic).Where(p => p.IsGenericMethod);
            var candidateGenerics = allGenericMethods.Where(p => p.Name == memberName);
            var candidates = candidateGenerics.Where(p => p.MakeGenericMethod(typeArguments).ArgumentsMatchSignature(arguments) == true);

            var method = candidates.FirstOrDefault();
            var methodInstance = method.MakeGenericMethod(typeArguments);

            return methodInstance;
        }


        internal static MethodInfo GetMethodByName(Type type, Type returnType, string memberName, ref object[] args)
		{
			if (type.IsProxy())
				type = type.BaseType;

			var candidateMethods = type.GetAllMethods()
				.Where(m => m.Name == memberName)
				.Concat(type.GetAllProperties()
					.Where(p => p.Name == memberName)
					.SelectMany(p => new[] { p.GetGetMethod(true), p.GetSetMethod(true) })
					.Where(m => m != null))
				.Select(m =>
				{
					if (m.IsGenericMethodDefinition
						&& returnType != typeof(void)
						&& m.GetGenericArguments().Length == 1
						&& m.ReturnType.ContainsGenericParameters)
					{
						var generics = new Dictionary<Type, Type>();
						if (MockingUtil.GetGenericsTypesFromActualType(m.ReturnType, returnType, generics))
						{
							return m.MakeGenericMethod(generics.Values.Single());
						}
					}
					return m;
				})
				.ToArray();

			MethodInfo mockedMethod = null;

			if (candidateMethods.Length == 1)
			{
				var singleCandidate = candidateMethods[0];
				var returnTypeMatches = ReturnTypeMatches(returnType, singleCandidate);
				var argsIgnored = args == null || args.Length == 0;
				if (returnTypeMatches && argsIgnored)
				{
					mockedMethod = singleCandidate;
					args = mockedMethod.GetParameters()
						.Select(p =>
						{
							var byref = p.ParameterType.IsByRef;
							var paramType = byref ? p.ParameterType.GetElementType() : p.ParameterType;
							if (paramType.IsPointer)
								paramType = typeof(IntPtr);
							var isAny = (Expression)typeof(ArgExpr).GetMethod("IsAny").MakeGenericMethod(paramType).Invoke(null, null);
							if (byref)
							{
								isAny = ArgExpr.Ref(isAny);
							}
							return isAny;
						})
						.ToArray();
				}
			}

			if (mockedMethod == null)
			{
				mockedMethod = FindMethodBySignature(candidateMethods, returnType, args);
				if (mockedMethod == null && returnType == typeof(void))
					mockedMethod = FindMethodBySignature(candidateMethods, null, args);
			}

			if (mockedMethod == null)
			{
				var mockedProperty = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
					.FirstOrDefault(property => property.Name == memberName);
				if (mockedProperty != null)
				{
					var getter = mockedProperty.GetGetMethod(true);
					if (getter != null && getter.ArgumentsMatchSignature(args))
						mockedMethod = getter;

					if (mockedMethod == null)
					{
						var setter = mockedProperty.GetSetMethod(true);
						if (setter != null && setter.ArgumentsMatchSignature(args))
							mockedMethod = setter;
					}

					if (mockedMethod == null)
						throw new MissingMemberException(BuildMissingMethodMessage(type, mockedProperty, memberName));
				}
			}

			if (mockedMethod == null)
				throw new MissingMemberException(BuildMissingMethodMessage(type, null, memberName));

			if (mockedMethod.ContainsGenericParameters)
			{
				mockedMethod = MockingUtil.GetRealMethodInfoFromGeneric(mockedMethod, args);
			}

			if (mockedMethod.DeclaringType != mockedMethod.ReflectedType)
			{
				mockedMethod = GetMethodByName(mockedMethod.DeclaringType, returnType, memberName, ref args);
			}

			return mockedMethod;
		}

		internal static string BuildMissingMethodMessage(Type type, PropertyInfo property, string memberName)
		{
			var sb = new StringBuilder();
			sb.AppendLine(
				property != null
				? String.Format("Found property '{0}' on type '{1}' but the passed arguments match the signature neither of the getter nor the setter.", memberName, type)
				: String.Format("Method '{0}' with the given signature was not found on type {1}", memberName, type));

			var methods = new Dictionary<MethodInfo, string>();

			if (property != null)
			{
				var getter = property.GetGetMethod(true);
				if (getter != null)
					methods.Add(getter, property.Name);
				var setter = property.GetSetMethod(true);
				if (setter != null)
					methods.Add(setter, property.Name);
			}

			foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Where(m => m.Name == memberName))
			{
				methods.Add(method, method.Name);
			}

			if (methods.Count == 0)
			{
				sb.AppendLine("No methods or properties found with the given name.");
			}
			else
			{
				sb.AppendLine("Review the available methods in the message below and optionally paste the appropriate arrangement snippet.");
				int i = 0;
				foreach (var kvp in methods)
				{
					sb.AppendLine("----------");
					var method = kvp.Key;
					sb.AppendLine(String.Format("Method {0}: {1}", i + 1, method));
					sb.AppendLine("C#: " + FormatMethodArrangementExpression(kvp.Value, method, SourceLanguage.CSharp));
					sb.AppendLine("VB: " + FormatMethodArrangementExpression(kvp.Value, method, SourceLanguage.VisualBasic));
					i++;
				}
			}

			return sb.ToString();
		}

		internal static string FormatMethodArrangementExpression(string memberName, MethodBase method, SourceLanguage language)
		{
			return String.Format("Mock.NonPublic.Arrange{0}({1}\"{2}\"{3}){4}",
				FormatGenericArg(method.GetReturnType(), language),
				method.IsStatic ? String.Empty : "mock, ",
				memberName,
				String.Join("", method.GetParameters().Select(p => ", " + FormatMethodParameterMatcher(p.ParameterType, language)).ToArray()),
				language == SourceLanguage.CSharp ? ";" : "");
		}

		internal static string FormatMethodParameterMatcher(Type paramType, SourceLanguage language)
		{
			if (paramType.IsByRef)
			{
				return String.Format("ArgExpr.Ref({0})", FormatMethodParameterMatcher(paramType.GetElementType(), language));
			}
			else
			{
				return String.Format("ArgExpr.IsAny{0}()", FormatGenericArg(paramType, language));
			}
		}

		internal static string FormatGenericArg(Type type, SourceLanguage language)
		{
			if (type == typeof(void))
			{
				return String.Empty;
			}
			switch (language)
			{
				case SourceLanguage.CSharp:
					return String.Format("<{0}>", type.GetShortCSharpName());
				case SourceLanguage.VisualBasic:
					return String.Format("(Of {0})", type.GetShortVisualBasicName());
				default:
					throw new ArgumentOutOfRangeException("language");
			}
		}

		internal static MethodInfo FindMethodBySignature(IEnumerable<MethodInfo> candidates, Type returnType, object[] args)
		{
			return candidates.FirstOrDefault(method => method.ArgumentsMatchSignature(args) && ReturnTypeMatches(returnType, method));
		}

		internal static string GetAssertionMessage(object[] args)
		{
			return null;
		}

		public static MethodInfo GetLocalFunction(object target, MethodInfo method, string localMemberName, Type[] localFunctionGenericTypes)
		{
			Type type = target.GetType();
			return GetLocalFunction(type, method, localMemberName, localFunctionGenericTypes);
		}

		public static MethodInfo GetLocalFunction(Type type, MethodInfo method, string localMemberName, Type[] localFunctionGenericTypes)
		{
#if !PORTABLE
			if (type.IsProxy())
			{
				type = type.BaseType;
			}

			string expectedName = String.Format("<{0}>g__{1}|", method.Name, localMemberName);

			MethodBody body = method.GetMethodBody();
			var disasembledBody = MethodBodyDisassembler.DisassembleMethodInfo(method);
			var callInstructions = disasembledBody.Where(instr => instr.OpCode == System.Reflection.Emit.OpCodes.Call).ToArray();
			MethodInfo localMethod = null;
			foreach (var instruction in callInstructions)
			{
                MethodBase methodBase = null;
                try
                {
                    methodBase = type.Module.ResolveMethod(instruction.Operand.Int);
                }catch(Exception)
                {

                }

                if (methodBase == null && localFunctionGenericTypes != null)
                {

                    try
                    {
                        methodBase = type.Module.ResolveMethod(instruction.Operand.Int, null, localFunctionGenericTypes);
                    }
                    catch (Exception)
                    {

                    }
                }

                if (methodBase != null && methodBase.DeclaringType == type && methodBase.Name.StartsWith(expectedName))
				{
					localMethod = methodBase as MethodInfo;
					break;
				}
			}

			if (localMethod == null)
			{
				throw new MissingMemberException(BuildMissingLocalFunctionMessage(type, method, localMemberName));
			}

			return localMethod;
#else
			return null;
#endif
		}

		public static MethodInfo GetMethodWithLocalFunction(object target, string methodName)
		{
			Type type = target.GetType();
			return GetMethodWithLocalFunction(type, methodName);
		}

		public static MethodInfo GetMethodWithLocalFunction(Type type, string methodName)
		{
			Type[] emptyParamTypes = new Type[] { };
			return GetMethodWithLocalFunction( type, methodName, emptyParamTypes, null);
		}

		public static MethodInfo GetMethodWithLocalFunction(object target, string methodName, Type[] methodParamTypes, Type[] methodGenericTypes)
		{
			Type type = target.GetType();
			return GetMethodWithLocalFunction(type, methodName, methodParamTypes, methodGenericTypes);
		}

		public static MethodInfo GetMethodWithLocalFunction(Type type, string methodName, Type[] methodParamTypes, Type[] methodGenericTypes)
		{
#if !PORTABLE
			if (type.IsProxy())
			{
				type = type.BaseType;
			}
			MethodInfo method = type.GetMethod(methodName, MockingUtil.AllMembers, null, methodParamTypes, null);

			if(method == null)
			{
				throw new MissingMemberException(MockingUtil.BuildMissingMethodMessage(type, null, methodName));
			}

            if(method.ContainsGenericParameters && (methodGenericTypes == null || methodGenericTypes.Length == 0))
            {
                throw new MissingMemberException(MockingUtil.BuildMissingMethodMessage(type, null, methodName));
            }

            if (method.ContainsGenericParameters)
            {
                method = method.MakeGenericMethod(methodGenericTypes);
            }

			return method;
#else
			return null;
#endif
		}

		public static string BuildMissingLocalFunctionMessage(Type type, MethodInfo method, string localFunctionName)
		{
			var sb = new StringBuilder();
			sb.AppendLine(String.Format("C# 7 local function '{0}' with the given signature was not found inside method '{1}' on type '{2}'.", localFunctionName, method.Name, type));
			sb.AppendLine("Note that mocking C# 7 local functions that are declared but never called is not supported.");
			return sb.ToString();
		}
	}
}
