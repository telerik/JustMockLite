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

namespace Telerik.Sitefinity.Web.UI.Fields.Config
{
    public abstract class FieldDefinitionElement : IFieldDefinition
    {
        public string ControlDefinitionName
        {
            get;
            set;
        }

        public string ViewName
        {
            get;
            set;
        }

        public DefinitionBase GetDefinition()
        {
            throw new NotImplementedException();
        }
    }

    public interface IFieldDefinition : IDefinition
    {
        string ControlDefinitionName { get; set; }

        string ViewName { get; set; }
    }

    public interface IDefinition
    {
        DefinitionBase GetDefinition();
    }

    public abstract class DefinitionBase : IDefinition
    {
        public DefinitionBase GetDefinition()
        {
            return null;
        }
    }
}
