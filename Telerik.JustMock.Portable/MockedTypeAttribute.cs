using System;

namespace Telerik.JustMock
{
	/// <summary>
	/// Declares that the proxy generator must emit a proxy for the given type.
	/// This attribute is used whenever the proxy generator cannot infer from that
	/// some type will be proxied.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class MockedTypeAttribute : Attribute
	{
		/// <summary>
		/// An array of interface types that the proxy will implement additionally.
		/// </summary>
		public Type[] Implements { get; set; }

		/// <summary>
		/// Declares the type for which a proxy must be emitted.
		/// </summary>
		/// <param name="mockedType">The type to proxy.</param>
		public MockedTypeAttribute(Type mockedType)
		{
		}
	}
}
