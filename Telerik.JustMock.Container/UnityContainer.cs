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
using System.Reflection;
using Microsoft.Practices.Unity;
using Telerik.JustMock.Container.Abstraction;

namespace Telerik.JustMock.Container
{
	/// <summary>
	/// Wrapper over original unity container.
	/// </summary>
	public class UnityContainer : IContainer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UnityContainer" /> class.
		/// </summary>
		/// <param name="dependenciesType">Type of the dependencies.</param>
		public UnityContainer(params Type[] dependenciesType)
		{
			this.dependenciesType = dependenciesType;
			this.container = new Microsoft.Practices.Unity.UnityContainer();
		}

		/// <summary>
		/// Registers the specified instance for its corresponding type.
		/// </summary>
		/// <param name="serviceType">Corresponding type.</param>
		/// <param name="instance">Instance to be registered.</param>
		public void Register(Type serviceType, object instance)
		{
			string instanceName = string.Format("{0}+{1}", serviceType.Name, instance.GetHashCode());

			if (!container.IsRegistered(serviceType, instanceName))
			{
				container.RegisterInstance(serviceType, instanceName, instance);
			}
		}

		/// <summary>
		/// Resolve the target type with necessary dependencies.
		/// </summary>
		/// <param name="serviceType">Service type.</param>
		/// <returns>Resolved object.</returns>
		public object Resolve(Type serviceType)
		{
			var instance = ResolveAll(serviceType).FirstOrDefault();
			if (instance != null)
				return instance;

			if (!serviceType.IsInterface)
			{
				var constructor = serviceType.GetConstructor(dependenciesType);

				if (constructor == null && dependenciesType.Length == 0)
					constructor = serviceType.GetConstructors().FirstOrDefault();

				if (constructor.GetParameters().Length > 0)
					return ResolveInstance(constructor);
				else
					return constructor.Invoke(null);
			}

			return null;
		}

		private object ResolveInstance(ConstructorInfo constructor)
		{
			var parameterInfos = constructor.GetParameters();

			var parameters = new List<object>();

			int count = 0;
			Type lastParameterType = null;

			for (int index = 0; index < parameterInfos.Length; index++)
			{
				Type parameterType = parameterInfos[index].ParameterType;
				if (lastParameterType != parameterType)
					count = 0;

				IList<object> instances = ResolveAll(parameterType);

				// add the concrete type directly if not registered.
				if (instances.Count > 0)
					parameters.Add(instances[count++]);
				else if (parameterType.IsClass)
					parameters.Add(ResolveInstance(parameterType.GetConstructors().First()));

				lastParameterType = parameterType;
			}

			return constructor.Invoke(parameters.ToArray());
		}

		/// <summary>
		/// Resolves all registered instances for a specific service type.
		/// </summary>
		/// <param name="serviceType">Service type.</param>
		/// <returns>Returns collection of the resolved objects.</returns>
		public IList<object> ResolveAll(Type serviceType)
		{
			return container.ResolveAll(serviceType).ToList();
		}

		private readonly IUnityContainer container;
		private readonly Type[] dependenciesType;
	}
}
