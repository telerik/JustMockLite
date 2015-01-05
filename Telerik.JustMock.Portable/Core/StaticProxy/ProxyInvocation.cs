using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Telerik.JustMock.Core.Castle.DynamicProxy;

namespace Telerik.JustMock.Core.StaticProxy
{
	public sealed class ProxyInvocation : IInvocation
	{
		private readonly MethodInfo method;
		private readonly object proxy;
		private readonly object[] arguments;

		public ProxyInvocation(RuntimeMethodHandle methodHandle, object proxy, object[] arguments)
		{
			this.method = (MethodInfo)MethodBase.GetMethodFromHandle(methodHandle);
			this.proxy = proxy;
			this.arguments = arguments;
		}

		public object[] Arguments
		{
			get { return this.arguments; }
		}

		public object Proxy
		{
			get { return this.proxy; }
		}

		public object ReturnValue { get; set; }

		public MethodInfo GetConcreteMethod()
		{
			return this.method;
		}

		public void Proceed()
		{
			throw new NotImplementedException();
		}

		public Type[] GenericArguments
		{
			get { throw new NotSupportedException(); }
		}

		public object InvocationTarget
		{
			get { throw new NotSupportedException(); }
		}

		public MethodInfo MethodInvocationTarget
		{
			get { throw new NotSupportedException(); }
		}

		public Type TargetType
		{
			get { throw new NotSupportedException(); }
		}

		public MethodInfo Method
		{
			get { throw new NotSupportedException(); }
		}

		public object GetArgumentValue(int index)
		{
			throw new NotSupportedException();
		}

		public MethodInfo GetConcreteMethodInvocationTarget()
		{
			throw new NotSupportedException();
		}

		public void SetArgumentValue(int index, object value)
		{
			throw new NotSupportedException();
		}
	}
}
