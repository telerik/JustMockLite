/*
 JustMock Lite
 Copyright © 2010-2015,2018 Progress Software Corporation

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

namespace Telerik.JustMock
{
    /// <summary>
    /// Specifies the behavior of the mock.
    /// </summary>
    public enum Behavior
    {
        /// <summary>
        /// Specifies that by default mock calls will behave like a stub, unless explicitly setup.
        /// </summary>
        Loose,

        /// <summary>
        /// Specifies that by default mock calls will return mock objects, unless explicitly setup.
        /// </summary>
        RecursiveLoose,

        /// <summary>
        /// Specifies that any calls made on the mock 
        /// will throw an exception if not explictly set.
        /// </summary>
        Strict,

        /// <summary>
        /// Specifies that by default all calls made on mock will invoke its 
        /// corresponding original member unless some expecations are set.
        /// </summary>
        CallOriginal
    }
}
