/*
 JustMock Lite
 Copyright © 2010-2015 Progress Software Corporation

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
using System.ComponentModel;
using Telerik.JustMock.Core;

namespace Telerik.JustMock
{
	/// <summary>
	/// Defines filters for calls , used in conjunction with assert.
	/// </summary>
	public sealed class Occurs
	{
		private readonly int? lowerBound;
		private readonly int? upperBound;

		internal Occurs(int? lowerBound, int? upperBound)
		{
			this.upperBound = upperBound;
			this.lowerBound = lowerBound;
		}

		/// <summary>
		/// Specifies that the expected call must never occur
		/// </summary>
		/// <returns>Occurs type.</returns>
		public static Occurs Never()
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new Occurs(0, 0);
			});
		}

		/// <summary>
		/// Specifies that the expected call should occur once.
		/// </summary>
		/// <returns>Occurs type.</returns>
		public static Occurs Once()
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new Occurs(1, 1);
			});
		}

		/// <summary>
		/// Specifies that the expected call should occur at least <paramref name="numberOfTimes"/>  times.
		/// </summary>
		/// <param name="numberOfTimes">Specific number of times</param>
		/// <returns>Occurs type.</returns>
		public static Occurs AtLeast(int numberOfTimes)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new Occurs(numberOfTimes, null);
			});
		}

		/// <summary>
		/// Specifies that the expected call should occur at least once.
		/// </summary>
		/// <returns>Occurs type.</returns>
		public static Occurs AtLeastOnce()
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new Occurs(1, null);
			});
		}

		/// <summary>
		/// Specifies that the expected call should occur at most <paramref name="numberOfTimes"/>  times.
		/// </summary>
		/// <param name="numberOfTimes">Specific number of times</param>
		/// <returns>Occurs type.</returns>
		public static Occurs AtMost(int numberOfTimes)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new Occurs(null, numberOfTimes);
			});
		}

		/// <summary>
		/// Specifies that the expected call should occur exactly <paramref name="numberOfTimes"/>  times.
		/// </summary>
		/// <param name="numberOfTimes">Specific number of times</param>
		/// <returns>Occurs type.</returns>
		public static Occurs Exactly(int numberOfTimes)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new Occurs(numberOfTimes, numberOfTimes);
			});
		}

		/// <summary>
		/// Specifies that occurrence is not available.
		/// </summary>
		/// <returns>Occurs type.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Occurs NotAvailable()
		{
			return null;
		}

		internal int? LowerBound { get { return this.lowerBound; } }
		internal int? UpperBound { get { return this.upperBound; } }
	}
}
