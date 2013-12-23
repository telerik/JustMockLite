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
using System.Linq;

namespace Telerik.JustMock.Analytics
{
	internal class CompositeAnalyticsTracker : IAnalyticsTracker
	{
		private readonly List<IAnalyticsTracker> trackers = new List<IAnalyticsTracker>();

		private bool isEnabled = true;
		public bool IsEnabled
		{
			get { return this.isEnabled; }
			set
			{
				this.isEnabled = value;
				foreach (var tracker in this.trackers)
					tracker.IsEnabled = value;
			}
		}

		public void AddTracker(IAnalyticsTracker tracker)
		{
			this.trackers.Add(tracker);
		}

		public void TrackException(Exception exception)
		{
			foreach (var tracker in this.trackers)
				tracker.TrackException(exception);
		}

		public void TrackEvent(AnalyticsData analyticsData)
		{
			foreach (var tracker in this.trackers)
				tracker.TrackEvent(analyticsData);
		}
	}
}
