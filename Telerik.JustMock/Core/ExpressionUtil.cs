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
using Telerik.JustMock.Core.Castle.Core;

namespace Telerik.JustMock.Core
{
	internal static class ExpressionUtil
	{
		public static object EvaluateExpression(this Expression expr)
		{
			while (expr.NodeType == ExpressionType.Convert)
			{
				var unary = expr as UnaryExpression;
				if (unary.Type.IsAssignableFrom(unary.Operand.Type))
					expr = unary.Operand;
				else break;
			}

			var constant = expr as ConstantExpression;
			if (constant != null)
				return constant.Value;

			bool canCallGetField = true;
#if COREFX
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

#if !DOTNET35
			var listInit = expr as ListInitExpression;
			if (listInit != null)
			{
				var collection = Expression.Variable(listInit.NewExpression.Type);

				var block = new List<Expression>
				{
					Expression.Assign(collection, listInit.NewExpression)
				};
				block.AddRange(listInit.Initializers.Select(init => Expression.Call(collection, init.AddMethod, init.Arguments.ToArray())));
				block.Add(collection);

				expr = Expression.Block(new[] { collection }, block.ToArray());
			}
#endif

			var lambda = Expression.Lambda(Expression.Convert(expr, typeof(object)));
			var delg = (Func<object>)lambda.Compile();

			return ProfilerInterceptor.GuardExternal(delg);
		}

		public static object[] GetArgumentsFromConstructorExpression(this LambdaExpression expression)
		{
			var newExpr = (NewExpression)expression.Body;
			return newExpr.Arguments.Select(arg => arg.EvaluateExpression()).ToArray();
		}

		public static string ConvertMockExpressionToString(Expression expr)
		{
			string result = expr.Type.ToString();

			var replacer = new ClosureWithSafeParameterReplacer();
			var lambda = expr as LambdaExpression;
			if (lambda != null)
				expr = lambda.Body;
			expr = replacer.Visit(expr);
			var cleanedExpr = MakeVoidLambda(expr, replacer.Parameters);

			result = cleanedExpr.ToString();
			result = replacer.CleanExpressionString(result);

			return result;
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

		private class ClosureWithSafeParameterReplacer : Telerik.JustMock.Core.Expressions.ExpressionVisitor
		{
			public readonly List<ParameterExpression> Parameters = new List<ParameterExpression>();
			public readonly Dictionary<string, string> ReplacementValues = new Dictionary<string, string>();

			private readonly Dictionary<object, string> stringReplacements = new Dictionary<object, string>(ReferenceEqualityComparer<object>.Instance);

			private bool hasMockReplacement;

			protected override Expression VisitConstant(ConstantExpression c)
			{
				if (c.Type.IsCompilerGenerated())
					return Expression.Parameter(c.Type, "__compiler_generated__");

				if (!hasMockReplacement && c.Type.IsProxy() || (c.Value != null && c.Value.GetType().IsProxy()))
				{
					hasMockReplacement = true;
					var param = Expression.Parameter(c.Type, "x");
					this.Parameters.Add(param);
					return param;
				}

				if (c.Value != null)
				{
					string name;
					var value = c.Value;
					if (!this.stringReplacements.TryGetValue(value, out name))
					{
						name = String.Format("__string_replacement_{0}__", this.stringReplacements.Count);
						this.stringReplacements.Add(value, name);

						var valueStr = "<Exception>";
						try
						{
							valueStr = ProfilerInterceptor.GuardExternal(() => value.ToString());
						}
						catch { }
						if (String.IsNullOrEmpty(valueStr))
						{
							valueStr = "#" + this.stringReplacements.Count;
						}
						this.ReplacementValues.Add(name, valueStr);
					}
					var param = Expression.Parameter(c.Type, name);
					this.Parameters.Add(param);
					return param;
				}

				return base.VisitConstant(c);
			}

			public string CleanExpressionString(string result)
			{
				result = result.Replace("__compiler_generated__.", "");
				foreach (var strReplacement in this.ReplacementValues.Keys)
				{
					result = result.Replace(strReplacement, '(' + this.ReplacementValues[strReplacement] + ')');
				}
				return result;
			}
		}
	}
}
