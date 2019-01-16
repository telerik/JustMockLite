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

namespace Telerik.Sitefinity.Fluent.Definitions.Fields
{
	/// <summary>
	/// Base fluent API facade that defines a definition for field element
	/// </summary>
	/// <typeparam name="TElement">The type of the element.</typeparam>
	/// <typeparam name="TActualFacade">The type of the actual facade.</typeparam>
	/// <typeparam name="TParentFacade">The type of the section parent facade.</typeparam>
	public abstract class FieldDefinitionFacade<TElement, TActualFacade, TParentFacade>
		where TElement : FieldDefinitionElement
		where TActualFacade : class
		where TParentFacade : class
	{
	}
}
