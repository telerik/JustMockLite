/*
 JustMock Lite
 Copyright © 2022 Progress Software Corporation

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
    /// <summary>
	/// Defines methods for setting and getting property values through an expression.
	/// </summary>
    public interface IPropertyExpressionBuilder<T>
    {
        /// <summary>
        /// Builds and expresison for setting the vlaue of a property.
        /// </summary>
        /// <typeparam name="value">The value that should be set to the property.</typeparam>
        Expression<Action> Set(T value);

        /// <summary>
        /// Builds and expresison for getting the vlaue of a property.
        /// </summary>
        Expression<Func<T>> Get();
    }
}
