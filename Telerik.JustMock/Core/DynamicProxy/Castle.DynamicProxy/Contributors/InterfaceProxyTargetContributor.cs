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
    using System.Diagnostics;
    using System.Reflection;

    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators;
    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators.Emitters;

    internal class InterfaceProxyTargetContributor : CompositeTypeContributor
    {
        private readonly bool canChangeTarget;
        private readonly Type proxyTargetType;

        public InterfaceProxyTargetContributor(Type proxyTargetType, bool canChangeTarget, INamingScope namingScope)
            : base(namingScope)
        {
            this.proxyTargetType = proxyTargetType;
            this.canChangeTarget = canChangeTarget;
        }

        protected override IEnumerable<MembersCollector> GetCollectors()
        {
            foreach (var @interface in interfaces)
            {
                var item = GetCollectorForInterface(@interface);
                item.Logger = Logger;
                yield return item;
            }
        }

        protected virtual MembersCollector GetCollectorForInterface(Type @interface)
        {
            return new InterfaceMembersOnClassCollector(@interface, false,
                proxyTargetType.GetTypeInfo().GetRuntimeInterfaceMap(@interface));
        }

        protected override MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class,
                                                              OverrideMethodDelegate overrideMethod)
        {
            if (!method.Proxyable)
            {
                return new ForwardingMethodGenerator(method,
                                                     overrideMethod,
                                                     (c, m) => c.GetField("__target"));
            }

            var invocation = GetInvocationType(method, @class);

            return new MethodWithInvocationGenerator(method,
                                                     @class.GetField("__interceptors"),
                                                     invocation,
                                                     (c, m) => c.GetField("__target"),
                                                     overrideMethod,
                                                     null);
        }

        private Type GetInvocationType(MetaMethod method, ClassEmitter @class)
        {
            var scope = @class.ModuleScope;

            Type[] invocationInterfaces;
            if (canChangeTarget)
            {
                invocationInterfaces = new[] { typeof(IInvocation), typeof(IChangeProxyTarget) };
            }
            else
            {
                invocationInterfaces = new[] { typeof(IInvocation) };
            }

            var key = new CacheKey(method.Method, CompositionInvocationTypeGenerator.BaseType, invocationInterfaces, null);

            // no locking required as we're already within a lock

            return scope.TypeCache.GetOrAddWithoutTakingLock(key, _ =>
                new CompositionInvocationTypeGenerator(method.Method.DeclaringType,
                                                       method,
                                                       method.Method,
                                                       canChangeTarget,
                                                       null)
                .Generate(@class, namingScope)
                .BuildType());
        }
    }
}
