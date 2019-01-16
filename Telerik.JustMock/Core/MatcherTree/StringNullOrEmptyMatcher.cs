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

namespace Telerik.JustMock.Core.MatcherTree
{
	internal class StringNullOrEmptyMatcher : CategoricalMatcherBase, IFunctionalMatcher
	{
		public Type Type { get { return typeof(string); } }

		public override string DebugView
		{
			get { return "null or empty string"; }
		}

		public override bool CanMatch(IMatcher matcher)
		{
			return matcher is IValueMatcher;
		}

		protected override bool MatchesCore(IMatcher other)
		{
			var valueMatcher = (IValueMatcher)other;
			var value = valueMatcher.Value;
			return value == null || (value as string) == String.Empty;
		}

		public override bool Equals(IMatcher other)
		{
			return other is StringNullOrEmptyMatcher;
		}

		public override Expression ToExpression(Type argumentType)
		{
			return Expression.Call(null, typeof(StringNullOrEmptyMatcher).GetMethod("Create"));
		}

		[ArgMatcher(Matcher = typeof(StringNullOrEmptyMatcher))]
		public static string Create()
		{
			throw new NotSupportedException();
		}
	}
}
