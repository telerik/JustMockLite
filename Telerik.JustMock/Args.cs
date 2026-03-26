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
using Telerik.JustMock.Core;

namespace Telerik.JustMock
{
    /// <summary>
    /// Configures how JustMock matches arguments and instances during verification.
    /// </summary>
    public sealed partial class Args
    {
        /// <summary>
        /// Gets or sets a value that indicates whether verification ignores argument values.
        /// </summary>
        /// <remarks>
        /// Unless explicitly specified, the arguments will be ignored by default if there is a filter present.
        /// </remarks>
        public bool? IsIgnored { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether verification ignores the target instance.
        /// </summary>
        /// <remarks>
        /// Unless explicitly specified, the instance will be ignored by default if there is a filter present
        /// and it takes as a first argument a 'this' argument.
        /// </remarks>
        public bool? IsInstanceIgnored { get; set; }

        /// <summary>
        /// Gets or sets a customized filter on the invocation arguments.
        /// </summary>
        /// <remarks>
        /// The delegate must match the asserted member signature. You can add an optional first parameter of the declaring type
        /// to inspect the instance on which the call occurred.
        /// </remarks>
        public Delegate Filter { get; set; }

        /// <summary>
        /// Creates an <see cref="Args"/> configuration that ignores argument values.
        /// </summary>
        /// <returns>An <see cref="Args"/> configuration for verification.</returns>
        public static Args Ignore()
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return new Args { IsIgnored = true };
            });
        }

        /// <summary>
        /// Creates an <see cref="Args"/> configuration that ignores the target instance.
        /// </summary>
        /// <returns>An <see cref="Args"/> configuration for verification.</returns>
        public static Args IgnoreInstance()
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return new Args { IsInstanceIgnored = true };
            });
        }

        /// <summary>
        /// Configures the current <see cref="Args"/> instance to ignore argument values.
        /// </summary>
        /// <returns>The current <see cref="Args"/> instance.</returns>
        public Args AndIgnoreArguments()
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                this.IsIgnored = true;
                return this;
            });
        }

        /// <summary>
        /// Configures the current <see cref="Args"/> instance to ignore the target instance.
        /// </summary>
        /// <returns>The current <see cref="Args"/> instance.</returns>
        public Args AndIgnoreInstance()
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                this.IsInstanceIgnored = true;
                return this;
            });
        }

        /// <summary>
        /// Creates an <see cref="Args"/> configuration that uses a custom predicate to match calls.
        /// </summary>
        /// <param name="predicate">Predicate that evaluates the invocation arguments.</param>
        /// <returns>An <see cref="Args"/> configuration for verification.</returns>
        public static Args Matching(Delegate predicate)
        {
            return ProfilerInterceptor.GuardInternal(() => new Args().AndMatching(predicate));
        }

        /// <summary>
        /// Configures the current <see cref="Args"/> instance to use a custom predicate to match calls.
        /// </summary>
        /// <param name="predicate">Predicate that evaluates the invocation arguments.</param>
        /// <returns>The current <see cref="Args"/> instance.</returns>
        public Args AndMatching(Delegate predicate)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                this.Filter = predicate;
                return this;
            });
        }

        internal static Args NotSpecified()
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return new Args();
            });
        }
    }
}
