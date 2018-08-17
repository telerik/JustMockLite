/*
 JustMock Lite
 Copyright © 2010-2018 Telerik EAD

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
using System.Reflection;
using Telerik.JustMock.Core;
using Telerik.JustMock.Helpers;

namespace Telerik.JustMock
{
    internal sealed class ValueHandle<TValue, TId> : LocalRefHandle<TValue>
    {
        private static class RefProvider
        {
            private static TValue value;

            public static TValue Value
            {
                set
                {
                    ProfilerInterceptor.GuardInternal(() => RefProvider.value = value);
                }
            }

            public static ref TValue GetRef()
            {
                return ref ProfilerInterceptor.GuardInternal((target, args) =>
                {
                    return ref RefProvider.value;
                }, null, null);
            }
        }

        public ValueHandle(TValue expected)
            : base(RefProvider.GetRef)
        {
            RefProvider.Value = expected;
        }
    }

    /// <summary>
    /// Creates handles used for mocking ref returns, used with conjunction with <see cref="Expectations.FuncExpectation{TReturn}.Returns(Delegate)"/>
    /// </summary>
    /// <example>
    /// var refHandle = LocalRef.WithValue(10);
    /// Mock.Arrange(mock, m => m.Echo(ref Arg.Ref(Arg.AnyInt).Value)).Returns(refHandle.Handle).OccursOnce();
    /// </example>
    public static class LocalRef
    {
        /// <summary>
        /// Creates ref return handle with given type and value
        /// </summary>
        /// <typeparam name="T">Ref return type</typeparam>
        /// <param name="value">Initial value</param>
        /// <returns>Ref return handle of type <see cref="LocalRefHandle{T}"/></returns>
        public static LocalRefHandle<T> WithValue<T>(T value)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type valueHandleType = typeof(ValueHandle<,>).MakeGenericType(new[] { typeof(T), DynamicTypeHelper.GetNextType<T>() });
                ConstructorInfo constructor = valueHandleType.GetConstructor(new[] { typeof(T) });

                return (LocalRefHandle<T>)constructor.Invoke(new object[] { value });
            });
        }
    }
}
