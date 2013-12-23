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
using System.Diagnostics;
using System.Linq;
using EQATEC.Analytics.Monitor;

namespace Telerik.JustMock.Analytics.EQATEC
{
	internal class JustMockEqatecAnalyticsTracker : IAnalyticsTracker
	{
		private const string JustMockApiKey = "8144A057C8544C8AB5728A283EEC9A7F";
		private const string JustMockPackageKey = "7184759771C341009EF7E189A844767C";
		private const string JustMockTestingKey = "31DEB0D3AF154095A321C6CB6DADF880";

		private readonly IAnalyticsMonitor monitor;

		private bool isEnabled = false;
		public bool IsEnabled
		{
			get
			{
				return this.isEnabled;
			}
			set
			{
				if (this.isEnabled != value)
				{
					this.isEnabled = value;
					if (!this.isEnabled)
					{
						monitor.Stop();
					}
					else
					{
						monitor.Start();
						this.TrackSystemAndJustMockInformation();
					}
				}
			}
		}

		private string InstallationId
		{
			get
			{
				return Guid.NewGuid().ToString();
			}
		}

		public JustMockEqatecAnalyticsTracker(AnalyticsAccountType accountType)
		{
			var productKey = "";

			if (AnalyticsConfiguration.AccountType == AnalyticsAccountType.JustMockTesting)
			{
				productKey = JustMockTestingKey;
			}
			else
			{
				productKey = accountType == AnalyticsAccountType.JustMockApi ? JustMockApiKey : JustMockPackageKey;
			}

			var settings = AnalyticsMonitorFactory.CreateSettings(productKey);
			settings.SynchronizeAutomatically = true;

			monitor = AnalyticsMonitorFactory.CreateMonitor(settings);
			monitor.SetInstallationInfo(InstallationId);
		}
		private void TrackSystemAndJustMockInformation()
		{
#if !SILVERLIGHT && !VISUALBASIC

			monitor.TrackFeature("Just Mock Version", JustMockInformation.JustMockVersion, null);
			monitor.TrackFeature("Silverlight", SystemInformation.SilverlightVersion, null);

			if(SystemInformation.IsWindows64)
				monitor.TrackFeature("Windows64", "1", null);

			var visualStudioVersions = SystemInformation.VisualStudioVersions;
			for (int i = 0; i < visualStudioVersions.Length; i++)
			{
				monitor.TrackFeature("Installed Visual Studio Version", visualStudioVersions[i], null);
			}

			var dotNetVersions = SystemInformation.DotNetVersions;
			for (int i = 0; i < dotNetVersions.Length; i++)
			{
				monitor.TrackFeature("Installed Dot Net Version", dotNetVersions[i], null);
			}
#endif

			if (JustMockInformation.IsJustTraceSamplingInstalled || JustMockInformation.IsJustTraceTracingInstalled || JustMockInformation.IsJustTraceMemoryInstalled || JustMockInformation.IsJustTraceWorkstationInstalled)
				monitor.TrackFeature("JustTrace", "1", null);

			if (JustMockInformation.IsNCover3Installed || JustMockInformation.IsNCover4Installed || JustMockInformation.IsNCover41Installed)
				monitor.TrackFeature("NCover", "1", null);

			if (JustMockInformation.IsOpenCoverInstalled || JustMockInformation.IsOpenCover64OldInstalled)
				monitor.TrackFeature("OpenCover", "1", null);

			if (JustMockInformation.IsPartCoverInstalled)
				monitor.TrackFeature("PartCover", "1", null);

			if (JustMockInformation.IsVsPerf9Installed || JustMockInformation.IsVsPerf9AltInstalled)
				monitor.TrackFeature("Visual Studio 2008", "1", null);

			if (JustMockInformation.IsVsPerf10Installed || JustMockInformation.IsVsPerf10AltInstalled)
				monitor.TrackFeature("Visual Studio 2010", "1", null);

			if (JustMockInformation.IsVsPerf11Installed || JustMockInformation.IsVsPerf11AltInstalled)
				monitor.TrackFeature("Visual Studio 2012", "1", null);

			if (JustMockInformation.IsIntelliTrace2010Installed)
				monitor.TrackFeature("IntelliTrace 2010", "1", null);

			if (JustMockInformation.IsIntelliTrace2012Installed)
				monitor.TrackFeature("IntelliTrace 2012", "1", null);

			if (JustMockInformation.IsMightyMouseInstalled || JustMockInformation.IsMightyMouse64Installed)
				monitor.TrackFeature("Mighty Mouse", "1", null);

#if !SILVERLIGHT
			monitor.TrackFeature("Current Working Visual Studio Version", JustMockInformation.GetVisualStudioVersion, null);
			monitor.TrackFeature("Current Working Visual Studio Edition", JustMockInformation.GetVisualStudioEdition, null);
			monitor.TrackFeature("Current Working Dot Net Version", JustMockInformation.GetDotNetVersion, null);
			monitor.TrackFeature("Current Working CLR Version", JustMockInformation.GetClrVersion, null);
#endif
		}

		public void TrackException(Exception exception)
		{
			if (!IsEnabled)
				return;

			monitor.TrackException(exception);
		}

		public void TrackEvent(AnalyticsData analyticsData)
		{
			if (!IsEnabled)
				return;

			monitor.TrackFeature(analyticsData.Id, "1", null);
		}
	}

	internal static class TrackerExtensions
	{
		public static string Categorize(string category, string feature)
		{
			if(feature != null)
				feature = feature.Replace('.', '_');

			return category + "." + feature;
		}

		public static void TrackFeature(this IAnalyticsMonitor monitor, string category, string feature, long? value)
		{
			Debug.Assert(!String.IsNullOrEmpty(category));

			string featureName = Categorize(category, feature);
			if (value == null)
			{
				monitor.TrackFeature(featureName);
			}
			else
			{
				monitor.TrackFeatureValue(featureName, value.Value);
			}
		}

		public static void TrackFeatureIfHasCategory(this IAnalyticsMonitor monitor, string category, string feature, long? value)
		{
			if (!String.IsNullOrEmpty(category))
			{
				monitor.TrackFeature(category, feature, value);
			}
		}
	}
}
