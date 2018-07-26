using Telerik.JustMock.Expectations.Abstraction.Local.Function;

namespace Telerik.JustMock.Expectations.Abstraction.Local
{

	public interface ILocalExpectation
	{
		IFunctionExpectation Function { get; }
	}
}