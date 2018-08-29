/*
 JustMock Lite
 Copyright © 2018 Telerik EAD

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

using Telerik.JustMock.Expectations.Abstraction.Local.Function;

namespace Telerik.JustMock.Expectations.Abstraction.Local
{
	public interface ILocalExpectation
	{
		/// <summary>
		/// Arrange and assert expectations on C# 7 local functions.
		/// </summary>
		IFunctionExpectation Function { get; }
	}
}