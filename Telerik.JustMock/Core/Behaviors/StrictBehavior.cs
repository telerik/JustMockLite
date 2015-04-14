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
using System.Reflection;

namespace Telerik.JustMock.Core.Behaviors
{
	internal class StrictBehavior : IBehavior
	{
		private readonly bool throwOnlyOnValueReturningMethods;

		public StrictBehavior(bool throwOnlyOnValueReturningMethods)
		{
			this.throwOnlyOnValueReturningMethods = throwOnlyOnValueReturningMethods;
		}

		public void Process(Invocation invocation)
		{
			if (!invocation.UserProvidedImplementation
				&& !invocation.Recording
				&& (invocation.Method.GetReturnType() != typeof(void) || !throwOnlyOnValueReturningMethods)
				&& !(invocation.Method is ConstructorInfo))
				throw new StrictMockException(invocation.Method.DeclaringType);
		}
	}
}

namespace Telerik.JustMock.Core
{
	/// <summary>
	/// Thrown when a strict mock is accessed inappropriately.
	/// </summary>
	public sealed class StrictMockException : MockException
	{
		internal StrictMockException(MemberInfo member)
			: base(String.Format("All calls on {0} should be arranged first.", member))
		{
		}
	}
}
