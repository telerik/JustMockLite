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

namespace Telerik.JustMock.Container.Abstraction
{
	/// <summary>
	/// Provides an abstraction between service locator and IOC container.
	/// </summary>
	public interface IContainer
	{
		/// <summary>
		/// Registers the specified instance for its corresponding type.
		/// </summary>
		void Register(Type @interface, object instance);

		/// <summary>
		/// Resolves the target instance with dependencies.
		/// </summary>
		object Resolve(Type targetType);

		/// <summary>
		/// Resolves all registered instances for a specific service type.
		/// </summary>
		IList<object> ResolveAll(Type serviceType);
	}
}
