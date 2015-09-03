/*
 JustMock Lite
 Copyright © 2010-2015 Telerik AD

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
using System.Linq;
using System.Reflection;
using Telerik.JustMock.Core.MatcherTree;

namespace Telerik.JustMock.Core.Behaviors
{
	internal class PreserveRefOutValuesBehavior : IBehavior
	{
		private readonly Dictionary<int, object> values = new Dictionary<int, object>();

		public PreserveRefOutValuesBehavior(IMethodMock methodMock)
		{
			var argMatchers = methodMock.CallPattern.ArgumentMatchers;
			var method = (MethodBase) methodMock.CallPattern.Member;
			var parameters = GetParameters(method);
			var offsetDueToExtensionMethod = method.IsExtensionMethod() ? 1 : 0;

			for (int i = 0; i < parameters.Length; ++i)
			{
				if (!parameters[i].ParameterType.IsByRef || argMatchers[i].ProtectRefOut)
					continue;

				var matcher = argMatchers[i] as IValueMatcher;
				if (matcher == null)
					continue;

				var value = matcher.Value;
				values.Add(i + offsetDueToExtensionMethod, value);
			}
		}

		public void Process(Invocation invocation)
		{
			foreach (var kvp in this.values)
			{
				invocation.Args[kvp.Key] = kvp.Value;
			}
		}

		public static void Attach(IMethodMock methodMock)
		{
			if (!(methodMock.CallPattern.Member is MethodBase))
				return;
			var behavior = new PreserveRefOutValuesBehavior(methodMock);
			var madeReplacements = ReplaceRefOutArgsWithAnyMatcher(methodMock.CallPattern);
			if (madeReplacements)
				methodMock.Behaviors.Add(behavior);
		}

		public static bool ReplaceRefOutArgsWithAnyMatcher(CallPattern callPattern)
		{
			bool madeReplacements = false;
			var parameters = GetParameters((MethodBase)callPattern.Member);
			for (int i = 0; i < parameters.Length; ++i)
			{
				if (parameters[i].ParameterType.IsByRef && !callPattern.ArgumentMatchers[i].ProtectRefOut)
				{
					callPattern.ArgumentMatchers[i] = new AnyMatcher();
					madeReplacements = true;
				}
			}

			return madeReplacements;
		}

		private static ParameterInfo[] GetParameters(MethodBase method)
		{
			var parameters = method.GetParameters();
			if (method.IsExtensionMethod())
				parameters = parameters.Skip(1).ToArray();
			return parameters;
		}
	}
}
