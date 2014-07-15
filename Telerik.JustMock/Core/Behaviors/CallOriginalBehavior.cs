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
using System.Linq;

namespace Telerik.JustMock.Core.Behaviors
{
	internal class CallOriginalBehavior : IBehavior
	{
		public void Process(Invocation invocation)
		{
			if (ShouldCallOriginal(invocation) && !invocation.UserProvidedImplementation)
			{
				invocation.UserProvidedImplementation = true;
				invocation.CallOriginal = true;
			}
		}

		public static bool ShouldCallOriginal(Invocation invocation)
		{
			return !invocation.Recording || invocation.RetainBehaviorDuringRecording;
		}
	}
}
