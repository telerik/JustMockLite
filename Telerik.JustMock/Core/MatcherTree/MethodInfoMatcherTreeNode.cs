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
using System.Reflection;

namespace Telerik.JustMock.Core.MatcherTree
{
	internal class MethodInfoMatcherTreeNode : MatcherTreeNode
	{
		public MethodBase MethodInfo { get; private set; }

		public MethodInfoMatcherTreeNode(MethodBase m)
			: base(null)
		{
			this.MethodInfo = m;
		}

		public override IMatcherTreeNode Clone()
		{
			return new MethodInfoMatcherTreeNode(MethodInfo);
		}

		public void AddChild(CallPattern callPattern, MethodMockMatcherTreeNode node)
		{
			AddChildInternal(callPattern, 0, node);
		}

		public void AddChild(CallPattern callPattern, IMethodMock methodMock,int id)
		{
			var node = new MethodMockMatcherTreeNode(methodMock, id);
			callPattern.MethodMockNode = node;
			AddChildInternal(callPattern, 0, node);
		}

		public List<MethodMockMatcherTreeNode> GetAllMethodMocks(CallPattern callPattern)
		{
			List<MethodMockMatcherTreeNode> results = new List<MethodMockMatcherTreeNode>();
			GetMethodMockInternal(callPattern, 0, results, MatchingOptions.Concretizing);
			return results.ToList();
		}

		public List<MethodMockMatcherTreeNode> GetMethodMock(CallPattern callPattern)
		{
			List<MethodMockMatcherTreeNode> results = new List<MethodMockMatcherTreeNode>();
			GetMethodMockInternal(callPattern, 0, results, MatchingOptions.Generalizing);
			return results;
		}

		public void AddOrUpdateOccurence(CallPattern callPattern, IMethodMock mock)
		{
			AddOrUpdateOccurenceInternal(callPattern, 0, mock);
		}

		public List<OccurrencesMatcherTreeNode> GetOccurences(CallPattern callPattern)
		{
			List<OccurrencesMatcherTreeNode> results = new List<OccurrencesMatcherTreeNode>();
			GetOccurencesInternal(callPattern, 0, results);
			return results;
		}
	}
}
