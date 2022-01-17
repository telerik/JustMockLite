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
using System.Reflection;
using Telerik.JustMock.Core;

namespace Telerik.JustMock
{
#if !PORTABLE
    /// <summary>
    /// Defines helper methods used for easily building expressions.
    /// </summary>
    public static class Expr
    {
        /// <summary>
        /// Creates a <see cref="IPropertyExpressionBuilder<T>"/> from an expression.
        /// Commonly used to easily build an expression for a property set.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        public static IPropertyExpressionBuilder<T> Property<T>(Expression<Func<T>> expression)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var memberExpression = expression.Body;

                if (memberExpression.NodeType != ExpressionType.MemberAccess)
                {
                    throw new MockException("Wrong expression used, property access expression looks like obj.Property or Class.StaticProperty");

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
