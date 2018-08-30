/*
 JustMock Lite
 Copyright © 2010-2015 Telerik EAD

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

		public override string ToString()
		{
			return stackTrace.ToString();
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
			var toStringMethod = stackTraceType.GetMethod("ToString");

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
