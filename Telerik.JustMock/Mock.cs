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
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Behaviors;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Expectations;
using Telerik.JustMock.Expectations.Abstraction;

#if !PORTABLE
using Telerik.JustMock.Expectations.Abstraction.Local;
#endif

namespace Telerik.JustMock
{
    /// <summary>
    /// Provides the main entry point for creating, arranging, and verifying mocks.
    /// </summary>
    public partial class Mock
    {
        static Mock()
        {
#if SILVERLIGHT && !PORTABLE
            if (-1 == typeof(object).Assembly.FullName.IndexOf("PublicKeyToken=7cec85d7bea7798e", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Telerik.JustMock.Silverlight should only be used inside the Silverlight runtime. For all other runtimes reference Telerik.JustMock instead.");
            }
#endif
        }

        /// <summary>
        /// Gets a value that indicates whether the JustMock profiler is attached.
        /// </summary>
        /// <returns><see langword="true"/> if the profiler is attached; otherwise - <see langword="false"/>.</returns>
        public static bool IsProfilerEnabled
        {
            get
            {
                return ProfilerInterceptor.GuardInternal(() => ProfilerInterceptor.IsProfilerAttached);
            }
        }

#if !PORTABLE
        /// <summary>
        /// Gets a value that indicates whether On Demand optimization is enabled.
        /// </summary>
        /// <returns><see langword="true"/> if On Demand optimization is enabled; otherwise - <see langword="false"/>.</returns>
        public static bool IsOnDemandEnabled
        {
            get
            {
                return ProfilerInterceptor.GuardInternal(() => ProfilerInterceptor.IsReJitEnabled);
            }
        }

        /// <summary>
        /// Gets helpers that let you arrange and verify non-public members.
        /// </summary>
        public static INonPublicExpectation NonPublic
        {
            get
            {
                return ProfilerInterceptor.GuardInternal(() => new NonPublicExpectation());
            }
        }

        /// <summary>
        /// Gets helpers that let you arrange and verify language features such as C# local functions.
        /// </summary>
        public static ILocalExpectation Local
        {
            get
            {
                return ProfilerInterceptor.GuardInternal(() => new LocalExpectation());
            }
        }
#endif

#region Raise Event methods

        /// <summary>
        /// Raises the specified event. If the event is not mocked and is declared on a C# or VB class
        /// and has the default implementation for add/remove, then that event can also be raised using this 
        /// method, even with the profiler off. The type on which the event is defined may need to be pre-intercepted
        /// using <see cref="Intercept"/> before calling Raise.
        /// </summary>
        /// <param name="eventExpression">Expression that identifies the event to raise.</param>
        /// <param name="args">Arguments to pass to the event handler delegate.</param>
        public static void Raise(Action eventExpression, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                object instance;
                var evt = MockingContext.CurrentRepository.ParseAddHandlerAction(eventExpression, out instance);
                RaiseEventBehavior.RaiseEventImpl(instance, evt, args);
            });
        }

#endregion

        /// <summary>
        /// Removes all arrangements from the current mocking context.
        /// </summary>
        /// <remarks>
        /// Use this method to clear arrangements created in the current test scope without affecting arrangements
        /// inherited from parent scopes such as fixture setup methods.
        /// </remarks>
        public static void Reset()
        {
            ProfilerInterceptor.GuardInternal(() => MockingContext.RetireRepository());
        }

        /// <summary>
        /// Removes all arrangements and clears all recorded invocations for the specified mock instance.
        /// The mock object itself remains usable; new arrangements may be added after this call.
        /// </summary>
        /// <param name="instance">The mock object to reset.</param>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is not a mock object created by JustMock.</exception>
        public static void Reset(object instance)
        {
            ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.ResetInstance(instance));
        }

        /// <summary>
        /// Clears the recorded invocations for all arrangements on the specified mock instance,
        /// without removing or modifying the arrangements themselves.
        /// Subsequent calls to <see cref="Mock.Assert"/> will count only invocations that occur after this call.
        /// </summary>
        /// <param name="instance">The mock object whose invocation history should be cleared.</param>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is not a mock object created by JustMock.</exception>
        public static void ClearInvocations(object instance)
        {
            ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.ClearInvocations(instance));
        }

#if !LITE_EDITION
        /// <summary>
        /// Explicitly enables the interception of the given type by the profiler. Interception is usually enabled
        /// implicitly by calls to <see cref="Mock.Create(Type)"/> or <see cref="Mock.Arrange"/>. This method is rarely needed in cases
        /// where you're trying to arrange setters or raise events on a partial mock.
        /// </summary>
        /// <typeparam name="TTypeToIntercept">The type to intercept</typeparam>
        public static void Intercept<TTypeToIntercept>()
        {
            ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.EnableInterception(typeof(TTypeToIntercept)));
        }

        /// <summary>
        /// Enables profiler interception for the specified type.
        /// </summary>
        /// <remarks>
        /// JustMock usually enables interception for you when you call <see cref="Create(Type)"/> or one of the
        /// <see cref="Arrange(System.Linq.Expressions.Expression{System.Action})"/> overloads. Call this method only when you need
        /// interception before the first arrangement, for example when you raise events or arrange setters on a partial mock.
        /// </remarks>
        /// <param name="typeToIntercept">Type to intercept.</param>
        public static void Intercept(Type typeToIntercept)
        {
            ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.EnableInterception(typeToIntercept));
        }

        /// <summary>
        /// Disables profiler interception for the specified type.
        /// </summary>
        /// <typeparam name="TTypeToIntercept">Type to intercept.</typeparam>
        public static void NotIntercept<TTypeToIntercept>()
        {
            ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.DisableInterception(typeof(TTypeToIntercept)));
        }

        /// <summary>
        /// Disables profiler interception for the specified type.
        /// </summary>
        /// <param name="typeToIntercept">Type to stop intercepting.</param>
        public static void NotIntercept(Type typeToIntercept)
        {
            ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.DisableInterception(typeToIntercept));
        }
#endif
    }
}
