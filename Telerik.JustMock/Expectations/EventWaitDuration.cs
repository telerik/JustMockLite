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
using Telerik.JustMock.Expectations.Abstraction;

namespace Telerik.JustMock.Expectations
{
	/// <summary>
	/// Defines the wait duration for a specific event.
	/// </summary>
	public sealed class EventWaitDuration : IWaitDuration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EventWaitDuration"/> class.
		/// </summary>
		/// <param name="miliSeconds">Time duration</param>
		public EventWaitDuration(int miliSeconds)
		{
			this.miliSeconds = miliSeconds;
		}

		/// <summary>
		/// Number of miliseconds to wait for executing the event.
		/// </summary>
		public int Miliseconds
		{
			get { return miliSeconds; }
		}

		private readonly int miliSeconds;
	}
}
