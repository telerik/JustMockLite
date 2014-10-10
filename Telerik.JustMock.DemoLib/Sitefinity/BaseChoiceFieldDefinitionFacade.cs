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

using Telerik.Sitefinity.Fluent.Definitions.Fields;
using Telerik.Sitefinity.Web.UI.Fields.Config;

namespace Telerik.Sitefinity
{
	/// <summary>
	/// Base Fluent API for wrapping of ChoiceFieldElement
	/// </summary>
	/// <typeparam name="TParentFacade">Type of the parent facade</typeparam>
	/// <typeparam name="TActualFacade">Type of the class implementing this abstract class</typeparam>
	/// <typeparam name="TConfig">Type of the configuration element</typeparam>
	public abstract class BaseChoiceFieldDefinitionFacade<TConfig, TActualFacade, TParentFacade>
		: FieldControlDefinitionFacade<TConfig, TActualFacade, TParentFacade>
		where TParentFacade : class
		where TActualFacade : class
		where TConfig : ChoiceFieldElement
	{
	}

}
