using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Telerik.JustMock.Core
{
	internal sealed class StackTrace
	{
		private readonly object stackTrace;

		internal StackTrace()
		{
			stackTrace = createStackTrace();
		}

		internal int FrameCount
		{
			get { return getFrameCount(stackTrace); }
		}

		internal Frame GetFrame(int index)
		{
			return new Frame(getFrame(stackTrace, index));
		}

		internal struct Frame
		{
			private readonly object frame;

			internal Frame(object frame)
			{
				this.frame = frame;
			}

			internal MethodBase GetMethod()
			{
				return getMethod(frame);
			}
		}

		private static readonly Func<object> createStackTrace;
		private static readonly Func<object, int> getFrameCount;
		private static readonly Func<object, int, object> getFrame;
		private static readonly Func<object, MethodBase> getMethod;

		static StackTrace()
		{
			var stackTraceType = Type.GetType("System.Diagnostics.StackTrace");
			var stackFrameType = Type.GetType("System.Diagnostics.StackFrame");
			var getFrameCountMethod = stackTraceType.GetProperty("FrameCount").GetGetMethod();
			var getFrameMethod = stackTraceType.GetMethod("GetFrame");
			var getMethodMethod = stackFrameType.GetMethod("GetMethod");

			createStackTrace = Expression.Lambda<Func<object>>(Expression.New(stackTraceType)).Compile();

			var objectParam = Expression.Parameter(typeof(object));
			var indexParam = Expression.Parameter(typeof(int));

			getFrameCount = Expression.Lambda<Func<object, int>>(
				Expression.Call(
					Expression.Convert(objectParam, stackTraceType), getFrameCountMethod),
					objectParam).Compile();

			getFrame = Expression.Lambda<Func<object, int, object>>(
				Expression.Call(
					Expression.Convert(objectParam, stackTraceType), getFrameMethod, indexParam),
					objectParam, indexParam).Compile();

			getMethod = Expression.Lambda<Func<object, MethodBase>>(
				Expression.Call(
					Expression.Convert(objectParam, stackFrameType), getMethodMethod),
					objectParam).Compile();
		}
	}
}
