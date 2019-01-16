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
	internal class TypeMatcher : CategoricalMatcherBase, ITypedMatcher
	{
		public Type Type { get; private set; }

		public override string DebugView
		{
			get { return String.Format("IsAny<{0}>", Type.GetShortCSharpName()); }
		}
		
		public TypeMatcher(Type type)
		{
			this.Type = type;
		}

		public override bool CanMatch(IMatcher matcher)
		{
			return matcher is ITypedMatcher;
		}

		public override bool Equals(IMatcher other)
		{
			var typeMatcher = other as TypeMatcher;
			if (typeMatcher == null)
				return false;

			return typeMatcher.Type == this.Type;
		}

		protected override bool MatchesCore(IMatcher other)
		{
			var typed = other as ITypedMatcher;
			return (typed.Type == null && (!this.Type.IsValueType || Nullable.GetUnderlyingType(this.Type) != null))
				|| (typed.Type != null && this.Type.IsAssignableFrom(typed.Type));
		}

		public override Expression ToExpression(Type argumentType)
		{
			return Expression.Call(null, typeof(TypeMatcher).GetMethod("Create").MakeGenericMethod(this.Type));
		}

		[ArgIgnore]
		public static T Create<T>()
		{
			throw new NotSupportedException();
		}
	}
}
