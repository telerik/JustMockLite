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
using System.Linq;

namespace Telerik.JustMock.Core.Behaviors
{
	internal class ImplementationOverrideBehavior : IBehavior
	{
		private static readonly object[] Empty = new object[0];

		private readonly Delegate implementationOverride;
		private readonly bool ignoreDelegateReturnValue;
		private readonly Func<object[], Delegate, object> overrideInvoker;

		public ImplementationOverrideBehavior(Delegate implementationOverride, bool ignoreDelegateReturnValue)
		{
			this.ignoreDelegateReturnValue = ignoreDelegateReturnValue;
			this.implementationOverride = implementationOverride;

			this.overrideInvoker = MockingUtil.MakeFuncCaller(implementationOverride);
		}

		public object CallOverride(Invocation invocation)
		{
			var args = implementationOverride.Method.GetParameters().Length > 0 && invocation.Args != null ? invocation.Args : Empty;

			var paramsCount = args.Length;
			var implementationParamsCount = implementationOverride.Method.GetParameters().Length;

			if (invocation.Member.IsExtensionMethod() && paramsCount - 1 == implementationParamsCount)
			{
				args = args.Skip(1).ToArray();
			}

			int extraParamCount = 1 + (implementationOverride.Target != null && implementationOverride.Method.IsStatic ? 1 : 0);
			if (!invocation.Member.IsStatic() && extraParamCount + paramsCount == implementationParamsCount)
			{
				args = new[] { invocation.Instance }.Concat(args).ToArray();
			}

			try
			{
				var returnValue = ProfilerInterceptor.GuardExternal(() => overrideInvoker(args, this.implementationOverride));
				return returnValue;
			}
			catch (InvalidCastException ex)
			{
				throw new MockException("The implementation callback has an incorrect signature", ex);
			}
		}

		public void Process(Invocation invocation)
		{
			var returnValue = CallOverride(invocation);
			if (implementationOverride.Method.ReturnType != typeof(void) && !this.ignoreDelegateReturnValue)
				invocation.ReturnValue = returnValue;
			invocation.UserProvidedImplementation = true;
		}
	}
}
