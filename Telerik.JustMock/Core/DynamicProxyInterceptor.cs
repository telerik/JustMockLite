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
using Telerik.JustMock.Core.Castle.DynamicProxy;
using Telerik.JustMock.Diagnostics;

namespace Telerik.JustMock.Core
{
	internal class DynamicProxyInterceptor : IInterceptor
	{
		private readonly MocksRepository constructionRepo;

		internal DynamicProxyInterceptor(MocksRepository constructionRepo)
		{
			this.constructionRepo = constructionRepo;
		}

		public void Intercept(IInvocation invocation)
		{
			if (ProfilerInterceptor.ReentrancyCounter > 0)
			{
				CallOriginal(invocation, false);
				return;
			}

			bool callOriginal = false;
			ProfilerInterceptor.GuardInternal(() =>
			{
				var mockInvocation = new Invocation(invocation.Proxy, invocation.GetConcreteMethod(), invocation.Arguments);

				DebugView.TraceEvent(IndentLevel.Dispatch, () => String.Format("Intercepted DP call: {0}", mockInvocation.InputToString()));
				DebugView.PrintStackTrace();

				var mock = mockInvocation.MockMixin;
				var repo = mock != null ? mock.Repository : this.constructionRepo;

				lock (repo)
				{
					repo.DispatchInvocation(mockInvocation);
				}

				invocation.ReturnValue = mockInvocation.ReturnValue;
				callOriginal = mockInvocation.CallOriginal;

				if (callOriginal)
				{
					DebugView.TraceEvent(IndentLevel.DispatchResult, () => "Calling original implementation");
				}
				else if (mockInvocation.IsReturnValueSet)
				{
					DebugView.TraceEvent(IndentLevel.DispatchResult, () => String.Format("Returning value '{0}'", invocation.ReturnValue));
				}
			});

			if (callOriginal)
				CallOriginal(invocation, true);
		}

		private void CallOriginal(IInvocation invocation, bool throwOnFail)
		{
			try
			{
				invocation.Proceed();
			}
			catch (NotImplementedException)
			{
				if (throwOnFail)
					throw new NotImplementedException("You can't call the original implementation of a method that does not have one (abstract or interface method).");
			}
		}
	}
}
