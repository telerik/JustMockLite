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

namespace Telerik.JustMock.Core
{
	internal class Lazy<T>
	{
		private readonly Func<T> initializer;
		private bool initialized;
		private T value;

		public Lazy(Func<T> initializer)
		{
			this.initializer = initializer;
		}

		public static implicit operator T(Lazy<T> lazy)
		{
			if (!lazy.initialized)
			{
				lazy.initialized = true;
				lazy.value = ProfilerInterceptor.GuardExternal(lazy.initializer);
			}
			return lazy.value;
		}
	}
}
