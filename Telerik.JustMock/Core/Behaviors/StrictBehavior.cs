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
using System.Reflection;
using System.Text;
using Telerik.JustMock.Core.Context;

namespace Telerik.JustMock.Core.Behaviors
{
	internal class StrictBehavior : IAssertableBehavior
	{
		private static readonly string MissingReturnValueMessage =
			"Member '{0}' on strict mock of type '{1}' has a non-void return value but no return value given in arrangement.\n";
		private static readonly string GenericErrorMessage =
			"Called unarranged member '{0}' on strict mock of type '{1}'\n";

		private readonly bool throwOnlyOnValueReturningMethods;

		private StringBuilder strictnessViolationMessage;

		public StrictBehavior(bool throwOnlyOnValueReturningMethods)
		{
			this.throwOnlyOnValueReturningMethods = throwOnlyOnValueReturningMethods;
		}

		public void Process(Invocation invocation)
		{
			if (!invocation.UserProvidedImplementation
				&& !invocation.Recording
				&& (invocation.Method.GetReturnType() != typeof(void) || !throwOnlyOnValueReturningMethods)
				&& !(invocation.Method is ConstructorInfo)
				&& !invocation.InArrange)
			{
				if (strictnessViolationMessage == null)
					strictnessViolationMessage = new StringBuilder();
				strictnessViolationMessage.AppendFormat(
					throwOnlyOnValueReturningMethods ? MissingReturnValueMessage : GenericErrorMessage,
					invocation.Method, invocation.Method.DeclaringType);

				throw new StrictMockException(strictnessViolationMessage.ToString());
			}
		}

		public void Assert()
		{
			if (this.strictnessViolationMessage != null)
				MockingContext.Fail(this.strictnessViolationMessage.ToString());
		}

		public string DebugView
		{
			get
			{
				return this.strictnessViolationMessage != null
					? "Strict mock violations:\n" + this.strictnessViolationMessage
					: "Strict mock with no violations";
			}
		}
	}
}
