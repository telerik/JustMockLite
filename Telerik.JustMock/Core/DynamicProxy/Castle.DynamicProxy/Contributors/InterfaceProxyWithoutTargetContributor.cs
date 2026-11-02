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
    using System.Linq;
    using System.Reflection;

    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators;
    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators.Emitters;
    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators.Emitters.SimpleAST;
    using Telerik.JustMock.Core.Castle.DynamicProxy.Internal;

    internal class InterfaceProxyWithoutTargetContributor : CompositeTypeContributor
    {
        private readonly GetTargetExpressionDelegate getTargetExpression;
        private readonly Dictionary<MethodInfo, MethodInfo> defaultImplementations = new Dictionary<MethodInfo, MethodInfo>();
        private Type defaultImplementationType;
        protected bool canChangeTarget = false;

        public InterfaceProxyWithoutTargetContributor(INamingScope namingScope, GetTargetExpressionDelegate getTarget)
            : base(namingScope)
        {
            getTargetExpression = getTarget;
        }

        public override void Generate(ClassEmitter @class)
        {
            var proxiedInterfaces = @class.TypeBuilder.GetInterfaces();
            if (!canChangeTarget && proxiedInterfaces.Any(type =>
                type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Any(method => method.IsVirtual && !method.IsAbstract)))
            {
                // Let the runtime resolve MethodImpl slots, including diamonds and reabstraction,
                // before the proxy's own overrides hide the default implementations.
                defaultImplementationType = new ClassEmitter(@class.ModuleScope,
                    namingScope.ParentScope.GetUniqueName("Castle.Proxies.DefaultInterfaceTarget"),
                    typeof(object), proxiedInterfaces, TypeAttributes.Public | TypeAttributes.Abstract,
                    forceUnsigned: !@class.InStrongNamedModule).BuildType();
                foreach (var type in proxiedInterfaces)
                {
                    var map = defaultImplementationType.GetInterfaceMap(type);
                    for (int i = 0; i < map.InterfaceMethods.Length; i++)
                    {
                        defaultImplementations[map.InterfaceMethods[i]] = map.TargetMethods[i];
                    }
                }
            }
            base.Generate(@class);
        }

        protected override IEnumerable<MembersCollector> GetCollectors()
        {
            foreach (var @interface in interfaces)
            {
                var item = new InterfaceMembersCollector(@interface);
                yield return item;
            }
        }

        protected override MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class,
                                                              OverrideMethodDelegate overrideMethod)
        {
            if (!method.Proxyable)
            {
                return new MinimalisticMethodGenerator(method, overrideMethod);
            }

            MethodInfo implementation;
            if (!defaultImplementations.TryGetValue(method.Method, out implementation))
            {
                implementation = method.Method;
            }
            IInvocationCreationContributor contributor;
            var invocation = GetInvocationType(method, implementation, @class, out contributor);
            GetTargetExpressionDelegate target = getTargetExpression;
            if (!canChangeTarget && implementation != null && !implementation.IsAbstract)
            {
                target = (c, m) => new TypeTokenExpression(defaultImplementationType ?? c.TypeBuilder);
            }
            return new MethodWithInvocationGenerator(method,
                                                     @class.GetField("__interceptors"),
                                                     invocation,
                                                     target,
                                                     overrideMethod,
                                                     contributor);
        }

        private Type GetInvocationType(MetaMethod method, MethodInfo implementation, ClassEmitter emitter,
                                       out IInvocationCreationContributor contributor)
        {
            contributor = null;
            var methodInfo = method.Method;

            if (canChangeTarget == false)
            {
                if (implementation == null || implementation.IsAbstract)
                {
                    return typeof(InterfaceMethodWithoutTargetInvocation);
                }

                if (implementation.IsPrivate)
                {
                    var resolvedMethod = new MetaMethod(methodInfo, implementation, method.Standalone, method.Proxyable, true);
                    var delegateType = GetDelegateType(resolvedMethod, emitter, implementation.DeclaringType);
                    contributor = GetDelegateContributor(delegateType, resolvedMethod, implementation.DeclaringType);
                    return new InheritanceInvocationTypeGenerator(implementation.DeclaringType, resolvedMethod, null, contributor)
                        .Generate(emitter, namingScope)
                        .BuildType();
                }

                var callback = CreateCallbackMethod(emitter, methodInfo, implementation);
                // The callback belongs to this proxy type, so its invocation cannot be cached by interface method.
                return new InheritanceInvocationTypeGenerator(callback.DeclaringType, method, callback, null)
                    .Generate(emitter, namingScope)
                    .BuildType();
            }

            var scope = emitter.ModuleScope;
            Type[] invocationInterfaces;
            if (canChangeTarget)
            {
                invocationInterfaces = new[] { typeof(IInvocation), typeof(IChangeProxyTarget) };
            }
            else
            {
                invocationInterfaces = new[] { typeof(IInvocation) };
            }
            var key = new CacheKey(methodInfo, CompositionInvocationTypeGenerator.BaseType, invocationInterfaces, null);

            // no locking required as we're already within a lock

            return scope.TypeCache.GetOrAddWithoutTakingLock(key, _ =>
                new CompositionInvocationTypeGenerator(methodInfo.DeclaringType,
                                                       method,
                                                       methodInfo,
                                                       canChangeTarget,
                                                       null)
                .Generate(emitter, namingScope)
                .BuildType());
        }
    }
}