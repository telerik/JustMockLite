/*
 JustMock Lite
 Copyright © 2010-2015 Telerik EAD

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
	/// Declares that a type cannot have a proxy generated. This is used in Xamarin.Android where
	/// the reference assembly metadata does not match the Java metadata. After adding instances
	/// of this attribute you need to Rebuild the project.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class UnproxyableTypeAttribute : Attribute
	{
		/// <summary>
		/// Declares an unproxyable type.
		/// </summary>
		/// <param name="mockedType">The type that cannot be proxied.</param>
		public UnproxyableTypeAttribute(Type mockedType)
		{
		}
	}
}
