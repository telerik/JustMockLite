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
using System.Reflection;
using Telerik.JustMock.Core.Expressions;

namespace Telerik.JustMock.Core
{
    internal static class FunctionalSpecParser
    {
        public interface IReturnArranger
        {
            void ArrangeReturn<T>(Expression<Func<T>> callPattern, LambdaExpression returnDelegate);
        }

        public static void ApplyFunctionalSpec<T>(T mock, Expression<Func<T, bool>> specExpr, IReturnArranger arranger)
        {
            try
            {
                var body = ExpressionReplacer.Replace(specExpr.Body, specExpr.Parameters[0], Expression.Constant(mock, typeof(T)));
                ApplySpecExpression(body, arranger);
            }
            catch (InvalidCastException ex)
            {
                throw new MockException("Incorrect functional spec expression format.", ex);
            }
        }

        private static void ApplySpecExpression(Expression expr, IReturnArranger arranger)
        {
            var asBinary = expr as BinaryExpression;
            if (asBinary != null)
            {
                ApplySpecExpression(asBinary, arranger);
                return;
            }

            if (expr.Type == typeof(bool))
            {
                Expression<Func<bool>> returnDelg = () => true;
                if (expr.NodeType == ExpressionType.Not)
                {
                    expr = ((UnaryExpression)expr).Operand;
                    returnDelg = () => false;
                }
                arranger.ArrangeReturn<bool>((Expression<Func<bool>>)Expression.Lambda(expr), returnDelg);
            }
        }

        private static void ApplySpecExpression(BinaryExpression expr, IReturnArranger arranger)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Equal:
                    {
                        var arrangement = expr.Left;
                        // the expression may end with a boxing conversion, remove that
                        while (arrangement.NodeType == ExpressionType.Convert)
                            arrangement = ((UnaryExpression)arrangement).Operand;

                        var action = expr.Right;
                        ParameterExpression[] parameters = null;

                        // if we're arranging a method, replace Param<T> with actual parameters
                        if (arrangement is MethodCallExpression)
                        {
                            var methodCall = (MethodCallExpression)arrangement;
                            var actionParameters = methodCall.Arguments.Select(arg => Expression.Parameter(arg.Type, "")).ToArray();
                            bool madeReplacements = false;
                            var actionWithParams = ExpressionReplacer.Replace(action,
                                exp =>
                                {
                                    var index = GetParamIndex(exp);
                                    return index != null;
                                },
                                exp =>
                                {
                                    madeReplacements = true;
                                    var index = GetParamIndex(exp);
                                    Expression param = actionParameters[index.Value];
                                    if (param.Type != exp.Type)
                                    {
                                        if (exp.Type != typeof(string))
                                            param = Expression.Convert(param, exp.Type);
                                        else
                                            param = Expression.Call(param, typeof(object).GetMethod("ToString"));
                                    }

                                    return param;
                                });

                            if (madeReplacements)
                            {
                                action = actionWithParams;
                                parameters = actionParameters;
                            }
                        }
                        if (action.Type != arrangement.Type)
                            action = Expression.Convert(action, arrangement.Type);

                        var callPattern = (Expression<Func<object>>)Expression.Lambda(Expression.Convert(arrangement, typeof(object)));
                        var returnLambda = Expression.Lambda(Expression.Convert(action, typeof(object)), parameters);
                        arranger.ArrangeReturn(callPattern, returnLambda);
                    }
                    break;

                case ExpressionType.AndAlso:
                    {
                        ApplySpecExpression(expr.Left, arranger);
                        ApplySpecExpression(expr.Right, arranger);
                    }
                    break;

                default:
                    throw new MockException("Unsupported operation in functional spec: " + expr.NodeType + ". Use only == and &&");
            }
        }

        private static int? GetParamIndex(Expression expr)
        {
            var conversion = expr as UnaryExpression;
            if (conversion != null && conversion.Operand.Type == typeof(Param.EverythingExcept))
                expr = conversion.Operand;

            var fieldExpr = expr as MemberExpression;
            if (fieldExpr == null)
                return null;

            var field = fieldExpr.Member as FieldInfo;
            if (field == null)
                return null;

            if (field.DeclaringType != typeof(Param)
                && (!field.DeclaringType.IsGenericType || field.DeclaringType.GetGenericTypeDefinition() != typeof(Param<>)))
                return null;

            if (field.Name[0] != '_')
                return null;

            int index;
            if (Int32.TryParse(field.Name.Substring(1), out index))
                return index - 1;
            return null;
        }
    }
}
