/*
 JustMock Lite
 Copyright © 2019 Progress Software Corporation

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

#if NETCORE

using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Telerik.JustMock.Core.Context
{
    internal class AsyncLocalWrapper : IAsyncContextResolver
    {
        static AsyncLocal<MethodBase> asyncCallPattern = new AsyncLocal<MethodBase>();
        public void CaptureContext()
        {
            MethodBase testMethod = MockingContext.GetTestMethod();
			asyncCallPattern.Value = testMethod;
        }

        public MethodBase GetContext()
        {
			return asyncCallPattern.Value;
        }
    }
}
#endif

