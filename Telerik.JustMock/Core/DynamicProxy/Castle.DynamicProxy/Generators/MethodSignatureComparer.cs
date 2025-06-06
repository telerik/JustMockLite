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
    using System.Collections.Generic;
    using System.Reflection;

    internal class MethodSignatureComparer : IEqualityComparer<MethodInfo>
    {
        public static readonly MethodSignatureComparer Instance = new MethodSignatureComparer();

        private static readonly Type preserveBaseOverridesAttribute = Type.GetType("System.Runtime.CompilerServices.PreserveBaseOverridesAttribute", throwOnError: false);

        public bool EqualGenericParameters(MethodInfo x, MethodInfo y)
        {
            if (x.IsGenericMethod != y.IsGenericMethod)
            {
                return false;
            }

            if (x.IsGenericMethod)
            {
                var xArgs = x.GetGenericArguments();
                var yArgs = y.GetGenericArguments();

                if (xArgs.Length != yArgs.Length)
                {
                    return false;
                }

                for (var i = 0; i < xArgs.Length; ++i)
                {
                    if (xArgs[i].IsGenericParameter != yArgs[i].IsGenericParameter)
                    {
                        return false;
                    }

                    if (!xArgs[i].IsGenericParameter && !xArgs[i].Equals(yArgs[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool EqualParameters(MethodInfo x, MethodInfo y)
        {
            var xArgs = x.GetParameters();
            var yArgs = y.GetParameters();

            if (xArgs.Length != yArgs.Length)
            {
                return false;
            }

            for (var i = 0; i < xArgs.Length; ++i)
            {
                if (!EqualSignatureTypes(xArgs[i].ParameterType, yArgs[i].ParameterType))
                {
                    return false;
                }
            }

            return true;
        }

        public bool EqualReturnTypes(MethodInfo x, MethodInfo y)
        {
            var xr = x.ReturnType;
            var yr = y.ReturnType;

            if (EqualSignatureTypes(xr, yr))
            {
                return true;
            }

            // This enables covariant method returns for .NET 5 and newer.
            // No need to check for runtime support, since such methods are marked with a custom attribute;
            // see https://github.com/dotnet/runtime/blob/main/docs/design/features/covariant-return-methods.md.
            if (preserveBaseOverridesAttribute != null)
            {
                return (x.IsDefined(preserveBaseOverridesAttribute, inherit: false) && yr.IsAssignableFrom(xr))
                    || (y.IsDefined(preserveBaseOverridesAttribute, inherit: false) && xr.IsAssignableFrom(yr));
            }

            return false;
        }

        private bool EqualSignatureTypes(Type x, Type y)
        {
            if (x.IsGenericParameter != y.IsGenericParameter)
            {
                return false;
            }
            else if (x.IsGenericType != y.IsGenericType)
            {
                return false;
            }

            if (x.IsGenericParameter)
            {
                if (x.GenericParameterPosition != y.GenericParameterPosition)
                {
                    return false;
                }
            }
            else if (x.IsGenericType)
            {
                var xGenericTypeDef = x.GetGenericTypeDefinition();
                var yGenericTypeDef = y.GetGenericTypeDefinition();

                if (xGenericTypeDef != yGenericTypeDef)
                {
                    return false;
                }

                var xArgs = x.GetGenericArguments();
                var yArgs = y.GetGenericArguments();

                if (xArgs.Length != yArgs.Length)
                {
                    return false;
                }

                for (var i = 0; i < xArgs.Length; ++i)
                {
                    if(!EqualSignatureTypes(xArgs[i], yArgs[i])) return false;
                }
            }
            else
            {
                if (!x.Equals(y))
                {
                    return false;
                }
            }
            return true;
        }

        public bool Equals(MethodInfo x, MethodInfo y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return EqualNames(x, y) &&
                   EqualGenericParameters(x, y) &&
                   EqualReturnTypes(x, y) &&
                   EqualParameters(x, y);
        }

        public int GetHashCode(MethodInfo obj)
        {
            return obj.Name.GetHashCode() ^ obj.GetParameters().Length; // everything else would be too cumbersome
        }

        private bool EqualNames(MethodInfo x, MethodInfo y)
        {
            return x.Name == y.Name;
        }
    }
}