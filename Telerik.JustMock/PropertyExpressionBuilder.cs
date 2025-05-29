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
using Telerik.JustMock.Core;

namespace Telerik.JustMock
{
#if !PORTABLE
    /// <summary>
    /// Defines methods for setting and getting property values through an expression.
    /// </summary>
    internal class PropertyExpressionBuilder<T> : IPropertyExpressionBuilder<T>
    {
        private Expression propertyExpression;

        public PropertyExpressionBuilder(Expression propertyExpression)
        {
            this.propertyExpression = propertyExpression;
        }

        public Expression<Action> Set(T value)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return Expression.Lambda<Action>(
                    Expression.Assign(this.propertyExpression, Expression.Constant(value, typeof(T))));
            });
        }

        public Expression<Action> Set(Expression<Func<T>> expression)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return Expression.Lambda<Action>(
                    Expression.Assign(this.propertyExpression, expression.Body));
            });
        }

        public Expression<Func<T>> Get()
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return Expression.Lambda<Func<T>>(this.propertyExpression);
            });
        }
    }
#endif
}
