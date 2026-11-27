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

namespace Telerik.JustMock.Core.Castle.DynamicProxy.Generators
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    internal abstract class MetaTypeElement
    {
        private readonly MemberInfo member;
        private string name;

        protected MetaTypeElement(MemberInfo member)
        {
            this.member = member;
            this.name = member.Name;
        }

        public bool CanBeImplementedExplicitly
        {
            get { return member.DeclaringType?.IsInterface ?? false; }
        }

        public string Name
        {
            get { return name; }
        }

        protected MemberInfo Member
        {
            get { return member; }
        }

        public abstract void SwitchToExplicitImplementation();

        protected void SwitchToExplicitImplementationName()
        {
            var name = member.Name;
            var sourceType = member.DeclaringType;
            var ns = sourceType.Namespace;
            Debug.Assert(ns == null || ns != "");

            var nameBuilder = new StringBuilder();
            if (ns != null)
            {
                nameBuilder.Append(ns);
                nameBuilder.Append('.');
            }
            AppendTypeName(nameBuilder, sourceType);
            nameBuilder.Append('.');
            nameBuilder.Append(name);
            this.name = nameBuilder.ToString();
        }

        private static void AppendTypeName(StringBuilder nameBuilder, Type type)
        {
            if (type.IsGenericParameter)
            {
                nameBuilder.Append(type.Name);
                return;
            }

            if (type.DeclaringType != null)
            {
                var declaringType = type.DeclaringType;
                var declaringTypeArgumentCount = declaringType.GetGenericArguments().Length;
                if (declaringTypeArgumentCount > 0)
                {
                    var nestedTypeArguments = type.GetGenericArguments();
                    var declaringTypeArguments = new Type[declaringTypeArgumentCount];
                    Array.Copy(nestedTypeArguments, declaringTypeArguments, declaringTypeArgumentCount);
                    declaringType = declaringType.GetGenericTypeDefinition()
                        .MakeGenericType(declaringTypeArguments);
                }
                AppendTypeName(nameBuilder, declaringType);
                nameBuilder.Append('.');
            }
            nameBuilder.Append(type.Name);
            if (type.IsGenericType)
            {
                var genericTypeArguments = type.GetGenericArguments();
                var inheritedArgumentCount = type.DeclaringType?.GetGenericArguments().Length ?? 0;
                if (inheritedArgumentCount < genericTypeArguments.Length)
                {
                    nameBuilder.Append('[');
                    for (int i = inheritedArgumentCount, n = genericTypeArguments.Length; i < n; ++i)
                    {
                        if (i > inheritedArgumentCount)
                        {
                            nameBuilder.Append(',');
                        }
                        AppendTypeName(nameBuilder, genericTypeArguments[i]);
                    }
                    nameBuilder.Append(']');
                }
            }
        }
    }
}
