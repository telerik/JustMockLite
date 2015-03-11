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
using System.Reflection;
using Telerik.JustMock.Core.MatcherTree;

namespace Telerik.JustMock.Core.Behaviors
{
	internal sealed class ConstructorMockBehavior : IBehavior
	{
		public void Process(Invocation invocation)
		{
			var mockMixin = MocksRepository.GetMockMixinFromInvocation(invocation);
			if (mockMixin == null)
			{
				mockMixin = invocation.Repository.CreateExternalMockMixin(null, invocation.Instance, Behavior.CallOriginal);
				mockMixin.IsInstanceConstructorMocked = true;
			}

			invocation.CallOriginal = !mockMixin.IsInstanceConstructorMocked;
		}

		public static void Attach(IMethodMock methodMock)
		{
			var callPattern = methodMock.CallPattern;
			if (!(callPattern.Method is ConstructorInfo)
				|| callPattern.Method.IsStatic
				|| !(callPattern.InstanceMatcher is AnyMatcher)
				|| typeof(string) == callPattern.Method.DeclaringType
#if !COREFX
				|| typeof(ContextBoundObject).IsAssignableFrom(callPattern.Method.DeclaringType)
#endif
				)
				return;

			methodMock.Behaviors.Add(new ConstructorMockBehavior());
		}
	}
}
