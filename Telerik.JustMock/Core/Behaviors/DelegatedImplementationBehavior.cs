/*
 JustMock Lite
 Copyright © 2010-2015,2021 Progress Software Corporation

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
using System.Collections.Generic;
using System.Linq;

namespace Telerik.JustMock.Core.Behaviors
{
	internal class DelegatedImplementationBehavior : IBehavior
	{
		private readonly IEnumerable<Type> types;
		private readonly object implementer;

		public DelegatedImplementationBehavior(object implementer, IEnumerable<Type> types)
		{
			this.implementer = implementer;
			this.types = types;
		}

		public void Process(Invocation invocation)
		{
			var mockMethod = invocation.Method;
			var inheritanceChain = mockMethod.GetInheritanceChain();

			var delegatedImplMethod =
				inheritanceChain.FirstOrDefault(
					method =>
						types.Any(
							type =>
								{
									var targetType = method.IsExtensionMethod() ? method.GetParameters()[0].ParameterType : method.DeclaringType;
									return targetType.IsAssignableFrom(type);
								}));
			if (delegatedImplMethod != null)
			{
				invocation.ReturnValue = delegatedImplMethod.Invoke(implementer, invocation.Args);
				invocation.UserProvidedImplementation = true;
			}
		}
	}
}
