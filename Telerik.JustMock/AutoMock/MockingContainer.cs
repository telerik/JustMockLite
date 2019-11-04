/*
 JustMock Lite
 Copyright © 2010-2015,2018 Progress Software Corporation

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
using Telerik.JustMock.AutoMock.Ninject;
using Telerik.JustMock.AutoMock.Ninject.Planning.Bindings.Resolvers;
using Telerik.JustMock.AutoMock.Ninject.Syntax;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Castle.DynamicProxy;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Expectations;
using Telerik.JustMock.Helpers;

namespace Telerik.JustMock.AutoMock
{
	/// <summary>
	/// Auto-mocking container that can automatically inject mocks for all
	/// dependencies of the tested class. The container is based on NInject and
	/// supports the core NInject syntax as well as syntax extensions for arranging
	/// mocks and injecting mocks into properties and constructor arguments.
	/// </summary>
	/// <typeparam name="T">The type of the class whose dependencies should be mocked.
	/// If this is an abstract class, then a Behavior.CallOriginal mock is created for the instance.
	/// Abstract members of the instance can be manipulated using the methods in the Mock class.
	/// </typeparam>
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

		/// <summary>
		/// Implementation detail.
		/// </summary>
		protected override bool ShouldAddComponent(Type component, Type implementation)
		{
			if (implementation == typeof(SelfBindingResolver))
			{
				return false;
			}

			return base.ShouldAddComponent(component, implementation);
		}

		/// <summary>
		/// Implementation detail.
		/// </summary>
		protected override void AddComponents()
		{
			base.AddComponents();

			this.Components.Add<IMissingBindingResolver, MockResolver>();
			this.Components.Add<IMockResolver, MockResolver>();
			this.mockResolver = this.Components.Get<IMockResolver>();

			if (Settings.ConstructorArgTypes == null)
			{
				this.Bind<T>().To(this.ImplementationType);
			}
			else
			{
				this.Bind<T>().ToConstructor(CreateConstructorExpression());
			}
		}

		private Type implementationType;
		private Type ImplementationType
		{
			get
			{
				if (this.implementationType == null)
				{
					var targetType = typeof(T);
					if (targetType.IsAbstract)
					{
						MockCreationSettings settings = MockCreationSettings.GetSettings(Behavior.CallOriginal);
						ProxyTypeInfo typeInfo = MockingContext.CurrentRepository.CreateClassProxyType(targetType, settings);

						this.implementationType = typeInfo.ProxyType;
						foreach (var mixin in typeInfo.Mixins)
						{
							this.Bind(mixin.Key).ToConstant(mixin.Value);
						}
					}
					else
					{
						this.implementationType = targetType;
					}
				}
				return this.implementationType;
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
			var constructor = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, argTypes, null);
			if (constructor == null)
				throw new MockException("Constructor with the following parameter types was not found: " + ", ".Join(argTypes));
			if (!constructor.IsPublic && !constructor.IsFamily && !constructor.IsFamilyOrAssembly)
				throw new MockException("Constructor is not accessible by derived types.");

			var implType = this.ImplementationType;
			if (implType.IsProxy())
			{
				constructor = implType.GetConstructors()
					.Single(ctor => ctor.GetParameters()
						.Select(p => p.ParameterType)
						.SkipWhile(pt => pt != typeof(IInterceptor[]))
						.Skip(1)
						.SequenceEqual(argTypes));
			}

			var inject = typeof(IConstructorArgumentSyntax).GetMethod("Inject");
			var x = Expression.Parameter(typeof(IConstructorArgumentSyntax), "x");
			var expr = Expression.New(constructor,
				constructor.GetParameters().Select(p => Expression.Call(x, inject.MakeGenericMethod(p.ParameterType))).ToArray());

			return (Expression<Func<IConstructorArgumentSyntax, T>>)Expression.Lambda(typeof(Func<IConstructorArgumentSyntax, T>), expr, x);
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
		/// <returns>
		/// Reference to <see cref="ActionExpectation"/> to setup the mock.
		/// </returns>
		public ActionExpectation Arrange<TInterface>(Expression<Action<TInterface>> expression)
		{
			return ProfilerInterceptor.GuardInternal(() => this.Get<TInterface>().Arrange(expression));
		}

		/// <summary>
		/// Entry-point for setting expectations.
		/// </summary>
		/// <typeparam name="TInterface">
		/// Mocking interface
		/// </typeparam>
		/// <param name="action">Target action</param>
		/// <returns>
		/// Reference to <see cref="ActionExpectation"/> to setup the mock.
		/// </returns>
		public ActionExpectation ArrangeSet<TInterface>(Action<TInterface> action)
		{
			return ProfilerInterceptor.GuardInternal(() => this.Get<TInterface>().ArrangeSet(action));
		}

		/// <summary>
		/// Asserts all expected setups.
		/// </summary>
		/// <param name="message">A message to display if the assertion fails.</param>
		public void AssertAll(string message = null)
		{
			ProfilerInterceptor.GuardInternal(() => this.mockResolver.ForEachMock(mock => mock.AssertAll(message)));
		}

		/// <summary>
		/// Asserts all expected calls that are marked as must or
		/// to be occurred a certain number of times.
		/// </summary>
		/// <param name="message">A message to display if the assertion fails.</param>
		public void Assert(string message = null)
		{
			ProfilerInterceptor.GuardInternal(() => this.mockResolver.ForEachMock(mock => mock.Assert(message)));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service type.</typeparam>
		/// <param name="expression">Target expression.</param>
		/// <param name="message">A message to display if the assertion fails.</param>
		public void Assert<TService>(Expression<Action<TService>> expression, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>().Assert(expression, message));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service type.</typeparam>
		/// <param name="expression">Target expression</param>
		/// <param name="message">A message to display if the assertion fails.</param>
		public void Assert<TService>(Expression<Func<TService, object>> expression, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>().Assert(expression, message));
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
		/// <param name="message">A message to display if the assertion fails.</param>
		/// <typeparam name="TService">Service Type.</typeparam>
		public void Assert<TService>(string bindingName, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>(bindingName).Assert(message));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="expression">Target expression.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		/// <param name="message">A message to display if the assertion fails.</param>
		public void Assert<TService>(Expression<Func<TService, object>> expression, Occurs occurs, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>().Assert(expression, occurs, message));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="expression">Target expression</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		/// <param name="message">A message to display if the assertion fails.</param>
		public void Assert<TService>(Expression<Action<TService>> expression, Occurs occurs, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>().Assert(expression, occurs, message));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="bindingName">Name.</param>
		/// <param name="expression">Target expression.</param>
		/// <param name="message">A message to display if the assertion fails.</param>
		public void Assert<TService>(string bindingName, Expression<Func<TService, object>> expression, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>(bindingName).Assert(expression, message));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="bindingName">Name.</param>
		/// <param name="expression">Target expression.</param>
		/// <param name="message">A message to display if the assertion fails.</param>
		public void Assert<TService>(string bindingName, Expression<Action<TService>> expression, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>(bindingName).Assert(expression, message));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="bindingName">Name.</param>
		/// <param name="expression">Target expression.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		/// <param name="message">A message to display if the assertion fails.</param>
		public void Assert<TService>(string bindingName, Expression<Func<TService, object>> expression, Occurs occurs, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>(bindingName).Assert(expression, occurs, message));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="bindingName">Name.</param>
		/// <param name="expression">Target expression.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		/// <param name="message">A message to display if the assertion fails.</param>
		public void Assert<TService>(string bindingName, Expression<Action<TService>> expression, Occurs occurs, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>(bindingName).Assert(expression, occurs, message));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service type.</typeparam>
		/// <param name="action">Target action.</param>
		/// <param name="message">A message to display if the assertion fails.</param>
		public void AssertSet<TService>(Action<TService> action, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>().AssertSet(action, message));
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service type.</typeparam>
		/// <param name="action">Target action.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		/// <param name="message">A message to display if the assertion fails.</param>
		public void AssertSet<TService>(Action<TService> action, Occurs occurs, string message = null)
		{
			ProfilerInterceptor.GuardInternal(() => this.Get<TService>().AssertSet(action, occurs, message));
		}
	}
}
