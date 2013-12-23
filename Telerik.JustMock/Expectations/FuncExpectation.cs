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
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.MatcherTree;
using Telerik.JustMock.Expectations.Abstraction;

namespace Telerik.JustMock.Expectations
{
	/// <summary>
	/// Defines the expectation for a sepcific method.
	/// </summary>
	public partial class FuncExpectation<TReturn> : CollectionExpectation<TReturn>, IFunc<TReturn>, IIgnorable<FuncExpectation<TReturn>>
	{
		internal FuncExpectation() {}
	   
		/// <summary>
		/// Defines the return value for a sepecific method expecation.
		/// </summary>
		/// <param name="value">any object value</param>
		/// <returns></returns>
		public IAssertable Returns(TReturn value)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessReturnsValue(value);
					return this;
				});
		}

		/// <summary>
		/// Specifies the function to evaluate and return.
		/// </summary>
		/// <param name="func">Target function to evaluate</param>
		/// <returns>Refarence to <see cref="IAssertable"/> interface</returns>
		public IAssertable Returns(Func<TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func ?? new Func<TReturn>(() => default(TReturn)), false);
					return this;
				});
		}

		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns(Func<TReturn, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}

		/// <summary>
		/// Specifies to ignore any argument for the target call.
		/// </summary>
		/// <returns>Func or Action Container</returns>
		public FuncExpectation<TReturn> IgnoreArguments()
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ApplyIgnoreArguments();
					return this;
				});
		}

		/// <summary>
		/// Specifies an additional condition that must be true for this arrangement to be
		/// considered when the arranged member is called. This condition is evaluated in addition
		/// to the conditions imposed by any argument matchers in the arrangement.
		/// 
		/// This method allows a more general way of matching arrangements than argument matchers do.
		/// </summary>
		/// <param name="condition">A function that should return 'true' when this
		/// arrangement should be considered and 'false' if this arrangement doesn't match the user criteria.</param>
		public FuncExpectation<TReturn> When(Func<bool> condition)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				((IMethodMock)this).AcceptCondition = condition;
				return this;
			});
		}
	}
}
