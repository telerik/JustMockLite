using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Telerik.JustMock.Core
{
	internal static class ProfilerInterceptor
	{
		public static bool IsProfilerAttached
		{
			get { return false; }
		}

		public static bool IsInterceptionEnabled { get; set; }

		public static int ReentrancyCounter { get; set; }

		public static void Initialize()
		{
		}

		public static object CreateInstanceWithArgsImpl(Type type, object[] args)
		{
			throw new NotSupportedException();
		}

		public static object GetUninitializedObjectImpl(Type type)
		{
			throw new NotSupportedException();
		}

		public static void RegisterGlobalInterceptor(MethodBase method, MocksRepository repo)
		{
			throw new NotSupportedException();
		}

		public static void UnregisterGlobalInterceptor(MethodBase method, MocksRepository repo)
		{
			throw new NotSupportedException();
		}

		public static void EnableInterception(Type type, bool enabled, MocksRepository behalf)
		{
			throw new NotSupportedException();
		}

		public static void ThrowElevatedMockingException(MemberInfo member = null)
		{
			var ex = member != null ? new ElevatedMockingException(member) : new ElevatedMockingException();
			throw ex;
		}

		public static void CreateDelegateFromBridge<T>(string bridgeMethodName, out T delg)
		{
			throw new NotSupportedException();
		}

		public static void CheckIfSafeToInterceptWholesale(Type type)
		{
		}

		public static bool TypeSupportsInstrumentation(Type type)
		{
			return false;
		}

		public static void RunClassConstructor(RuntimeTypeHandle type)
		{
		}

		public static void GuardInternal(Action action)
		{
			action();
		}

		public static T GuardInternal<T>(Func<T> func)
		{
			return func();
		}

		public static void GuardExternal(Action action)
		{
			action();
		}

		public static T GuardExternal<T>(Func<T> func)
		{
			return func();
		}
	}
}
