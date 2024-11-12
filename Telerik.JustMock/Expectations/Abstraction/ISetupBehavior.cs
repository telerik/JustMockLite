/*
 JustMock Lite
 Copyright © 2010-2015 Progress Software Corporation

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

namespace Telerik.JustMock.Expectations.Abstraction
{
    /// <summary>
    /// Defines the expected behavior for a setup.
    /// </summary>
    public interface ISetupBehavior
    {
        /// <summary>
        /// Specifies that JustMock should invoke different mock instance for each setup.
        /// </summary>
        /// <remarks>
        /// When this modifier is applied
        /// for similar type call, the flow of setups will be maintained.
        /// </remarks>
        IAssertable InSequence();

        /// <summary>
        /// Specifies that the arrangement will be respected regardless of the thread
        /// on which the call to the arranged member happens.
        /// </summary>
        /// <remarks>
        /// This is only needed for arrangements of static members. Arrangements on
        /// instance members are always respected, regardless of the current thread.
        /// 
        /// Cross-thread arrangements are active as long as the current context
        /// (test method) is on the call stack. Be careful when arranging
        /// static members cross-thread because the effects of the arrangement may
        /// affect and even crash the testing framework.
        /// </remarks>
        IAssertable OnAllThreads();
    }
}
