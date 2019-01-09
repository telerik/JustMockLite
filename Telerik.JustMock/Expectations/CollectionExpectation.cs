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
	public class CollectionExpectation<TReturn> : CommonExpectation<FuncExpectation<TReturn>>, IReturnCollection
	{
		internal CollectionExpectation() { }

#if !LITE_EDITION

		/// <summary>
		/// Returns a enumerable collection for the target query.
		/// </summary>
		/// <typeparam name="TArg">Argument type</typeparam>
		/// <param name="collection">Enumerable collection</param>
		/// <returns>Instance of <see cref="IAssertable"/></returns>
		public IAssertable ReturnsCollection<TArg>(IEnumerable<TArg> collection)
		{
			ProfilerInterceptor.GuardInternal(() => ProcessReturnsCollection(collection));
			return this;
		}
#endif

		/// <summary>
		/// Implementation detail.
		/// </summary>
		/// <param name="value"></param>
		protected void ProcessReturnsValue(TReturn value)
		{
			this.CheckConstructorArrangement();

			this.ProcessDoInstead(new Func<TReturn>(() => value), false);

			if ((object)value != null)
			{
				var mock = MocksRepository.GetMockMixin(value, typeof(TReturn));
				if (mock != null && this.Mock != null)
					this.Mock.DependentMocks.Add(value);
			}
		}

		private void ProcessReturnsCollection(IEnumerable collection)
		{
			this.CheckConstructorArrangement();

			var mock = (IMethodMock)this;
			mock.Behaviors.Add(new MockCollectionBehavior(typeof(TReturn), mock.Repository, collection));
		}

		private void CheckConstructorArrangement()
		{
			var mock = (IMethodMock)this;
			var method = mock.CallPattern.Method;
			if (!method.IsConstructor)
				return;

			if (method.GetParameters().Any(p => p.ParameterType == typeof(IntPtr)))
			{
				throw new MockException("Arranging the return value of a constructor that has an IntPtr argument is not supported.");
			}

			if (method.DeclaringType.IsValueType)
			{
				throw new MockException("Arranging the return value of a constructor call is not supported for value types.");
			}
		}
	}
}
