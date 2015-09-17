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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telerik.JustMock.Diagnostics;

namespace Telerik.JustMock.Core.MatcherTree
{
	internal enum MatchingOptions
	{
		/// <summary>
		/// Specifies that the provided call pattern is a specific case
		/// and we're interested in known call patterns who are general cases of the provided one.
		/// </summary>
		Generalizing,
		/// <summary>
		/// Specifies that the provided call pattern is a general case
		/// and we're interested in known call patterns who are specific cases of the provided one.
		/// </summary>
		Concretizing,
		/// <summary>
		/// Specifies that we want only exact matches between the provided call pattern
		/// and known call patterns.
		/// </summary>
		Exact,
	}

	internal class MatcherTreeNode : IMatcherTreeNode
	{
		public MatcherTreeNode(IMatcher matcher)
		{
			Matcher = matcher;
			Children = new List<IMatcherTreeNode>();
		}

		public IMatcherTreeNode Parent { get; set; }
		public List<IMatcherTreeNode> Children { get; private set; }

		public IMatcher Matcher { get; set; }

		public virtual IMatcherTreeNode Clone()
		{
			return new MatcherTreeNode(Matcher);
		}

		protected void AddChildInternal(CallPattern callPattern, int depth, MatcherTreeNode leaf)
		{
			if (depth == callPattern.ArgumentMatchers.Count + 1)
			{
				this.Children.Add(leaf);
				leaf.Parent = this;
				return;
			}

			var matcher = depth == 0 ? callPattern.InstanceMatcher : callPattern.ArgumentMatchers[depth - 1];
			var found = this.GetMatchingChild(matcher, MatchingOptions.Exact, depth);
			if (found != null)
			{
				found.AddChildInternal(callPattern, depth + 1, leaf);
			}
			else
			{
				var node = new MatcherTreeNode(matcher);
				Children.Add(node);
				node.Parent = this;
				node.AddChildInternal(callPattern, depth + 1, leaf);
			}
		}

		protected void GetMethodMockInternal(CallPattern callPattern, int depth, List<MethodMockMatcherTreeNode> results, MatchingOptions matchingOptions)
		{
			if (depth == callPattern.ArgumentMatchers.Count + 1)
			{
				var resultNode = this.Children.OfType<MethodMockMatcherTreeNode>().ToList();
				if (resultNode.Count != 0)
				{
					results.AddRange(resultNode);

					foreach (var result in resultNode)
					{
						DebugView.TraceEvent(IndentLevel.Matcher, () => String.Format("Found candidate arrangement (id={0}) {1} {2}",
							result.Id, result.MethodMock.ArrangementExpression,
							result.MethodMock.IsSequential ? String.Format("(in sequence, used: {0})", result.MethodMock.IsUsed ? "yes" : "no") : ""));
					}

				}
				return;
			}

			var matcher = depth == 0 ? callPattern.InstanceMatcher : callPattern.ArgumentMatchers[depth - 1];
			var children = this.GetMatchingChildren(matcher, matchingOptions, depth);

			foreach (var child in children)
			{
				child.GetMethodMockInternal(callPattern, depth + 1, results, matchingOptions);
			}
		}

		protected void AddOrUpdateOccurenceInternal(CallPattern callPattern, int depth, IMethodMock mock)
		{
			if (depth == callPattern.ArgumentMatchers.Count + 1)
			{
				var resultNode = this.Children.FirstOrDefault() as OccurrencesMatcherTreeNode;
				if (resultNode != null)
				{
					if (mock != null)
						resultNode.Mocks.Add(mock);
					resultNode.Calls++;
				}
				return;
			}

			var matcher = depth == 0 ? callPattern.InstanceMatcher : callPattern.ArgumentMatchers[depth - 1];
			var child = this.GetMatchingChild(matcher, MatchingOptions.Exact, depth);

			if (child != null)
			{
				child.AddOrUpdateOccurenceInternal(callPattern, depth + 1, mock);
			}
			else
			{
				this.AddChildInternal(callPattern, depth, new OccurrencesMatcherTreeNode(mock));
			}
		}

		protected void GetOccurencesInternal(CallPattern callPattern, int depth, List<OccurrencesMatcherTreeNode> results)
		{
			if (depth == callPattern.ArgumentMatchers.Count + 1)
			{
				var resultNode = this.Children.OfType<OccurrencesMatcherTreeNode>()
					.Where(node => NodeMatchesFilter(callPattern, node));
				results.AddRange(resultNode);
				return;
			}

			var matcher = depth == 0 ? callPattern.InstanceMatcher : callPattern.ArgumentMatchers[depth - 1];
			var children = this.GetMatchingChildren(matcher, MatchingOptions.Concretizing, depth);
			foreach (var child in children)
			{
				child.GetOccurencesInternal(callPattern, depth + 1, results);
			}
		}

		private static bool NodeMatchesFilter(CallPattern callPattern, IMatcherTreeNode node)
		{
			var filter = callPattern.Filter;
			if (filter == null)
				return true;

			var args = new List<object>();
			var nodeIter = node;
			while (nodeIter != null)
			{
				var valueMatcher = nodeIter.Matcher as IValueMatcher;
				if (valueMatcher != null)
				{
					args.Add(valueMatcher.Value);
				}
				nodeIter = nodeIter.Parent;
			}

			if (!callPattern.Member.IsStatic() && filter.Method.GetParameters().Length + 1 == args.Count)
			{
				args.RemoveAt(args.Count - 1);
			}

			args.Reverse();
			var argsArray = args.ToArray();

			object state;
			MockingUtil.BindToMethod(MockingUtil.Default, new[] { filter.Method }, ref argsArray, null, null, null, out state);

			var filterFunc = MockingUtil.MakeFuncCaller(filter);
			var isMatch = (bool)ProfilerInterceptor.GuardExternal(() => filterFunc(argsArray, filter));

			DebugView.TraceEvent(IndentLevel.Matcher, () => String.Format("Matcher predicate {0} call to {2} with arguments ({1})",
				isMatch ? "passed" : "rejected", String.Join(", ", args.Select(x => x.ToString()).ToArray()),
				callPattern.Member));

			return isMatch;
		}

		private IEnumerable<MatcherTreeNode> GetMatchingChildren(IMatcher matcher, MatchingOptions options, int depth)
		{
			var matchableChildren = this.Children.Where(child => child.Matcher != null);
			switch (options)
			{
				case MatchingOptions.Concretizing:
					return matchableChildren.Where(child => TraceMatch(matcher, child.Matcher, depth)).Cast<MatcherTreeNode>();
				case MatchingOptions.Generalizing:
					return matchableChildren.Where(child => TraceMatch(child.Matcher, matcher, depth)).Cast<MatcherTreeNode>();
				case MatchingOptions.Exact:
					return matchableChildren.Where(child => child.Matcher.Equals(matcher)).Cast<MatcherTreeNode>();
				default:
					throw new ArgumentException("options");
			}
		}

		private static bool TraceMatch(IMatcher baseMatcher, IMatcher targetMatcher, int depth)
		{
			bool isMatch = baseMatcher.Matches(targetMatcher);

			DebugView.TraceEvent(IndentLevel.Matcher, () =>
				String.Format("{3}: {0} -> \"{1}\" {4} \"{2}\"",
					isMatch ? "Match" : "No match",
					targetMatcher.DebugView,
					baseMatcher.DebugView,
					depth == 0 ? "this" : "arg " + depth,
					isMatch ? "is" : "is not"));

			return isMatch;
		}

		private MatcherTreeNode GetMatchingChild(IMatcher matcher, MatchingOptions options, int depth)
		{
			return this.GetMatchingChildren(matcher, options, depth).FirstOrDefault();
		}
	}
}
