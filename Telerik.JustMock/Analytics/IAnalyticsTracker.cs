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
using System.Reflection;

#if VSADDIN
using Telerik.JustMock.AddIn;
#endif

namespace Telerik.JustMock.Analytics
{
	internal interface IAnalyticsTracker
	{
		bool IsEnabled { get; set; }
		void TrackEvent(AnalyticsData analyticsData);
		void TrackException(Exception exception);
	}

	internal class AnalyticsTracker
	{
		public static AnalyticsTracker Instance { get; private set; }

		public static IAnalyticsTracker Current { get; private set; }

		private static Dictionary<string, byte[]> resourceAssembliesHash = new Dictionary<string, byte[]>();

		static AnalyticsTracker()
		{
			resourceAssembliesHash.Add("EQATEC.Analytics.Monitor", Resources.EQATEC_Analytics_Monitor);
			resourceAssembliesHash.Add("EQATEC.Analytics.Monitor.SL", Resources.EQATEC_Analytics_MonitorSL);

#if SILVERLIGHT
			System.Windows.AssemblyPart assemblyPart = new System.Windows.AssemblyPart();
			assemblyPart.Load(new System.IO.MemoryStream(resourceAssembliesHash["EQATEC.Analytics.Monitor.SL"]));
#else
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
#endif
			Instance = new AnalyticsTracker();
		}

		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
#if !SILVERLIGHT
			var assemblyName = args.Name.Split(',').First();

			if (resourceAssembliesHash.ContainsKey(assemblyName))
			{
				return Assembly.Load(resourceAssembliesHash[assemblyName]);
			}
#endif
			return null;
		}

		public void CreateDefaultTracker(bool isTrackerEnabled, AnalyticsAccountType accountType = AnalyticsAccountType.JustMockApi)
		{
			var theTrackers = new CompositeAnalyticsTracker();
			theTrackers.AddTracker(new EQATEC.JustMockEqatecAnalyticsTracker(accountType));

			bool enableAnalytics = !AnalyticsConfiguration.IsAnalyticsGloballyDisabled() && isTrackerEnabled;

			IAnalyticsTracker tracker = theTrackers;
			tracker.IsEnabled = enableAnalytics;

			RegisterTracker(tracker);
		}

		public static void RegisterTracker(IAnalyticsTracker tracker)
		{
			Current = tracker;
		}
	}
}
