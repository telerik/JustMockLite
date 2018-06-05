/*
 JustMock Lite
 Copyright © 2010-2015 Telerik AD

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
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Behaviors;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Expectations.Abstraction;
using Telerik.JustMock.Expectations.DynaMock;

namespace Telerik.JustMock.Expectations
{
	internal sealed class NonPublicExpectation : INonPublicExpectation
	{
		private static bool ReturnTypeMatches(Type returnType, MethodInfo method)
		{
			return returnType == null
				|| method.ReturnType == returnType
				|| (method.ReturnType.IsPointer && returnType == typeof(IntPtr));
		}

		private static MethodInfo GetMethodByName(Type type, Type returnType, string memberName, ref object[] args)
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

		private static string BuildMissingMethodMessage(Type type, PropertyInfo property, string memberName)
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

		private static MethodInfo FindMethodBySignature(IEnumerable<MethodInfo> candidates, Type returnType, object[] args)
		{
			return candidates.FirstOrDefault(method => method.ArgumentsMatchSignature(args) && ReturnTypeMatches(returnType, method));
		}

		private static string GetAssertionMessage(object[] args)
		{
			return null;
		}

		public ActionExpectation Arrange(object target, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var type = target.GetType();
					var mixin = MocksRepository.GetMockMixin(target, null);
					if (mixin != null)
						type = mixin.DeclaringType;

					var method = GetMethodByName(type, typeof(void), memberName, ref args);
					return MockingContext.CurrentRepository.Arrange(target, method, args, () => new ActionExpectation());
				});
		}

		public ActionExpectation ArrangeLocal(object target, string memberName, string localMemberName, params object[] args)
		{
			Type type = target.GetType();

			MethodInfo[] allMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

			MethodInfo method = type.GetMethod(memberName);
			MethodBody body = method.GetMethodBody();
			byte[] il = body.GetILAsByteArray();

			MethodInfo localMethod = null;
			for (int i = 0; i < il.Length; i++)
			{
				byte opCode = il[i];
				if (opCode == 0x28) ////System.Reflection.Emit.OpCodes.Call)
				{
					int token = BitConverter.ToInt32(il, i + 1);
					foreach(var m in allMethods)
					{
						if(token == m.MetadataToken && m.Name.Contains(localMemberName))
						{
							localMethod = m;
							break;
						}
					}
				}
				if(localMethod != null)
				{
					break;
				}
			}

			return Arrange(target, localMethod.Name, args);

		}

		public ActionExpectation Arrange(object target, MethodInfo method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Arrange(target, method, args, () => new ActionExpectation()));
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(object target, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var type = target.GetType();
					var mixin = MocksRepository.GetMockMixin(target, null);
					if (mixin != null)
						type = mixin.DeclaringType;

					var method = GetMethodByName(type, typeof(TReturn), memberName, ref args);
					return MockingContext.CurrentRepository.Arrange(target, method, args, () => new FuncExpectation<TReturn>());
				});
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(object target, MethodInfo method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Arrange(target, method, args, () => new FuncExpectation<TReturn>()));
		}

		public void Assert<TReturn>(object target, string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var type = target.GetType();
					var mixin = MocksRepository.GetMockMixin(target, null);
					if (mixin != null)
						type = mixin.DeclaringType;

					var message = GetAssertionMessage(args);
					var method = GetMethodByName(type, typeof(TReturn), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, target, method, args, null);
				});
		}

		public void Assert(object target, MethodInfo method, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = GetAssertionMessage(args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, target, method, args, null);
				});
		}

		public void Assert(object target, string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var type = target.GetType();
					var mixin = MocksRepository.GetMockMixin(target, null);
					if (mixin != null)
						type = mixin.DeclaringType;

					var message = GetAssertionMessage(args);
					var method = GetMethodByName(type, typeof(void), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, target, method, args, null);
				});
		}

		public void Assert<TReturn>(object target, string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var type = target.GetType();
					var mixin = MocksRepository.GetMockMixin(target, null);
					if (mixin != null)
						type = mixin.DeclaringType;

					var message = GetAssertionMessage(args);
					var method = GetMethodByName(type, typeof(TReturn), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, target, method, args, occurs);
				});
		}

		public void Assert(object target, MethodInfo method, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = GetAssertionMessage(args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, target, method, args, occurs);
				});
		}

		public void Assert(object target, string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var type = target.GetType();
					var mixin = MocksRepository.GetMockMixin(target, null);
					if (mixin != null)
						type = mixin.DeclaringType;

					var message = GetAssertionMessage(args);
					var method = GetMethodByName(type, typeof(void), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, target, method, args, occurs);
				});
		}

		public int GetTimesCalled(object target, MethodInfo method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				MockingContext.CurrentRepository.GetTimesCalledFromMethodInfo(target, method, args));
		}

		public int GetTimesCalled(object target, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var type = target.GetType();
				var mixin = MocksRepository.GetMockMixin(target, null);
				if (mixin != null)
					type = mixin.DeclaringType;

				var method = GetMethodByName(type, typeof(void), memberName, ref args);
				return MockingContext.CurrentRepository.GetTimesCalledFromMethodInfo(target, method, args);
			});
		}

#if !LITE_EDITION

		public FuncExpectation<TReturn> Arrange<T, TReturn>(string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(typeof(T), typeof(TReturn), memberName, ref args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new FuncExpectation<TReturn>());
				});
		}

		public ActionExpectation Arrange<T>(string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(typeof(T), typeof(void), memberName, ref args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new ActionExpectation());
				});
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(Type targetType, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(targetType, typeof(TReturn), memberName, ref args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new FuncExpectation<TReturn>());
				});
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(MethodBase method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Arrange(null, method, args, () => new FuncExpectation<TReturn>()));
		}

		public ActionExpectation Arrange(Type targetType, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(targetType, typeof(void), memberName, ref args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new ActionExpectation());
				});
		}

		public ActionExpectation Arrange(MethodBase method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Arrange(null, method, args, () => new ActionExpectation()));
		}

		public void Assert<T>(string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = GetAssertionMessage(args);
					var method = GetMethodByName(typeof(T), typeof(void), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, occurs);
				});
		}

		public void Assert(MethodBase method, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = GetAssertionMessage(args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, occurs);
				});
		}

		public void Assert<T, TReturn>(string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = GetAssertionMessage(args);
					var method = GetMethodByName(typeof(T), typeof(TReturn), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, occurs);
				});
		}

		public void Assert<T>(string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = GetAssertionMessage(args);
					var method = GetMethodByName(typeof(T), typeof(void), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, null);
				});
		}

		public void Assert(MethodBase method, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = GetAssertionMessage(args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, null);
				});
		}

		public void Assert<T, TReturn>(string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = GetAssertionMessage(args);
					var method = GetMethodByName(typeof(T), typeof(TReturn), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, null);
				});
		}

		public void Assert(Type targetType, string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = GetAssertionMessage(args);
					var method = GetMethodByName(targetType, typeof(void), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, occurs);
				});
		}

		public void Assert<TReturn>(Type targetType, string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = GetAssertionMessage(args);
					var method = GetMethodByName(targetType, typeof(TReturn), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, occurs);
				});
		}

		public void Assert(Type targetType, string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = GetAssertionMessage(args);
					var method = GetMethodByName(targetType, typeof(void), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, null);
				});
		}

		void INonPublicExpectation.Assert<TReturn>(Type targetType, string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var message = GetAssertionMessage(args);
					var method = GetMethodByName(targetType, typeof(TReturn), memberName, ref args);
					MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, null);
				});
		}

		public int GetTimesCalled(MethodBase method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				MockingContext.CurrentRepository.GetTimesCalledFromMethodInfo(null, method, args));

		}

		public int GetTimesCalled(Type type, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var method = GetMethodByName(type, typeof(void), memberName, ref args);
				return MockingContext.CurrentRepository.GetTimesCalledFromMethodInfo(null, method, args);
			});
		}
#endif

		#region Raise event

		public void Raise(object instance, EventInfo eventInfo, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() => RaiseEventBehavior.RaiseEventImpl(instance, eventInfo, args));
		}

		public void Raise(EventInfo eventInfo, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() => RaiseEventBehavior.RaiseEventImpl(null, eventInfo, args));
		}

		public void Raise(object instance, string eventName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				RaiseEventBehavior.RaiseEventImpl(instance, MockingUtil.GetUnproxiedType(instance).GetEvent(eventName,
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), args);
			});
		}

		public void Raise(Type type, string eventName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				RaiseEventBehavior.RaiseEventImpl(null, type.GetEvent(eventName,
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static), args);
			});
		}
		#endregion

		public PrivateAccessor MakePrivateAccessor(object instance)
		{
			return new PrivateAccessor(instance);
		}

		public PrivateAccessor MakeStaticPrivateAccessor(Type type)
		{
			return PrivateAccessor.ForType(type);
		}

		public dynamic Wrap(object instance)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
#if !LITE_EDITION
					Mock.Intercept(instance.GetType());
#endif
					return new ExpressionContainer(Expression.Constant(instance, MockingUtil.GetUnproxiedType(instance)));
				}
			);
		}

		public dynamic WrapType(Type type)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
#if !LITE_EDITION
					Mock.Intercept(type);
#endif
					return new ExpressionContainer(Expression.Constant(type.GetDefaultValue(), type)) { IsStatic = true };
				}
			);
		}

		public ActionExpectation Arrange(dynamic dynamicExpression)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				MockingContext.CurrentRepository.Arrange(((IExpressionContainer)dynamicExpression).ToLambda(), () => new ActionExpectation())
			);
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(dynamic dynamicExpression)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				MockingContext.CurrentRepository.Arrange(((IExpressionContainer)dynamicExpression).ToLambda(), () => new FuncExpectation<TReturn>())
			);
		}

		public void Assert(dynamic dynamicExpression, Occurs occurs, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() =>
				MockingContext.CurrentRepository.Assert(message, null, ((IExpressionContainer)dynamicExpression).ToLambda(), null, occurs)
			);
		}

		public void Assert(dynamic dynamicExpression, Args args, Occurs occurs, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() =>
				MockingContext.CurrentRepository.Assert(message, null, ((IExpressionContainer)dynamicExpression).ToLambda(), args, occurs)
			);
		}
	}
}
