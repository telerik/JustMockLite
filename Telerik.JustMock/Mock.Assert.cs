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
using System.Linq.Expressions;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Context;

namespace Telerik.JustMock
{
	public partial class Mock
	{
		/// <summary>
		/// Asserts a specific call from expression.
		/// </summary>
		/// <param name="expression">Target expression</param>
		/// <typeparam name="TReturn">Return type for the assert expression</typeparam>
		public static void Assert<TReturn>(Expression<Func<TReturn>> expression)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression);
			});
		}

		/// <summary>
		/// Asserts a specific call from expression.
		/// </summary>
		/// <param name="expression">Target expression</param>
		/// <typeparam name="TReturn">Return type for the assert expression</typeparam>
		/// <param name="args">Assert argument</param>
		public static void Assert<TReturn>(Expression<Func<TReturn>> expression, Args args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression, args, null);
			});
		}


		/// <summary>
		/// Asserts a specific call from expression.
		/// </summary>
		/// <param name="expression">Target expression</param>
		/// <param name="occurs">Specifies how many times a call has occurred</param>
		/// <typeparam name="TReturn">Return type for the target call</typeparam>
		public static void Assert<TReturn>(Expression<Func<TReturn>> expression, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression, null, occurs);
			});
		}

		/// <summary>
		/// Asserts the specified call from expression.
		/// </summary>
		/// <param name="expression">The action to verify.</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		/// <typeparam name="TReturn">Return type for the target call</typeparam>
		public static void Assert<TReturn>(Expression<Func<TReturn>> expression, Args args, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression, args, occurs);
			});
		}

#if !VISUALBASIC
		/// <summary>
		/// Asserts a specific call from expression.
		/// </summary>
		/// <param name="expression">Action expression defining the action to verify.</param>
		public static void Assert(Expression<Action> expression)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression);
			});
		}
#endif

		/// <summary>
		/// Asserts the specified call from expression.
		/// </summary>
		/// <param name="expression">The action to verify.</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		public static void Assert(Expression<Action> expression, Args args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression, args, null);
			});
		}

		/// <summary>
		/// Asserts the specified call from expression.
		/// </summary>
		/// <param name="expression">The action to verify.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public static void Assert(Expression<Action> expression, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression, null, occurs);
			});
		}

		/// <summary>
		/// Asserts the specified call from expression.
		/// </summary>
		/// <param name="expression">The action to verify.</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public static void Assert(Expression<Action> expression, Args args, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression, args, occurs);
			});
		}

		/// <summary>
		/// Asserts a specific call from expression.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="func">Contains the target mock call</param>
		/// <typeparam name="T">Target type</typeparam>
		/// <typeparam name="TResult">The type of the return value of the method</typeparam>
		public static void Assert<T, TResult>(T target, Func<T, TResult> func)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AssertAction(() => func(target));
			});
		}

		/// <summary>
		/// Asserts a specific call from expression.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="occurs">Specifies how many times a call has occurred</param>
		/// <param name="func">Contains the target mock call</param>
		/// <typeparam name="T">Target type</typeparam>
		/// <typeparam name="TResult">The type of the return value of the method</typeparam>
		public static void Assert<T, TResult>(T target, Func<T, TResult> func, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AssertAction(() => func(target), null, occurs);
			});
		}

		/// <summary>
		/// Asserts the specific property set operation.
		/// </summary>
		/// <param name="action">Action defining the set operation</param>
		public static void AssertSet(Action action)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AssertAction(action);
			});
		}

		/// <summary>
		/// Asserts the specific property set operation.
		/// </summary>
		/// <param name="action">Action defining the set operation</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public static void AssertSet(Action action, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AssertAction(action, null, occurs);
			});
		}

		/// <summary>
		/// Asserts the specific property set operation.
		/// </summary>
		/// <param name="action">Action defining the set operation</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public static void AssertSet(Action action, Args args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AssertAction(action, args, null);
			});
		}

		/// <summary>
		/// Asserts the specific property set operation.
		/// </summary>
		/// <param name="action">Action defining the set operation</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public static void AssertSet(Action action, Args args, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AssertAction(action, args, occurs);
			});
		}

		/// <summary>
		/// Asserts all expected calls that are marked as must or
		/// to be occurred a certain number of times.
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <param name="mocked">Target instance</param>
		public static void Assert<T>(T mocked)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(mocked);
			});
		}

		/// <summary>
		/// Asserts all expected setups.
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <param name="mocked">Target instance</param>
		public static void AssertAll<T>(T mocked)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AssertAll(mocked);
			});
		}

		/// <summary>
		/// Returns the number of times the specified member was called.
		/// </summary>
		/// <param name="expression">The action to inspect</param>
		/// <returns>Number of calls</returns>
		public static int GetTimesCalled<TReturn>(Expression<Func<TReturn>> expression)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalled(expression, null));
		}

		/// <summary>
		/// Returns the number of times the specified member was called.
		/// </summary>
		/// <param name="expression">The action to inspect</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		/// <returns>Number of calls</returns>
		public static int GetTimesCalled<TReturn>(Expression<Func<TReturn>> expression, Args args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalled(expression, args));
		}

		/// <summary>
		/// Returns the number of times the specified member was called.
		/// </summary>
		/// <param name="expression">The action to inspect</param>
		/// <returns>Number of calls</returns>
		public static int GetTimesCalled(Expression<Action> expression)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalled(expression, null));
		}

		/// <summary>
		/// Returns the number of times the specified member was called.
		/// </summary>
		/// <param name="expression">The action to inspect</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		/// <returns>Number of calls</returns>
		public static int GetTimesCalled(Expression<Action> expression, Args args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalled(expression, args));
		}

		/// <summary>
		/// Returns the number of times the specified setter or event subscription method was called.
		/// </summary>
		/// <param name="action">The setter or event subscription method to inspect</param>
		/// <returns>Number of calls</returns>
		public static int GetTimesSetCalled(Action action)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalledFromAction(action, null));
		}

		/// <summary>
		/// Returns the number of times the specified setter or event subscription method was called.
		/// </summary>
		/// <param name="action">The setter or event subscription method to inspect</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		/// <returns>Number of calls</returns>
		public static int GetTimesSetCalled(Action action, Args args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalledFromAction(action, args));
		}
	}
}
