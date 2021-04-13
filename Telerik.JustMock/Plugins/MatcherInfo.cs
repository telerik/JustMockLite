/*
 JustMock Lite
 Copyright © 2021 Progress Software Corporation

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
using System.Reflection;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.MatcherTree;

namespace Telerik.JustMock.Plugins
{
    public class MatcherInfo
    {
        public enum MatcherKind
        {
            Unknown,
            Any,
            Value,
            Type,
            NullOrEmpty,
            Range,
            Predicate,
            Params
        }

        public MatcherKind Kind { get; private set; }
        public Type ArgType { get; private set; }
        public int ArgPosition { get; private set; }
        public string ArgName { get; private set; }
        public bool IsParamsArg { get; private set; }
        public string ExpressionString { get; private set; }

        private MatcherInfo(MatcherKind kind, Type argType, int argPosition, string argName, string expressionString)
        {
            this.Kind = kind;
            this.ArgType = argType;
            this.ArgPosition = argPosition;
            this.ArgName = argName;
            this.ExpressionString = expressionString;
        }

        public static MatcherInfo FromMatcherAndParamInfo(object matcherObject, ParameterInfo paramInfo, out MatcherInfo[] paramsMatchers)
        {
            var kind = MatcherKind.Unknown;
            var argType = typeof(void);
            var expressionString = "n/a";
            paramsMatchers = null;

            ITypedMatcher typedMatcher;
            if (MockingUtil.TryGetAs(matcherObject, out typedMatcher))
            {
                if (matcherObject.GetType() == typeof(ValueMatcher))
                {
                    kind = MatcherKind.Value;
                }
                else if (matcherObject.GetType() == typeof(TypeMatcher))
                {
                    kind = MatcherKind.Type;
                }
                else if (matcherObject.GetType() == typeof(StringNullOrEmptyMatcher))
                {
                    kind = MatcherKind.NullOrEmpty;
                }
                else if (matcherObject.GetType() == typeof(RangeMatcher<>).MakeGenericType(typedMatcher.Type))
                {
                    kind = MatcherKind.Range;
                }
                else if (matcherObject.GetType() == typeof(PredicateMatcher<>).MakeGenericType(typedMatcher.Type))
                {
                    kind = MatcherKind.Predicate;
                }

                if (kind != MatcherKind.Unknown)
                {
                    IMatcher matcher;
                    if (MockingUtil.TryGetAs(matcherObject, out matcher))
                    {
                        argType =
                            typedMatcher.Type != null
                                ?
                                typedMatcher.Type
                                :
                                paramInfo.ParameterType.IsArray
                                    ?
                                        paramInfo.ParameterType.GetElementType()
                                        :
                                        paramInfo.ParameterType;
                        expressionString = matcher.DebugView;
                    }
                }
            }
            else if (matcherObject.GetType() == typeof(ParamsMatcher))
            {
                IContainerMatcher containerMatcher;
                if (MockingUtil.TryGetAs<IContainerMatcher>(matcherObject, out containerMatcher))
                {
                    kind = MatcherKind.Params;
                    paramsMatchers = containerMatcher.Matchers.Select(
                        contained =>
                            {
                                MatcherInfo[] dummy;
                                var result = FromMatcherAndParamInfo(contained, paramInfo, out dummy);
                                result.IsParamsArg = true;
                                return result;
                            })
                        .ToArray();
                }
            }
            else if (matcherObject.GetType() == typeof(AnyMatcher))
            {
                kind = MatcherKind.Any;
            }

            return new MatcherInfo(kind, argType, paramInfo.Position, paramInfo.Name, expressionString);
        }
    }
}
