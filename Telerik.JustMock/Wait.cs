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
        /// Specifies the number of seconds to wait for executing an event.
        /// </summary>
        /// <param name="seconds">Seconds to wait</param>
        /// <returns>IWaitDuration type</returns>
        public static IWaitDuration For(int seconds)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return For(TimeSpan.FromSeconds(seconds));
            });
        }

        /// <summary>
        /// Specifies the number of seconds to wait for executing an event.
        /// </summary>
        /// <param name="duration">Time duration to wait</param>
        /// <returns>IWaitDuration type</returns>
        public static IWaitDuration For(TimeSpan duration)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return new EventWaitDuration((int)duration.TotalMilliseconds);
            });
        }
    }
}
