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
using Microsoft.Practices.ServiceLocation;
using Telerik.JustMock.Container.Abstraction;

namespace Telerik.JustMock.Container
{
	/// <summary>
	/// JustMockServiceLocator class.
	/// </summary>
	public sealed class JustMockServiceLocator : ServiceLocatorImplBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JustMockServiceLocator"/> class.
		/// </summary>
		internal JustMockServiceLocator(IContainer container)
		{
			this.container = container;
		}

		/// <summary>
		/// Gets the container associated with this locator.
		/// </summary>
		internal IContainer Container
		{
			get
			{
				return container;
			}
		}

		/// <summary>
		/// Gets all the instances for a target type.
		/// </summary>
		protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
		{
			if (serviceType != null)
			{
				return container.ResolveAll(serviceType);
			}
			return null;
		}

		/// <summary>
		/// Gets the specific instance for target type.
		/// </summary>
		protected override object DoGetInstance(Type serviceType, string key)
		{
			if (serviceType != null)
			{
				return container.Resolve(serviceType);
			}
			return null;
		}

		private readonly IContainer container;
	}
}
