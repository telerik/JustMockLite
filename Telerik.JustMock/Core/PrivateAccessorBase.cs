/*
 JustMock Lite
 Copyright © 2010-2018 Telerik EAD

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
using System.Linq;
using System.Reflection;

namespace Telerik.JustMock.Core
{
    public abstract class PrivateAccessorBase
    {
        internal static bool CanCall(MethodBase method, bool hasInstance)
        {
            return method.IsStatic || hasInstance;
        }

        internal static PropertyInfo ResolveProperty(Type type, string name, bool ignoreCase, object[] indexArgs, bool hasInstance, object setterValue = null, bool getter = true)
        {
            var candidates = type.GetAllProperties().Where(prop => MockingUtil.StringEqual(prop.Name, name, ignoreCase)).ToArray();
            if (candidates.Length == 1)
                return candidates[0];

            if (!getter)
            {
                Array.Resize(ref indexArgs, indexArgs.Length + 1);
                indexArgs[indexArgs.Length - 1] = setterValue;
            }

            var propMethods = candidates
                .Select(prop => getter ? prop.GetGetMethod(true) : prop.GetSetMethod(true))
                .Where(m => m != null && CanCall(m, hasInstance))
                .ToArray();

            indexArgs = indexArgs ?? MockingUtil.NoObjects;
            object state;
            var foundGetter = MockingUtil.BindToMethod(MockingUtil.AllMembers, propMethods, ref indexArgs, null, null, null, out state);
            return candidates.First(prop => (getter ? prop.GetGetMethod(true) : prop.GetSetMethod(true)) == foundGetter);
        }
    }
}