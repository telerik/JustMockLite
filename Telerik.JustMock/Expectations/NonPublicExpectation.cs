/*
 JustMock Lite
 Copyright © 2010-2014 Telerik AD

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
using System.Linq;
using System.Reflection;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Behaviors;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Expectations.Abstraction;

namespace Telerik.JustMock.Expectations
{
	internal sealed class NonPublicExpectation : INonPublicExpectation
	{
		private static MethodInfo GetMethodByName(Type type, Type returnType, string memberName, object[] args)
		{
			if (type.IsProxy())
				type = type.BaseType;

			var mockedMethod = FindMethodByName(type, returnType, memberName, args);
			if (mockedMethod == null && returnType == typeof(void))
				mockedMethod = FindMethodByName(type, null, memberName, args);

			if (mockedMethod == null)
			{
				var mockedProperty = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
					.FirstOrDefault(property => property.Name == memberName);
				if (mockedProperty != null)
				{
					var getter = mockedProperty.GetGetMethod(true);
					if (getter.ArgumentsMatchSignature(args))
						mockedMethod = getter;

					if (mockedMethod == null)
					{
						var setter = mockedProperty.GetSetMethod(true);
						if (setter.ArgumentsMatchSignature(args))
							mockedMethod = setter;
					}

					if (mockedMethod == null)
						throw new ArgumentException(String.Format("Found property '{0}' on type '{1}' but the passed arguments match the signature neither of the getter nor the setter.", memberName, type));
				}
			}

			if (mockedMethod == null)
				throw new ArgumentException(String.Format("Method '{0}' not found on type {1}", memberName, type));

			if (mockedMethod.ContainsGenericParameters)
			{
				mockedMethod = MockingUtil.GetRealMethodInfoFromGeneric(mockedMethod, args);
			}

			if (mockedMethod.DeclaringType != mockedMethod.ReflectedType)
			{
				mockedMethod = GetMethodByName(mockedMethod.DeclaringType, returnType, memberName, args);
			}

			return mockedMethod;
		}

		private static MethodInfo FindMethodByName(Type type, Type returnType, string memberName, object[] args)
		{
			return type.GetAllMethods()
				.FirstOrDefault(method => method.Name == memberName
					&& method.ArgumentsMatchSignature(args)
					&& (returnType == null || method.ReturnType == returnType));
		}

		public ActionExpectation Arrange(object target, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var type = target.GetType();
					var mixin = MocksRepository.GetMockMixin(target, null);
					if(mixin != null)
						type = mixin.DeclaringType;

					var method = GetMethodByName(type, typeof(void), memberName, args);
					return MockingContext.CurrentRepository.Arrange(target, method, args, () => new ActionExpectation());
				});
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

					var method = GetMethodByName(type, typeof(TReturn), memberName, args);
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

					var method = GetMethodByName(type, typeof(TReturn), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(target, method, args, null);
				});
		}

		public void Assert(object target, MethodInfo method, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.AssertMethodInfo(target, method, args, null));
		}

		public void Assert(object target, string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var type = target.GetType();
					var mixin = MocksRepository.GetMockMixin(target, null);
					if (mixin != null)
						type = mixin.DeclaringType;

					var method = GetMethodByName(type , typeof(void), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(target, method, args, null);
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

					var method = GetMethodByName(type, typeof(TReturn), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(target, method, args, occurs);
				});
		}

		public void Assert(object target, MethodInfo method, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.AssertMethodInfo(target, method, args, occurs));
		}

		public void Assert(object target, string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var type = target.GetType();
					var mixin = MocksRepository.GetMockMixin(target, null);
					if (mixin != null)
						type = mixin.DeclaringType;

					var method = GetMethodByName(type, typeof(void), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(target, method, args, occurs);
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

				var method = GetMethodByName(type, typeof(void), memberName, args);
				return MockingContext.CurrentRepository.GetTimesCalledFromMethodInfo(target, method, args);
			});
		}

#if !LITE_EDITION

		public FuncExpectation<TReturn> Arrange<T, TReturn>(string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(typeof(T), typeof(TReturn), memberName, args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new FuncExpectation<TReturn>());
				});
		}

		public ActionExpectation Arrange<T>(string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(typeof(T), typeof(void), memberName, args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new ActionExpectation());
				});
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(Type targetType, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(targetType, typeof(TReturn), memberName, args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new FuncExpectation<TReturn>());
				});
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(MethodInfo method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Arrange(null, method, args, () => new FuncExpectation<TReturn>()));
		}

		public ActionExpectation Arrange(Type targetType, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(targetType, typeof(void), memberName, args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new ActionExpectation());
				});
		}

		public ActionExpectation Arrange(MethodInfo method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Arrange(null, method, args, () => new ActionExpectation()));
		}

		public void Assert<T>(string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(typeof(T), typeof(void), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, occurs);
				});
		}

		public void Assert(MethodInfo method, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, occurs));
		}

		public void Assert<T, TReturn>(string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(typeof(T), typeof(TReturn), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, occurs);
				});
		}

		public void Assert<T>(string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(typeof(T), typeof(void), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, null);
				});
		}

		public void Assert(MethodInfo method, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, null));
		}

		public void Assert<T, TReturn>(string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(typeof(T), typeof(TReturn), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, null);
				});
		}

		public void Assert(Type targetType, string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(targetType, typeof(void), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, occurs);
				});
		}

		public void Assert<TReturn>(Type targetType, string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(targetType, typeof(TReturn), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, occurs);
				});
		}

		public void Assert(Type targetType, string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(targetType, typeof(void), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, null);
				});
		}

		void INonPublicExpectation.Assert<TReturn>(Type targetType, string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(targetType, typeof(TReturn), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, null);
				});
		}

		public int GetTimesCalled(MethodInfo method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				MockingContext.CurrentRepository.GetTimesCalledFromMethodInfo(null, method, args));

		}

		public int GetTimesCalled(Type type, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var method = GetMethodByName(type, typeof(void), memberName, args);
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
	}
}
