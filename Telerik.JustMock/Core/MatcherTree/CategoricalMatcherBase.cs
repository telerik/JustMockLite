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
	internal interface IUniverseMatcher
	{ }

	internal interface ITypedMatcher : IUniverseMatcher
	{
		Type Type { get; }
	}

	internal interface IFunctionalMatcher : ITypedMatcher
	{ }

	internal interface ICompositeMatcher : IUniverseMatcher
	{ }

	internal interface IValueMatcher : IFunctionalMatcher, ICompositeMatcher
	{
		object Value { get; }
	}

	internal abstract class CategoricalMatcherBase : IMatcher
	{
		public bool ProtectRefOut { get; set; }

		public abstract bool CanMatch(IMatcher matcher);

		public abstract bool Equals(IMatcher other);

		public abstract Expression ToExpression(Type argumentType);

		public abstract string DebugView { get; }

		protected abstract bool MatchesCore(IMatcher other);

		public bool Matches(IMatcher other)
		{
			if (this.Equals(other))
				return true;

			if (!this.CanMatch(other))
				return false;

			return this.MatchesCore(other);
		}

		public override bool Equals(object obj)
		{
			var matcher = obj as IMatcher;
			return matcher != null && this.Equals(matcher);
		}

		public override int GetHashCode()
		{
			throw new NotSupportedException();
		}
	}
}
