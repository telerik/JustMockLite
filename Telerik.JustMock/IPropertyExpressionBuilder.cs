/*
 JustMock Lite
 Copyright © 2022,2025 Progress Software Corporation

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

namespace Telerik.JustMock
{
#if !PORTABLE
    /// <summary>
    /// Defines methods for building LINQ expressions that represent property get and set operations.
    /// This interface is used to facilitate setting up expectations and verifications for property
    /// access. It provides a strongly-typed approach to creating expressions that can be used
    /// to intercept, verify, and emulate property behavior during testing.
    /// </summary>
    public interface IPropertyExpressionBuilder<T>
    {
        /// <summary>
        /// Builds an expression for property set arrangement with a specific value.
        /// This method creates an expression that represents assigning the specified value to the property,
        /// which can be used to verify that a property was set to a particular value or to arrange
        /// behavior when such an assignment occurs.
        /// </summary>
        /// <param name="value">The exact value to match when the property is set.</param>
        /// <returns>An Action expression representing the property set operation.</returns>
        Expression<Action> Set(T value);

        /// <summary>
        /// Builds an expression for property set arrangement using a lambda.
        /// This method allows for more complex matching logic when arranging property set operations,
        /// such as matching ranges of values or applying custom validation criteria.
        /// </summary>
        /// <param name="expression">A lambda expression that produces the value to match when the property is set.</param>
        /// <returns>An Action expression representing the property set operation with the specified matching logic.</returns>
        Expression<Action> Set(Expression<Func<T>> expression);

        /// <summary>
        /// Builds an expression for property get arrangement.
        /// This method creates an expression that represents retrieving the value of the property,
        /// which can be used to arrange behavior when the property is accessed, such as returning
        /// specific values or throwing exceptions.
        /// </summary>
        /// <returns>A Func expression representing the property get operation.</returns>
        Expression<Func<T>> Get();
    }
#endif
}
