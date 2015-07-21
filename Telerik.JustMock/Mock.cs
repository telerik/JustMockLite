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
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Behaviors;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Expectations;
using Telerik.JustMock.Expectations.Abstraction;

namespace Telerik.JustMock
{
	/// <summary>
	/// Entry point for setting up and asserting mocks.
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
		/// Gets a value indicating whether the JustMock profiler is enabled.
		/// </summary>
		/// <returns></returns>
		public static bool IsProfilerEnabled
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() => ProfilerInterceptor.IsProfilerAttached);
			}
		}

		/// <summary>
		/// Arrange and assert expectations on non-public members.
		/// </summary>
		public static INonPublicExpectation NonPublic
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() => new NonPublicExpectation());
			}
		}

		#region Raise Event methods

		/// <summary>
		/// Raises the specified event. If the event is not mocked and is declared on a C# or VB class
		/// and has the default implementation for add/remove, then that event can also be raised using this 
		/// method, even with the profiler off. The type on which the event is defined may need to be pre-intercepted
		/// using <see cref="Intercept"/> before calling Raise.
		/// </summary>
		/// <param name="eventExpression">Event expression</param>
		/// <param name="args">Delegate arguments</param>
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
		/// Removes all existing arrangements within the current mocking context (e.g. current test method).
		/// Arrangements made in parent mocking contexts (e.g. in fixture setup method) are preserved.
		/// </summary>
		public static void Reset()
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.RetireRepository());
		}

#if !LITE_EDITION
		/// <summary>
		/// Explicitly enables the interception of the given type by the profiler. Interception is usually enabled
		/// implicitly by calls to <see cref="Create"/> or <see cref="Arrange"/>. This method is rarely needed in cases
		/// where you're trying to arrange setters or raise events on a partial mock.
		/// </summary>
		/// <typeparam name="TTypeToIntercept">The type to intercept</typeparam>
		public static void Intercept<TTypeToIntercept>()
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.EnableInterception(typeof(TTypeToIntercept)));
		}

		/// <summary>
		/// Explicitly enables the interception of the given type by the profiler. Interception is usually enabled
		/// implicitly by calls to <see cref="Create"/> or <see cref="Arrange"/>. This method is rarely needed in cases
		/// where you're trying to arrange setters or raise events on a partial mock.
		/// </summary>
		/// <param name="typeToIntercept">The type to intercept</param>
		public static void Intercept(Type typeToIntercept)
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.EnableInterception(typeToIntercept));
		}
#endif
	}
}
