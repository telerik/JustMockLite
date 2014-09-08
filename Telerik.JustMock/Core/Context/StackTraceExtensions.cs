/*
 JustMock Lite
 Copyright © 2010-2014 Telerik AD

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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Telerik.JustMock.Core.Castle.Core.Internal;

namespace Telerik.JustMock.Core.Context
{
	internal static class StackTraceExtensions
	{
		private static readonly Dictionary<MethodBase, MethodBase> entryPointCache = new Dictionary<MethodBase, MethodBase>();
		private static readonly Lock entryPointCacheLock = Lock.Create();

		private static readonly Type IAsyncStateMachineType;
		private static readonly Type StateMachineAttributeType;
		private static readonly PropertyInfo StateMachineTypeProperty;

		static StackTraceExtensions()
		{
			IAsyncStateMachineType = Type.GetType("System.Runtime.CompilerServices.IAsyncStateMachine");
			StateMachineAttributeType = Type.GetType("System.Runtime.CompilerServices.StateMachineAttribute");
			if (StateMachineAttributeType != null)
				StateMachineTypeProperty = StateMachineAttributeType.GetProperty("StateMachineType");
		}

		public static IEnumerable<MethodBase> EnumerateFrames(this StackTrace stackTrace)
		{
			return EnumerateFramesVerbatim(stackTrace)
				.Select(GetEntryPointFromStateMachineCached);
		}

		private static MethodBase GetEntryPointFromStateMachineCached(MethodBase method)
		{
			if (StateMachineAttributeType == null)
				return method;

			using (var locker = entryPointCacheLock.ForReadingUpgradeable())
			{
				MethodBase entryMethod;
				if (entryPointCache.TryGetValue(method, out entryMethod))
					return entryMethod;
				locker.Upgrade();
				if (entryPointCache.TryGetValue(method, out entryMethod))
					return entryMethod;

				entryMethod = GetEntryPointFromStateMachine(method) ?? method;
				entryPointCache.Add(method, entryMethod);
				return entryMethod;
			}
		}

		private static MethodBase GetEntryPointFromStateMachine(MethodBase method)
		{
			var declType = method.DeclaringType;
			if (declType == null
				|| !Attribute.IsDefined(declType, typeof(CompilerGeneratedAttribute))
				|| !declType.GetInterfaces().Contains(IAsyncStateMachineType)
				|| declType.DeclaringType == null)
				return null;

			var parentType = declType.DeclaringType;
			var allMethods = parentType.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

			return allMethods.FirstOrDefault(m =>
				{
					var stateMachineAttr = Attribute.GetCustomAttribute(m, StateMachineAttributeType);
					if (stateMachineAttr == null)
						return false;
					var stateMachineImpl = StateMachineTypeProperty.GetValue(stateMachineAttr, null);
					return stateMachineImpl == declType;
				});
		}

		private static IEnumerable<MethodBase> EnumerateFramesVerbatim(StackTrace stackTrace)
		{
			var count = stackTrace.FrameCount;
			for (int i = 0; i < count; ++i)
				yield return stackTrace.GetFrame(i).GetMethod();
		}
	}
}
