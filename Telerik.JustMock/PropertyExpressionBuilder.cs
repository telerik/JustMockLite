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
using Telerik.JustMock.Core;

namespace Telerik.JustMock
{
    /// <summary>
    /// Defines methods for setting and getting property values through an expression.
    /// </summary>
    public class PropertyExpressionBuilder<T> : IPropertyExpressionBuilder<T>
    {
        private Expression propertyExpression;

        public PropertyExpressionBuilder(Expression propertyExpression)
        {
            this.propertyExpression = propertyExpression;
        }

        /// <summary>
        /// Builds and expresison for setting the vlaue of a property.
        /// </summary>
        /// <typeparam name="value">The value that should be set to the property.</typeparam>
        public Expression<Action> Set(T value)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return Expression.Lambda<Action>(
                    Expression.Assign(this.propertyExpression, Expression.Constant(value)));
            });
        }

        /// <summary>
        /// Builds and expresison for getting the vlaue of a property.
        /// </summary>
        public Expression<Func<T>> Get()
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return Expression.Lambda<Func<T>>(this.propertyExpression);
            });
        }
    }
}
