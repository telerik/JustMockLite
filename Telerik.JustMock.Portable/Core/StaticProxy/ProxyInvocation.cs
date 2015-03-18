using System;
using System.Reflection;
using Telerik.JustMock.Core.Castle.DynamicProxy;

namespace Telerik.JustMock.Core.StaticProxy
{
	public sealed class ProxyInvocation : IInvocation
	{
		private readonly Delegate callback;
		private readonly Func<Delegate, object[], object> callbackCaller;
		private readonly RuntimeMethodHandle methodHandle;
		private readonly RuntimeTypeHandle typeHandle;
		private readonly object proxy;
		private readonly object[] arguments;

		public ProxyInvocation(Delegate callback, Func<Delegate, object[], object> callbackCaller,
			RuntimeMethodHandle methodHandle, RuntimeTypeHandle typeHandle, object proxy, object[] arguments)
		{
			this.callback = callback;
			this.callbackCaller = callbackCaller;
			this.methodHandle = methodHandle;
			this.typeHandle = typeHandle;
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
			return (MethodInfo)MethodBase.GetMethodFromHandle(this.methodHandle, this.typeHandle);
		}

		public void Proceed()
		{
			if (callback == null)
				throw new NotImplementedException("Proxied method provides no base implementation.");

			this.ReturnValue = this.callbackCaller(callback, this.Arguments);
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
