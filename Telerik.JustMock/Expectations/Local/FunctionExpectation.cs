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
using System.Collections.Generic;
using System.Reflection;
using Telerik.JustMock.Core;
using Telerik.JustMock.Expectations.Abstraction.Local;
using Telerik.JustMock.Expectations.Abstraction.Local.Function;

namespace Telerik.JustMock.Expectations
{
    internal sealed class FunctionExpectation : IFunctionExpectation
    {
        public ActionExpectation Arrange(object target, string methodName, string localFunctionName, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyParamTypes = new Type[] { };
                return this.Arrange(target, methodName, emptyParamTypes, localFunctionName, args);
            });
        }

        public ActionExpectation Arrange(object target, string methodName, Type[] memberParamTypes, string localFunctionName, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MethodInfo method = MockingUtil.GetMethodWithLocalFunction(target, methodName, memberParamTypes, null);
                return this.Arrange(target, method, localFunctionName, args);
            });
        }

        public ActionExpectation Arrange(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyLocalFunctionGenericTypes = new Type[] { };
                return this.Arrange(target, methodName, localFunctionName, methodGenericTypes, emptyLocalFunctionGenericTypes, args);
            });
        }

        public ActionExpectation Arrange(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyParamTypes = new Type[] { };
                return this.Arrange(target, methodName, emptyParamTypes, localFunctionName, methodGenericTypes, localFunctionGenericTypes, args);
            });
        }

        public ActionExpectation Arrange(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyLocalFunctionGenericTypes = new Type[] { };
                return this.Arrange(target, methodName, methodParamTypes, localFunctionName, methodGenericTypes, emptyLocalFunctionGenericTypes, args);
            });
        }

        public ActionExpectation Arrange(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MethodInfo method = MockingUtil.GetMethodWithLocalFunction(target, methodName, methodParamTypes, methodGenericTypes);
                List<Type> combinedTypes = new List<Type>();
                if(methodGenericTypes != null)
                {
                    combinedTypes.AddRange(methodGenericTypes);
                }
                if (localFunctionGenericTypes != null)
                {
                    combinedTypes.AddRange(localFunctionGenericTypes);
                }

                MethodInfo localMethod = MockingUtil.GetLocalFunction(target, method, localFunctionName, combinedTypes.ToArray());

                return Mock.NonPublic.Arrange(target, localMethod, args);
            });
        }

        public ActionExpectation Arrange(object target, MethodInfo method, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				MethodInfo localMethod = MockingUtil.GetLocalFunction(target, method, localFunctionName, null);
				return Mock.NonPublic.Arrange(target, localMethod, args);
			});
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type[] emptyParamTypes = new Type[] { };
				return this.Arrange<TReturn>(target, methodName, emptyParamTypes, localFunctionName, args);
			});
		}

        public FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] localFunctionGenericTypes = new Type[] { };
                return this.Arrange<TReturn>(target, methodName, localFunctionName, methodGenericTypes, localFunctionGenericTypes, args);
            });
        }

        public FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyParamTypes = new Type[] { };
                return this.Arrange<TReturn>(target, methodName, emptyParamTypes, localFunctionName, methodGenericTypes, localFunctionGenericTypes, args);
            });
        }

        public FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] localFunctionGenericTypes = new Type[] { };
                return this.Arrange<TReturn>(target, methodName, methodParamTypes, localFunctionName, methodGenericTypes, localFunctionGenericTypes, args);
            });
        }

        public FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MethodInfo method = MockingUtil.GetMethodWithLocalFunction(target, methodName, methodParamTypes, methodGenericTypes);
                List<Type> combinedTypes = new List<Type>();
                combinedTypes.AddRange(methodGenericTypes);
                combinedTypes.AddRange(localFunctionGenericTypes);

                MethodInfo localMethod = MockingUtil.GetLocalFunction(target, method, localFunctionName, combinedTypes.ToArray());

                return Mock.NonPublic.Arrange<TReturn>(target, localMethod, args);
            });
        }




        public FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, Type[] memberParamTypes, string localFunctionName, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MethodInfo method = MockingUtil.GetMethodWithLocalFunction(target, methodName, memberParamTypes, null);
                return this.Arrange<TReturn>(target, method, localFunctionName, args);
            });
        }

        public FuncExpectation<TReturn> Arrange<TReturn>(object target, MethodInfo method, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = target.GetType();
				MethodInfo localMethod = MockingUtil.GetLocalFunction(type, method, localFunctionName, null);

				return Mock.NonPublic.Arrange<TReturn>(target, localMethod, args);
			});
		}
		
		public FuncExpectation<TReturn> Arrange<T, TReturn>(string methodName, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = typeof(T);
				return this.Arrange<TReturn>(type, methodName, localFunctionName, args);
			});
		}

        public FuncExpectation<TReturn> Arrange<T, TReturn>(string methodName, string localMemberName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyLocalFunctionGenericTypes = new Type[] { };
                return this.Arrange<T, TReturn>(methodName, localMemberName, methodGenericTypes, emptyLocalFunctionGenericTypes, args);
            });
        }

        public FuncExpectation<TReturn> Arrange<T, TReturn>(string methodName, string localMemberName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptymethodParamTypes = new Type[] { };
                return this.Arrange<T, TReturn>(methodName, emptymethodParamTypes, localMemberName, methodGenericTypes, localFunctionGenericTypes, args);
            });
        }

        public FuncExpectation<TReturn> Arrange<T, TReturn>(string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = typeof(T);
				return this.Arrange<TReturn>(type, methodName, methodParamTypes, localFunctionName, args);
			});
		}

        public FuncExpectation<TReturn> Arrange<T, TReturn>(string methodName, Type[] methodParamTypes, string localMemberName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyLocalFunctionGenericTypes = new Type[] { };
                return this.Arrange<T, TReturn>(methodName, methodParamTypes, localMemberName, methodGenericTypes, emptyLocalFunctionGenericTypes, args);
            });
        }

        public FuncExpectation<TReturn> Arrange<T, TReturn>(string methodName, Type[] methodParamTypes, string localMemberName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MethodInfo method = MockingUtil.GetMethodWithLocalFunction(typeof(T), methodName, methodParamTypes, methodGenericTypes);
                List<Type> combinedTypes = new List<Type>();
                combinedTypes.AddRange(methodGenericTypes);
                combinedTypes.AddRange(localFunctionGenericTypes);

                MethodInfo localMethod = MockingUtil.GetLocalFunction(typeof(T), method, localMemberName, combinedTypes.ToArray());

                return Mock.NonPublic.Arrange<TReturn>(typeof(T), localMethod, args);
            });
        }

        public FuncExpectation<TReturn> Arrange<T, TReturn>(MethodInfo method, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = typeof(T);
				return this.Arrange<TReturn>(type, method, localFunctionName, args);
			});
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(Type type, string methodName, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type[] emptyParamTypes = new Type[] { };
				return this.Arrange<TReturn>(type, methodName, emptyParamTypes, localFunctionName, args);
			});
		}


        public FuncExpectation<TReturn> Arrange<TReturn>(Type type, string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyLocalFunctionGenericTypes = new Type[] { };
                return this.Arrange<TReturn>(type, methodName, localFunctionName, methodGenericTypes, emptyLocalFunctionGenericTypes, args);
            });
        }

        public FuncExpectation<TReturn> Arrange<TReturn>(Type type, string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyMethodParamTypes = new Type[] { };
                return this.Arrange<TReturn>(type, methodName, emptyMethodParamTypes, localFunctionName, methodGenericTypes, localFunctionGenericTypes, args);
            });
        }

        public FuncExpectation<TReturn> Arrange<TReturn>(Type type, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				MethodInfo method = MockingUtil.GetMethodWithLocalFunction(type, methodName, methodParamTypes, null);
				return this.Arrange<TReturn>(type, method, localFunctionName, args);
			});
		}

        public FuncExpectation<TReturn> Arrange<TReturn>(Type type, string methodName, Type[] methodParamTypes, string localMemberName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyLocalFunctionGenericTypes = new Type[] { };
                return this.Arrange<TReturn>(type, methodName, methodParamTypes, localMemberName, methodGenericTypes, emptyLocalFunctionGenericTypes, args);
            });
        }

        public FuncExpectation<TReturn> Arrange<TReturn>(Type type, string methodName, Type[] methodParamTypes, string localMemberName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MethodInfo method = MockingUtil.GetMethodWithLocalFunction(type, methodName, methodParamTypes, methodGenericTypes);
                List<Type> combinedTypes = new List<Type>();
                combinedTypes.AddRange(methodGenericTypes);
                combinedTypes.AddRange(localFunctionGenericTypes);

                MethodInfo localMethod = MockingUtil.GetLocalFunction(type, method, localMemberName, combinedTypes.ToArray());

                return Mock.NonPublic.Arrange<TReturn>(type, localMethod, args);
            });
        }

        public FuncExpectation<TReturn> Arrange<TReturn>(Type type, MethodInfo method, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				MethodInfo localMethod = MockingUtil.GetLocalFunction(type, method, localFunctionName, null);
				return Mock.NonPublic.Arrange<TReturn>(type, localMethod, args);
			});
		}
		
		public ActionExpectation Arrange<T>(string methodName, string localMemberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = typeof(T);
				return this.Arrange(type, methodName, localMemberName, args);
			});
		}

        public ActionExpectation Arrange<T>(string methodName, string localMemberName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyLocalFunctionGenericTypes = new Type[] { };
                return this.Arrange<T>(methodName, localMemberName, methodGenericTypes, emptyLocalFunctionGenericTypes, args);
            });
        }

        public ActionExpectation Arrange<T>(string methodName, string localMemberName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyMethodParam = new Type[] { };
                return this.Arrange<T>(methodName, emptyMethodParam, localMemberName, methodGenericTypes, localFunctionGenericTypes, args);
            });
        }

        public ActionExpectation Arrange<T>(string methodName, Type[] methodParamTypes, string localMemberName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyGenericLocalTypes = new Type[] { };
                return this.Arrange<T>(methodName, methodParamTypes, localMemberName, methodGenericTypes, emptyGenericLocalTypes, args);
            });

        }

        public ActionExpectation Arrange<T>(string methodName, Type[] methodParamTypes, string localMemberName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MethodInfo method = MockingUtil.GetMethodWithLocalFunction(typeof(T), methodName, methodParamTypes, methodGenericTypes);
                List<Type> combinedTypes = new List<Type>();
                combinedTypes.AddRange(methodGenericTypes);
                combinedTypes.AddRange(localFunctionGenericTypes);

                MethodInfo localMethod = MockingUtil.GetLocalFunction(typeof(T), method, localMemberName, combinedTypes.ToArray());

                return Mock.NonPublic.Arrange(localMethod, args);
            });

        }

        public ActionExpectation Arrange<T>(string methodName, Type[] methodParamTypes, string localMemberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = typeof(T);
				return this.Arrange(type, methodName, methodParamTypes, localMemberName, args);
			});
		}

		public ActionExpectation Arrange<T>(MethodInfo method, string localMemberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = typeof(T);
				return this.Arrange(type, method, localMemberName, args);
			});
		}

		public ActionExpectation Arrange(Type type, string methodName, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type[] emptyParamTypes = new Type[] { };
				return this.Arrange(type, methodName, emptyParamTypes, localFunctionName, args);
			});
		}


        public ActionExpectation Arrange(Type type, string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyLocalFunctionGenericTypes = new Type[] { };
                return this.Arrange(type, methodName, localFunctionName, methodGenericTypes, emptyLocalFunctionGenericTypes, args);
            });
        }

        public ActionExpectation Arrange(Type type, string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyMethodParamTypes = new Type[] { };
                return this.Arrange(type, methodName, emptyMethodParamTypes, localFunctionName, methodGenericTypes, localFunctionGenericTypes, args);
            });
        }

        public ActionExpectation Arrange(Type type, string methodName, Type[] methodParamTypes, string localMemberName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyLocalFunctionGenericTypes = new Type[] { };
                return this.Arrange(type, methodName, methodParamTypes, localMemberName, methodGenericTypes, emptyLocalFunctionGenericTypes, args);
            });
        }

        public ActionExpectation Arrange(Type type, string methodName, Type[] methodParamTypes, string localMemberName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MethodInfo method = MockingUtil.GetMethodWithLocalFunction(type, methodName, methodParamTypes, methodGenericTypes);
                List<Type> combinedTypes = new List<Type>();
                combinedTypes.AddRange(methodGenericTypes);
                combinedTypes.AddRange(localFunctionGenericTypes);

                MethodInfo localMethod = MockingUtil.GetLocalFunction(type, method, localMemberName, combinedTypes.ToArray());

                return Mock.NonPublic.Arrange(type, localMethod, args);
            });
        }

        public ActionExpectation Arrange(Type type, string methodName, Type[] methodParamTypes, string localMemberName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				MethodInfo method = MockingUtil.GetMethodWithLocalFunction(type, methodName, methodParamTypes, null);
				return this.Arrange(type, method, localMemberName, args);
			});
		}

		public ActionExpectation Arrange(Type type, MethodInfo method, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				MethodInfo localFunction = MockingUtil.GetLocalFunction(type, method, localFunctionName, null);
				return Mock.NonPublic.Arrange(type, localFunction, args);
			});
		}

		public object Call(object target, string methodName, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = target.GetType();
				MethodInfo method = MockingUtil.GetMethodWithLocalFunction(target, methodName);
				MethodInfo localFunction = MockingUtil.GetLocalFunction(type, method, localFunctionName, null);

				return Mock.NonPublic.MakePrivateAccessor(target).CallMethod(localFunction, args);
			});
		}

        public T Call<T>(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var resObject = this.Call(target, methodName, localFunctionName, methodGenericTypes, args);
                T res = (T)resObject;
                return res;
            });
        }

        public T Call<T>(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var resObject = this.Call(target, methodName, localFunctionName, methodGenericTypes, localFunctionGenericTypes, args);
                T res = (T)resObject;
                return res;
            });
        }

        public T Call<T>(object target, string methodName, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type[] emptyParamTypes = new Type[] { };
				var resObject = this.Call(target, methodName, emptyParamTypes, localFunctionName, args);
				T res = (T)resObject;
				return res;
			});
        }

        public object Call(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyLocalFunctionGenericTypes = new Type[] { };
                return this.Call(target, methodName, methodParamTypes, localFunctionName, methodGenericTypes, emptyLocalFunctionGenericTypes, args);
            });
        }

        public object Call(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MethodInfo method = MockingUtil.GetMethodWithLocalFunction(target, methodName, methodParamTypes, methodGenericTypes);
                List<Type> combinedTypes = new List<Type>();
                combinedTypes.AddRange(methodGenericTypes);
                combinedTypes.AddRange(localFunctionGenericTypes);

                MethodInfo localMethod = MockingUtil.GetLocalFunction(target.GetType(), method, localFunctionName, combinedTypes.ToArray());

                return Mock.NonPublic.MakePrivateAccessor(target).CallMethod(localMethod, args);
            });
        }

        public object Call(object target, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = target.GetType();
				MethodInfo method = MockingUtil.GetMethodWithLocalFunction(target, methodName, methodParamTypes, null);
				MethodInfo localFunction = MockingUtil.GetLocalFunction(type, method, localFunctionName, null);

				return Mock.NonPublic.MakePrivateAccessor(target).CallMethod(localFunction, args);
			});
        }

        public object Call(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyLocalFunctionGenericTypes = new Type[] { };
                return this.Call(target, methodName, localFunctionName, methodGenericTypes, emptyLocalFunctionGenericTypes, args);
            });
        }

        public object Call(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyMethodParamTypes = new Type[] { };
                return this.Call(target, methodName, emptyMethodParamTypes, localFunctionName, methodGenericTypes, localFunctionGenericTypes, args);
            });
        }

        public T Call<T>(object target, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var resObject = this.Call(target, methodName, methodParamTypes, localFunctionName, args);
				T res = (T)resObject;
				return res;
			});
        }

        public T Call<T>(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var resObject = this.Call(target, methodName, methodParamTypes, localFunctionName, methodGenericTypes, args);
                T res = (T)resObject;
                return res;
            });
        }
        public T Call<T>(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var resObject = this.Call(target, methodName, methodParamTypes, localFunctionName, methodGenericTypes, localFunctionGenericTypes, args);
                T res = (T)resObject;
                return res;
            });
        }

        public object Call(object target, MethodInfo method, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = target.GetType();
				MethodInfo localFunction = MockingUtil.GetLocalFunction(type, method, localFunctionName, null);
				return Mock.NonPublic.MakePrivateAccessor(target).CallMethod(localFunction, args);
			});
		}

        public T Call<T>(object target, MethodInfo method, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var resObject = this.Call(target, method, localFunctionName, args);
				T res = (T)resObject;
				return res;
			});
		}

		public void Assert(object target, MethodInfo method, string localFunctionName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = target.GetType();
				MethodInfo localFunction = MockingUtil.GetLocalFunction(type, method, localFunctionName, null);

				Mock.NonPublic.Assert(target, localFunction, args);
			});
        }

        public void Assert(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyLocalFunctionGenericTypes = new Type[] { };
                this.Assert(target, methodName, localFunctionName, methodGenericTypes, emptyLocalFunctionGenericTypes, args);
            });
        }

        public void Assert(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyMethodParamTypes = new Type[] { };
                this.Assert(target, methodName, emptyMethodParamTypes, localFunctionName, methodGenericTypes, localFunctionGenericTypes, args);
            });
        }

        public void Assert(object target, string methodName, string localFunctionName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				Type[] emptyParamTypes = new Type[] { };
				this.Assert(target, methodName, emptyParamTypes, localFunctionName, args);
			});
		}


        public void Assert(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyLocalFunctionGenericTypes = new Type[] { };
                this.Assert(target, methodName, methodParamTypes, localFunctionName, methodGenericTypes, emptyLocalFunctionGenericTypes, args);
            });
        }

        public void Assert(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MethodInfo method = MockingUtil.GetMethodWithLocalFunction(target, methodName, methodParamTypes, methodGenericTypes);
                List<Type> combinedTypes = new List<Type>();
                combinedTypes.AddRange(methodGenericTypes);
                combinedTypes.AddRange(localFunctionGenericTypes);

                MethodInfo localMethod = MockingUtil.GetLocalFunction(target.GetType(), method, localFunctionName, combinedTypes.ToArray());

                Mock.NonPublic.Assert(localMethod, args);
            });
        }

        public void Assert(object target, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MethodInfo method = MockingUtil.GetMethodWithLocalFunction(target, methodName, methodParamTypes, null);
				this.Assert(target, method, localFunctionName, args);
			});
		}

		public void Assert(object target, MethodInfo method, string localFunctionName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = target.GetType();
				MethodInfo localFunction = MockingUtil.GetLocalFunction(type, method, localFunctionName, null);

				Mock.NonPublic.Assert(target, localFunction, occurs, args);
			});
		}

        public void Assert(object target, string methodName, string localFunctionName, Occurs occurs, Type[] methodGenericTypes, params object[] args)
        {

            ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyLocalFunctionGenericTypes = new Type[] { };
                this.Assert(target, methodName, localFunctionName, occurs, methodGenericTypes, emptyLocalFunctionGenericTypes, args);
            });
        }

        public void Assert(object target, string methodName, string localFunctionName, Occurs occurs, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyMethodParamTypes = new Type[] { };
                this.Assert(target, methodName, emptyMethodParamTypes, localFunctionName, occurs,  methodGenericTypes, localFunctionGenericTypes, args);
            });
        }

        public void Assert(object target, string methodName, string localFunctionName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				Type[] emptyParamTypes = new Type[] { };
				this.Assert(target, methodName, emptyParamTypes, localFunctionName, occurs, args);
			});
		}


        public void Assert(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Occurs occurs, Type[] methodGenericTypes, params object[] args)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                Type[] emptyLocalFunctionGenericTypes = new Type[] { };
                this.Assert(target, methodName, methodParamTypes, localFunctionName, occurs, methodGenericTypes, emptyLocalFunctionGenericTypes, args);
            });
        }

        public void Assert(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Occurs occurs, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args)
        {

            ProfilerInterceptor.GuardInternal(() =>
            {
                MethodInfo method = MockingUtil.GetMethodWithLocalFunction(target, methodName, methodParamTypes, methodGenericTypes);
                List<Type> combinedTypes = new List<Type>();
                combinedTypes.AddRange(methodGenericTypes);
                combinedTypes.AddRange(localFunctionGenericTypes);

                MethodInfo localMethod = MockingUtil.GetLocalFunction(target.GetType(), method, localFunctionName, combinedTypes.ToArray());

                Mock.NonPublic.Assert(localMethod, occurs, args);
            });
        }

        public void Assert(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Occurs occurs, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MethodInfo method = MockingUtil.GetMethodWithLocalFunction(target, methodName, methodParamTypes, null);

				this.Assert(target, method, localFunctionName, occurs, args);
			});
		}
	}
}
