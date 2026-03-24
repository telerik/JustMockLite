/*
 JustMock Lite
 Copyright © 2010-2015,2018,2025 Progress Software Corporation

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
using System.Text;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Expectations;

namespace Telerik.JustMock
{
    public partial class Mock
    {
        /// <summary>
        /// Arranges a return-value call for the specified type.
        /// </summary>
        /// <typeparam name="T">Type that declares the member to arrange.</typeparam>
        /// <typeparam name="TResult">Return type of the arranged member.</typeparam>
        /// <param name="func">Delegate that performs the call to arrange.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to setup the mock.</returns>
        public static FuncExpectation<TResult> Arrange<T, TResult>(Func<TResult> func)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var repo = MockingContext.CurrentRepository;
                repo.EnableInterception(typeof(T));
                return repo.Arrange(() => func(), () => new FuncExpectation<TResult>());
            });
        }

        /// <summary>
        /// Arranges a return-value call by using an expression.
        /// </summary>
        /// <typeparam name="TResult">Return type of the arranged member.</typeparam>
        /// <param name="expression">Expression that identifies the call to arrange.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to setup the mock.</returns>
        public static FuncExpectation<TResult> Arrange<TResult>(Expression<Func<TResult>> expression)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return MockingContext.CurrentRepository.Arrange(expression, () => new FuncExpectation<TResult>());
            });
        }

        /// <summary>
        /// Arranges a return-value call for a specific instance.
        /// </summary>
        /// <typeparam name="T">Type that declares the member to arrange.</typeparam>
        /// <typeparam name="TResult">Return type of the arranged member.</typeparam>
        /// <param name="obj">Instance on which the arranged call should occur.</param>
        /// <param name="func">Delegate that performs the call to arrange.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to setup the mock.</returns>
        public static FuncExpectation<TResult> Arrange<T, TResult>(T obj, Func<T, TResult> func)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var repo = MockingContext.CurrentRepository;
                repo.EnableInterception(typeof(T));
                return repo.Arrange(() => func(obj), () => new FuncExpectation<TResult>());
            });
        }

        /// <summary>
        /// Arranges a void call for a specific instance.
        /// </summary>
        /// <typeparam name="T">Type that declares the member to arrange.</typeparam>
        /// <param name="obj">Instance on which the arranged call should occur.</param>
        /// <param name="func">Delegate that performs the call to arrange.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to setup the mock.</returns>
        public static ActionExpectation Arrange<T>(T obj, Action<T> action)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var repo = MockingContext.CurrentRepository;
                repo.EnableInterception(typeof(T));
                return repo.Arrange(() => action(obj), () => new ActionExpectation());
            });
        }

        /// <summary>
        /// Arranges a void call by using an expression.
        /// </summary>
        /// <param name="expression">Expression that identifies the call to arrange.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to setup the mock.</returns>
        public static ActionExpectation Arrange(Expression<Action> expression)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                return MockingContext.CurrentRepository.Arrange(expression, () => new ActionExpectation());
            });
        }

        /// <summary>
        /// Arranges a property setter.
        /// <example>
        /// <code>
        /// Mock.ArrangeSet(() => foo.MyValue = 10).Throws(new InvalidOperationException());
        /// </code>
        /// This arrangement throws <see cref="InvalidOperationException"/> when <c>foo.MyValue</c> is set to <c>10</c>.
        /// </example>
        /// </summary>
        /// <remarks>
        /// This overload is not compatible with On Demand interception.
        /// </remarks>
        /// <param name="action">Setter expression that identifies the property assignment to arrange.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to setup the mock.</returns>
        public static ActionExpectation ArrangeSet(Action action)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
#if !PORTABLE
                if (Mock.IsOnDemandEnabled)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("ArrangeSet(Action) is not compatible with the OnDemand feature.");
                    sb.AppendLine("Please use one of these alternatives:");
                    sb.AppendLine("  1. Mock.ArrangeSet<OwnerTypeOfProperty>(Action)");
                    sb.AppendLine("  2. Mock.Arrange(Expression) with a property set expression");
                    sb.AppendLine();
                    sb.AppendLine("Examples:");
                    sb.AppendLine("  Mock.ArrangeSet<MockObject>(() => mockObject.SomeProperty = 5);");
                    sb.AppendLine("  Mock.Arrange(Expr.Property(() => mockObject.SomeProperty).Set(5));");

                    throw new MockException(sb.ToString());
                }
#endif

                return MockingContext.CurrentRepository.Arrange(action, () => new ActionExpectation());
            });
        }

#if !PORTABLE
        /// <summary>
        /// Arranges a property setter and enables interception for the specified declaring type.
        /// <example>
        /// <code>
        /// Mock.ArrangeSet(() => foo.MyValue = 10).Throws(new InvalidOperationException());
        /// </code>
        /// This arrangement throws <see cref="InvalidOperationException"/> when <c>foo.MyValue</c> is set to <c>10</c>.
        /// </example>
        /// </summary>
        /// <typeparam name="T">Type that declares the property to arrange.</typeparam>
        /// <param name="action">Setter expression that identifies the property assignment to arrange.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to setup the mock.</returns>
        public static ActionExpectation ArrangeSet<T>(Action action)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var repo = MockingContext.CurrentRepository;
                repo.EnableInterception(typeof(T));
                return repo.Arrange(action, () => new ActionExpectation());
            });
        }
#endif

        /// <summary>
        /// Arranges methods and properties by applying a functional specification.
        /// </summary>
        /// <typeparam name="T">Mock type.</typeparam>
        /// <param name="mock">Mock on which to apply the specification. If this value is <see langword="null"/>, JustMock applies the specification to all instances.</param>
        /// <param name="functionalSpecification">Functional specification that describes the arranged behavior.</param>
        /// <remarks>
        /// Use this when you want to express behavior as a specification instead of arranging each member separately.
        /// </remarks>
        public static void ArrangeLike<T>(T mock, Expression<Func<T, bool>> functionalSpecification)
        {
            ProfilerInterceptor.GuardInternal(() => FunctionalSpecParser.ApplyFunctionalSpec(mock, functionalSpecification, ReturnArranger.Instance));
        }

        /// <summary>
        /// Arranges a void call for the specified type.
        /// </summary>
        /// <typeparam name="T">Type that declares the member to arrange.</typeparam>
        /// <param name="action">Delegate that performs the call to arrange.</param>
        /// <returns>Reference to <see cref="ActionExpectation"/> to setup the mock.</returns>
        public static ActionExpectation Arrange<T>(Action action)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var repo = MockingContext.CurrentRepository;
                repo.EnableInterception(typeof(T));
                return repo.Arrange(action, () => new ActionExpectation());
            });
        }
    }
}
