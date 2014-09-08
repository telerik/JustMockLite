/*
 JustMock Lite
 Copyright © 2010-2014 Telerik AD

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
using Telerik.JustMock.AutoMock.Ninject;
using Telerik.JustMock.AutoMock.Ninject.Planning.Bindings.Resolvers;
using Telerik.JustMock.AutoMock.Ninject.Syntax;
using Telerik.JustMock.Core;
using Telerik.JustMock.Expectations;
using Telerik.JustMock.Helpers;

namespace Telerik.JustMock.AutoMock
{
	/// <summary>
	/// Entry-point class for auto mocking.
	/// </summary>
	/// <typeparam name="T">The type of the mocked class.</typeparam>
	public sealed class MockingContainer<T> : StandardKernel where T : class
	{
		private T resolvedInstance;
		private IMockResolver mockResolver;

		/// <summary>
		/// Initializes a new instance of the <see cref="MockingContainer{T}" /> class.
		/// </summary>
		/// <param name="settings">Optional settings that modify the way the auto-mocking container works.</param>
		public MockingContainer(AutoMockSettings settings = null)
			: base(settings ?? new AutoMockSettings())
		{
		}

		protected override bool ShouldAddComponent(Type component, Type implementation)
		{
			if (implementation == typeof(SelfBindingResolver))
			{
				return false;
			}

			return base.ShouldAddComponent(component, implementation);
		}

		protected override void AddComponents()
		{
			base.AddComponents();

			this.Components.Add<IMissingBindingResolver, MockResolver>();
			this.Components.Add<IMockResolver, MockResolver>();
			this.mockResolver = this.Components.Get<IMockResolver>();

			if (Settings.ConstructorArgTypes == null)
			{
				this.Bind<T>().ToSelf();
			}
			else
			{
				this.Bind<T>().ToConstructor(CreateConstructorExpression());
			}
		}

		/// <summary>
		/// Gets the kernel settings.
		/// </summary>
		public new AutoMockSettings Settings
		{
			get { return ProfilerInterceptor.GuardInternal(() => (AutoMockSettings)base.Settings); }
		}

		private Expression<Func<IConstructorArgumentSyntax, T>> CreateConstructorExpression()
		{
			var argTypes = Settings.ConstructorArgTypes;
			var constructor = typeof(T).GetConstructor(argTypes);
			if (constructor == null)
				throw new MockException("Constructor with the following parameter types was not found: " + ", ".Join(argTypes));

			var inject = typeof(IConstructorArgumentSyntax).GetMethod("Inject");
			var x = Expression.Parameter(typeof(IConstructorArgumentSyntax), "x");
			var expr = Expression.New(constructor,
				Settings.ConstructorArgTypes.Select(type => Expression.Call(x, inject.MakeGenericMethod(type))).ToArray());

			return (Expression<Func<IConstructorArgumentSyntax, T>>) Expression.Lambda(typeof(Func<IConstructorArgumentSyntax, T>), expr, x);
		}

		/// <summary>
		/// Resolves the instance of the underlying type with all dependencies injected.
		/// </summary>
		public T Instance
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
					{
						if (resolvedInstance == null)
						{
							resolvedInstance = this.Get<T>();
						}
						return resolvedInstance;
					});
			}
		}

		/// <summary>
		/// Entry-point for setting expectations.
		/// </summary>
		/// <typeparam name="TInterface">Mocking interface</typeparam>
		/// <param name="expression">Target expression</param>
		/// <returns>
		/// Reference to <see cref="FuncExpectation{TResult}"/> to setup the mock.
		/// </returns>
		public FuncExpectation<object> Arrange<TInterface>(Expression<Func<TInterface, object>> expression)
		{
			return ProfilerInterceptor.GuardInternal(() => this.Get<TInterface>().Arrange(expression));
		}

		/// <summary>
		/// Entry-point for setting expectations.
		/// </summary>
		/// <typeparam name="TInterface">
		/// Mocking interface
		/// </typeparam>
		/// <param name="expression">Target expression</param>
		/// <param name="take">Specifics the instance to resolve</param>
		/// <returns>
		/// Reference to <see cref="ActionExpectation"/> to setup the mock.
		/// </returns>
		public ActionExpectation Arrange<TInterface>(Expression<Action<TInterface>> expression)
		{
			return ProfilerInterceptor.GuardInternal(() => this.Get<TInterface>().Arrange(expression));
		}

		/// <summary>
		/// Asserts all expected setups.
		/// </summary>
		public void AssertAll()
		{
			ProfilerInterceptor.GuardInternal(() => this.mockResolver.ForEachMock(mock => mock.AssertAll()));
		}

		/// <summary>
		/// Asserts all expected calls that are marked as must or
		/// to be occurred a certain number of times.
		/// </summary>
		public void Assert()
		{
			ProfilerInterceptor.GuardInternal(() => this.mockResolver.ForEachMock(mock => mock.Assert()));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service type.</typeparam>
		/// <param name="expression">Target expression.</param>
		public void Assert<TService>(Expression<Action<TService>> expression)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>().Assert(expression));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service type.</typeparam>
		/// <param name="expression">Target expression</param>
		public void Assert<TService>(Expression<Func<TService, object>> expression)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>().Assert(expression));
		}

		/// <summary>
		/// Asserts a specific dependency
		/// </summary>
		/// <typeparam name="TService">Service type.</typeparam>
		public void Assert<TService>()
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>().Assert());
		}

		/// <summary>
		/// Asserts a specific dependency
		/// </summary>
        /// <param name="bindingName">Name.</param>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="take">Specifies the instance to resolve.</param>
		public void Assert<TService>(string bindingName)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>(bindingName).Assert());
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="expression">Target expression.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public void Assert<TService>(Expression<Func<TService, object>> expression, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>().Assert(expression, occurs));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="expression">Target expression</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public void Assert<TService>(Expression<Action<TService>> expression, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>().Assert(expression, occurs));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
        /// <param name="bindingName">Name.</param>
		/// <param name="expression">Target expression.</param>
		/// <param name="take">Specifies the instance to resolve.</param>
		public void Assert<TService>(string bindingName, Expression<Func<TService, object>> expression)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>(bindingName).Assert(expression));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
        /// <param name="bindingName">Name.</param>
		/// <param name="expression">Target expression.</param>
		/// <param name="take">Specifies the instance to resolve.</param>
		public void Assert<TService>(string bindingName, Expression<Action<TService>> expression)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>(bindingName).Assert(expression));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
        /// <param name="bindingName">Name.</param>
		/// <param name="expression">Target expression.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		/// <param name="take">Specifies the instance to resolve.</param>
		public void Assert<TService>(string bindingName, Expression<Func<TService, object>> expression, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>(bindingName).Assert(expression, occurs));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
        /// <typeparam name="TService">Service Type.</typeparam>
        /// <param name="bindingName">Name.</param>
		/// <param name="expression">Target expression.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		/// <param name="take">Specifies the instance to resolve.</param>
		public void Assert<TService>(string bindingName, Expression<Action<TService>> expression, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>(bindingName).Assert(expression, occurs));
		}
	}
}
