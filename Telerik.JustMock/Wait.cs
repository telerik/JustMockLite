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
using Telerik.JustMock.Expectations;
using Telerik.JustMock.Expectations.Abstraction;

namespace Telerik.JustMock
{
    /// <summary>
    /// Specifies the duration to wait before executing an event.
    /// </summary>
    public static class Wait
    {
        /// <summary>
        /// Creates a wait duration from a number of seconds.
        /// </summary>
        /// <param name="seconds">Number of seconds to wait before the event is raised.</param>
        /// <returns>An object that represents the wait duration.</returns>
        public static IWaitDuration For(int seconds)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return For(TimeSpan.FromSeconds(seconds));
            });
        }

        /// <summary>
        /// Creates a wait duration from a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="duration">Time to wait before the event is raised.</param>
        /// <returns>An object that represents the wait duration.</returns>
        public static IWaitDuration For(TimeSpan duration)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return new EventWaitDuration((int)duration.TotalMilliseconds);
            });
        }
    }
}
