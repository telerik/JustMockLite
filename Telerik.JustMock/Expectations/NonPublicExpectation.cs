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
	/// <summary>
	/// Setups non-public expectations.
	/// </summary>
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
					mockedMethod = returnType == typeof(void) ? mockedProperty.GetSetMethod(true) : mockedProperty.GetGetMethod(true);
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

		/// <summary>
		/// Setups a non-public method for mocking.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="memberName">Member name</param>
		/// <param name="args">Method arguments</param>
		/// <returns>Refernce to setup actions calls</returns>
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

		/// <summary>
		/// Setups a non-public method for mocking.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="method">Method to setup taken from reflection.</param>
		/// <param name="args">Method arguments</param>
		/// <returns>Refernce to setup actions calls</returns>
		public ActionExpectation Arrange(object target, MethodInfo method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Arrange(target, method, args, () => new ActionExpectation()));
		}

		/// <summary>
		/// Setups a non-public method for mocking.
		/// </summary>
		/// <typeparam name="TReturn">Return type</typeparam>
		/// <param name="target">Target instance.</param>
		/// <param name="memberName">Target member name</param>
		/// <param name="args">Method arguments</param>
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

		/// <summary>
		/// Setups a non-public method for mocking.
		/// </summary>
		/// <typeparam name="TReturn">Return type</typeparam>
		/// <param name="target">Target instance</param>
		/// <param name="method">Method to setup taken from reflection.</param>
		/// <param name="args">Method arguments</param>
		/// <returns>Refernce to setup actions calls</returns>
		public FuncExpectation<TReturn> Arrange<TReturn>(object target, MethodInfo method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Arrange(target, method, args, () => new FuncExpectation<TReturn>()));
		}

		/// <summary>
		/// Asserts the specified member that it is called as expected.
		/// </summary>
		/// <param name="target">Target mock</param>
		/// <param name="memberName">Name of the member</param>
		/// <param name="args">Method arguments</param>
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

		/// <summary>
		/// Asserts the specified method that it is called as expected.
		/// </summary>
		/// <param name="target">Target mock</param>
		/// <param name="method">Method to assert taken from reflection.</param>
		/// <param name="args">Method arguments</param>
		public void Assert(object target, MethodInfo method, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.AssertMethodInfo(target, method, args, null));
		}

		/// <summary>
		/// Asserts the specified member that it is called as expected.
		/// </summary>
		/// <param name="target">Target mock</param>
		/// <param name="memberName">Name of the member</param>
		/// <param name="args">Method arguments</param>
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

		/// <summary>
		/// Asserts the specified member that it is called as expected.
		/// </summary>
		/// <param name="target">Target mock</param>
		/// <param name="memberName">Name of the member</param>
		/// <param name="occurs">Specifies the number of times a call should occur.</param>
		/// <typeparam name="TReturn">Return type of the method</typeparam>
		/// <param name="args">Method arguments</param>
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

		/// <summary>
		/// Asserts the specified method that it is called as expected.
		/// </summary>
		/// <param name="target">Target mock</param>
		/// <param name="method">Method to assert taken from reflection.</param>
		/// <param name="args">Method arguments</param>
		public void Assert(object target, MethodInfo method, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.AssertMethodInfo(target, method, args, occurs));
		}

		/// <summary>
		/// Asserts the specified member that it is called as expected.
		/// </summary>
		/// <param name="target">Target mock</param>
		/// <param name="memberName">Name of the member</param>
		/// <param name="occurs">Specifies the number of times a call should occur.</param>
		/// <param name="args">Method arguments</param>
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


#if !LITE_EDITION

		/// <summary>
		/// Arranges a method for mocking.
		/// </summary>
		/// <typeparam name="T">Type of the target.</typeparam>
		/// <typeparam name="TReturn">Return type</typeparam>
		/// <param name="memberName">Target member name</param>
		/// <param name="args">Method arguments</param>
		public FuncExpectation<TReturn> Arrange<T, TReturn>(string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(typeof(T), typeof(TReturn), memberName, args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new FuncExpectation<TReturn>());
				});
		}

		/// <summary>
		/// Arranges a method for mocking.
		/// </summary>
		/// <typeparam name="T">Type of the target.</typeparam>
		/// <param name="memberName">Target member name</param>
		/// <param name="args">Method arguments</param>
		public ActionExpectation Arrange<T>(string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(typeof(T), typeof(void), memberName, args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new ActionExpectation());
				});
		}

		/// <summary>
		/// Arranges a method for mocking.
		/// </summary>
		/// <typeparam name="TReturn">Return type</typeparam>
		/// <param name="targetType">Target type</param>
		/// <param name="memberName">Target member name</param>
		/// <param name="args">Method arguments</param>
		public FuncExpectation<TReturn> Arrange<TReturn>(Type targetType, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(targetType, typeof(TReturn), memberName, args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new FuncExpectation<TReturn>());
				});
		}

		/// <summary>
		/// Arranges a method for mocking.
		/// </summary>
		/// <typeparam name="TReturn">Return type</typeparam>
		/// <param name="method">Target method</param>
		/// <param name="args">Method arguments</param>
		public FuncExpectation<TReturn> Arrange<TReturn>(MethodInfo method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Arrange(null, method, args, () => new FuncExpectation<TReturn>()));
		}

		/// <summary>
		/// Arranges a method for mocking.
		/// </summary>
		/// <param name="targetType">Target type</param>
		/// <param name="memberName">Target member name</param>
		/// <param name="args">Method arguments</param>
		public ActionExpectation Arrange(Type targetType, string memberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(targetType, typeof(void), memberName, args);
					return MockingContext.CurrentRepository.Arrange(null, method, args, () => new ActionExpectation());
				});
		}

		/// <summary>
		/// Arranges a method for mocking.
		/// </summary>
		/// <typeparam name="TReturn">Return type</typeparam>
		/// <param name="method">Target method</param>
		/// <param name="args">Method arguments</param>
		public ActionExpectation Arrange(MethodInfo method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Arrange(null, method, args, () => new ActionExpectation()));
		}

		/// <summary>
		/// Asserts the specified member that it is called as expected.
		/// </summary>
		/// <typeparam name="T">Specify the target type</typeparam>
		/// <param name="memberName">Name of the member</param>
		/// <param name="occurs">Specifies the number of times a call should occur.</param>
		/// <param name="args">Method arguments</param>
		public void Assert<T>(string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(typeof(T), typeof(void), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, occurs);
				});
		}

		/// <summary>
		/// Asserts the specified method that it is called as expected.
		/// </summary>
		/// <param name="method">Target method</param>
		/// <param name="occurs">Specifies the number of times a call should occur.</param>
		/// <param name="args">Method arguments</param>
		public void Assert(MethodInfo method, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, occurs));
		}

		/// <summary>
		/// Asserts the specified member that it is called as expected.
		/// </summary>
		/// <typeparam name="T">Specify the target type</typeparam>
		/// <typeparam name="TReturn">Specify the return type for the method</typeparam>
		/// <param name="memberName">Name of the member</param>
		/// <param name="occurs">Specifies the number of times a call should occur.</param>
		/// <param name="args">Method arguments</param>
		public void Assert<T, TReturn>(string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(typeof(T), typeof(TReturn), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, occurs);
				});
		}

		/// <summary>
		/// Asserts the specified member that it is called as expected.
		/// </summary>
		/// <typeparam name="T">Specify the target type</typeparam>
		/// <param name="memberName">Name of the member</param>
		/// <param name="args">Method arguments</param>
		public void Assert<T>(string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(typeof(T), typeof(void), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, null);
				});
		}

		/// <summary>
		/// Asserts the specified method that it is called as expected.
		/// </summary>
		/// <param name="method">Target method</param>
		/// <param name="args">Method arguments</param>
		public void Assert(MethodInfo method, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, null));
		}

		/// <summary>
		/// Asserts the specified member that it is called as expected.
		/// </summary>
		/// <typeparam name="T">Specify the target type</typeparam>
		/// <typeparam name="TReturn">Specify the return type for the method</typeparam>
		/// <param name="memberName">Name of the member</param>
		/// <param name="args">Method arguments</param>
		public void Assert<T, TReturn>(string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(typeof(T), typeof(TReturn), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, null);
				});
		}

		/// <summary>
		/// Asserts the specified member that it is called as expected.
		/// </summary>
		/// <param name="targetType">Type of the target</param>
		/// <param name="memberName">Name of the member</param>
		/// <param name="occurs">Specifies the number of times a call should occur.</param>
		/// <param name="args">Method arguments</param>
		public void Assert(Type targetType, string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(targetType, typeof(void), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, occurs);
				});
		}

		/// <summary>
		/// Asserts the specified member that it is called as expected.
		/// </summary>
		/// <typeparam name="TReturn">Sepcify the return type method</typeparam>
		/// <param name="targetType">Type of the target</param>
		/// <param name="memberName">Name of the member</param>
		/// <param name="occurs">Specifies the number of times a call should occur.</param>
		/// <param name="args">Method arguments</param>
		public void Assert<TReturn>(Type targetType, string memberName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(targetType, typeof(TReturn), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, occurs);
				});
		}

		/// <summary>
		/// Asserts the specified member that it is called as expected.
		/// </summary>
		/// <param name="targetType">Type of the target</param>
		/// <param name="memberName">Name of the member</param>
		/// <param name="args">Method arguments</param>
		public void Assert(Type targetType, string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(targetType, typeof(void), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, null);
				});
		}

		/// <summary>
		/// Asserts the specified member that it is called as expected.
		/// </summary>
		/// <typeparam name="TReturn">Sepcify the return type method</typeparam>
		/// <param name="targetType">Type of the target</param>
		/// <param name="memberName">Name of the member</param>
		/// <param name="args">Method arguments</param>
		void INonPublicExpectation.Assert<TReturn>(Type targetType, string memberName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var method = GetMethodByName(targetType, typeof(TReturn), memberName, args);
					MockingContext.CurrentRepository.AssertMethodInfo(null, method, args, null);
				});
		}
#endif

		#region Raise event
		/// <summary>
		/// Raises an event specified using reflection. If the event is declared on a C# or VB class
		/// and has the default implementation for add/remove, then that event can also be raised using this 
		/// method, even with the profiler off.
		/// </summary>
		/// <param name="instance">Instance on which to raise the event.</param>
		/// <param name="eventInfo">The event to raise.</param>
		/// <param name="args">Arguments to pass to the event handlers.</param>
		public void Raise(object instance, EventInfo eventInfo, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() => RaiseEventBehavior.RaiseEventImpl(instance, eventInfo, args));
		}

		/// <summary>
		/// Raises a static event specified using reflection. If the event is declared on a C# or VB class
		/// and has the default implementation for add/remove, then that event can also be raised using this 
		/// method, even with the profiler off.
		/// </summary>
		/// <param name="eventInfo">The event to raise.</param>
		/// <param name="args">Arguments to pass to the event handlers.</param>
		public void Raise(EventInfo eventInfo, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() => RaiseEventBehavior.RaiseEventImpl(null, eventInfo, args));
		}

		/// <summary>
		/// Raises an event by name. If the event is declared on a C# or VB class
		/// and has the default implementation for add/remove, then that event can also be raised using this 
		/// method, even with the profiler off.
		/// </summary>
		/// <param name="instance">Instance on which to raise the event.</param>
		/// <param name="eventName">The name of event to raise.</param>
		/// <param name="args">Arguments to pass to the event handlers.</param>
		public void Raise(object instance, string eventName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				RaiseEventBehavior.RaiseEventImpl(instance, MockingUtil.GetUnproxiedType(instance).GetEvent(eventName,
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), args);
			});
		}

		/// <summary>
		/// Raises a static event by name. If the event is declared on a C# or VB class
		/// and has the default implementation for add/remove, then that event can also be raised using this 
		/// method, even with the profiler off.
		/// </summary>
		/// <param name="eventName">The type on which the event is declared.</param>
		/// <param name="eventName">The name of event to raise.</param>
		/// <param name="args">Arguments to pass to the event handlers.</param>
		public void Raise(Type type, string eventName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				RaiseEventBehavior.RaiseEventImpl(null, type.GetEvent(eventName,
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static), args);
			});
		}
		#endregion

		/// <summary>
		/// Creates an accessor object that can invoke non-public methods and get/set non-public properties and fields.
		/// Equivalent to <code>new PrivateAccessor(instance)</code>.
		/// </summary>
		/// <param name="instance">Instance to which non-public access will be given.</param>
		/// <returns>Non-public accessor.</returns>
		public PrivateAccessor MakePrivateAccessor(object instance)
		{
			return new PrivateAccessor(instance);
		}

		/// <summary>
		/// Creates an accessor object that can invoke static (Shared in Visual Basic) non-public methods and static get/set non-public properties and fields.
		/// Equivalent to <code>PrivateAccessor.ForType(type)</code>.
		/// </summary>
		/// <param name="type">Type whose static members will be given non-public access to.</param>
		/// <returns>Non-public accessor.</returns>
		public PrivateAccessor MakeStaticPrivateAccessor(Type type)
		{
			return PrivateAccessor.ForType(type);
		}
	}
}
