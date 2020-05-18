/*
 JustMock Lite
 Copyright © 2020 Progress Software Corporation

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
	/// Allows specification of a matching condition for an argument for a non-public method, rather
	/// a specific value.
	/// </summary>
    public partial interface IArgExpr
    {
        /// <summary>
        /// Matches argument for any value.
        /// </summary>
        /// <typeparam name="T">Type for the argument</typeparam>
        /// <returns>Argument type</returns>
        Expression IsAny<T>();

        /// <summary>
        /// Matches argument for the expected condition.
        /// </summary>
        /// <typeparam name="T">
        /// Contains the type of the argument.
        /// </typeparam>
        /// <param name="match">Matcher expression</param>
        /// <returns>Argument type</returns>
        Expression Matches<T>(Expression<Predicate<T>> match);

        /// <summary>
		/// Matches argument for null value.
		/// </summary>
		/// <typeparam name="T">Type for the argument</typeparam>
		/// <returns>Argument type</returns>
        Expression IsNull<T>();

        /// <summary>
		/// Returns a value from a ref or out argument.
		/// </summary>
		/// <typeparam name="T">Type for the argument</typeparam>
		/// <param name="value">Value to match.</param>
		/// <returns>Argument type</returns>
        Expression Out<T>(T value);

        /// <summary>
		/// Matches a value for ref argument.
		/// </summary>
		/// <typeparam name="T">Type for the argument</typeparam>
		/// <param name="value">Value to match.</param>
		/// <returns>Argument type</returns>
        Expression Ref<T>(T value);
    }
}
