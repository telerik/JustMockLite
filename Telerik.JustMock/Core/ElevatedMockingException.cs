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
using System.Reflection;

namespace Telerik.JustMock.Core
{
	[Serializable]
	public sealed class ElevatedMockingException : MockException
	{
		private const string ProfilerNeededMessage =
#if LITE_EDITION
			" JustMock Lite can only mock interface members, virtual/abstract members in non-sealed classes, delegates and all members on classes derived from MarshalByRefObject on instances created with Mock.Create or Mock.CreateLike. For any other scenario you need to use the full version of JustMock.";
#else
			" The profiler must be enabled to mock, arrange or execute the specified target.";
#endif

		internal ElevatedMockingException(MemberInfo target)
			: this(String.Format("Cannot mock '{0}'.", target))
		{ }

		internal ElevatedMockingException()
			: this((string)null)
		{ }

		private ElevatedMockingException(string details)
			: base(ConstructMessage(details))
		{
		}

		private static string ConstructMessage(string details)
		{
			var message = details + ProfilerNeededMessage;
#if !LITE_EDITION
#if !SILVERLIGHT
			var detectedProfilers = ClrProfilerSetupHelper.GetEnabledProfilersLocations();
			if (!String.IsNullOrEmpty(detectedProfilers))
			{
				message += "\nDetected active third-party profilers:" + detectedProfilers + "\nDisable the profilers or link them from the JustMock configuration utility. Restart the test runner and, if necessary, Visual Studio after linking.";
			}
#else
			message += " If you have enabled other profiler-based tools (e.g. code coverage, performance or memory profilers, mocking tools, debugging tools), either disable them or link them from the JustMock configuration utility. Restart the test runner and, if necessary, Visual Studio after linking.";
#endif
#endif
			return message;
		}
	}
}
