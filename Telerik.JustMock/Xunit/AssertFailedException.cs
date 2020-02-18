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

using System;
#if !PORTABLE
using System.ComponentModel;
#endif

namespace Telerik.JustMock.XUnit
{
    /// <summary>
    /// Represents common assertion failure exception across 1.x and 2.x xUnit versions
    /// </summary>
#if !PORTABLE
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public class AssertFailedException : Exception
    {
        public AssertFailedException()
            : base()
        {

        }

        public AssertFailedException(string message)
            : base(message)
        {

        }

        public AssertFailedException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
