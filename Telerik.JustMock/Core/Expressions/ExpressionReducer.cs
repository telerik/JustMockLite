/*
 JustMock Lite
 Copyright © 2010-2014 Telerik AD

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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Telerik.JustMock.Core.Expressions
{
	internal class ExpressionReducer : ExpressionVisitor
	{
		public static Expression Reduce(Expression expression)
		{
			var depAnalyzer = new ParameterDependencyAnalyzer();
			depAnalyzer.Visit(expression);

			var reducer = new ExpressionReducer(depAnalyzer.DependentExpressions);
			return reducer.Visit(expression);
		}

		private readonly HashSet<Expression> dependentExpressions;

		private ExpressionReducer(HashSet<Expression> dependentExpressions)
		{
			this.dependentExpressions = dependentExpressions;
		}

		public override Expression Visit(Expression exp)
		{
			if (exp != null && !this.dependentExpressions.Contains(exp))
			{
				return Expression.Constant(exp.EvaluateExpression());
			}
			else
			{
				return base.Visit(exp);
			}
		}

		private class ParameterDependencyAnalyzer : ExpressionVisitor
		{
			public readonly HashSet<Expression> DependentExpressions = new HashSet<Expression>();

			private readonly Stack<bool> walkStack = new Stack<bool>();

			public override Expression Visit(Expression exp)
			{
				if (exp == null)
					return null;

				if (exp is ParameterExpression)
				{
					DependentExpressions.Add(exp);
					walkStack.Push(true);
					return exp;
				}

				walkStack.Push(false);

				var count = walkStack.Count;
				var ret = base.Visit(exp);

				if (count < walkStack.Count)
				{
					bool isDependent = false;
					while (walkStack.Count > count)
						isDependent = walkStack.Pop() || isDependent;

					if (isDependent)
					{
						DependentExpressions.Add(exp);
						walkStack.Pop();
						walkStack.Push(true);
					}
				}

				return ret;
			}
		}
	}
}
