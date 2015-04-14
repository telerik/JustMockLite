/*
 JustMock Lite
 Copyright © 2010-2015 Telerik AD

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

namespace Telerik.JustMock
{
	/// <summary>
	/// Declares that the proxy generator must emit a proxy for the given type.
	/// This attribute is used whenever the proxy generator cannot infer from that
	/// some type will be proxied.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class MockedTypeAttribute : Attribute
	{
		/// <summary>
		/// An array of interface types that the proxy will implement additionally.
		/// </summary>
		public Type[] Implements { get; set; }

		/// <summary>
		/// Declares the type for which a proxy must be emitted.
		/// </summary>
		/// <param name="mockedType">The type to proxy.</param>
		public MockedTypeAttribute(Type mockedType)
		{
		}
	}
}
