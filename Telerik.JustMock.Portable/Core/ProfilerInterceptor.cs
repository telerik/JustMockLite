using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		[ThreadStatic]
		private static int reentrancyCounter;
		public static int ReentrancyCounter
		{
			get { return reentrancyCounter; }
			set { reentrancyCounter = value; }
		}

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

		[DebuggerHidden]
		public static void GuardInternal(Action guardedAction)
		{
			try
			{
				ReentrancyCounter++;
				guardedAction();
			}
			finally
			{
				ReentrancyCounter--;
			}
		}

		[DebuggerHidden]
		public static T GuardInternal<T>(Func<T> guardedAction)
		{
			try
			{
				ReentrancyCounter++;
				return guardedAction();
			}
			finally
			{
				ReentrancyCounter--;
			}
		}

		[DebuggerHidden]
		public static void GuardExternal(Action guardedAction)
		{
			var oldCounter = ProfilerInterceptor.ReentrancyCounter;
			try
			{
				ProfilerInterceptor.ReentrancyCounter = 0;
				guardedAction();
			}
			finally
			{
				ProfilerInterceptor.ReentrancyCounter = oldCounter;
			}
		}

		[DebuggerHidden]
		public static T GuardExternal<T>(Func<T> guardedAction)
		{
			var oldCounter = ProfilerInterceptor.ReentrancyCounter;
			try
			{
				ProfilerInterceptor.ReentrancyCounter = 0;
				return guardedAction();
			}
			finally
			{
				ProfilerInterceptor.ReentrancyCounter = oldCounter;
			}
		}
	}
}
