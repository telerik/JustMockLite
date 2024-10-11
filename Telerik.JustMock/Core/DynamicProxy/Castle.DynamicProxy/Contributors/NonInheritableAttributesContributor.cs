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

    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators;
    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators.Emitters;
    using Telerik.JustMock.Core.Castle.DynamicProxy.Internal;

    /// <summary>
    ///   Reproduces the proxied type's non-inheritable custom attributes on the proxy type.
    /// </summary>
    internal sealed class NonInheritableAttributesContributor : ITypeContributor
    {
        private readonly Type targetType;

        public NonInheritableAttributesContributor(Type targetType)
        {
            this.targetType = targetType;
        }

        public void Generate(ClassEmitter emitter)
        {
            foreach (var attribute in targetType.GetNonInheritableAttributes())
            {
                emitter.DefineCustomAttribute(attribute.Builder);
            }
        }

        public void CollectElementsToProxy(IProxyGenerationHook hook, MetaType model)
        {
        }
    }
}
