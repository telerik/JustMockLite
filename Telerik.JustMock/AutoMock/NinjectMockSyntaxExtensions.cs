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
using System.Linq.Expressions;
using System.Reflection;
using Telerik.JustMock.AutoMock.Ninject.Infrastructure;
using Telerik.JustMock.AutoMock.Ninject.Planning.Targets;
using Telerik.JustMock.AutoMock.Ninject.Syntax;
using Telerik.JustMock.Core;

namespace Telerik.JustMock.AutoMock
{
    public static class NinjectMockSyntaxExtensions
    {
        /// <summary>
        /// Indicates that the service should be mocked. The mock is activated in the singleton scope.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="builder">The fluent syntax.</param>
        /// <returns>The fluent syntax.</returns>
        public static IBindingWhenInNamedWithOrOnSyntax<T> ToMock<T>(this IBindingToSyntax<T> builder)
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    var result = builder.To<T>();
                    builder.BindingConfiguration.ScopeCallback = StandardScopeCallbacks.Singleton;
                    builder.Kernel.Components.Get<IMockResolver>().AttachToBinding(builder.BindingConfiguration, typeof(T));
                    return result;
                });
        }

        /// <summary>
        /// Specifies the arrangements to make before the mock is injected.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="arrangeActions">A delegate that contains the arrangements to make. Usually,
        /// it will contain a series of Mock.Arrange statements for the necessary arrangements and expectations.</param>
        /// <param name="builder">The fluent syntax.</param>
        /// <returns>The fluent syntax.</returns>
        public static IBindingOnSyntax<T> AndArrange<T>(this IBindingOnSyntax<T> builder, Action<T> arrangeActions)
        {
            return ProfilerInterceptor.GuardInternal(() => builder.OnActivation(arrangeActions));
        }

        /// <summary>
        /// Specifies that the binding should be considered only when injecting into a constructor parameter with the given name.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="builder">The fluent syntax.</param>
        /// <param name="parameterName">The name of the constructor parameter.</param>
        /// <returns>The fluent syntax.</returns>
        public static IBindingInNamedWithOrOnSyntax<T> InjectedIntoParameter<T>(this IBindingWhenSyntax<T> builder, string parameterName)
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    return builder.When(req =>
                    {
                        var target = req.Target as ParameterTarget;
                        return req.Target == null || target != null && target.Site.Name == parameterName;
                    });
                });
        }

        /// <summary>
        /// Specifies that the binding should be considered only when injecting into the given property.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <typeparam name="TTarget">The type defining the property.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="builder">The fluent syntax.</param>
        /// <param name="propertyExpr">An expression referencing the property, e.g. <code>(TransactionService s) => s.CreditCardService</code></param>
        /// <returns>The fluent syntax.</returns>
        public static IBindingInNamedWithOrOnSyntax<T> InjectedIntoProperty<T, TTarget, TProperty>(this IBindingWhenSyntax<T> builder, Expression<Func<TTarget, TProperty>> propertyExpr)
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    MemberInfo member = ((MemberExpression)propertyExpr.Body).Member;
                    if (!(member is PropertyInfo))
                        throw new ArgumentException("Expression should represent a property. Note that field injection is not supported.");

                    return builder.When(req =>
                    {
                        var target = req.Target as PropertyTarget;
                        return req.Target == null || target != null && target.Site == member;
                    });
                });
        }

        /// <summary>
        /// Specifies that the binding should be considered only when injecting into a property with the given name.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="builder">The fluent syntax.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The fluent syntax.</returns>
        public static IBindingInNamedWithOrOnSyntax<T> InjectedIntoProperty<T>(this IBindingWhenSyntax<T> builder, string propertyName)
        {
            return ProfilerInterceptor.GuardInternal(() =>
                {
                    return builder.When(req =>
                    {
                        var target = req.Target as PropertyTarget;
                        return req.Target == null || target != null && target.Site.Name == propertyName;
                    });
                });
        }
    }
}
