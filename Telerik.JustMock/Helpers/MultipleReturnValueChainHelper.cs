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
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Expectations;
using Telerik.JustMock.Expectations.Abstraction;

namespace Telerik.JustMock.Helpers
{
	/// <summary>
	/// Sets behavior after last value.
	/// </summary>
	public enum AfterLastValue
	{
		/// <summary>
		/// The last value in the values array will be returned on each call.
		/// </summary>
		KeepReturningLastValue,

		/// <summary>
		/// An assertion failure exception will be thrown on the call after the one that returns the last value.
		/// </summary>
		ThrowAssertionFailed,

		/// <summary>
		/// The member will start returning the same values starting from the beginning.
		/// </summary>
		StartFromBeginning,
	}

	/// <summary>
	/// Provides ability to chain Returns method that returns a single value.
	/// </summary>
	public static class MultipleReturnValueChainHelper
	{
		/// <summary>
		/// Defines the return value for a specific method expectation.
		/// </summary>
		/// <typeparam name="TReturn">Type of the return value.</typeparam>
		/// <param name="assertable">Reference to <see cref="IAssertable" /> interface.</param>
		/// <param name="value">Any object value.</param>
		/// <returns>Reference to <see cref="IMustBeCalled" /> interface</returns>
		public static IAssertable Returns<TReturn>(this IAssertable assertable, TReturn value)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var callPattern = ((IMethodMock)assertable).CallPattern.Clone();
				var func = assertable as IFunc<TReturn>;
				if (func == null)
				{
					var methodReturnType = callPattern.Method.GetReturnType();
					var returnTypeMessage = methodReturnType.IsAssignableFrom(typeof(TReturn))
						? String.Format("The arranged function is not set up to return a value of type {0}.  If this is a non-public arrangement then make sure that the call to Arrange specifies the correct return type, e.g. Mock.NonPublic.Arrange<int>(...) if the method returns 'int'.", typeof(TReturn))
						: String.Format("The chained return value type '{0}' is not compatible with the arranged method's return type '{1}'", typeof(TReturn), methodReturnType);

					throw new MockException(returnTypeMessage);
				}

				var methodMock = MockingContext.CurrentRepository.Arrange(callPattern, () => { return new FuncExpectation<TReturn>(); });
				((IMethodMock) assertable).IsSequential = true;
				((IMethodMock) methodMock).IsSequential = true;
				return methodMock.Returns(value);
			});
		}

		/// <summary>
		/// Specifies that the arranged member will return consecutive values from the given array.
		/// If the arranged member is called after it has returned the last value, an exception is thrown.
		/// </summary>
		/// <typeparam name="TReturn">Type of the return value.</typeparam>
		/// <param name="func">The arranged member.</param>
		/// <param name="values">The array of values that will be returned by the arranged member.</param>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public static IAssertable ReturnsMany<TReturn>(this IFunc<TReturn> func, params TReturn[] values)
		{
			return ProfilerInterceptor.GuardInternal(() => ReturnsMany(func, values, AfterLastValue.KeepReturningLastValue));
		}

		/// <summary>
		/// Specifies that the arranged member will return consecutive values from the given array.
		/// If the arranged member is called after it has returned the last value, the behavior depends on the behavior parameter.
		/// </summary>
		/// <typeparam name="TReturn">Type of return value</typeparam>
		/// <param name="func">The arranged member</param>
		/// <param name="values">The list of values that will be returned by the arranged member. The list may be modified after the arrangement is made.</param>
		/// <param name="behavior">The behavior after the last value has been returned.</param>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public static IAssertable ReturnsMany<TReturn>(this IFunc<TReturn> func, IList<TReturn> values, AfterLastValue behavior)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					if (values == null || values.Count == 0)
						throw new ArgumentException("Expected at least one value to return", "values");

					Action<ReturnsManyImpl<TReturn>> afterEndAction = null;
					switch (behavior)
					{
						case AfterLastValue.ThrowAssertionFailed:
							afterEndAction = impl => MockingContext.Fail("List of arranged return values exhausted.");
							break;
						case AfterLastValue.KeepReturningLastValue:
							afterEndAction = impl => impl.CurrentIndex = values.Count - 1;
							break;
						case AfterLastValue.StartFromBeginning:
							afterEndAction = impl => impl.CurrentIndex = 0;
							break;
						default:
							throw new ArgumentException("behavior");
					}

					return func.Returns(new ReturnsManyImpl<TReturn>(values, afterEndAction).GetNext);
				});
		}

		private class ReturnsManyImpl<TReturn>
		{
			internal int CurrentIndex;
			private readonly IList<TReturn> values;
			private readonly Action<ReturnsManyImpl<TReturn>> afterEndAction;

			public ReturnsManyImpl(IList<TReturn> values, Action<ReturnsManyImpl<TReturn>> afterEndAction)
			{
				this.afterEndAction = afterEndAction;
				this.values = values;
			}

			internal TReturn GetNext()
			{
				if (CurrentIndex >= values.Count)
					afterEndAction(this);

				return values[CurrentIndex++];
			}
		}
	}
}
