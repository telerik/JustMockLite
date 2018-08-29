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
using System.Linq;
using System.Reflection;

namespace Telerik.JustMock.Core
{
    internal class PrivateRefReturnAccessor : IPrivateRefReturnAccessor
    {
        private readonly object instance;
        private readonly Type type;

        public PrivateRefReturnAccessor(object instance, Type type)
        {
            this.instance = instance;
            this.type = type;
        }

        public ref TRefReturn CallMethod<TRefReturn>(string name, params object[] args)
        {
            return ref ProfilerInterceptor.GuardInternal((target, arguments) =>
            {
                arguments = arguments ?? MockingUtil.NoObjects;
                var candidates = type.GetAllMethods()
                    .Where(m => m.Name == name && MockingUtil.CanCall(m, this.instance != null))
                    .Select(m => MockingUtil.TrySpecializeGenericMethod(m, arguments.Select(a => a != null ? a.GetType() : null).ToArray()) ?? m)
                    .ToArray();
                object state;
                var method = MockingUtil.BindToMethod(MockingUtil.AllMembers, candidates, ref arguments, null, null, null, out state);

                ProfilerInterceptor.RefReturn<TRefReturn> @delegate =
                    MockingUtil.CreateDynamicMethodInvoker<TRefReturn>(target, method as MethodInfo, arguments);

                return ref ProfilerInterceptor.GuardExternal(@delegate, target, arguments);
            }, this.instance, args ?? MockingUtil.NoObjects);
        }

        public ref TRefReturn GetProperty<TRefReturn>(string name)
        {
            return ref ProfilerInterceptor.GuardInternal((target, arguments) =>
            {
                var prop = MockingUtil.ResolveProperty(this.type, name, false, null, this.instance != null);
                var method = prop.GetGetMethod(true);

                ProfilerInterceptor.RefReturn<TRefReturn> @delegate =
                    MockingUtil.CreateDynamicMethodInvoker<TRefReturn>(target, method, arguments);

                return ref ProfilerInterceptor.GuardExternal(@delegate, target, arguments);
            }, this.instance, MockingUtil.NoObjects);
        }
    }
}