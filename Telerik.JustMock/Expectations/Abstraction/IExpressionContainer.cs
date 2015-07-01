using System.Linq.Expressions;

namespace Telerik.JustMock.Expectations.Abstraction
{
	public interface IExpressionContainer
	{
		Expression Expression { get; }
	}

	public static class ExpressionContainerExtensions
	{
		public static Expression ToLambda(this IExpressionContainer expressionContainer)
		{
			return Expression.Lambda(expressionContainer.Expression);
		}
	}
}
