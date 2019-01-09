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
using System.Linq;
using System.Linq.Expressions;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Behaviors;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Core.Expressions;
using Telerik.JustMock.Expectations;

namespace Telerik.JustMock.Helpers
{
	/// <summary>
	/// Contains fluent extensions for setting up or asserting mock expectations.
	/// </summary>
	public static class FluentHelper
	{
		private static TContainer DoArrange<TContainer>(object obj, Type objType, LambdaExpression expression, Func<TContainer> containerFactory) where TContainer : IMethodMock
		{
			var repo = MockingContext.CurrentRepository;
			var instanceParam = expression.Parameters[0];
			var instanceConstant = Expression.Constant(obj);
			var parameterlessBody = ExpressionReplacer.Replace(expression.Body, instanceParam, instanceConstant);
			var parameterlessArrangeStmt = Expression.Lambda(parameterlessBody);
			return repo.Arrange(parameterlessArrangeStmt, containerFactory);
		}

		private static void DoAssert(string message, object obj, Type objType, LambdaExpression expression, Args args, Occurs occurs)
		{
			var repo = MockingContext.CurrentRepository;
			Expression parameterlessArrangeStmt = null;
			if (expression != null)
			{
				var instanceParam = expression.Parameters[0];
				var instanceConstant = Expression.Constant(obj);
				var parameterlessBody = ExpressionReplacer.Replace(expression.Body, instanceParam, instanceConstant);
				parameterlessArrangeStmt = Expression.Lambda(parameterlessBody);
			}
			repo.Assert(message, obj, parameterlessArrangeStmt, args, occurs);
		}

		/// <summary>
		/// Setups the target call to act in a specific way.
		/// </summary>
		/// <typeparam name="T">Mock type</typeparam>
		/// <typeparam name="TResult">Return type for the target setup.</typeparam>
		/// <param name="obj">Target instance.</param>
		/// <param name="expression">Target expression.</param>
		/// <returns>
		/// Reference to <see cref="FuncExpectation{TResult}" /> to setup the mock.
		/// </returns>
		public static FuncExpectation<TResult> Arrange<T, TResult>(this T obj, Expression<Func<T, TResult>> expression)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return DoArrange(obj, typeof(T), expression, () => new FuncExpectation<TResult>());
			});
		}

		/// <summary>
		/// Setups target property set operation to act in a specific way.
		/// <example>
		/// <code>
		/// Mock.ArrangeSet(() =&gt;; foo.MyValue = 10).Throws(new InvalidOperationException());
		/// </code>
		/// This will throw InvalidOperationException for when foo.MyValue is set with 10.
		/// </example>
		/// </summary>
		/// <typeparam name="T">Mock type.</typeparam>
		/// <param name="obj">Target mock object.</param>
		/// <param name="action">Target action.</param>
		/// <returns>Reference to <see cref="ActionExpectation" /> to setup the mock.</returns>
		public static ActionExpectation ArrangeSet<T>(this T obj, Action<T> action)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Arrange(() => action(obj), () => new ActionExpectation()));
		}

		/// <summary>
		/// Setups the target call to act in a specific way.
		/// </summary>
		/// <typeparam name="T">Return type for the target setup.</typeparam>
		/// <param name="obj">Target instance.</param>
		/// <param name="expression">Target expression.</param>
		/// <returns>Reference to <see cref="ActionExpectation" /> to setup the mock.</returns>
		public static ActionExpectation Arrange<T>(this T obj, Expression<Action<T>> expression)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return DoArrange(obj, typeof(T), expression, () => new ActionExpectation());
			});
		}

		/// <summary>
		/// Arranges the return values of properties and methods according to the given functional specification.
		/// </summary>
		/// <typeparam name="T">Type of the mock.</typeparam>
		/// <param name="mock">The mock on which to make the arrangements.</param>
		/// <param name="functionalSpecification">The functional specification to apply to this mock.</param>
		/// <remarks>
		/// See article "Create Mocks By Example" for further information on how to write functional specifications.
		/// </remarks>
		public static void ArrangeLike<T>(this T mock, Expression<Func<T, bool>> functionalSpecification)
		{
			ProfilerInterceptor.GuardInternal(() => FunctionalSpecParser.ApplyFunctionalSpec(mock, functionalSpecification, ReturnArranger.Instance));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="T">Type of the mock.</typeparam>
		/// <typeparam name="TReturn">Return type for the target setup.</typeparam>
		/// <param name="obj">Target object.</param>
		/// <param name="action">Target action.</param>
		public static void Assert<T, TReturn>(this T obj, Expression<Func<T, TReturn>> action, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				obj.Assert(action, null, message);
			});
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="T">Type of the mock.</typeparam>
		/// <typeparam name="TReturn">Return type for the target setup.</typeparam>
		/// <param name="obj">Target object.</param>
		/// <param name="expression">Target expression</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public static void Assert<T, TReturn>(this T obj, Expression<Func<T, TReturn>> expression, Occurs occurs, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				DoAssert(message, obj, typeof(T), expression, null, occurs);
			});
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="T">Type of the mock.</typeparam>
		/// <param name="obj">Target object.</param>
		/// <param name="action">Target action.</param>
		public static void Assert<T>(this T obj, Expression<Action<T>> action, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				DoAssert(message, obj, typeof(T), action, null, null);
			});
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="T">Type of the mock.</typeparam>
		/// <param name="obj">Target mock object</param>
		/// <param name="expression">Target expression</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public static void Assert<T>(this T obj, Expression<Action<T>> expression, Occurs occurs, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				DoAssert(message, obj, typeof(T), expression, null, occurs);
			});
		}

		/// <summary>
		/// Raises the specified event. If the event is not mocked and is declared on a C# or VB class
		/// and has the default implementation for add/remove, then that event can also be raised using this
		/// method, even with the profiler off.
		/// </summary>
		/// <typeparam name="T">Type of the object.</typeparam>
		/// <param name="obj">Target instance.</param>
		/// <param name="eventExpression">Event expression.</param>
		/// <param name="args">EventArgs argument.</param>
		/// <remarks>
		/// Use this method for raising events based on <see cref="System.EventHandler" />. The instance given
		/// in the event expression is used as an argument for the 'sender' parameter.
		/// </remarks>
		public static void Raise<T>(this T obj, Action<T> eventExpression, EventArgs args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				var evt = repo.ParseAddHandlerAction(obj, eventExpression);
				RaiseEventBehavior.RaiseEventImpl(obj, evt, new object[] { obj, args });
			});
		}

		/// <summary>
		/// Raises the specified event. If the event is not mocked and is declared on a C# or VB class
		/// and has the default implementation for add/remove, then that event can also be raised using this
		/// method, even with the profiler off.
		/// </summary>
		/// <typeparam name="T">Type of the object.</typeparam>
		/// <param name="obj">Target instance.</param>
		/// <param name="eventExpression">Event expression.</param>
		/// <param name="args">Delegate arguments.</param>
		public static void Raise<T>(this T obj, Action<T> eventExpression, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				var evt = repo.ParseAddHandlerAction(obj, eventExpression);
				RaiseEventBehavior.RaiseEventImpl(obj, evt, args);
			});
		}

		/// <summary>
		/// Asserts all expected calls that are marked as must or
		/// to be occurred a certain number of times.
		/// </summary>
		/// <typeparam name="T">Type of the mock.</typeparam>
		/// <param name="target">Target instance.</param>
		public static void Assert<T>(this T target, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Assert(message, target));
		}

		/// <summary>
		/// Asserts all expected setups.
		/// </summary>
		/// <typeparam name="T">Type of the mock.</typeparam>
		/// <param name="target">Target instance.</param>
		public static void AssertAll<T>(this T target, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.AssertAll(message, target));
		}
	}
}
