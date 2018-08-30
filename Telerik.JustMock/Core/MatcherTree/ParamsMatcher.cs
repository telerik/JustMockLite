/*
 JustMock Lite
 Copyright © 2010-2015 Telerik EAD

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
	internal class ParamsMatcher : IMatcher, ICompositeMatcher
	{
		private IMatcher[] matchers;
		public ParamsMatcher(IMatcher[] matchers)
		{
			this.matchers = matchers;
		}

		public bool ProtectRefOut
		{
			get { return false; }
			set { throw new InvalidOperationException(); }
		}

		public string DebugView
		{
			get { return "params[]"; }
		}

		public bool CanMatch(IMatcher matcher)
		{
			return matcher is ICompositeMatcher;
		}

		public bool Matches(IMatcher other)
		{
			var paramsMatcher = other as ParamsMatcher;
			if (paramsMatcher != null)
			{
				if (this.matchers.Length != paramsMatcher.matchers.Length)
					return false;

				for (int i = 0; i < this.matchers.Length; i++)
				{
					if (!this.matchers[i].Matches(paramsMatcher.matchers[i]))
						return false;
				}

				return true;
			}

			var matcher = other as IValueMatcher;
			var array = matcher != null ? matcher.Value as Array: other as Array;

			if (array != null)
			{
				if (array.Length != this.matchers.Length)
					return false;

				for (int i = 0; i < matchers.Length; i++)
				{
					if (!matchers[i].Matches(new ValueMatcher(array.GetValue(i))))
						return false;
				}

				return true;
			}

			return false;
		}

		public bool Equals(IMatcher other)
		{
			var paramsMatcher = other as ParamsMatcher;
			if (paramsMatcher == null)
				return false;

			if (paramsMatcher.matchers.Length != this.matchers.Length)
				return false;

			for(int i=0;i<this.matchers.Length;i++)
			{
				if(!this.matchers[i].Equals(paramsMatcher.matchers[i]))
					return false;
			}

			return true;
		}

		public Expression ToExpression(Type argumentType)
		{
			throw new NotSupportedException();
		}
	}
}
