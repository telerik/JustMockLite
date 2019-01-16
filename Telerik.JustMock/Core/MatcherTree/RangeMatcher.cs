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
	internal class RangeMatcher<T> : CategoricalMatcherBase, IFunctionalMatcher where T : IComparable
	{
		private readonly T from;
		private readonly T to;
		private readonly RangeKind kind;

		public Type Type { get { return typeof(T); } }

		public override string DebugView
		{
			get { return String.Format("From '{0}' To '{1}' {2}", from, to, kind); }
		}

		public RangeMatcher(T from, T to, RangeKind kind)
		{
			this.from = from;
			this.to = to;
			this.kind = kind;
		}

		public override bool CanMatch(IMatcher matcher)
		{
			return matcher is IValueMatcher || matcher is RangeMatcher<T>;
		}

		protected override bool MatchesCore(IMatcher other)
		{
			var otherRange = other as RangeMatcher<T>;
			if (otherRange != null)
			{
				return (this.kind == otherRange.kind || otherRange.kind == RangeKind.Exclusive)
					? ProfilerInterceptor.GuardExternal(() => this.from.CompareTo(otherRange.from) <= 0 && this.to.CompareTo(otherRange.to) >= 0)
					: ProfilerInterceptor.GuardExternal(() => this.from.CompareTo(otherRange.from) < 0 && this.to.CompareTo(otherRange.to) > 0);
			}

			var matcher = (IValueMatcher) other;
			if (matcher.Value == null)
				return false;
			if (!typeof(T).IsAssignableFrom(matcher.Type))
				return false;

			T val = (T) matcher.Value;
			if (kind == Telerik.JustMock.RangeKind.Inclusive)
			{
				return ProfilerInterceptor.GuardExternal(() => val.CompareTo(from) >= 0 && val.CompareTo(to) <= 0);
			}
			else
			{
				return ProfilerInterceptor.GuardExternal(() => val.CompareTo(from) > 0 && val.CompareTo(to) < 0);
			}
		}

		public override bool Equals(IMatcher other)
		{
			var rangeMatcher = other as RangeMatcher<T>;
			if (rangeMatcher == null)
				return false;

			return rangeMatcher.to.CompareTo(this.to) == 0
				&& rangeMatcher.from.CompareTo(this.from) == 0
				&& rangeMatcher.kind.CompareTo(this.kind) == 0;
		}

		public override Expression ToExpression(Type argumentType)
		{
			return Expression.Call(null, typeof(RangeMatcher<T>).GetMethod("Create"),
				Expression.Constant(this.from), Expression.Constant(this.to), Expression.Constant(this.kind));
		}

		[ArgMatcher(Matcher = typeof(RangeMatcher<>))]
		public static T Create(T from, T to, RangeKind kind)
		{
			throw new NotSupportedException();
		}
	}
}
