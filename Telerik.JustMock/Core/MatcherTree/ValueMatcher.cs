/*
 JustMock Lite
 Copyright © 2010-2015 Progress Software Corporation

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
using System.Runtime.CompilerServices;
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
				var thisType = this.Value.GetType();
				var otherType = valueMatcher.Value.GetType();
				var thisEnumerableType = thisType.GetImplementationOfGenericInterface(typeof(IEnumerable<>));
				var otherEnumerableType = otherType.GetImplementationOfGenericInterface(typeof(IEnumerable<>));
				if (thisEnumerableType != null
					&& thisEnumerableType == otherEnumerableType
					&& IsSystemCollection(thisType)
					&& IsSystemCollection(otherType))
				{
					var elementType = thisEnumerableType.GetGenericArguments()[0];
					var sequenceEqualsMethod = typeof(Enumerable).GetMethods()
						.FirstOrDefault(method => method.Name == "SequenceEqual" && method.GetParameters().Length == 2)
						.MakeGenericMethod(elementType);
					return (bool) sequenceEqualsMethod.Invoke(null, new object[] { this.Value, valueMatcher.Value });
				}
                else if (IsAnonymousType(thisType) && IsAnonymousType(otherType))
                {
                    var thisTypeProperties = thisType.GetProperties();
                    var otherTypeProperties = otherType.GetProperties();
                    if (thisTypeProperties.Length != otherTypeProperties.Length)
                    {
                        return false;
                    }
                    for (int i = 0; i < thisTypeProperties.Length; ++i)
                    {
                        var thisTypeProperty = thisTypeProperties[i];
                        var otherTypeProperty = otherTypeProperties[i];
                        if (!thisTypeProperty.Name.Equals(otherTypeProperty.Name) || !thisTypeProperty.PropertyType.Equals(otherTypeProperty.PropertyType))
                        {
                            return false;
                        }
                        object thisTypePropertyValue = thisTypeProperty.GetGetMethod().Invoke(this.Value, null);
                        object otherTypePropertyValue = otherTypeProperty.GetGetMethod().Invoke(valueMatcher.Value, null);
                        if (!thisTypePropertyValue.Equals(otherTypePropertyValue))
                        {
                            return false;
                        }
                    }

                    return true;
                }
			}

			return false;
		}

        public static Boolean IsAnonymousType(Type type)
        {
            return type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0
                 && type.FullName.Contains("AnonymousType");
        }


        private static bool IsSystemCollection(Type type)
		{
			return type.FullName.StartsWith("System.Collections.") && type.IsClass && !type.IsAbstract
				 || type.IsArray;
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
