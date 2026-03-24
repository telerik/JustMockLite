/*
 JustMock Lite
 Copyright © 2010-2015,2022 Progress Software Corporation

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
using System.ComponentModel;
using System.Linq.Expressions;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.MatcherTree;

namespace Telerik.JustMock
{
    /// <summary>
    /// Provides expression-based argument matchers for non-public member arrangements.
    /// </summary>
#if !PORTABLE
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public partial class ArgExpr
    {
        internal ArgExpr() { }

        /// <summary>
        /// Matches any value of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <returns>An expression that represents the matcher.</returns>
        public static Expression IsAny<T>()
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Expression<Func<T>> expr = () => Arg.IsAny<T>();
                return expr.Body;
            });
        }

        /// <summary>
        /// Matches any value of the specified type.
        /// </summary>
        /// <param name="type">Type of the argument.</param>
        /// <param name="args">Constructor arguments for matcher creation when required.</param>
        /// <returns>An expression that represents the matcher.</returns>
        public static Expression IsAny(Type type, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Expression<Func<object>> expr = () => Arg.IsAny(type, args);
                return expr.Body;
            });
        }

        /// <summary>
        /// Matches an argument when it satisfies the specified predicate.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <param name="match">Predicate that evaluates the argument value.</param>
        /// <returns>An expression that represents the matcher.</returns>
        public static Expression Matches<T>(Expression<Predicate<T>> match)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Expression<Func<T>> expr = () => Arg.Matches<T>(match);
                return expr.Body;
            });
        }

        /// <summary>
        /// Matches a <see langword="null"/> value.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <returns>An expression that represents the matcher.</returns>
        public static Expression IsNull<T>()
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Expression<Func<T>> expr = () => Arg.IsNull<T>();
                return expr.Body;
            });
        }

        /// <summary>
        /// Supplies a value for a <c>ref</c> or <c>out</c> argument.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <param name="value">Value to assign.</param>
        /// <returns>An expression that represents the argument value.</returns>
        public static Expression Out<T>(T value)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Expression<Func<T>> expr = () => OutArg(value);
                return expr.Body;
            });
        }

        [OutArg]
        private static T OutArg<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Matches a <c>ref</c> argument by value.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <param name="value">Value to match.</param>
        /// <returns>An expression that represents the matcher.</returns>
        public static Expression Ref<T>(T value)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Expression<Func<T>> expr = () => Arg.Ref<T>(value).Value;
                return expr.Body;
            });
        }
    }
}
