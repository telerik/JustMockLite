/*
 JustMock Lite
 Copyright © 2018 Progress Software Corporation

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
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Context;

namespace Telerik.JustMock.Expectations.Abstraction
{
    internal sealed class NonPublicRefReturnExpectation : INonPublicRefReturnExpectation
    {
        public FuncExpectation<TRefReturn> Arrange<TRefReturn>(object target, string memberName, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var type = target.GetType();
                var mixin = MocksRepository.GetMockMixin(target, null);
                if (mixin != null)
                {
                    type = mixin.DeclaringType;
                }

                var returnType = typeof(TRefReturn).MakeByRefType();
                var method = MockingUtil.GetMethodByName(type, returnType, memberName, ref args);

                return MockingContext.CurrentRepository.Arrange(target, method, args, () => new FuncExpectation<TRefReturn>());
            });
        }

        public void Assert<TRefReturn>(object target, string memberName, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var type = target.GetType();
                var mixin = MocksRepository.GetMockMixin(target, null);
                if (mixin != null)
                {
                    type = mixin.DeclaringType;
                }

                var returnType = typeof(TRefReturn).MakeByRefType();
                var method = MockingUtil.GetMethodByName(type, returnType, memberName, ref args);

                var message = MockingUtil.GetAssertionMessage(args);
                MockingContext.CurrentRepository.AssertMethodInfo(message, target, method, args, null);
            });
        }

        public void Assert<TRefReturn>(object target, string memberName, Occurs occurs, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var type = target.GetType();
                var mixin = MocksRepository.GetMockMixin(target, null);
                if (mixin != null)
                {
                    type = mixin.DeclaringType;
                }

                var returnType = typeof(TRefReturn).MakeByRefType();
                var method = MockingUtil.GetMethodByName(type, returnType, memberName, ref args);

                var message = MockingUtil.GetAssertionMessage(args);
                MockingContext.CurrentRepository.AssertMethodInfo(message, target, method, args, occurs);
            });
        }

        public int GetTimesCalled<TRefReturn>(object target, string memberName, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var type = target.GetType();
                var mixin = MocksRepository.GetMockMixin(target, null);
                if (mixin != null)
                {
                    type = mixin.DeclaringType;
                }

                var returnType = typeof(TRefReturn).MakeByRefType();
                var method = MockingUtil.GetMethodByName(type, returnType, memberName, ref args);

                return MockingContext.CurrentRepository.GetTimesCalledFromMethodInfo(target, method, args);
            });
        }

#if !LITE_EDITION
        public FuncExpectation<TRefReturn> Arrange<T, TRefReturn>(string memberName, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var returnType = typeof(TRefReturn).MakeByRefType();
                var method = MockingUtil.GetMethodByName(typeof(T), returnType, memberName, ref args);

                return MockingContext.CurrentRepository.Arrange(null, method, args, () => new FuncExpectation<TRefReturn>());
            });
        }

        public FuncExpectation<TRefReturn> Arrange<TRefReturn>(Type targetType, string memberName, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var returnType = typeof(TRefReturn).MakeByRefType();
                var method = MockingUtil.GetMethodByName(targetType, returnType, memberName, ref args);

                return MockingContext.CurrentRepository.Arrange(null, method, args, () => new FuncExpectation<TRefReturn>());
            });
        }

        public void Assert<T, TRefReturn>(string memberName, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var returnType = typeof(TRefReturn).MakeByRefType();
                var method = MockingUtil.GetMethodByName(typeof(T), returnType, memberName, ref args);

                var message = MockingUtil.GetAssertionMessage(args);
                MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, null);
            });
        }

        public void Assert<TRefReturn>(Type targetType, string memberName, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var returnType = typeof(TRefReturn).MakeByRefType();
                var method = MockingUtil.GetMethodByName(targetType, returnType, memberName, ref args);

                var message = MockingUtil.GetAssertionMessage(args);
                MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, null);
            });
        }

        public void Assert<T, TRefReturn>(string memberName, Occurs occurs, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var returnType = typeof(TRefReturn).MakeByRefType();
                var method = MockingUtil.GetMethodByName(typeof(T), returnType, memberName, ref args);

                var message = MockingUtil.GetAssertionMessage(args);
                MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, occurs);
            });
        }

        public void Assert<TRefReturn>(Type targetType, string memberName, Occurs occurs, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var returnType = typeof(TRefReturn).MakeByRefType();
                var method = MockingUtil.GetMethodByName(targetType, returnType, memberName, ref args);

                var message = MockingUtil.GetAssertionMessage(args);
                MockingContext.CurrentRepository.AssertMethodInfo(message, null, method, args, occurs);
            });
        }

        public int GetTimesCalled<T, TRefReturn>(string memberName, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var returnType = typeof(TRefReturn).MakeByRefType();
                var method = MockingUtil.GetMethodByName(typeof(T), returnType, memberName, ref args);

                return MockingContext.CurrentRepository.GetTimesCalledFromMethodInfo(null, method, args);
            });
        }

        public int GetTimesCalled<TRefReturn>(Type targetType, string memberName, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var returnType = typeof(TRefReturn).MakeByRefType();
                var method = MockingUtil.GetMethodByName(targetType, returnType, memberName, ref args);

                return MockingContext.CurrentRepository.GetTimesCalledFromMethodInfo(null, method, args);
            });
        }
#endif
    }
}
