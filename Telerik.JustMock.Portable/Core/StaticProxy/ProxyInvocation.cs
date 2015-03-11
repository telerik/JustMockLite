using System;
using System.Reflection;
using Telerik.JustMock.Core.Castle.DynamicProxy;

namespace Telerik.JustMock.Core.StaticProxy
{
	public sealed class ProxyInvocation : IInvocation
	{
		private readonly MethodInfo method;
		private readonly object proxy;
		private readonly object[] arguments;
		private readonly Delegate callback;

		public ProxyInvocation(Delegate callback, RuntimeMethodHandle methodHandle, RuntimeTypeHandle typeHandle, object proxy, object[] arguments)
		{
			this.callback = callback;
			this.method = (MethodInfo)MethodBase.GetMethodFromHandle(methodHandle, typeHandle);
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
			if (callback == null)
				throw new NotImplementedException("Proxied method provides no base implementation.");

			//TODO: generate invocation code statically as an optimization
			var invoke = callback.GetType().GetMethod("Invoke");
			this.ReturnValue = invoke.Invoke(callback, this.Arguments);
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
