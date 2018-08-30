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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.JustMock.Core.MatcherTree
{
	internal class OccurrencesMatcherTreeNode : MatcherTreeNode
	{
		public int Calls { get; set; }
		public HashSet<IMethodMock> Mocks { get; private set; }

		public OccurrencesMatcherTreeNode()
			: base(null)
		{
			Mocks = new HashSet<IMethodMock>();
			Calls = 1;
		}

		public OccurrencesMatcherTreeNode(IMethodMock mock)
			: this()
		{
			if (mock != null)
				Mocks.Add(mock);
		}
		
		public override IMatcherTreeNode Clone()
		{
			return new OccurrencesMatcherTreeNode
			{
				Mocks = new HashSet<IMethodMock>(Mocks),
				Calls = Calls,
			};
		}

		public string GetDebugView()
		{
			var matchers = new List<IMatcherTreeNode>();
			var parent = this.Parent;
			while (!(parent is MethodInfoMatcherTreeNode))
			{
				matchers.Add(parent);
				parent = parent.Parent;
			}
			matchers.Reverse();

			var method = ((MethodInfoMatcherTreeNode)parent).MethodInfo;

			var sb = new StringBuilder();
			bool isInstance = !method.IsStatic || method.IsExtensionMethod();
			var argMatchers = isInstance ? matchers.Skip(1) : matchers;

			if (isInstance)
				sb.AppendFormat("({0}).", matchers[0].Matcher.DebugView);
			else
				sb.AppendFormat("{0}.", method.DeclaringType);

			sb.AppendFormat("{0}({1}) called {2} time{3}; (signature: {4})",
				method.Name,
				", ".Join(argMatchers.Select(m => m.Matcher.DebugView)),
				this.Calls, this.Calls != 1 ? "s" : "",
				method);

			return sb.ToString();
		}
	}
}
