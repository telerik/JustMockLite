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
using System.Linq;
using System.Linq.Expressions;

namespace Telerik.JustMock.Core.MatcherTree
{
	internal class AnyMatcher : CategoricalMatcherBase, IUniverseMatcher
	{
		public override string DebugView
		{
			get { return "any"; }
		}

		public override bool CanMatch(IMatcher other)
		{
			return true;
		}

		protected override bool MatchesCore(IMatcher other)
		{
			return true;
		}

		public override bool Equals(IMatcher other)
		{
			return other is AnyMatcher;
		}

		public override Expression ToExpression(Type argumentType)
		{
			return Expression.Call(null, typeof(AnyMatcher).GetMethod("Create").MakeGenericMethod(argumentType));
		}

		[ArgMatcher(Matcher = typeof(AnyMatcher))]
		public static T Create<T>()
		{
			throw new NotSupportedException();
		}
	}
}
