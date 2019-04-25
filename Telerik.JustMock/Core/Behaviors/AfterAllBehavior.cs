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

using System;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Expectations.Abstraction;

namespace Telerik.JustMock.Core.Behaviors
{
    internal class AfterAllBehavior : IAssertableBehavior
    {
        private readonly IPrerequisite[] prerequisites;
        private string processedStackTrace;

        public string DebugView
        {
            get { return String.Format("{0}: after all prerequisite expectations.", this.IsMet ? "Met" : "Unmet"); }
        }

        public AfterAllBehavior(IPrerequisite[] prerequisites)
        {
            this.prerequisites = prerequisites;
        }

        private string ExecutionMessage
        {
            get
            {
                var message = this.processedStackTrace;
                return !String.IsNullOrEmpty(message) ? message : "--no calls--\n";
            }
        }

        private bool IsMet
        {
            get
            {
                foreach (var prerequisite in this.prerequisites)
                {
                    if (!prerequisite.IsMet)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void Process(Invocation invocation)
        {
            this.processedStackTrace = invocation.InputToString() + " called at:\n" + MockingContext.GetStackTrace("    ");
        }

        public void Assert()
        {
            if (!this.IsMet)
            {
                MockingContext.Fail("Not all prerequisites are met. Actual call processed:\n{0}", this.ExecutionMessage);
            }
        }
    }
}
