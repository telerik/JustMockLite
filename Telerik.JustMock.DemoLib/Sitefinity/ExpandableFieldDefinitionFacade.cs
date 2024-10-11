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

using Telerik.Sitefinity.Web.UI.Fields;
using Telerik.Sitefinity.Web.UI.Fields.Enums;

namespace Telerik.Sitefinity.Fluent.Definitions.Fields
{
    /// <summary>
    /// Fluent API wrapper for <c>ExpandableFieldElement</c>
    /// </summary>
    /// <typeparam name="TParentFacade">Type of the parent facade</typeparam>
    public class ExpandableFieldDefinitionFacade<TParentFacade>
        : FieldControlDefinitionFacade<ExpandableFieldElement, ExpandableFieldDefinitionFacade<TParentFacade>, TParentFacade>
        , IHasFieldControls<ExpandableFieldDefinitionFacade<TParentFacade>>
        where TParentFacade : class
    {
        internal virtual IHasFieldControls<ExpandableFieldDefinitionFacade<TParentFacade>> FieldsCollection
        {
            get { return this.fields; }
        }

        /// <inheritdoc />
        public ChoiceFieldDefinitionFacade<ExpandableFieldDefinitionFacade<TParentFacade>> AddChoiceField<TFieldControl>(string fieldName, RenderChoicesAs renderAs)
            where TFieldControl : ChoiceField
        {
            return this.FieldsCollection.AddChoiceField<TFieldControl>(fieldName, renderAs);
        }

        IHasFieldControls<ExpandableFieldDefinitionFacade<TParentFacade>> fields;
    }
}
