// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Telerik.JustMock.Core.Castle.DynamicProxy.Contributors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators;

    internal class ClassMembersCollector : MembersCollector
    {
        private readonly Dictionary<Tuple<RuntimeMethodHandle, RuntimeTypeHandle>, Type> concreteTypesBySlot;

        public ClassMembersCollector(Type targetType)
            : base(targetType)
        {
            concreteTypesBySlot = new Dictionary<Tuple<RuntimeMethodHandle, RuntimeTypeHandle>, Type>();
            foreach (var candidateType in targetType.GetInheritanceChain())
            {
                var methods = candidateType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var method in methods.Where(method => method.IsVirtual))
                {
                    var slot = GetMethodKey(MockingUtil.GetClassVirtualBaseDefinition(method));
                    if (!concreteTypesBySlot.ContainsKey(slot))
                    {
                        concreteTypesBySlot.Add(slot, method.DeclaringType);
                    }
                }
            }
        }

        protected override MetaMethod GetMethodToGenerate(MethodInfo method, IProxyGenerationHook hook, bool isStandalone)
        {
            Type concreteType;
            if (method.IsVirtual
                && concreteTypesBySlot.TryGetValue(
                    GetMethodKey(MockingUtil.GetClassVirtualBaseDefinition(method)), out concreteType)
                && concreteType != method.DeclaringType)
            {
                return null;
            }

            if (ProxyUtil.IsAccessibleMethod(method) == false)
            {
                return null;
            }

            var accepted = AcceptMethod(method, true, hook);
            if (!accepted && !method.IsAbstract)
            {
                //we don't need to do anything...
                return null;
            }

            var metaMethod = new MetaMethod(method, method, isStandalone, accepted, !method.IsAbstract);
            if (method.DeclaringType != type && type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Any(candidate => candidate.DeclaringType != method.DeclaringType
                    && method.DeclaringType.IsAssignableFrom(candidate.DeclaringType)
                    && MethodSignatureComparer.Instance.Equals(method, candidate)))
            {
                // A hidden slot needs a separate name and an explicit MethodImpl, not an implicit override.
                metaMethod.SwitchToExplicitImplementation();
            }
            return metaMethod;
        }

        private static Tuple<RuntimeMethodHandle, RuntimeTypeHandle> GetMethodKey(MethodInfo method)
        {
            if (method.IsGenericMethod)
            {
                method = method.GetGenericMethodDefinition();
            }
            return Tuple.Create(method.MethodHandle, method.DeclaringType.TypeHandle);
        }

        protected override IEnumerable<MethodInfo> GetMethods()
        {
            return type.GetAllMethods().Where(method => !method.IsStatic).Distinct();
        }

        protected override IEnumerable<PropertyInfo> GetProperties()
        {
            return type.GetAllProperties().Where(property =>
                !(property.GetGetMethod(true) ?? property.GetSetMethod(true)).IsStatic);
        }
    }
}