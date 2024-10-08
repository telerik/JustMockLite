/*
 JustMock Lite
 Copyright © 2022, 2024 Progress Software Corporation

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
using System.Threading.Tasks;
using Telerik.JustMock.Core;
using Telerik.JustMock.Expectations;
using Telerik.JustMock.Expectations.Abstraction;

namespace Telerik.JustMock
{
    /// <summary>
	/// Defines the expectation for a specific method.
	/// </summary>
    public static partial class AsyncExtensions
    {
        /// <summary>
		/// Specifies the return value for an asynchronous method.
		/// </summary>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<TResult>(this FuncExpectation<Task<TResult>> mock, TResult value)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                mock.ReturnsAsync(() => value);
                return mock;
            });
        }
        
        /// <summary>
		/// Specifies the return value for an asynchronous method.
		/// </summary>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<TResult>(this FuncExpectation<Task> mock, TResult value)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                mock.ReturnsAsync(() => value);
                return mock;
            });
        }
        
        /// <summary>
        /// Specifies a function to evaluate and return the value for an asynchronous method.
        /// </summary>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<TResult>(this FuncExpectation<Task<TResult>> mock, Func<TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns(() => Task.FromResult(valueFunction()));
            });
        }


         /// <summary>
        /// Specifies a function to evaluate and return the value for an asynchronous method.
        /// </summary>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<TResult>(this FuncExpectation<Task> mock, Func<TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns(() => Task.FromResult(valueFunction()));
            });
        }

#if NETCORE
        /// <summary>
		/// Specifies the return value for an asynchronous method.
		/// </summary>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<TResult>(this FuncExpectation<ValueTask<TResult>> mock, TResult value)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                mock.ReturnsAsync(() => value);
                return mock;
            });
        }

        /// <summary>
        /// Specifies a function to evaluate and return the value for an asynchronous method.
        /// </summary>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<TResult>(this FuncExpectation<ValueTask<TResult>> mock, Func<TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns(() => new ValueTask<TResult>(valueFunction()));
            });
        }
#endif
    }
}
