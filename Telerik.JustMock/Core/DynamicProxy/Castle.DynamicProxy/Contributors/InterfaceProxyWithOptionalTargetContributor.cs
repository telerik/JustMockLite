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
    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators;
    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators.Emitters;

    internal class InterfaceProxyWithOptionalTargetContributor : InterfaceProxyWithoutTargetContributor
    {
        private readonly GetTargetReferenceDelegate getTargetReference;

        public InterfaceProxyWithOptionalTargetContributor(INamingScope namingScope, GetTargetExpressionDelegate getTarget,
                                                           GetTargetReferenceDelegate getTargetReference)
            : base(namingScope, getTarget)
        {
            this.getTargetReference = getTargetReference;
            canChangeTarget = true;
        }

        protected override MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class,
                                                              OverrideMethodDelegate overrideMethod)
        {
            if (!method.Proxyable)
            {
                return new OptionallyForwardingMethodGenerator(method, overrideMethod, getTargetReference);
            }

            return base.GetMethodGenerator(method, @class, overrideMethod);
        }
    }
}