using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.JustMock.Core.StaticProxy
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ProxyAttribute : Attribute
	{
		public Type ProxiedType { get; private set; }

		public Type[] Mixins { get; set; }

		public Type[] AdditionalInterfaces { get; set; }

		public ProxyAttribute(Type proxiedType)
		{
			this.ProxiedType = proxiedType;
		}
	}
}
