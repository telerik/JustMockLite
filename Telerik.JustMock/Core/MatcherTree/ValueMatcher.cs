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
using Telerik.JustMock.Core.Expressions;

namespace Telerik.JustMock.Core.MatcherTree
{
	internal class ValueMatcher : CategoricalMatcherBase, IValueMatcher
	{
		public object Value { get; private set; }
		public Type Type { get { return this.Value != null ? this.Value.GetType() : null; } }

		public override string DebugView
		{
			get { return FormatValue(Value); }
		}

		public ValueMatcher(object value)
		{
			this.Value = value;
		}

		public override bool CanMatch(IMatcher matcher)
		{
			return matcher is IValueMatcher;
		}

		public override bool Equals(IMatcher other)
		{
			var valueMatcher = other as IValueMatcher;
			if (valueMatcher == null)
				return false;

			return MockingUtil.SafeEquals(this.Value, valueMatcher.Value);
		}

		protected override bool MatchesCore(IMatcher other)
		{
			var valueMatcher = (IValueMatcher) other;
			var valueAsExpression = this.Value as Expression;
			var otherValueAsExpression = valueMatcher.Value as Expression;

			if (valueAsExpression != null && otherValueAsExpression != null)
			{
				valueAsExpression = ExpressionReducer.Reduce(valueAsExpression);
				otherValueAsExpression = ExpressionReducer.Reduce(otherValueAsExpression);
				return ExpressionComparer.AreEqual(valueAsExpression, otherValueAsExpression);
			}

			if (this.Value != null && valueMatcher.Value != null)
			{
				var thisEnumerableType = this.Value.GetType().GetImplementationOfGenericInterface(typeof(IEnumerable<>));
				var otherEnumerableType = valueMatcher.Value.GetType().GetImplementationOfGenericInterface(typeof(IEnumerable<>));
				if (thisEnumerableType != null && thisEnumerableType == otherEnumerableType)
				{
					var elementType = thisEnumerableType.GetGenericArguments()[0];
					var sequenceEqualsMethod = typeof(Enumerable).GetMethods()
						.FirstOrDefault(method => method.Name == "SequenceEqual" && method.GetParameters().Length == 2)
						.MakeGenericMethod(elementType);
					return (bool) sequenceEqualsMethod.Invoke(null, new object[] { this.Value, valueMatcher.Value });
				}
			}

			return false;
		}

		public override Expression ToExpression(Type argumentType)
		{
			return Expression.Call(null, typeof(ValueMatcher).GetMethod("Create").MakeGenericMethod(this.Type),
				Expression.Constant(this.Value));
		}

		[ArgMatcher(Matcher = typeof(ValueMatcher))]
		public static T Create<T>(T value)
		{
			throw new NotSupportedException();
		}

		public static string FormatValue(object value)
		{
			if (value == null)
				return "null";

			if (value is string)
				return String.Format("\"{0}\"", value);

			if (value is char)
				return String.Format("'{0}'", value);

			var valueType = MockingUtil.GetUnproxiedType(value);
			string valueStr = valueType.ToString();

			try
			{
				valueStr = value.ToString();
			}
			catch { }

			if (valueStr == valueType.ToString())
				return valueStr;

			return String.Format("({0}) {1}", valueType.GetShortCSharpName(), valueStr);
		}
	}
}
