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
using System.Threading;
using Telerik.JustMock.Core.Castle.Core.Internal;

namespace Telerik.JustMock.Core
{
	internal class ThreadLocalProperty<T> where T : class
	{
		private readonly WeakKeyDictionary<Thread, T> values = new WeakKeyDictionary<Thread, T>();

		public T Get()
		{
			return GetValueOnThread(Thread.CurrentThread);
		}

		public T GetOrSetDefault(Func<T> getDefault)
		{
			var value = GetValueOnThread(Thread.CurrentThread);
			if (value == null)
			{
				value = getDefault();
				Set(value);
			}

			return value;
		}

		public void Set(T value)
		{
			lock (values)
				values[Thread.CurrentThread] = value;
		}

		public ICollection<T> GetAllThreadsValues()
		{
			lock(values)
				return values.Values.ToArray();
		}

		private T GetValueOnThread(Thread thread)
		{
			T value;
			lock (values)
				values.TryGetValue(thread, out value);
			return value;
		}
	}
}
