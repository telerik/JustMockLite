/*
 JustMock Lite
 Copyright © 2010-2014 Progress Software Corporation

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

namespace Telerik.Sitefinity.Model
{
    public interface IContent
    {
        /// <summary>
        /// Gets or sets the date and time the content was created.        /// </summary>
        /// <value>The date created.</value>
        DateTime DateCreated { get; set; }
    }
}
