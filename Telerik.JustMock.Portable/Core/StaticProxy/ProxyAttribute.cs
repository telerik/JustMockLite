using System;

namespace Telerik.JustMock.Core.StaticProxy
{
	/// <summary>
	/// This attribute is only used internally.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ProxyAttribute : Attribute
	{
		/// <summary> </summary>
		public Type ProxiedType { get; private set; }

		/// <summary> </summary>
		public Type[] Mixins { get; set; }

		/// <summary> </summary>
		public Type[] AdditionalInterfaces { get; set; }

		/// <summary> </summary>
		/// <param name="proxiedType"></param>
		public ProxyAttribute(Type proxiedType)
		{
			this.ProxiedType = proxiedType;
		}
	}
}
