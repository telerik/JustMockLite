﻿// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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
    using System.Reflection;

    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators;

    internal class InterfaceMembersCollector : MembersCollector
    {
        public InterfaceMembersCollector(Type @interface)
            : base(@interface)
        {
        }

        protected override MetaMethod GetMethodToGenerate(MethodInfo method, IProxyGenerationHook hook, bool isStandalone)
        {
            if (ProxyUtil.IsAccessibleMethod(method) == false)
            {
                return null;
            }

            var proxyable = AcceptMethod(method, true, hook);
            if (!proxyable && !method.IsAbstract)
            {
                // we don't need to do anything
                return null;
            }

            return new MetaMethod(method, method, isStandalone, proxyable, false);
        }
    }
}