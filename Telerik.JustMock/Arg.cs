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
using System.Linq.Expressions;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Core.MatcherTree;

namespace Telerik.JustMock
{
    /// <summary>
    /// Provides argument matchers that you can use when you arrange or verify calls.
    /// </summary>
    public static partial class Arg
    {
        private static ArgExprImplementation expr = new ArgExprImplementation();

        /// <summary>
        /// Specifies argument matchers used in non-public method arrangements.
        /// </summary>
        public static IArgExpr Expr
        {
            get
            {
                return ProfilerInterceptor.GuardInternal(() =>
                {
                    return expr;
                });
            }
        }

        /// <summary>
        /// Matches an argument when it satisfies the specified predicate.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <param name="match">Predicate that evaluates the argument value.</param>
        /// <returns>A placeholder value that records the matcher in the current arrangement or assertion.</returns>
        [ArgMatcher(Matcher = typeof(PredicateMatcher<>))]
        public static T Matches<T>(Expression<Predicate<T>> match)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.AddMatcherInContext(new PredicateMatcher<T>(match));
                return default(T);
            });
        }

        /// <summary>
        /// Matches an argument when it falls inside the specified range.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <param name="from">Range start value.</param>
        /// <param name="to">Range end value.</param>
        /// <param name="kind">Range comparison mode.</param>
        /// <returns>A placeholder value that records the matcher in the current arrangement or assertion.</returns>
        [ArgMatcher(Matcher = typeof(RangeMatcher<>))]
        public static T IsInRange<T>(T from, T to, RangeKind kind) where T : IComparable
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.AddMatcherInContext(new RangeMatcher<T>(from, to, kind));
                return default(T);
            });
        }

        /// <summary>
        /// Matches any value of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <returns>A placeholder value that records the matcher in the current arrangement or assertion.</returns>
        [ArgIgnore]
        public static T IsAny<T>()
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.AddMatcherInContext(new TypeMatcher(typeof(T)));
                return default(T);
            });
        }

        /// <summary>
        /// Matches argument for null value.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <returns>A placeholder value that records the matcher in the current arrangement or assertion.</returns>
        [ArgMatcher(Matcher = typeof(ValueMatcher), MatcherArgs = new object[] { null })]
        public static T IsNull<T>()
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.AddMatcherInContext(new ValueMatcher(null));
                return default(T);
            });
        }

        /// <summary>
        /// Matches a string that is null or empty.
        /// </summary>
        /// <returns>A placeholder string that records the matcher in the current arrangement or assertion.</returns>
        [ArgMatcher(Matcher = typeof(StringNullOrEmptyMatcher))]
        public static string NullOrEmpty
        {
            get
            {
                return ProfilerInterceptor.GuardInternal(() =>
                {
                    MockingContext.CurrentRepository.AddMatcherInContext(new StringNullOrEmptyMatcher());
                    return String.Empty;
                });
            }
        }

        /// <summary>
        /// Matches the specified value explicitly.
        /// </summary>
        /// <remarks>
        /// Use this method when you combine concrete values and broader matchers in the same delegate-based arrangement or assertion.
        /// </remarks>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <param name="value">Value to match.</param>
        /// <returns>A placeholder value that records the matcher in the current arrangement or assertion.</returns>
        [ArgMatcher(Matcher = typeof(ValueMatcher))]
        public static T Is<T>(T value)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockingContext.CurrentRepository.AddMatcherInContext(new ValueMatcher(value));
                return default(T);
            });
        }

        /// <summary>
        /// Wraps a value so that you can pass it to a <c>ref</c> or <c>out</c> parameter matcher.
        /// </summary>
        /// <typeparam name="T">Type of the wrapped value.</typeparam>
        public sealed class OutRefResult<T>
        {
            /// <summary>
            /// Gets the wrapped value that you pass by reference.
            /// </summary>
            [RefArg]
            public T Value;
        }

        /// <summary>
        /// Applies a matcher to a <c>ref</c> parameter.
        /// </summary>
        /// <remarks>
        /// By default, JustMock treats <c>ref</c> parameters like arranged output values. Use this method when you want to match
        /// the incoming <c>ref</c> argument in the same way that you match regular arguments.
        /// </remarks>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <example>
        /// interface IHasRef
        /// {
        ///     int PassRef(ref int a);
        /// }
        /// 
        /// var mock = Mock.Create&lt;IHasRef&gt;()
        /// Mock.Arrange(() => mock.PassRef(ref Arg.Ref(100).Value).Returns(200);
        /// 
        /// The above example arranges PassRef to return 200 whenever its argument is 100.
        /// </example>
        /// <param name="value">Concrete value or matcher to apply to the <c>ref</c> argument.</param>
        /// <returns>A wrapper whose <see cref="OutRefResult{T}.Value"/> member must be passed by reference.</returns>
        public static OutRefResult<T> Ref<T>(T value)
        {
            return ProfilerInterceptor.GuardInternal(() => new OutRefResult<T>() { Value = value });
        }
    }
}
