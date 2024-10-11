/*
 JustMock Lite
 Copyright © 2022 Progress Software Corporation

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
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1) => Task.FromResult(valueFunction(arg1)));
            });
        }

        /// <summary>
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, T2, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, T2, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1, T2 arg2) => Task.FromResult(valueFunction(arg1, arg2)));
            });
        }

        /// <summary>
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, T2, T3, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, T2, T3, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1, T2 arg2, T3 arg3) => Task.FromResult(valueFunction(arg1, arg2, arg3)));
            });
        }

        /// <summary>
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, T2, T3, T4, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, T2, T3, T4, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1, T2 arg2, T3 arg3, T4 arg4) => Task.FromResult(valueFunction(arg1, arg2, arg3, arg4)));
            });
        }

        /// <summary>
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, T2, T3, T4, T5, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, T2, T3, T4, T5, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => Task.FromResult(valueFunction(arg1, arg2, arg3, arg4, arg5)));
            });
        }

        /// <summary>
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, T2, T3, T4, T5, T6, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => Task.FromResult(valueFunction(arg1, arg2, arg3, arg4, arg5, arg6)));
            });
        }

        /// <summary>
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => Task.FromResult(valueFunction(arg1, arg2, arg3, arg4, arg5, arg6, arg7)));
            });
        }

        /// <summary>
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => Task.FromResult(valueFunction(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8)));
            });
        }

        /// <summary>
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) => Task.FromResult(valueFunction(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9)));
            });
        }

        /// <summary>
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T10">Type of the tenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) => Task.FromResult(valueFunction(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10)));
            });
        }

        /// <summary>
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T10">Type of the tenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T11">Type of the eleventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) => Task.FromResult(valueFunction(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11)));
            });
        }

        /// <summary>
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T10">Type of the tenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T11">Type of the eleventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T12">Type of the twelveth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) => Task.FromResult(valueFunction(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12)));
            });
        }

        /// <summary>
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T10">Type of the tenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T11">Type of the eleventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T12">Type of the twelveth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T13">Type of the thirteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) => Task.FromResult(valueFunction(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13)));
            });
        }

        /// <summary>
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T10">Type of the tenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T11">Type of the eleventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T12">Type of the twelveth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T13">Type of the thirteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T14">Type of the fourteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) => Task.FromResult(valueFunction(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14)));
            });
        }

        /// <summary>
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T10">Type of the tenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T11">Type of the eleventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T12">Type of the twelveth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T13">Type of the thirteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T14">Type of the fourteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T15">Type of the fifteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) => Task.FromResult(valueFunction(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15)));
            });
        }

        /// <summary>
        /// Specifies the delegate that will execute and return the value for the expected member.
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T10">Type of the tenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T11">Type of the eleventh parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T12">Type of the twelveth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T13">Type of the thirteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T14">Type of the fourteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T15">Type of the fifteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="T16">Type of the sixteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="valueFunction">The function that will evaluate the return value.</param>
        /// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
        public static IAssertable ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this FuncExpectation<Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> valueFunction)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                if (valueFunction == null)
                {
                    return mock.ReturnsAsync(new Func<TResult>(() => default(TResult)));
                }

                return mock.Returns((T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) => Task.FromResult(valueFunction(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16)));
            });
        }
    }
}