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

using Telerik.Sitefinity.Web.UI.Fields.Config;
using Telerik.Sitefinity.Web.UI.Fields.Contracts;

namespace Telerik.Sitefinity
{
    public class ExpandableFieldElement : FieldControlDefinitionElement, IExpandableFieldDefinition
    {
        public IChoiceFieldDefinition ExpandFieldDefinition
        {
            get { return null; }
        }
    }

    public interface IExpandableFieldDefinition : IFieldControlDefinition
    {
        /// <summary>
        /// Gets the definition for the control that when clicked expands the hidden part of the whole 
        /// control.
        /// </summary>
        /// <value>The expand control.</value>
        IChoiceFieldDefinition ExpandFieldDefinition { get; }
    }

}
