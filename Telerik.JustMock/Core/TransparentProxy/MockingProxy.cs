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
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Telerik.JustMock.Core.Castle.DynamicProxy;

namespace Telerik.JustMock.Core.TransparentProxy
{
	internal sealed class MockingProxy : RealProxy
	{
		public readonly MarshalByRefObject WrappedInstance;

		private readonly IMockMixin mockMixin;
		private readonly IInterceptor interceptor;

		public MockingProxy(MarshalByRefObject wrappedInstance, IInterceptor interceptor, IMockMixin mockMixin)
			: base(wrappedInstance.GetType())
		{
			this.WrappedInstance = wrappedInstance;
			this.interceptor = interceptor;
			this.mockMixin = mockMixin;
		}

		public override IMessage Invoke(IMessage msg)
		{
			var methodCall = msg as IMethodCallMessage;
			if (methodCall == null)
			{
				return null;
			}

			var invocation = new ProxyInvocation(this, methodCall);
			try
			{
				this.interceptor.Intercept(invocation);
			}
			catch (Exception ex)
			{
				invocation.Exception = ex;
			}

			if (invocation.Exception != null)
			{
				return new ReturnMessage(invocation.Exception, methodCall);
			}

			return new ReturnMessage(invocation.ReturnValue,
				invocation.Arguments, invocation.Arguments.Length,
				methodCall.LogicalCallContext, methodCall);
		}

		public static bool CanCreate(Type type)
		{
			return typeof(MarshalByRefObject).IsAssignableFrom(type);
		}

		public static object CreateProxy(object wrappedInstance, MocksRepository repository, IMockMixin mockMixin)
		{
			var realProxy = new MockingProxy((MarshalByRefObject)wrappedInstance, repository.Interceptor, mockMixin);
			return realProxy.GetTransparentProxy();
		}

		public static bool CanIntercept(object instance, MethodBase method)
		{
			return instance != null
				&& RemotingServices.GetRealProxy(instance) is MockingProxy
				&& method is MethodInfo;
		}

		private static MockingProxy GetRealProxy(object instance)
		{
			return instance != null ? RemotingServices.GetRealProxy(instance) as MockingProxy : null;
		}

		public static IMockMixin GetMockMixin(object instance)
		{
			var proxy = GetRealProxy(instance);
			return proxy != null ? proxy.mockMixin : null;
		}

		public static object Unwrap(object maybeProxy)
		{
			var proxy = GetRealProxy(maybeProxy);
			return proxy != null ? proxy.WrappedInstance : maybeProxy;
		}
	}
}
