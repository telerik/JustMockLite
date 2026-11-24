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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telerik.JustMock.Core.TransparentProxy;
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
        private static readonly object nullReferenceKey = new object();
        private Dictionary<object, MatcherTreeNode> referenceOccurrenceChildren;

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
            if (depth == callPattern.ArgumentMatchers.Count+1)
            {
                this.Children.Add(leaf);
                leaf.Parent = this;
                return;
            }

            var matcher = depth == 0 ? callPattern.InstanceMatcher : callPattern.ArgumentMatchers[depth - 1];
            var found = this.GetMatchingChild(matcher, MatchingOptions.Exact, depth);
            if (found != null)
            {
                found.AddChildInternal(callPattern, depth+1, leaf);
            }
            else
            {
                var node = new MatcherTreeNode(matcher);
                Children.Add(node);
                node.Parent = this;
                node.AddChildInternal(callPattern, depth+1, leaf);
            }
        }

        protected void GetMethodMockInternal(CallPattern callPattern, int depth, List<MethodMockMatcherTreeNode> results, MatchingOptions matchingOptions)
        {
            if (depth == callPattern.ArgumentMatchers.Count + 1)
            {
                var resultNode = this.Children.Select(x => x as MethodMockMatcherTreeNode).ToList();
                results.AddRange(resultNode);

                foreach (var result in resultNode)
                {
                    DebugView.TraceEvent(IndentLevel.Matcher, () => String.Format("Found candidate arrangement (id={0}) {1} {2}",
                        result.Id, result.MethodMock.ArrangementExpression,
                        result.MethodMock.IsSequential ? String.Format("(in sequence, used: {0})", result.MethodMock.IsUsed ? "yes" : "no") : ""));
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
            if (depth == callPattern.ArgumentMatchers.Count+1)
            {
                if (this.Children.Count == 0)
                {
                    var leaf = new OccurrencesMatcherTreeNode(mock);
                    this.Children.Add(leaf);
                    leaf.Parent = this;
                }
                else
                {
                    var resultNode = (OccurrencesMatcherTreeNode)this.Children[0];
                    if (mock != null)
                        resultNode.Mocks.Add(mock);
                    resultNode.Calls++;
                }
                return;
            }

            var matcher = depth == 0 ? callPattern.InstanceMatcher : callPattern.ArgumentMatchers[depth - 1];
            var valueMatcher = depth > 0 ? matcher as IValueMatcher : null;
            bool useReference = valueMatcher != null
                && (valueMatcher.Value == null || !valueMatcher.Value.GetType().IsValueType);
            var referenceKey = useReference ? GetReferenceKey(valueMatcher.Value) : null;
            var child = useReference
                ? GetReferenceOccurrenceChild(referenceKey)
                : this.GetMatchingChild(matcher, MatchingOptions.Exact, depth);

            if (child == null)
            {
                child = new MatcherTreeNode(useReference ? new ReferenceMatcher(valueMatcher.Value) : matcher);
                this.Children.Add(child);
                child.Parent = this;
                if (useReference && referenceOccurrenceChildren != null)
                    referenceOccurrenceChildren.Add(referenceKey, child);
            }

            child.AddOrUpdateOccurenceInternal(callPattern, depth + 1, mock);
        }

        private MatcherTreeNode GetReferenceOccurrenceChild(object key)
        {
            // Only argument nodes are indexed. Reset removes whole receiver subtrees, and
            // repository copies lazily rebuild their own indexes from the copied children.
            if (referenceOccurrenceChildren == null && this.Children.Count >= 8)
            {
                referenceOccurrenceChildren = new Dictionary<object, MatcherTreeNode>(ByRefComparer<object>.Instance);
                foreach (var child in this.Children)
                {
                    var referenceMatcher = child.Matcher as ReferenceMatcher;
                    if (referenceMatcher != null
                        && (referenceMatcher.Value == null || !referenceMatcher.Value.GetType().IsValueType))
                        referenceOccurrenceChildren.Add(GetReferenceKey(referenceMatcher.Value), (MatcherTreeNode)child);
                }
            }

            if (referenceOccurrenceChildren != null)
            {
                MatcherTreeNode child;
                referenceOccurrenceChildren.TryGetValue(key, out child);
                return child;
            }

            foreach (var child in this.Children)
            {
                var referenceMatcher = child.Matcher as ReferenceMatcher;
                if (referenceMatcher != null
                    && (referenceMatcher.Value == null || !referenceMatcher.Value.GetType().IsValueType)
                    && ReferenceEquals(GetReferenceKey(referenceMatcher.Value), key))
                    return (MatcherTreeNode)child;
            }
            return null;
        }

        private static object GetReferenceKey(object value)
        {
            return MockingProxy.Unwrap(value) ?? nullReferenceKey;
        }

        protected void GetOccurencesInternal(CallPattern callPattern, int depth, List<OccurrencesMatcherTreeNode> results)
        {
            if (depth == callPattern.ArgumentMatchers.Count+1)
            {
                foreach (OccurrencesMatcherTreeNode node in this.Children)
                {
                    if (NodeMatchesFilter(callPattern, node))
                        results.Add(node);
                }
                return;
            }

            var matcher = depth == 0 ? callPattern.InstanceMatcher : callPattern.ArgumentMatchers[depth-1];
            foreach (MatcherTreeNode child in this.Children)
            {
                if (TraceMatch(matcher, child.Matcher, depth))
                    child.GetOccurencesInternal(callPattern, depth+1, results);
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

            if (!callPattern.Method.IsStatic && filter.Method.GetParameters().Length + 1 == args.Count)
            {
                args.RemoveAt(args.Count - 1);
            }

            args.Reverse();
            var argsArray = args.ToArray();

            object state;
            MockingUtil.BindToMethod(MockingUtil.Default, new[] { filter.Method }, ref argsArray, null, null, null, out state);

            var filterFunc = MockingUtil.MakeFuncCaller(filter);
            var isMatch = (bool) ProfilerInterceptor.GuardExternal(() => filterFunc(argsArray, filter));

            DebugView.TraceEvent(IndentLevel.Matcher, () => String.Format("Matcher predicate {0} call to {2} with arguments ({1})",
                isMatch ? "passed" : "rejected", String.Join(", ", args.Select(x => x.ToString()).ToArray()),
                callPattern.Method));

            return isMatch;
        }

        private IEnumerable<MatcherTreeNode> GetMatchingChildren(IMatcher matcher, MatchingOptions options, int depth)
        {
            switch (options)
            {
                case MatchingOptions.Concretizing:
                    return this.Children.Where(child => TraceMatch(matcher, child.Matcher, depth)).Cast<MatcherTreeNode>();
                case MatchingOptions.Generalizing:
                    return this.Children.Where(child => TraceMatch(child.Matcher, matcher, depth)).Cast<MatcherTreeNode>();
                case MatchingOptions.Exact:
                    return this.Children.Where(child => child.Matcher.Equals(matcher)).Cast<MatcherTreeNode>();
                default:
                    throw new ArgumentException("options");
            }
        }

        private static bool TraceMatch(IMatcher baseMatcher, IMatcher targetMatcher, int depth)
        {
            bool isMatch = baseMatcher.Matches(targetMatcher);

            if (DebugView.IsTraceActive)
                TraceMatchResult(baseMatcher, targetMatcher, depth, isMatch);

            return isMatch;
        }

        private static void TraceMatchResult(IMatcher baseMatcher, IMatcher targetMatcher, int depth, bool isMatch)
        {
            DebugView.TraceEvent(IndentLevel.Matcher, () =>
                String.Format("{3}: {0} -> \"{1}\" {4} \"{2}\"",
                    isMatch ? "Match" : "No match",
                    targetMatcher.DebugView,
                    baseMatcher.DebugView,
                    depth == 0 ? "this" : "arg " + depth,
                    isMatch ? "is" : "is not"));

        }

        private MatcherTreeNode GetMatchingChild(IMatcher matcher, MatchingOptions options, int depth)
        {
            return this.GetMatchingChildren(matcher, options, depth).FirstOrDefault();
        }
    }
}
