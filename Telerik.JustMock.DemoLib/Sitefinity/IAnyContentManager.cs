/*
 JustMock Lite
 Copyright © 2010-2014 Telerik AD

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using ModelContent = Telerik.Sitefinity.GenericContent.Model.Content;

namespace Telerik.Sitefinity.Fluent.AnyContent
{
    public interface IAnyContentManager
    {
        /// <summary>
        /// Unpublish a content item in live state.
        /// </summary>
        /// <param name="item">Live item to unpublish.</param>
        /// <param name="culture">The culture in which to perform the operation. 
        /// <remarks>In monolingual the culture is ignored.
        /// In multilingual mode if null - the current ui culture will be used.
        /// </remarks></param>
        /// <returns>Master (draft) state.</returns>
        ModelContent Unpublish(ModelContent item, CultureInfo culture);
    }
}
