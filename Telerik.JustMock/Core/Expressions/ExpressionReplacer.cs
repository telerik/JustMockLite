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
using System.Linq;
using System.Linq.Expressions;

namespace Telerik.JustMock.Core.Expressions
{
    internal class ExpressionReplacer : ExpressionVisitor
    {
        private Predicate<Expression> searchExpression;
        private Func<Expression, Expression> replaceExpression;

        public static Expression Replace(Expression source, Expression searchExpr, Expression replaceExpr)
        {
            return Replace(source, exp => Equals(exp, searchExpr), exp => replaceExpr);
        }

        public static Expression Replace(Expression source, Predicate<Expression> searchPred, Func<Expression, Expression> replaceFunc)
        {
            var replacer = new ExpressionReplacer { searchExpression = searchPred, replaceExpression = replaceFunc };
            return replacer.Visit(source);
        }

        public override Expression Visit(Expression exp)
        {
            return this.searchExpression(exp) ? this.replaceExpression(exp) : base.Visit(exp);
        }
    }
}
