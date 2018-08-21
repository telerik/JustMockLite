using System;
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
				return Arrange(target, methodName, emptyParamTypes, localFunctionName, args);
			});
		}

		public ActionExpectation Arrange(object target, string methodName, Type[] memberParamTypes, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				MethodInfo method = MockingUtil.GetMethodWithLocalFunction(target, methodName, memberParamTypes);
				return Arrange(target, method, localFunctionName, args);
			});
		}
		public ActionExpectation Arrange(object target, MethodInfo method, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				MethodInfo localMethod = MockingUtil.GetLocalFunction(target, method, localFunctionName);
				return Mock.NonPublic.Arrange(target, localMethod, args);
			});
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type[] emptyParamTypes = new Type[] { };
				return Arrange<TReturn>(target, methodName, emptyParamTypes, localFunctionName, args);
			});
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, Type[] memberParamTypes, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				MethodInfo method = MockingUtil.GetMethodWithLocalFunction(target, methodName, memberParamTypes);
				return Arrange<TReturn>(target, method, localFunctionName, args);
			});
		}

		public FuncExpectation<TReturn> Arrange<TReturn>(object target, MethodInfo method, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = target.GetType();
				MethodInfo localMethod = MockingUtil.GetLocalFunction(type, method, localFunctionName);

				return Mock.NonPublic.Arrange<TReturn>(target, localMethod, args);
			});
		}

		public object Call(object target, string methodName, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = target.GetType();
				MethodInfo method = MockingUtil.GetMethodWithLocalFunction(target, methodName);
				MethodInfo localFunction = MockingUtil.GetLocalFunction(type, method, localFunctionName);

				return Mock.NonPublic.MakePrivateAccessor(target).CallMethod(localFunction, args);
			});
		}

		public T Call<T>(object target, string methodName, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var resObject = Call(target, methodName, localFunctionName, args);
				T res = (T)resObject;
				return res;
			});
		}

		public object Call(object target, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = target.GetType();
				MethodInfo method = MockingUtil.GetMethodWithLocalFunction(target, methodName, methodParamTypes);
				MethodInfo localFunction = MockingUtil.GetLocalFunction(type, method, localFunctionName);

				return Mock.NonPublic.MakePrivateAccessor(target).CallMethod(localFunction, args);
			});
		}

		public T Call<T>(object target, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var resObject = Call(target, methodName, methodParamTypes, localFunctionName, args);
				T res = (T)resObject;
				return res;
			});
		}

		public object Call(object target, MethodInfo method, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Type type = target.GetType();
				MethodInfo localFunction = MockingUtil.GetLocalFunction(type, method, localFunctionName);
				return Mock.NonPublic.MakePrivateAccessor(target).CallMethod(localFunction, args);
			});
		}

		public T Call<T>(object target, MethodInfo method, string localFunctionName, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var resObject = Call(target, method, localFunctionName, args);
				T res = (T)resObject;
				return res;
			});
		}
	}
}