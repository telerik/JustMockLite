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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Telerik.JustMock.Abstraction;
using Telerik.JustMock.Core;

namespace Telerik.JustMock.Setup
{
	internal class FluentConfig<T> : FluentConfig, IFluentConfig<T>
	{
		private readonly List<Type> implementedInterfaces = new List<Type>();

		public IFluentConfig<T> Implements<TInterface>()
		{
			implementedInterfaces.Add(typeof(TInterface));
			return this;
		}

		public IFluentConfig<T> CallConstructor(Expression<Func<T>> expression)
		{
			if (mockConstructor == true)
			{
				throw new MockException("The constructor is already configured to be mocked. Remove the previous call to MockConstructor() if you want to call a constructor.");
			}

			this.arguments = expression.GetArgumentsFromConstructorExpression();
			return this;
		}

		public override object CreateMock(Type mockType, MocksRepository repository)
		{
			return repository.Create(mockType, this.arguments, this.behavior, this.implementedInterfaces.ToArray(),
				this.mockConstructor, this.additionalProxyTypeAttributes, null, null, null, this.interceptorFilter);
		}
	}

	internal class FluentConfig : IFluentConfig
	{
		protected Behavior? behavior;
		protected bool? mockConstructor;
		protected List<CustomAttributeBuilder> additionalProxyTypeAttributes;
		protected Expression<Predicate<MethodInfo>> interceptorFilter;
		protected object[] arguments;

		public IFluentConfig AddAttributeToProxy(CustomAttributeBuilder attributeBuilder)
		{
			if (additionalProxyTypeAttributes == null)
				additionalProxyTypeAttributes = new List<CustomAttributeBuilder>();
			additionalProxyTypeAttributes.Add(attributeBuilder);
			return this;
		}

		public IFluentConfig SetBehavior(Behavior behavior)
		{
			this.behavior = behavior;
			return this;
		}

		public IFluentConfig MockConstructor()
		{
			if (mockConstructor == false)
			{
				throw new MockException("A constructor is already configured to be called. Remove the previous call to CallConstructor() if you want to mock the constructor.");
			}

			this.mockConstructor = true;
			return this;
		}

		public virtual object CreateMock(Type mockType, MocksRepository repository)
		{
			return repository.Create(mockType, this.arguments, this.behavior, null, this.mockConstructor,
				this.additionalProxyTypeAttributes, null, null, null, this.interceptorFilter);
		}

		public IFluentConfig SetInterceptorFilter(Expression<Predicate<MethodInfo>> filter)
		{
			this.interceptorFilter = filter;
			return this;
		}

		public IFluentConfig CallConstructor(object[] args)
		{
			if (mockConstructor == true)
			{
				throw new MockException("The constructor is already configured to be mocked. Remove the previous call to MockConstructor() if you want to call a constructor.");
			}

			this.arguments = args;
			return this;
		}
	}
}
