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

namespace Telerik.JustMock.Expectations.Abstraction
{
	/// <summary>
	/// Defines the expected behavior for a setup.
	/// </summary>
	public interface ISetupBehavior
	{
		/// <summary>
		/// Specifies that justmock should invoke different mock instance for each setup.
		/// </summary>
		/// <remarks>
		/// When this modifier is applied
		/// for similar type call, the flow of setups will be maintained.
		/// </remarks>
		IAssertable InSequence();
	}
}
