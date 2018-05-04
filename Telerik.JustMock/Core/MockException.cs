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

namespace Telerik.JustMock.Core
{
	/// <summary>
	/// Mock exception.
	/// </summary>
	[Serializable]
	public class MockException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MockException"/> class.
		/// </summary>
		public MockException() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="MockException"/> class.
		/// </summary>
		/// <param name="message">Exception message.</param>
		public MockException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="MockException"/> class.
		/// </summary>
		/// <param name="message">Exception message.</param>
		/// <param name="innerException">Inner exception.</param>
		public MockException(string message, Exception innerException) : base(message, innerException) { }

#if !COREFX
		/// <summary>
		/// Serialization constructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected MockException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif

		internal static void ThrowUnsafeTypeException(Type type)
		{
			throw new MockException(String.Format("Cannot mock type '{0}' because it might be unsafe. You could still create a mock with Behavior.CallOriginal. Alternatively, you could still try mocking this member by adding the line 'Telerik.JustMock.Setup.AllowedMockableTypes.Add<{0}>();' to your test but mind that this might result in a hard crash of the CLR runtime.", type));
		}
	}
}
