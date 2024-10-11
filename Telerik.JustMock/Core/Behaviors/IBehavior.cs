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

using System;

namespace Telerik.JustMock.Core.Behaviors
{
    /// <summary>
    /// An implementation detail interface. Not intended for external usage.
    /// </summary>
    public interface IBehavior
    {
        /// <summary> </summary>
        /// <param name="invocation"></param>
        void Process(Invocation invocation);
    }

    internal interface IAssertableBehavior : IBehavior
    {
        void Assert();
        string DebugView { get; }
    }
}
