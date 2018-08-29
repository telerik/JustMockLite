/*
 JustMock Lite
 Copyright © 2018 Telerik EAD

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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Telerik.JustMock.Core
{
	internal static partial class MockingUtil
	{
		public static MethodInfo GetLocalFunction(object target, MethodInfo method, string localMemberName)
		{
			Type type = target.GetType();
			return GetLocalFunction(type, method, localMemberName);
		}

		public static MethodInfo GetLocalFunction(Type type, MethodInfo method, string localMemberName)
		{
			MethodBody body = method.GetMethodBody();
			byte[] il = body.GetILAsByteArray();

			MethodInfo localMethod = null;
			for (int i = 0; i < il.Length; i++)
			{
				byte opCode = il[i];
				if (opCode == 0x28) ////System.Reflection.Emit.OpCodes.Call)
				{
					int token = BitConverter.ToInt32(il, i + 1);

					MethodBase methodBase = type.Module.ResolveMethod(token);
					
					if(methodBase.DeclaringType == type)
					{
						var regEx = new System.Text.RegularExpressions.Regex(@"<(?<ContainingMethod>([a-z,A-Z,0-9]+))>g__(?<LocalFunction>([[a-z,A-Z,0-9]+))[|][0-9]+_[0-9]+");
						System.Text.RegularExpressions.Match match = regEx.Match(methodBase.Name);
						if (match.Success)
						{
							string containigMethod = match.Groups["ContainingMethod"].Value;
							string localFunction = match.Groups["LocalFunction"].Value;
							if (method.Name.Equals(containigMethod) && localMemberName.Equals(localFunction))
							{

								localMethod = methodBase as MethodInfo;
								break;
							}
						}
					}
					if (localMethod != null)
					{
						break;
					}
				}
			}
			if (localMethod == null)
				throw new MissingMemberException(BuildMissingMethodMessage(type, null, localMemberName));

			return localMethod;
		}

		public static MethodInfo GetMethodWithLocalFunction(object target, string methodName)
		{
			Type type = target.GetType();
			return GetMethodWithLocalFunction(type, methodName);
		}

		public static MethodInfo GetMethodWithLocalFunction(Type type, string methodName)
		{
			Type[] emptyParamTypes = new Type[] { };
			return GetMethodWithLocalFunction( type, methodName, emptyParamTypes);
		}

		public static MethodInfo GetMethodWithLocalFunction(object target, string methodName, Type[] methodParamTypes)
		{
			Type type = target.GetType();
			return GetMethodWithLocalFunction(type, methodName, methodParamTypes);
		}

		public static MethodInfo GetMethodWithLocalFunction(Type type, string methodName, Type[] methodParamTypes)
		{
			MethodInfo method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |BindingFlags.Static, null, methodParamTypes, null);
			if(method == null)
			{
				throw new MissingMemberException(MockingUtil.BuildMissingMethodMessage(type, null, methodName));
			}
			return method;
		}

		public static string BuildMissingMethodMessage(Type type, PropertyInfo property, string memberName)
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

		private static string FormatMethodArrangementExpression(string memberName, MethodBase method, SourceLanguage language)
		{
			return String.Format("Mock.NonPublic.Arrange{0}({1}\"{2}\"{3}){4}",
				FormatGenericArg(method.GetReturnType(), language),
				method.IsStatic ? String.Empty : "mock, ",
				memberName,
				String.Join("", method.GetParameters().Select(p => ", " + FormatMethodParameterMatcher(p.ParameterType, language)).ToArray()),
				language == SourceLanguage.CSharp ? ";" : "");
		}

		private static string FormatMethodParameterMatcher(Type paramType, SourceLanguage language)
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

		private static string FormatGenericArg(Type type, SourceLanguage language)
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
	}
}
