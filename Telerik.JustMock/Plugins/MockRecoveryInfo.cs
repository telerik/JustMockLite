/*
 JustMock Lite
 Copyright © 2020 Progress Software Corporation

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

#if !PORTABLE
namespace Telerik.JustMock.Plugins
{
    public class MockRecoveryInfo
    {
        public MockInfo Mock { get; private set; }
        public InvocationInfo[] Invocations { get; private set; }

        public MockRecoveryInfo(MockInfo mock, InvocationInfo[] invocations)
        {
            this.Mock = mock;
            this.Invocations = invocations;
        }
    }
}
#endif
