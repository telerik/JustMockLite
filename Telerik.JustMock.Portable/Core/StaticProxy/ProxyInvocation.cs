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
using System.Diagnostics;
using System.Reflection;
using Telerik.JustMock.Core.Castle.DynamicProxy;

namespace Telerik.JustMock.Core.StaticProxy
{
	/// <summary>
	/// This type is only used internally.
	/// </summary>
	public sealed class ProxyInvocation : IInvocation
	{
		private readonly Delegate callback;
		private readonly Func<Delegate, object[], object> callbackCaller;
		private readonly RuntimeMethodHandle methodHandle;
		private readonly RuntimeTypeHandle typeHandle;
		private readonly object proxy;
		private readonly object[] arguments;

		/// <summary> </summary>
		[DebuggerHidden]
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

		/// <summary> </summary>
		public object[] Arguments
		{
			get { return this.arguments; }
		}

		/// <summary> </summary>
		public object Proxy
		{
			get { return this.proxy; }
		}

		/// <summary> </summary>
		public object ReturnValue { get; set; }

		/// <summary> </summary>
		public MethodInfo GetConcreteMethod()
		{
			return (MethodInfo)MethodBase.GetMethodFromHandle(this.methodHandle, this.typeHandle);
		}

		/// <summary> </summary>
		public void Proceed()
		{
			if (callback == null)
				throw new NotImplementedException("Proxied method provides no base implementation.");

			this.ReturnValue = this.callbackCaller(callback, this.Arguments);
		}

		/// <summary> </summary>
		public Type[] GenericArguments
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary> </summary>
		public object InvocationTarget
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary> </summary>
		public MethodInfo MethodInvocationTarget
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary> </summary>
		public Type TargetType
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary> </summary>
		public MethodInfo Method
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary> </summary>
		public object GetArgumentValue(int index)
		{
			throw new NotSupportedException();
		}

		/// <summary> </summary>
		public MethodInfo GetConcreteMethodInvocationTarget()
		{
			throw new NotSupportedException();
		}

		/// <summary> </summary>
		public void SetArgumentValue(int index, object value)
		{
			throw new NotSupportedException();
		}
	}
}
