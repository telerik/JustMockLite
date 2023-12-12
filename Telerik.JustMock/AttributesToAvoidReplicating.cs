/*
 JustMock Lite
 Copyright © 2023 Progress Software Corporation

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

namespace Telerik.JustMock
{
    /// <summary>
    /// A list of attributes that must not be replicated when building a proxy. JustMock
    /// tries to copy all attributes from the types and methods being proxied, but that is
    /// not always a good idea for every type of attribute. Add additional attributes
    /// to this list that prevent the proxy from working correctly.
    /// </summary>
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class AttributesToAvoidReplicating
    {
        public static void Add(Type attribute)
        {
            Core.ProfilerInterceptor.GuardInternal(() => Core.Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add(attribute));
        }

        public static void Add<T>()
        {
            Core.ProfilerInterceptor.GuardInternal(() => Core.Castle.DynamicProxy.Generators.AttributesToAvoidReplicating.Add<T>());
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
