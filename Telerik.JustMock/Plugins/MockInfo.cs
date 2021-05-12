/*
 JustMock Lite
 Copyright © 2020 - 2021 Progress Software Corporation

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
using System.Reflection;

#if !PORTABLE
namespace Telerik.JustMock.Plugins
{
    public class MockInfo
    {
        public MethodMockInfo MethodMock { get; private set; }

        private MockInfo(string name, MemberTypes memberType, Type declaringType, Type reflectedType)
        {
            this.MethodMock = new MethodMockInfo(name, memberType, declaringType, reflectedType);
        }

        public static MockInfo FromMethodBase(MethodBase method)
        {
            return new MockInfo(method.Name, method.MemberType, method.DeclaringType, method.ReflectedType);
        }
    }
}
#endif
