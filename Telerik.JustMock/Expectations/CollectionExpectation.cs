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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Behaviors;
using Telerik.JustMock.Expectations.Abstraction;

namespace Telerik.JustMock.Expectations
{
	/// <summary>
	/// Defines expectation for a <see cref="IQueryable"/> collection.
	/// </summary>
	public class CollectionExpectation<TReturn> : CommonExpectation<IFunc<TReturn>>, IReturnCollection
	{
		internal CollectionExpectation() {}

#if !LITE_EDITION

		/// <summary>
		/// Returns a enumerable collecton for the target query.
		/// </summary>
		/// <typeparam name="TArg">Argument type</typeparam>
		/// <typeparam name="TReturn">Return type for the collection</typeparam>
		/// <param name="funcExpectation">Defines the setup instance under which the collection will be set.</param>
		/// <param name="collection">Enumerable colleciton</param>
		/// <returns>Instance of <see cref="IAssertable"/></returns>
		public IAssertable ReturnsCollection<TArg>(IEnumerable<TArg> collection)
		{
			ProfilerInterceptor.GuardInternal(() => ProcessReturnsCollection(collection));
			return this;
		}
#endif

		protected void ProcessReturnsValue(TReturn value)
		{
			this.CheckNotConstructorArrangement();

			this.ProcessDoInstead(new Func<TReturn>(() => value), false);

			if ((object) value != null)
			{
				var mock = MocksRepository.GetMockMixin(value, typeof(TReturn));
				if (mock != null && this.Mock != null)
					this.Mock.DependentMocks.Add(value);
			}
		}

		private void ProcessReturnsCollection(IEnumerable collection)
		{
			this.CheckNotConstructorArrangement();

			var mock = (IMethodMock)this;
			mock.Behaviors.Add(new MockCollectionBehavior(typeof(TReturn), mock.Repository, collection));
		}

		private void CheckNotConstructorArrangement()
		{
			var mock = (IMethodMock)this;
			if (mock.CallPattern.Method.IsConstructor)
			{
				throw new MockException("Arranging the return value of a constructor call is not supported.");
			}
		}
	}
}
