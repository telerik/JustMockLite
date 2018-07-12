/*
 JustMock Lite
 Copyright © 2018 Telerik AD

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
		public static MethodInfo GetLocalMethod(Type type, MethodInfo method, string localMemberName)
		{
			MethodInfo[] allStaticNonPublicMethods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo[] potentialLocalMethods = allStaticNonPublicMethods.Where(m => (m.Name.Contains(method.Name) && m.Name.Contains(localMemberName))).ToArray();

			if (potentialLocalMethods.Length == 0)
			{
				throw new MissingMemberException(BuildMissingMethodMessage(type, null, localMemberName));
			}
			else if (potentialLocalMethods.Length == 1)
			{
				return potentialLocalMethods.First();
			}
			else
			{
				MethodInfo localMethod = null;
				MethodInfo[] allMethods = type.GetAllMethods().ToArray();
				int methodIndex = Array.IndexOf(allMethods, method);
				methodIndex += 1; //Indices in the names of local methods are 1-based

				var regEx = new System.Text.RegularExpressions.Regex(@"<[a-z,A-z]>g__[a-z,A-z]|(?<MethodId>\d+)_(?<LocalId>\d+)");
				foreach (var candidate in potentialLocalMethods)
				{
					System.Text.RegularExpressions.Match match = regEx.Match(candidate.Name);
					if (match.Success)
					{
						int index = int.Parse(match.Groups["MethodId"].Value);
						if (methodIndex == index)
						{
							localMethod = candidate;
							break;
						}
					}
				}
				if (localMethod != null)
				{
					return localMethod;
				}
				else
				{
					throw new MissingMemberException(BuildMissingMethodMessage(type, null, localMemberName));
				}
			}
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
