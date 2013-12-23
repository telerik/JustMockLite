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
using System.Reflection;

namespace Telerik.JustMock.Core
{
	internal static class ExpressionUtil
	{
		public static object EvaluateExpression(this Expression expr)
		{
			var constant = expr as ConstantExpression;
			if (constant != null)
				return constant.Value;

			bool canCallGetField = true;
#if SILVERLIGHT
			canCallGetField = ProfilerInterceptor.IsProfilerAttached;
#endif
			if (canCallGetField)
			{
				var memberAccess = expr as MemberExpression;
				if (memberAccess != null)
				{
					var asField = memberAccess.Member as FieldInfo;
					if (asField != null)
					{
						return SecuredReflectionMethods.GetField(asField, memberAccess.Expression != null
							? memberAccess.Expression.EvaluateExpression() : null);
					}
				}
			}

			var lambda = Expression.Lambda(Expression.Convert(expr, typeof(object)));
			var delg = (Func<object>) lambda.Compile();

			return ProfilerInterceptor.GuardExternal(delg);
		}

		public static object[] GetArgumentsFromConstructorExpression(this LambdaExpression expression)
		{
			var newExpr = (NewExpression)expression.Body;
			return newExpr.Arguments.Select(arg => arg.EvaluateExpression()).ToArray();
		}

		public static string ConvertMockExpressionToString(Expression expr)
		{
			var cleanedExpr = ReplaceClosuresWithIdentifiers(expr);
			var str = cleanedExpr.ToString().Replace("__variable__.", "");
			return str;
		}

		private static LambdaExpression ReplaceClosuresWithIdentifiers(Expression expr)
		{
			var replacer = new ClosureWithParameterReplacer();
			var lambda = expr as LambdaExpression;
			if (lambda != null)
				expr = lambda.Body;
			expr = replacer.Visit(expr);
			var identLambda = MakeVoidLambda(expr, replacer.Parameters);
			return identLambda;
		}

		private static LambdaExpression MakeVoidLambda(Expression body, List<ParameterExpression> parameters)
		{
			Type delegateType = null;
			switch (parameters.Count)
			{
				case 0: delegateType = typeof(Action); break;
				case 1: delegateType = typeof(Action<>); break;
				case 2: delegateType = typeof(Action<,>); break;
				case 3: delegateType = typeof(Action<,,>); break;
				case 4: delegateType = typeof(Action<,,,>); break;
				default:
					delegateType = typeof(ExpressionUtil).Assembly.GetType("Telerik.JustMock.Action`" + parameters.Count);
					break;
			}

			if (delegateType != null)
			{
				if (delegateType.IsGenericTypeDefinition)
				{
					delegateType = delegateType.MakeGenericType(parameters.Select(p => p.Type).ToArray());
				}
				return Expression.Lambda(delegateType, body, parameters.ToArray());
			}
			else
				return Expression.Lambda(body, parameters.ToArray());
		}

		private class ClosureWithParameterReplacer : Telerik.JustMock.Core.Expressions.ExpressionVisitor
		{
			public readonly List<ParameterExpression> Parameters = new List<ParameterExpression>();

			protected override Expression VisitConstant(ConstantExpression c)
			{
				if (c.Type.IsCompilerGenerated())
					return Expression.Parameter(c.Type, "__variable__");

				if (c.Type.IsProxy())
				{
					var param = Expression.Parameter(c.Type, "x");
					Parameters.Add(param);
					return param;
				}

				return base.VisitConstant(c);
			}
		}
	}
}
