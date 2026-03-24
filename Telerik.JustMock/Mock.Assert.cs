/*
 JustMock Lite
 Copyright © 2010-2015,2018,2021 Progress Software Corporation

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
        /// <param name="expression">Expression that identifies the call to verify.</param>
        /// <param name="message">Custom failure message.</param>
        /// <typeparam name="TReturn">Return type of the verified member.</typeparam>
        public static void Assert<TReturn>(Expression<Func<TReturn>> expression, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.Assert(message, null, expression);
            });
        }

        /// <summary>
        /// Asserts that the specified return-value call occurs by using custom argument matching rules.
        /// </summary>
        /// <param name="expression">Expression that identifies the call to verify.</param>
        /// <param name="args">Argument matching rules for the verification.</param>
        /// <param name="message">Custom failure message.</param>
        /// <typeparam name="TReturn">Return type of the verified member.</typeparam>
        public static void Assert<TReturn>(Expression<Func<TReturn>> expression, Args args, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.Assert(message, null, expression, args, null);
            });
        }


        /// <summary>
        /// Asserts that the specified return-value call satisfies the supplied occurrence rule.
        /// </summary>
        /// <param name="expression">Expression that identifies the call to verify.</param>
        /// <param name="occurs">Expected call-count rule.</param>
        /// <param name="message">Custom failure message.</param>
        /// <typeparam name="TReturn">Return type of the verified member.</typeparam>
        public static void Assert<TReturn>(Expression<Func<TReturn>> expression, Occurs occurs, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.Assert(message, null, expression, null, occurs);
            });
        }

        /// <summary>
        /// Asserts that the specified return-value call matches the supplied argument rules and occurrence rule.
        /// </summary>
        /// <param name="expression">Expression that identifies the call to verify.</param>
        /// <param name="args">Argument matching rules for the verification.</param>
        /// <param name="occurs">Expected call-count rule.</param>
        /// <param name="message">Custom failure message.</param>
        /// <typeparam name="TReturn">Return type of the verified member.</typeparam>
        public static void Assert<TReturn>(Expression<Func<TReturn>> expression, Args args, Occurs occurs, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.Assert(message, null, expression, args, occurs);
            });
        }

        /// <summary>
        /// Asserts that the specified void call occurs.
        /// </summary>
        /// <param name="expression">Expression that identifies the call to verify.</param>
        /// <param name="message">Custom failure message.</param>
        public static void Assert(Expression<Action> expression, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.Assert(message, null, expression);
            });
        }

        /// <summary>
        /// Asserts that the specified void call occurs by using custom argument matching rules.
        /// </summary>
        /// <param name="expression">Expression that identifies the call to verify.</param>
        /// <param name="args">Argument matching rules for the verification.</param>
        /// <param name="message">Custom failure message.</param>
        public static void Assert(Expression<Action> expression, Args args, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.Assert(message, null, expression, args, null);
            });
        }

        /// <summary>
        /// Asserts that the specified void call satisfies the supplied occurrence rule.
        /// </summary>
        /// <param name="expression">Expression that identifies the call to verify.</param>
        /// <param name="occurs">Specifies the number of times a mock call should occur.</param>
        /// <param name="message">Custom failure message.</param>
        public static void Assert(Expression<Action> expression, Occurs occurs, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.Assert(message, null, expression, null, occurs);
            });
        }

        /// <summary>
        /// Asserts that the specified void call matches the supplied argument rules and occurrence rule.
        /// </summary>
        /// <param name="expression">Expression that identifies the call to verify.</param>
        /// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
        /// <param name="occurs">Specifies the number of times a mock call should occur.</param>
        /// <param name="message">Custom failure message.</param>
        public static void Assert(Expression<Action> expression, Args args, Occurs occurs, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.Assert(message, null, expression, args, occurs);
            });
        }

        /// <summary>
        /// Asserts that the specified return-value call occurs on the supplied instance.
        /// </summary>
        /// <param name="target">Instance on which the call should occur.</param>
        /// <param name="func">Delegate that performs the call to verify.</param>
        /// <param name="message">Custom failure message.</param>
        /// <typeparam name="T">Type of the target instance.</typeparam>
        /// <typeparam name="TResult">Return type of the verified member.</typeparam>
        public static void Assert<T, TResult>(T target, Func<T, TResult> func, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.AssertAction(message, () => func(target));
            });
        }

        /// <summary>
        /// Asserts that the specified return-value call occurs on the supplied instance and satisfies the occurrence rule.
        /// </summary>
        /// <param name="target">Instance on which the call should occur.</param>
        /// <param name="func">Delegate that performs the call to verify.</param>
        /// <param name="occurs">Specifies how many times a call has occurred</param>
        /// <param name="message">Custom failure message.</param>
        /// <typeparam name="T">Type of the target instance.</typeparam>
        /// <typeparam name="TResult">Return type of the verified member.</typeparam>
        public static void Assert<T, TResult>(T target, Func<T, TResult> func, Occurs occurs, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.AssertAction(message, () => func(target), null, occurs);
            });
        }

        /// <summary>
        /// Asserts that the specified property setter or event subscription occurs.
        /// </summary>
        /// <param name="action">Action that identifies the setter or event subscription to verify.</param>
        /// <param name="message">Custom failure message.</param>
        public static void AssertSet(Action action, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.AssertSetAction(message, action);
            });
        }

        /// <summary>
        /// Asserts that the specified property setter or event subscription satisfies the supplied occurrence rule.
        /// </summary>
        /// <param name="action">Action that identifies the setter or event subscription to verify.</param>
        /// <param name="occurs">Specifies the number of times a mock call should occur.</param>
        /// <param name="message">Custom failure message.</param>
        public static void AssertSet(Action action, Occurs occurs, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.AssertSetAction(message, action, null, occurs);
            });
        }

        /// <summary>
        /// Asserts that the specified property setter or event subscription occurs by using custom argument matching rules.
        /// </summary>
        /// <param name="action">Action that identifies the setter or event subscription to verify.</param>
        /// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
        /// <param name="message">Custom failure message.</param>
        public static void AssertSet(Action action, Args args, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.AssertSetAction(message, action, args, null);
            });
        }

        /// <summary>
        /// Asserts that the specified property setter or event subscription matches the supplied argument rules and occurrence rule.
        /// </summary>
        /// <param name="action">Action that identifies the setter or event subscription to verify.</param>
        /// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
        /// <param name="occurs">Specifies the number of times a mock call should occur.</param>
        /// <param name="message">Custom failure message.</param>
        public static void AssertSet(Action action, Args args, Occurs occurs, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.AssertSetAction(message, action, args, occurs);
            });
        }

        /// <summary>
        /// Asserts all required arrangements for the specified mock instance.
        /// </summary>
        /// <typeparam name="T">Type of the mock instance.</typeparam>
        /// <param name="mocked">Mock instance to verify.</param>
        /// <param name="message">Custom failure message.</param>
        public static void Assert<T>(T mocked, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.Assert(message, mocked));
        }

        /// <summary>
        /// Asserts all arrangements for the specified mock instance.
        /// </summary>
        /// <typeparam name="T">Type of the mock instance.</typeparam>
        /// <param name="mocked">Mock instance to verify.</param>
        /// <param name="message">Custom failure message.</param>
        public static void AssertAll<T>(T mocked, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.AssertAll(message, mocked));
        }

        /// <summary>
        /// Asserts all arrangements declared on the specified type.
        /// </summary>
        /// <param name="type">Type that declares the arranged members to verify.</param>
        /// <param name="message">Custom failure message.</param>
        public static void Assert(Type type, string message = null)
        {
            ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.AssertIgnoreInstance(message, type, ignoreMethodMockOccurrences: false));
        }

        /// <summary>
        /// Asserts all arrangements declared on the specified type.
        /// </summary>
        /// <typeparam name="T">Type that declares the arranged members to verify.</typeparam>
        /// <param name="message">Custom failure message.</param>
        public static void Assert<T>(string message = null)
        {
            ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.AssertIgnoreInstance(message, typeof(T), ignoreMethodMockOccurrences: false));
        }

        /// <summary>
        /// Asserts all arrangements in the current mocking context.
        /// </summary>
        /// <param name="message">Custom failure message.</param>
        public static void AssertAll(string message = null)
        {
            ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.AssertAll(message));
        }

        /// <summary>
        /// Returns the number of times the specified member was called.
        /// </summary>
        /// <param name="expression">Expression that identifies the call to inspect.</param>
        /// <returns>The number of matching calls.</returns>
        public static int GetTimesCalled<TReturn>(Expression<Func<TReturn>> expression)
        {
            return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalled(expression, null));
        }

        /// <summary>
        /// Returns the number of times the specified member was called.
        /// </summary>
        /// <param name="expression">Expression that identifies the call to inspect.</param>
        /// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
        /// <returns>The number of matching calls.</returns>
        public static int GetTimesCalled<TReturn>(Expression<Func<TReturn>> expression, Args args)
        {
            return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalled(expression, args));
        }

        /// <summary>
        /// Returns the number of times the specified member was called.
        /// </summary>
        /// <param name="expression">Expression that identifies the call to inspect.</param>
        /// <returns>The number of matching calls.</returns>
        public static int GetTimesCalled(Expression<Action> expression)
        {
            return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalled(expression, null));
        }

        /// <summary>
        /// Returns the number of times the specified member was called.
        /// </summary>
        /// <param name="expression">Expression that identifies the call to inspect.</param>
        /// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
        /// <returns>The number of matching calls.</returns>
        public static int GetTimesCalled(Expression<Action> expression, Args args)
        {
            return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalled(expression, args));
        }

        /// <summary>
        /// Returns the number of times the specified setter or event subscription method was called.
        /// </summary>
        /// <param name="action">Action that identifies the setter or event subscription to inspect.</param>
        /// <returns>The number of matching calls.</returns>
        public static int GetTimesSetCalled(Action action)
        {
            return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalledFromAction(action, null));
        }

        /// <summary>
        /// Returns the number of times the specified setter or event subscription method was called.
        /// </summary>
        /// <param name="action">Action that identifies the setter or event subscription to inspect.</param>
        /// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
        /// <returns>The number of matching calls.</returns>
        public static int GetTimesSetCalled(Action action, Args args)
        {
            return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalledFromAction(action, args));
        }
    }
}
