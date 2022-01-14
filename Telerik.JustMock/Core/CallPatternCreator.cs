/*
 JustMock Lite
 Copyright © 2019 Progress Software Corporation

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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Telerik.JustMock.Core.MatcherTree;
using Telerik.JustMock.Core.Recording;

namespace Telerik.JustMock.Core
{
    internal class CallPatternCreator
    {
        internal static CallPattern FromExpression(MocksRepository repository, Expression expr)
        {
            var callPattern = new CallPattern();

            expr = ((LambdaExpression)expr).Body;

            // the expression may end with a boxing conversion, remove that
            while (expr.NodeType == ExpressionType.Convert)
                expr = ((UnaryExpression)expr).Operand;

            Expression target;
            MethodBase method = null;
            Expression[] args;

            // We're parsing either a property/field expression or a method call.
            // parse the top of the expression tree and extract the expressions
            // that will turn into the constituents of the call pattern.
            if (expr is MemberExpression)
            {
                var memberExpr = (MemberExpression)expr;
                if (!(memberExpr.Member is PropertyInfo))
                    throw new MockException("Fields cannot be mocked, only properties.");

                var property = (PropertyInfo)memberExpr.Member;
                target = memberExpr.Expression;
                method = property.GetGetMethod(true);
                args = null;
            }
            else if (expr is MethodCallExpression)
            {
                var methodCall = (MethodCallExpression)expr;

                method = methodCall.Method;
                target = methodCall.Object;
                args = methodCall.Arguments.ToArray();

                if (target != null && !target.Type.IsInterface && !target.Type.IsProxy() && target.Type != method.DeclaringType)
                    method = MockingUtil.GetConcreteImplementer((MethodInfo)method, target.Type);
            }
            else if (expr is NewExpression)
            {
                if (Mock.IsOnDemandEnabled)
                {
                    throw new MockException("Mocking the new operator is not avaiable with OnDemand option enabled. Please use .IgnoreInstance()");
                }

                var newExpr = (NewExpression)expr;

                method = newExpr.Constructor;

                if (method == null && newExpr.Type.IsValueType)
                {
                    throw new MockException("Empty constructor of value type is not associated with runnable code and cannot be intercepted.");
                }

                target = null;
                args = newExpr.Arguments.ToArray();
            }
            else if (expr is InvocationExpression)
            {
                var invocation = (InvocationExpression)expr;
                target = invocation.Expression;
                args = invocation.Arguments.ToArray();
            }
            else if (expr.NodeType == ExpressionType.Assign)
            {
                var binary = (BinaryExpression)expr;
                if (binary.Left is MemberExpression)
                {
                    MemberExpression memberExpr = (MemberExpression)binary.Left;
                    if (!(memberExpr.Member is PropertyInfo))
                        throw new MockException("Fields cannot be mocked, only properties.");

                    var property = (PropertyInfo)memberExpr.Member;
                    target = memberExpr.Expression;
                    method = property.GetSetMethod(true);
                    args = new[] { binary.Right };
                }
                else if (binary.Left is IndexExpression)
                {
                    IndexExpression indexExpr = (IndexExpression)binary.Left;
                    target = indexExpr.Object;
                    method = indexExpr.Indexer.GetSetMethod(true);
                    args = indexExpr.Arguments.Concat(new[] { binary.Right }).ToArray();
                }
                else throw new MockException("Left-hand of assignment is not a member or indexer.");
            }
            else if (expr is IndexExpression)
            {
                var index = (IndexExpression)expr;
                target = index.Object;
                var property = index.Indexer;
                method = property.GetGetMethod(true);
                args = index.Arguments.ToArray();
            }
            else throw new MockException("The expression does not represent a method call, property access, new expression or a delegate invocation.");

            // Create the matcher for the instance part of the call pattern.
            // If the base of the target expression is a new expression (new T()),
            // or null (e.g. (null as T) or ((T) null)), then use AnyMatcher for the instance part,
            // otherwise evaluate the instance expression and use a value matcher with the evaluated result.
            var rootTarget = expr;
            Expression prevToRoot = null;
            while (true)
            {
                var memberExpr = rootTarget as MemberExpression;
                if (memberExpr != null && memberExpr.Expression != null && memberExpr.Member is PropertyInfo)
                {
                    prevToRoot = rootTarget;
                    rootTarget = memberExpr.Expression;
                    continue;
                }

                var callExpr = rootTarget as MethodCallExpression;
                if (callExpr != null && callExpr.Object != null)
                {
                    prevToRoot = rootTarget;
                    rootTarget = callExpr.Object;
                    continue;
                }

                if (rootTarget != null && (rootTarget.NodeType == ExpressionType.Convert || rootTarget.NodeType == ExpressionType.TypeAs))
                {
                    rootTarget = ((UnaryExpression)rootTarget).Operand;
                    continue;
                }

                if (rootTarget is InvocationExpression)
                {
                    prevToRoot = rootTarget;
                    rootTarget = ((InvocationExpression)rootTarget).Expression;
                    continue;
                }

                if (rootTarget is BinaryExpression)
                {
                    prevToRoot = rootTarget;
                    rootTarget = ((BinaryExpression)rootTarget).Left;
                    continue;
                }

                if (rootTarget is IndexExpression)
                {
                    prevToRoot = rootTarget;
                    rootTarget = ((IndexExpression)rootTarget).Object;
                    continue;
                }

                break;
            }

            object targetMockObject = null;
            Type targetMockType = null;
            bool isStatic = false;
            var rootMatcher = MocksRepository.TryCreateMatcherFromArgMember(rootTarget);

            if (rootMatcher != null)
            {
                callPattern.InstanceMatcher = rootMatcher;
            }
            else if (rootTarget is MemberExpression)
            {
                var memberExpr = (MemberExpression)rootTarget;
                targetMockObject = memberExpr.Member is FieldInfo ? memberExpr.EvaluateExpression()
                    : memberExpr.Expression != null ? memberExpr.Expression.EvaluateExpression()
                    : null;
                targetMockType = memberExpr.Member is FieldInfo ? memberExpr.Type : memberExpr.Member.DeclaringType;

                var asPropertyInfo = memberExpr.Member as PropertyInfo;
                isStatic = asPropertyInfo != null
                    ? (asPropertyInfo.GetGetMethod(true) ?? asPropertyInfo.GetSetMethod(true)).IsStatic
                    : false;
            }
            else if (rootTarget is MethodCallExpression)
            {
                var methodCallExpr = (MethodCallExpression)rootTarget;
                targetMockObject = methodCallExpr.Object != null ? methodCallExpr.Object.EvaluateExpression() : null;
                targetMockType = methodCallExpr.Method.DeclaringType;
                isStatic = methodCallExpr.Method.IsStatic;
            }
            else if (rootTarget is NewExpression)
            {
                callPattern.InstanceMatcher = new AnyMatcher();
            }
            else if (rootTarget is ConstantExpression)
            {
                var constant = (ConstantExpression)rootTarget;
                if (constant.Value == null)
                    callPattern.InstanceMatcher = new AnyMatcher();
                else
                {
                    if (constant.Type.IsCompilerGenerated() && prevToRoot != null && prevToRoot.Type != typeof(void))
                    {
                        targetMockObject = prevToRoot.EvaluateExpression();
                        targetMockType = prevToRoot.Type;
                    }
                    else
                    {
                        targetMockObject = constant.Value;
                        targetMockType = constant.Type;
                    }
                }
            }
            if (targetMockObject != null)
                targetMockType = targetMockObject.GetType();

            if (callPattern.InstanceMatcher != null && prevToRoot != expr && prevToRoot != null)
            {
                throw new MockException("Using a matcher for the root member together with recursive mocking is not supported. Arrange the property or method of the root member in a separate statement.");
            }

            if (callPattern.InstanceMatcher == null)
            {
                // TODO: implicit creation of mock mixins shouldn't explicitly refer to behaviors, but
                // should get them from some configuration made outside the Core.
                Debug.Assert(targetMockObject != null || targetMockType != null);
                MockingUtil.UnwrapDelegateTarget(ref targetMockObject);
                var mixin = MocksRepository.GetMockMixin(targetMockObject, targetMockType);
                if (mixin == null)
                {
                    if (isStatic)
                    {
                        MockCreationSettings settings = MockCreationSettings.GetSettings(Behavior.CallOriginal);
                        repository.InterceptStatics(targetMockType, settings, false);
                    }
                    else if (targetMockObject != null)
                    {
                        MockCreationSettings settings = MockCreationSettings.GetSettings(Behavior.CallOriginal);
                        repository.CreateExternalMockMixin(targetMockType, targetMockObject, settings);
                    }
                }

                var targetValue = target != null ? target.EvaluateExpression() : null;

                var delgMethod = MockingUtil.UnwrapDelegateTarget(ref targetValue);
                if (delgMethod != null)
                {
                    method = delgMethod.GetInheritanceChain().First(m => !m.DeclaringType.IsProxy());
                }

                callPattern.InstanceMatcher = new ReferenceMatcher(targetValue);
            }

            // now we have the method part of the call pattern
            Debug.Assert(method != null);
            callPattern.SetMethod(method, checkCompatibility: true);

            //Finally, construct the arguments part of the call pattern.
            using (repository.StartArrangeArgMatching())
            {
                bool hasParams = false;
                bool hasSingleValueInParams = false;

                if (args != null && args.Length > 0)
                {
                    var lastParameter = method.GetParameters().Last();
                    if (Attribute.IsDefined(lastParameter, typeof(ParamArrayAttribute)) && args.Last() is NewArrayExpression)
                    {
                        hasParams = true;
                        var paramsArg = (NewArrayExpression)args.Last();
                        args = args.Take(args.Length - 1).Concat(paramsArg.Expressions).ToArray();
                        if (paramsArg.Expressions.Count == 1)
                            hasSingleValueInParams = true;
                    }

                    foreach (var argument in args)
                    {
                        callPattern.ArgumentMatchers.Add(MocksRepository.CreateMatcherForArgument(argument));
                    }

                    if (hasParams)
                    {
                        int paramsCount = method.GetParameters().Count();
                        if (hasSingleValueInParams)
                        {
                            IMatcher matcher = callPattern.ArgumentMatchers[paramsCount - 1];
                            ITypedMatcher typeMatcher = matcher as ITypedMatcher;
                            if (typeMatcher != null && typeMatcher.Type != method.GetParameters().Last().ParameterType)
                            {
                                callPattern.ArgumentMatchers[paramsCount - 1] = new ParamsMatcher(new IMatcher[] { matcher });
                            }
                        }
                        else
                        {
                            IEnumerable<IMatcher> paramMatchers = callPattern.ArgumentMatchers.Skip(paramsCount - 1).Take(callPattern.ArgumentMatchers.Count - paramsCount + 1);
                            callPattern.ArgumentMatchers = callPattern.ArgumentMatchers.Take(paramsCount - 1).ToList();
                            callPattern.ArgumentMatchers.Add(new ParamsMatcher(paramMatchers.ToArray()));
                        }
                    }
                }
            }

            MethodBase methodFromCallPattern = repository.GetMethodFromCallPattern(callPattern);

            callPattern.AdjustForExtensionMethod();
            callPattern.SetMethod(methodFromCallPattern, checkCompatibility: false);

            return callPattern;
        }

        internal static CallPattern FromMethodBase(MocksRepository repository, object instance, MethodBase method, object[] arguments)
        {
            var callPattern = new CallPattern
            {
                InstanceMatcher =
                   method.IsStatic ? new ReferenceMatcher(null)
                   : instance == null ? (IMatcher)new AnyMatcher()
                   : new ReferenceMatcher(instance),
            };

            callPattern.SetMethod(method, checkCompatibility: true);

            using (repository != null ? repository.StartArrangeArgMatching() : null)
            {
                var parameters = method.GetParameters();
                if (arguments == null || arguments.Length == 0)
                {
                    callPattern.ArgumentMatchers.AddRange(method.GetParameters().Select(p => (IMatcher)new TypeMatcher(p.ParameterType)));
                }
                else
                {
                    if (arguments.Length != method.GetParameters().Length)
                    {
                        throw new MockException("Argument count mismatch.");
                    }

                    callPattern.ArgumentMatchers.AddRange(arguments.Select(arg => MocksRepository.CreateMatcherForArgument(arg)));
                }
            }

            callPattern.AdjustForExtensionMethod();

            return callPattern;
        }

        internal static CallPattern FromMethodBase(object instance, MethodBase method, object[] arguments)
        {
            return FromMethodBase(null, instance, method, arguments);
        }

        internal static CallPattern FromAction(MocksRepository repository, Action memberAction, bool dispatchToMethodMocks = false)
        {
            var callPattern = new CallPattern();

            Invocation lastInvocation = null;

            var recorder = new DelegatingRecorder();
            recorder.Record += invocation => lastInvocation = invocation;
            using (repository.StartRecording(recorder, dispatchToMethodMocks))
            {
                memberAction();
            }

            if (lastInvocation == null)
            {
                throw new MockException("The specified action did not call a mocked method.");
            }

            callPattern.SetMethod(lastInvocation.Method, checkCompatibility: true);
            callPattern.InstanceMatcher = new ReferenceMatcher(lastInvocation.Instance);

            // Because it's impossible to distinguish between a literal value passed as an argument and
            // one coming from a matcher, it is impossible to tell exactly which arguments are literal and which are matchers.
            // So, we assume that the user always first specifies some literal values, and then some matchers.
            // We assume that the user will never pass a literal after a matcher.
            using (repository.StartArrangeArgMatching())
            {
                for (int i = 0; i < lastInvocation.Args.Length; ++i)
                {
                    var indexInMatchers = i - (lastInvocation.Args.Length - repository.MatchersInContext.Count);
                    var matcher = indexInMatchers >= 0 ? repository.MatchersInContext[indexInMatchers] : new ValueMatcher(lastInvocation.Args[i]);
                    callPattern.ArgumentMatchers.Add(matcher);
                }
            }
            repository.MatchersInContext.Clear();

            callPattern.AdjustForExtensionMethod();

            return callPattern;
        }

        internal static CallPattern FromInvocation(Invocation invocation)
        {
            CallPattern callPattern = new CallPattern();
            callPattern.SetMethod(invocation.Method, checkCompatibility: false);
            callPattern.InstanceMatcher = new ReferenceMatcher(invocation.Instance);

            foreach (var argument in invocation.Args)
            {
                callPattern.ArgumentMatchers.Add(new ValueMatcher(argument));
            }

            callPattern.AdjustForExtensionMethod();

            return callPattern;
        }
    }
}
