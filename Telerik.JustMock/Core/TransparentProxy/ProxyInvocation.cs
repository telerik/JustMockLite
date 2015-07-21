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
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using Telerik.JustMock.Core.Castle.DynamicProxy;

namespace Telerik.JustMock.Core.TransparentProxy
{
	internal sealed class ProxyInvocation : IInvocation
	{
		private readonly IMethodCallMessage message;
		private readonly MockingProxy proxy;
		private readonly object[] args;

		public ProxyInvocation(MockingProxy proxy, IMethodCallMessage message)
		{
			this.message = message;
			this.proxy = proxy;
			this.args = message.Args;
		}

		public object[] Arguments
		{
			get { return this.args; }
		}

		public object Proxy
		{
			get { return this.proxy.GetTransparentProxy(); }
		}

		public object ReturnValue { get; set; }

		public Exception Exception { get; set; }

		public MethodInfo GetConcreteMethod()
		{
			var method = (MethodInfo)this.message.MethodBase;
			return method.DeclaringType.IsProxy() ? (MethodInfo)method.GetInheritanceChain().Skip(1).First() : method;
		}

		public void Proceed()
		{
			var returnMsg = RemotingServices.ExecuteMessage(this.proxy.WrappedInstance, this.message);
			if (returnMsg.Exception != null)
			{
				this.Exception = returnMsg.Exception;
			}
			else
			{
				returnMsg.Args.CopyTo(this.args, 0);
				this.ReturnValue = returnMsg.ReturnValue;
			}
		}

		public object GetArgumentValue(int index)
		{
			throw new NotSupportedException();
		}

		public MethodInfo GetConcreteMethodInvocationTarget()
		{
			throw new NotSupportedException();
		}

		public Type[] GenericArguments
		{
			get { throw new NotSupportedException(); }
		}

		public object InvocationTarget
		{
			get { throw new NotSupportedException(); }
		}

		public MethodInfo Method
		{
			get { throw new NotSupportedException(); }
		}

		public MethodInfo MethodInvocationTarget
		{
			get { throw new NotSupportedException(); }
		}

		public void SetArgumentValue(int index, object value)
		{
			throw new NotSupportedException();
		}

		public Type TargetType
		{
			get { throw new NotSupportedException(); }
		}
	}
}
