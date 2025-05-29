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
using System.Reflection;
using Telerik.JustMock.Core;

namespace Telerik.JustMock
{
#if !PORTABLE
    /// <summary>
    /// Provides utility methods for simplifying the creation of expression trees used in mocking scenarios.
    /// This class offers a fluent API for building property access expressions that can be used to configure
    /// mock behavior for property getters and setters.
    /// </summary>
    public static class Expr
    {
        /// <summary>
        /// Creates an <see cref="IPropertyExpressionBuilder{T}"/> for the specified property access expression.
        /// This method analyzes a lambda expression that accesses a property and builds a property expression
        /// that can be used to configure mock behavior for property getters or setters.
        /// </summary>
        /// <typeparam name="T">The type of the property being accessed.</typeparam>
        /// <param name="expression">
        /// A lambda expression representing the property access, such as <c>() => obj.Property</c> for
        /// instance properties or <c>() => Class.StaticProperty</c> for static properties.
        /// </param>
        /// <returns>
        /// An <see cref="IPropertyExpressionBuilder{T}"/> that provides fluent methods for configuring
        /// property getter and setter behavior in mock objects.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the provided expression is not a property access expression. The expression must
        /// be in the form of accessing a property, such as <c>obj.Property</c> or <c>Class.StaticProperty</c>.
        /// </exception>
        /// <exception cref="MockException">
        /// Thrown when attempting to mock a field instead of a property. JustMock only supports mocking
        /// properties, not fields.
        /// </exception>
        /// <example>
        /// <code>
        /// // Setting up a mock for a property getter
        /// Mock.Arrange(Expr.Property(() => mock.SomeProperty)).Returns(expectedValue);
        /// 
        /// // Setting up a mock for a property setter
        /// Mock.Arrange(Expr.Property(() => mock.SomeProperty).Set(Arg.IsAny&lt;string&gt;).DoNothing();
        /// </code>
        /// </example>
        public static IPropertyExpressionBuilder<T> Property<T>(Expression<Func<T>> expression)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var memberExpression = expression.Body;

                if (memberExpression.NodeType != ExpressionType.MemberAccess)
                {
                    throw new ArgumentException("Wrong expression used, property access expression looks like obj.Property or Class.StaticProperty", nameof(expression));
                }

                MemberExpression outerMemberExpression = (MemberExpression)memberExpression;
                if (!(outerMemberExpression.Member is PropertyInfo))
                {
                    throw new MockException("Fields cannot be mocked, only properties.");
                }

                PropertyInfo outerPropertyInfo = (PropertyInfo)outerMemberExpression.Member;
                MemberExpression innerMember = (MemberExpression)outerMemberExpression.Expression;
                Expression objExpression = null;
                Type objType = null;
                if (innerMember != null)
                {
                    FieldInfo innerField = (FieldInfo)innerMember.Member;
                    ConstantExpression innerMemberConstant = (ConstantExpression)innerMember.Expression;
                    object outerObj = innerField.GetValue(innerMemberConstant.Value);
                    objExpression = Expression.Constant(outerObj);
                    objType = outerObj.GetType();
                }
                else
                {
                    objType = outerPropertyInfo.DeclaringType;
                }

                return new PropertyExpressionBuilder<T>(Expression.Property(objExpression, objType, outerPropertyInfo.Name));
            });
        }
    }
#endif
}
