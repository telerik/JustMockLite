/*
 JustMock Lite
 Copyright © 2010-2023 Progress Software Corporation

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
using System.Linq;

namespace Telerik.JustMock.Core.StaticProxy
{
    /// <summary>
    /// An implementation detail. Not intended for external usage.
    /// </summary>
    public static class ProxySourceRegistry
    {
        internal struct ProxyKey : IEquatable<ProxyKey>
        {
            public readonly RuntimeTypeHandle Type;
            public readonly RuntimeTypeHandle[] AdditionalImplementedTypes;

            public ProxyKey(RuntimeTypeHandle type, RuntimeTypeHandle[] additionalImplementedTypes)
            {
                this.Type = type;
                this.AdditionalImplementedTypes = additionalImplementedTypes;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = Type.GetHashCode();

                    if (AdditionalImplementedTypes != null)
                    {
                        foreach (var h in AdditionalImplementedTypes)
                            hash = 37 * hash + h.GetHashCode();
                    }

                    return hash;
                }
            }

            public override bool Equals(object obj)
            {
                if (!(obj is ProxyKey))
                    return false;
                return this.Equals((ProxyKey)obj);
            }

            public bool Equals(ProxyKey other)
            {
                return Type == other.Type
                    && (AdditionalImplementedTypes == null && other.AdditionalImplementedTypes == null
                    || AdditionalImplementedTypes.SequenceEqual(other.AdditionalImplementedTypes));
            }
        }

        /// <summary>
        /// Set by generated code. When 'true' the "mock type not found" exception contains additional information about the trial.
        /// </summary>
        public static bool IsTrialWeaver;

        internal static readonly Dictionary<ProxyKey, RuntimeTypeHandle> ProxyTypes = new Dictionary<ProxyKey, RuntimeTypeHandle>();

        internal static readonly Dictionary<RuntimeTypeHandle, RuntimeTypeHandle> DelegateBackendTypes = new Dictionary<RuntimeTypeHandle, RuntimeTypeHandle>();

        /// <summary>
        /// Implementation detail.
        /// </summary>
        /// <param name="proxyTypeHandle"></param>
        /// <param name="proxiedTypeHandle"></param>
        /// <param name="additionalImplementedTypes"></param>
        public static void Register(RuntimeTypeHandle proxyTypeHandle, RuntimeTypeHandle proxiedTypeHandle, RuntimeTypeHandle[] additionalImplementedTypes)
        {
            // duplicates may come from different test assemblies
            ProxyTypes[new ProxyKey(proxiedTypeHandle, additionalImplementedTypes)] = proxyTypeHandle;
        }

        /// <summary>
        /// Implementation detail.
        /// </summary>
        /// <param name="delegateType"></param>
        /// <param name="backendType"></param>
        public static void RegisterDelegateBackend(RuntimeTypeHandle delegateType, RuntimeTypeHandle backendType)
        {
            DelegateBackendTypes[delegateType] = backendType;
        }
    }
}
