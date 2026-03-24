/*
 JustMock Lite
 Copyright © 2020,2022 Progress Software Corporation

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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock.Core.MatcherTree;

namespace Telerik.JustMock
{
    /// <summary>
    /// Defines expression-based argument matchers for non-public member arrangements.
    /// </summary>
    public partial interface IArgExpr
    {
        /// <summary>
        /// Matches any value of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <returns>An expression that represents the matcher.</returns>
        Expression IsAny<T>();

        /// <summary>
        /// Matches any value of the specified runtime type.
        /// </summary>
        /// <param name="type">Type of the argument.</param>
        /// <param name="args">Constructor arguments for matcher creation when required.</param>
        /// <returns>An expression that represents the matcher.</returns>
        Expression IsAny(Type type, params object[] args);

        /// <summary>
        /// Matches an argument when it satisfies the specified predicate.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <param name="match">Predicate that evaluates the argument value.</param>
        /// <returns>An expression that represents the matcher.</returns>
        Expression Matches<T>(Expression<Predicate<T>> match);

        /// <summary>
        /// Matches a <see langword="null"/> value.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <returns>An expression that represents the matcher.</returns>
        Expression IsNull<T>();

        /// <summary>
        /// Supplies a value for a <c>ref</c> or <c>out</c> argument.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <param name="value">Value to assign.</param>
        /// <returns>An expression that represents the argument value.</returns>
        Expression Out<T>(T value);

        /// <summary>
        /// Matches a <c>ref</c> argument by value.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <param name="value">Value to match.</param>
        /// <returns>An expression that represents the matcher.</returns>
        Expression Ref<T>(T value);
    }
}
