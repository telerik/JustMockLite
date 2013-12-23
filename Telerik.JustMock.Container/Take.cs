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
using System.Linq.Expressions;

namespace Telerik.JustMock.Container
{
	/// <summary>
	/// Defines the auto mocking instance to resolve.
	/// </summary>
	public sealed class Take
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Take" /> class.
		/// </summary>
		/// <param name="predicate">Predicate of the instance.</param>
		public Take(Expression<Func<IServiceLocator, Type, object>> predicate)
		{
			this.predicate = predicate;
		}

		/// <summary>
		/// Specifies to resolve the first instance.
		/// </summary>
		/// <returns>Take configuration.</returns>
		public static Take First()
		{
			return new Take((serviceLocator, x) => serviceLocator.GetAllInstances(x).First());
		}

		/// <summary>
		/// Specifies to resolve the last instance.
		/// </summary>
		/// <returns>Take configuration.</returns>
		public static Take Last()
		{
			return new Take((serviceLocator, x) => serviceLocator.GetAllInstances(x).Last());
		}

		/// <summary>
		/// Specifies to resolve instance at index.
		/// </summary>
		/// <param name="index">Position of the instance that will be taken.</param>
		/// <returns>Take configuration.</returns>
		public static Take At(int index)
		{
			return new Take((serviceLocator, x) => serviceLocator.GetAllInstances(x).Skip(index).First());
		}

		internal TInterface Instance<TInterface>(IServiceLocator serviceLocator)
		{
			return (TInterface) predicate.Compile()(serviceLocator, typeof(TInterface));
		}

		private readonly Expression<Func<IServiceLocator, Type, object>> predicate;
	}
}
