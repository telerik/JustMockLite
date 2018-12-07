/*
 JustMock Lite
 Copyright © 2018 Telerik EAD

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
using System.Reflection;
using System.Reflection.Emit;

namespace Telerik.JustMock.Helpers
{
    /// <summary>
    /// Constructs dynamic types in memory
    /// </summary>
    internal static class DynamicTypeHelper
    {
        private const string DynamicAssemblyName = "Telerik.JustMock.DynamicTypes";
        private static Dictionary<Type, int> typeIndices = new Dictionary<Type, int>();

        private static ModuleBuilder moduleBuilder =
#if !NETCORE
            AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(DynamicTypeHelper.DynamicAssemblyName), AssemblyBuilderAccess.RunAndCollect)
                    .DefineDynamicModule(DynamicTypeHelper.DynamicAssemblyName);
#else
            AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName(DynamicTypeHelper.DynamicAssemblyName), AssemblyBuilderAccess.RunAndCollect)
                    .DefineDynamicModule(DynamicTypeHelper.DynamicAssemblyName);
#endif

        public static Type GetNextType<T>()
        {
            Type valueType = typeof(T);
            Type nextType = null;
            lock (DynamicTypeHelper.typeIndices)
            {
                int currentTypeIndex = DynamicTypeHelper.typeIndices.ContainsKey(valueType) ? DynamicTypeHelper.typeIndices[valueType] : 0;
                string newTypeName = DynamicTypeHelper.DynamicAssemblyName + "_" + valueType.Name + "_" + currentTypeIndex++;
                nextType = moduleBuilder.GetType(newTypeName);
                if (nextType == null)
                {
                    TypeBuilder typeBuilder = moduleBuilder.DefineType(newTypeName, TypeAttributes.Public);
                    nextType = typeBuilder.CreateType();
                }
                DynamicTypeHelper.typeIndices[valueType] = currentTypeIndex;
            }

            return nextType;
        }

        public static void Reset()
        {
            lock (DynamicTypeHelper.typeIndices)
            {
                DynamicTypeHelper.typeIndices.Clear();
            }
        }
    }
}
