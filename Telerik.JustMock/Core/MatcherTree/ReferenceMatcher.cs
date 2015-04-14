/*
 JustMock Lite
 Copyright © 2010-2015 Telerik AD

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
	internal class ReferenceMatcher : CategoricalMatcherBase, IValueMatcher
	{
		private readonly object reference;

		public object Value { get { return reference; } }

		public Type Type { get { return this.Value != null ? this.Value.GetType() : null; } }

		public override string DebugView
		{
			get { return "ByRef " + ValueMatcher.FormatValue(Value); }
		}

		public ReferenceMatcher(object reference)
		{
			this.reference = reference;
		}

		public override bool CanMatch(IMatcher matcher)
		{
			return matcher is IValueMatcher;
		}

		public override bool Equals(IMatcher other)
		{
			var referenceMatcher = other as ReferenceMatcher;
			if (referenceMatcher == null)
				return false;

			return CompareValueTo(other);
		}

		protected override bool MatchesCore(IMatcher other)
		{
			return CompareValueTo(other);
		}

		private bool CompareValueTo(IMatcher other)
		{
			var valueMatcher = other as IValueMatcher;
			if (valueMatcher == null)
				return false;

			if (this.IsValueType)
				return Equals(this.reference, valueMatcher.Value);
			return ReferenceEquals(this.reference, valueMatcher.Value);
		}

		private bool IsValueType
		{
			get { return reference != null && reference.GetType().IsValueType; }
		}

		public override Expression ToExpression(Type argumentType)
		{
			return Expression.Call(null, typeof(ReferenceMatcher).GetMethod("Create"),
				Expression.Constant(this.Value));
		}

		[ArgMatcher(Matcher = typeof(ReferenceMatcher))]
		public static object Create(object value)
		{
			throw new NotSupportedException();
		}
	}
}
