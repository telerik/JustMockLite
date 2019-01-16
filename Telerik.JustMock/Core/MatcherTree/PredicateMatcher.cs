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
using System.Linq;
using System.Linq.Expressions;
using Telerik.JustMock.Core.Expressions;

namespace Telerik.JustMock.Core.MatcherTree
{
	internal class PredicateMatcher<T> : CategoricalMatcherBase, IFunctionalMatcher
	{
		private readonly Predicate<T> predicate;
		private readonly Expression predicateExpression;

		public Type Type { get { return typeof(T); } }

		public override string DebugView
		{
			get { return predicateExpression.ToString(); }
		}
		
		public PredicateMatcher(Expression<Predicate<T>> expression)
		{
			this.predicate = expression.Compile();
			this.predicateExpression = expression;
		}

		public override bool CanMatch(IMatcher matcher)
		{
			return matcher is IValueMatcher;
		}

		protected override bool MatchesCore(IMatcher other)
		{
			var matcher = (IValueMatcher) other;
			var value = matcher.Value;

			if (value == null && typeof(T).IsValueType)
				return false;
			if (value != null && !typeof(T).IsAssignableFrom(matcher.Type))
				return false;

			return ProfilerInterceptor.GuardExternal(() => predicate((T)value));
		}

		public override bool Equals(IMatcher other)
		{
			var predicateMatcher = other as PredicateMatcher<T>;
			if (predicateMatcher == null)
				return false;

			return ExpressionComparer.AreEqual(predicateExpression, predicateMatcher.predicateExpression);
		}

		public override Expression ToExpression(Type argumentType)
		{
			return Expression.Call(null, typeof(PredicateMatcher<T>).GetMethod("Create"),
				Expression.Constant(predicateExpression));
		}

		[ArgMatcher(Matcher = typeof(PredicateMatcher<>))]
		public static T Create(Expression<Predicate<T>> expression)
		{
			throw new NotSupportedException();
		}
	}
}
