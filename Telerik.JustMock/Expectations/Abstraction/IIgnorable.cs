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
using System.Linq;

namespace Telerik.JustMock.Expectations.Abstraction
{
	interface IIgnorable<TContainer>
	{
		/// <summary>
		/// Specifies to ignore any argument for the target call.
		/// </summary>
		/// <returns>Func or Action Container</returns>
		TContainer IgnoreArguments();

		/// <summary>
		/// Specifies an additional condition that must be true for this arrangement to be
		/// considered when the arranged member is called. This condition is evaluated in addition
		/// to the conditions imposed by any argument matchers in the arrangement.
		/// 
		/// This method allows a more general way of matching arrangements than argument matchers do.
		/// </summary>
		/// <param name="condition">A function that should return 'true' when this
		/// arrangement should be considered and 'false' if this arrangement doesn't match the user criteria.</param>
		TContainer When(Func<bool> condition);
	}
}
