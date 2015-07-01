using System.Dynamic;
using System.Linq.Expressions;
using Telerik.JustMock.Expectations.Abstraction;

namespace Telerik.JustMock.Expectations.DynaMock
{
	public class ExpressionContainer : IDynamicMetaObjectProvider, IExpressionContainer
	{
		public Expression Expression { get; set; }

		public bool IsStatic { get; set; }

		public ExpressionContainer(Expression expression)
		{
			this.Expression = expression;
		}

		public DynamicMetaObject GetMetaObject(Expression parameter)
		{
			return new ExpressionRecorder(parameter, BindingRestrictions.Empty, this);
		}
	}
}
