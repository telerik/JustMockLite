using Telerik.JustMock.Expectations.Abstraction.Local.Function;

namespace Telerik.JustMock.Expectations.Abstraction.Local
{
	public interface ILocalExpectation
	{
		/// <summary>
		/// Arrange and assert expectations on C# 7 local functions.
		/// </summary>
		IFunctionExpectation Function { get; }
	}
}