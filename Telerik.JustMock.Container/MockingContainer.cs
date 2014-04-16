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
using Microsoft.Practices.ServiceLocation;
using Telerik.JustMock.Container.Abstraction;
using Telerik.JustMock.Helpers;
using Telerik.JustMock.Expectations;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;

namespace Telerik.JustMock.Container
{
	/// <summary>
	/// Entry-point class for auto mocking.
	/// </summary>
	/// <typeparam name="T">The type of the mocked class.</typeparam>
	[Obsolete]
	public class MockingContainer<T> where T : class
	{
		private readonly IServiceLocator serviceLocator;

		private T instance;

		/// <summary>
		/// Initializes a new instance of the <see cref="MockingContainer{T}" /> class.
		/// <param name="dependenciesType">Specifies the type(s) on which the constructor is dependent.
		/// <remarks>Empty for resolving container with default/first constructor.</remarks></param>
		/// </summary>
		/// <param name="dependenciesType">Type of the dependencies.</param>
		public MockingContainer(params Type[] dependenciesType)
		{
			this.container = new UnityContainer(dependenciesType);

			this.serviceLocator = new JustMockServiceLocator(container);

			this.dependencies = new List<object>();

			RegisterDependencies(typeof(T));
		}

		/// <summary>
		/// Resolves the instance of the underlying type with all dependencies injected.
		/// </summary>
		public T Instance
		{
			get
			{
				if (instance == null)
				{
					instance = (T)container.Resolve(typeof(T));
				}
				return instance;
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
			return this.serviceLocator.GetInstance<TInterface>().Arrange(expression);
		}

		/// <summary>
		/// Entry-point for setting expectations.
		/// </summary>
		/// <typeparam name="TInterface">Mocking interface</typeparam>
		/// <param name="expression">Target expression</param>
		/// <param name="take">Specifics the instance to resolve.</param>
		/// <returns>
		/// Reference to <see cref="FuncExpectation{TResult}" /> to setup the mock.
		/// </returns>
		public FuncExpectation<object> Arrange<TInterface>(Expression<Func<TInterface, object>> expression, Take take)
		{
			return take.Instance<TInterface>(this.serviceLocator).Arrange(expression);
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
			return this.serviceLocator.GetInstance<TInterface>().Arrange(expression);
		}

		/// <summary>
		/// Entry-point for setting expectations.
		/// </summary>
		/// <typeparam name="TInterface">
		/// Mocking interface
		/// </typeparam>
		/// <param name="expression">Target expression</param>
		/// <param name="take">Specifics the instance to resolve.</param>
		/// <returns>
		/// Reference to <see cref="ActionExpectation"/> to setup the mock.
		/// </returns>
		public ActionExpectation Arrange<TInterface>(Expression<Action<TInterface>> expression, Take take)
		{
			return take.Instance<TInterface>(this.serviceLocator).Arrange(expression);
		}

		/// <summary>
		/// Asserts all expected setups.
		/// </summary>
		public void AssertAll()
		{
			foreach (Type serviceType in dependencies)
			{
				this.serviceLocator.GetAllInstances(serviceType).ToList().ForEach(x => x.AssertAll());
			}
		}

		/// <summary>
		/// Asserts all expected calls that are marked as must or
		/// to be occurred a certain number of times.
		/// </summary>
		public void Assert()
		{
			foreach (Type serviceType in dependencies)
			{
				this.serviceLocator.GetAllInstances(serviceType).ToList().ForEach(x => x.Assert());
			}
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service type.</typeparam>
		/// <param name="expression">Target expression.</param>
		public void Assert<TService>(Expression<Action<TService>> expression)
		{
			this.serviceLocator.GetInstance<TService>().Assert(expression);
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service type.</typeparam>
		/// <param name="expression">Target expression</param>
		public void Assert<TService>(Expression<Func<TService, object>> expression)
		{
			Assert(expression, Occurs.NotAvailable());
		}

		/// <summary>
		/// Asserts a specific dependency
		/// </summary>
		/// <typeparam name="TService">Service type.</typeparam>
		public void Assert<TService>()
		{
			this.serviceLocator.GetInstance<TService>().Assert();
		}

		/// <summary>
		/// Asserts a specific dependency
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="take">Specifies the instance to resolve.</param>
		public void Assert<TService>(Take take)
		{
			take.Instance<TService>(this.serviceLocator).Assert();
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="expression">Target expression.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public void Assert<TService>(Expression<Func<TService, object>> expression, Occurs occurs)
		{
			this.serviceLocator.GetInstance<TService>().Assert(expression, occurs);
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="expression">Target expression</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public void Assert<TService>(Expression<Action<TService>> expression, Occurs occurs)
		{
			this.serviceLocator.GetInstance<TService>().Assert(expression, occurs);
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="expression">Target expression.</param>
		/// <param name="take">Specifies the instance to resolve.</param>
		public void Assert<TService>(Expression<Func<TService, object>> expression, Take take)
		{
			take.Instance<TService>(this.serviceLocator).Assert(expression);
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="expression">Target expression.</param>
		/// <param name="take">Specifies the instance to resolve.</param>
		public void Assert<TService>(Expression<Action<TService>> expression, Take take)
		{
			take.Instance<TService>(this.serviceLocator).Assert(expression);
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="expression">Target expression.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		/// <param name="take">Specifies the instance to resolve.</param>
		public void Assert<TService>(Expression<Func<TService, object>> expression, Occurs occurs, Take take)
		{
			take.Instance<TService>(this.serviceLocator).Assert(expression, occurs);
		}

		/// <summary>
		/// Asserts the specific call
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <param name="expression">Target expression.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		/// <param name="take">Specifies the instance to resolve.</param>
		public void Assert<TService>(Expression<Action<TService>> expression, Occurs occurs, Take take)
		{
			take.Instance<TService>(this.serviceLocator).Assert(expression, occurs);
		}

		/// <summary>
		/// Returns the registered service instance.
		/// </summary>
		/// <typeparam name="TService">Service Type.</typeparam>
		/// <returns>The resolved instance.</returns>
		public TService Get<TService>()
		{
			return this.serviceLocator.GetInstance<TService>();
		}

		/// <summary>
		/// Returns the registered service instance.
		/// </summary>
		/// <typeparam name="TService">The type of instance to resolve.</typeparam>
		/// <param name="take">Specifies the kind of instance to resolve.</param>
		/// <returns>The resolved instance.</returns>
		public TService Get<TService>(Take take)
		{
			return take.Instance<TService>(this.serviceLocator);
		}

		private void RegisterDependencies(Type targetType, params Type[] dependenciesType)
		{
			foreach (var constructor in targetType.GetConstructors())
			{
				var parameterInfos = constructor.GetParameters();
				foreach (var parameterInfo in parameterInfos)
				{
					RegisterService(parameterInfo.ParameterType);
				}
			}

		}

		private void RegisterService(Type serviceType)
		{
			if (!serviceType.IsInterface && serviceType.GetConstructors().Length > 0)
			{
				if (serviceType.GetConstructor(Type.EmptyTypes) == null)
				{
					RegisterDependencies(serviceType);
					AppendToDependenciesList(serviceType);
					return;
				}
			}

			if (serviceType.IsSealed && !Mock.IsProfilerEnabled)
				throw new AutoMockException(Messages.ProfilerMustBeEnabled);

			container.Register(serviceType, Mock.Create(serviceType));

			AppendToDependenciesList(serviceType);
		}

		private void AppendToDependenciesList(Type serviceType)
		{
			if (!dependencies.Contains(serviceType))
				dependencies.Add(serviceType);
		}

		private readonly IList dependencies;
		private readonly IContainer container;
	}
}
