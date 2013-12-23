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
using Telerik.Sitefinity.Fluent.Definitions.Fields;
using Telerik.Sitefinity.Web.UI.Fields.Enums;

namespace Telerik.Sitefinity.Fluent.Definitions
{
    public interface IHasFieldControls<TActualFacade>
        where TActualFacade : class
    {
        /// <summary>
        /// Add a choice field and specify the type of the control that will use this settings to render it
        /// </summary>
        /// <typeparam name="TFieldControl">Field control inheriting <c>ChoiceField</c> that will use this settings to render the field</typeparam>
        /// <param name="fieldName">Name of the field to bind to</param>
        /// <param name="renderAs">Specify how the control will be rendered</param>
        /// <returns>Child facade for further customization of the choice field</returns>
        /// <remarks>Sets both FieldName and DataFieldName to the value of <paramref name="fieldName"/></remarks>
        /// <exception cref="ArgumentNullException">When <paramref name="fieldName"/> is null or empty</exception>
        ChoiceFieldDefinitionFacade<TActualFacade> AddChoiceField<TFieldControl>(string fieldName, RenderChoicesAs renderAs)
            where TFieldControl : Telerik.Sitefinity.Web.UI.Fields.ChoiceField
        ;
    }
}
