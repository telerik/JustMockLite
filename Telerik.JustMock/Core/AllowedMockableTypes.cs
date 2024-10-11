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
using System.Collections.Generic;
using Telerik.JustMock.Core;

namespace Telerik.JustMock.Setup
{
    /// <summary>
    /// Contains a list of types that are exempt from sanity checks when mocking.
    /// </summary>
    /// <remarks>
    /// It might not be safe to mock some types, but sometimes other types might be safe but come out as false positives in the sanity checks.
    /// Add these types to the list to try to mock them anyway. Mind that mocking certain types will not be possible, even if
    /// they're added to this list. Also mind that 
    /// </remarks>
    public static class AllowedMockableTypes
    {
        /// <summary>
        /// The collection of types that are exempt from sanity checks when mocking.
        /// </summary>
        public static readonly ICollection<Type> List = new HashSet<Type>();

        /// <summary>
        /// Adds a type to the list of exemptions. You can also add a type by calling <code>AllowedMockableTypes.List.Add(typeof(T)).</code>
        /// if you can't or don't want to use this helper method.
        /// </summary>
        /// <typeparam name="T">The type to add.</typeparam>
        public static void Add<T>()
        {
            ProfilerInterceptor.GuardInternal(() => List.Add(typeof(T)));
        }
    }
}
