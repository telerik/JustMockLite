using System.Linq.Expressions;

namespace Telerik.JustMock.Expectations.Abstraction
{
	public interface IExpressionContainer
	{
		Expression Expression { get; }
	}
}
