/*
 JustMock Lite
 Copyright © 2025 Progress Software Corporation

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

namespace Telerik.JustMock.Core.Licensing
{
    /// <summary>
    /// The exception that is thrown in case of licensing error
    /// </summary>
    [Serializable]
    public class LicenseException : MockException
    {
        public LicenseException() : base() { }

        public LicenseException(string message) : base(message) { }

        public LicenseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
