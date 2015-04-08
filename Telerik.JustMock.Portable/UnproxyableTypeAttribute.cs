using System;

namespace Telerik.JustMock
{
	/// <summary>
	/// Declares that a type cannot have a proxy generated. This is used in Xamarin.Android where
	/// the reference assembly metadata does not match the Java metadata. After adding instances
	/// of this attribute you need to Rebuild the project.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class UnproxyableTypeAttribute : Attribute
	{
		/// <summary>
		/// Declares an unproxyable type.
		/// </summary>
		/// <param name="mockedType">The type that cannot be proxied.</param>
		public UnproxyableTypeAttribute(Type mockedType)
		{
		}
	}
}
