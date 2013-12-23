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

#if !SILVERLIGHT && !LITE_EDITION

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Telerik.JustMock.Core
{
	internal static class ClrProfilerSetupHelper
	{
		private static readonly Dictionary<Guid, string> KnownProfilerGuids = new Dictionary<Guid, string>
		{
			{ new Guid("{06051352-82A1-41DB-BA4F-12D9D3DF4767}"), "JustTrace" },
			{ new Guid("{55347B27-3DDF-47FD-ACDC-490C8B04E111}"), "JustTrace" },
			{ new Guid("{9180FC24-0BC1-4A0E-ABEC-5F860697BD66}"), "JustTrace" },
			{ new Guid("{4681113A-86F0-44CE-855B-FED84C0924E5}"), "JustTrace" },
			{ new Guid("{B7ABE522-A68F-44F2-925B-81E7488E9EC0}"), "JustMock" },
			{ new Guid("{1542C21D-80C3-45E6-A56C-A9C1E4BEB7B8}"), "OpenCover" },
			{ new Guid("{A7A1EDD8-D9A9-4D51-85EA-514A8C4A9100}"), "OpenCover" },
			{ new Guid("{717FF691-2ADF-4AC0-985F-1DD3C42FDF90}"), "PartCover" },
			{ new Guid("{6287B5F9-08A1-45e7-9498-B5B2E7B02995}"), "NCover" },
			{ new Guid("{3FB1CC1E-1C17-4A37-9C18-BF3DB8F10E46}"), "NCover 1.5.8" },
			{ new Guid("{9721F7EB-5F92-447c-9F75-79278052B7BA}"), "NCover 3.x" },
			{ new Guid("{7D35BD6E-BC90-44D2-9DC7-4E555B643072}"), "NCover 4.x" },
			{ new Guid("{3E8C918E-5A84-4971-A6AF-20B97F48B6B3}"), "NCover 4.x" },
			{ new Guid("{B146457E-9AED-4624-B1E5-968D274416EC}"), "TypeMock Isolator" },
			{ new Guid("{5AC86959-7927-4177-9802-A540F1967874}"), "Fundamental Functions Code Coverage" },
			{ new Guid("{5017EBB4-4220-4965-A61F-035BEF6E98A9}"), "ANTS Performance Profiler" },
			{ new Guid("{60E3DCF2-1A65-4a44-9F41-038582C8181C}"), "ANTS Performance Profiler version 5.x" },
			{ new Guid("{A07CBDF0-B23B-4A96-A889-18E3C3004EB2}"), "ANTS Performance Profiler version 4.x" },
			{ new Guid("{4BEFAC55-31FA-4CF0-84B7-327248147851}"), "ANTS Memory Profiler version 4.x" },
			{ new Guid("{A7B0D313-5541-4DB1-B1C3-6665B7A428F9}"), "ANTS Memory Profiler" },
			{ new Guid("{D60F7519-2600-4865-8AC1-C621C9CE41A2}"), "ANTS Memory Profiler version 5.x" },
			{ new Guid("{9AE7D44D-DE91-47cb-9ABA-84BCAA0E5F54}"), "ANTS Profiler version 3.x" },
			{ new Guid("{0BC39BD5-4873-4221-A71C-6EB7B7CFF981}"), "dotTrace" },
			{ new Guid("{0BC39BD5-4873-4221-A71C-6EB7B7CFF982}"), "dotCover" },
			{ new Guid("{0A56A683-003A-41A1-A0AC-0F94C4913C48}"), "Visual Studio 2008 Profiler" },
			{ new Guid("{6468EC6C-94BD-40D3-BD93-4414565DAFBF}"), "Visual Studio 2008 Profiler" },
			{ new Guid("{09547427-F119-4a1f-A5B2-3D866632EFE0}"), "Visual Studio 2010 Profiler" },
			{ new Guid("{F1216318-0905-4fe8-B2E8-105CEB7CD689}"), "Visual Studio 2010 Profiler" },
			{ new Guid("{301EC75B-AD5A-459C-A4C4-911C878FA196}"), "Visual Studio 2010 IntelliTrace" },
			{ new Guid("{44A86CAD-F7EE-429C-83EB-F3CDE3B87B70}"), "Visual Studio 2012 Profiler" },
			{ new Guid("{D9DB81DB-81FE-4611-815D-144AD522E1B1}"), "Visual Studio 2012 Profiler" },
			{ new Guid("{B19F184A-CC62-4137-9A6F-AF0F91730165}"), "Visual Studio 2012 Code Coverage/IntelliTrace" },
			{ new Guid("{8C29BC4E-1F57-461a-9B51-1200C32E6F1F}"), "CLRProfiler" },
			{ new Guid("{FF68FEB9-E58A-4B75-A2B8-90CE7D915A26}"), "New Relic Agent" },
		};

		private const string NETFrameworkKey = @"SOFTWARE\Microsoft\.NETFramework";
		private const string NETFrameworkWoW64Key = @"SOFTWARE\Wow6432Node\Microsoft\.NETFramework";

		private const string NetFxCurrentUserKeyName = @"HKEY_CURRENT_USER\" + NETFrameworkKey;
		private const string NetFxCurrentUserKeyNameX32 = @"HKEY_CURRENT_USER\" + NETFrameworkWoW64Key;
		private const string SystemEnvironmentKeyName = @"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment";
		private const string UserEnvironmentKeyName = @"HKEY_CURRENT_USER\Environment";

		private const string CorSilverlightProfilerKey = "CORECLR_PROFILER";
		private const string CorGeneralProfilerKey = "COR_PROFILER";
		private const string CorSilverlightEnableProfilingKey = "CORECLR_ENABLE_PROFILING";
		private const string CorGeneralEnableProfilingKey = "COR_ENABLE_PROFILING";

		public static string GetEnabledProfilersLocations()
		{
			var locationsBuilder = new StringBuilder("\n");

			try
			{
				CheckRegistryLocation(NetFxCurrentUserKeyName, "current user registry setting", locationsBuilder);
				CheckRegistryLocation(NetFxCurrentUserKeyNameX32, "current user 32-bit registry setting", locationsBuilder);
				CheckRegistryLocation(SystemEnvironmentKeyName, "global system environment", locationsBuilder);
				CheckRegistryLocation(UserEnvironmentKeyName, "global user environment", locationsBuilder);
				CheckProcessEnvironment(locationsBuilder);
			}
			catch(Exception ex)
			{
			}

			return locationsBuilder.ToString().TrimEnd();
		}

		private static void CheckRegistryLocation(string keyName, string locationDescription, StringBuilder locationsBuilder)
		{
			var profiler = Registry.GetValue(keyName, CorGeneralProfilerKey, null);
			var enableProfiling = Registry.GetValue(keyName, CorGeneralEnableProfilingKey, null);

			if ((profiler != null || enableProfiling != null))
			{
				AddLocation(profiler != null ? profiler.ToString() : string.Empty, locationDescription, locationsBuilder);
			}
		}

		private static void CheckProcessEnvironment(StringBuilder locationsBuilder)
		{
			var profiler = Environment.GetEnvironmentVariable(CorGeneralProfilerKey);
			var enableProfiling = Environment.GetEnvironmentVariable(CorGeneralEnableProfilingKey);
			if (!String.IsNullOrEmpty(profiler) || !String.IsNullOrEmpty(enableProfiling))
			{
				AddLocation(profiler ?? "", "process environment", locationsBuilder);
			}
		}

		private static void AddLocation(string profiler, string locationDescription, StringBuilder locationsBuilder)
		{
			string profilerName = null;
			try
			{
				KnownProfilerGuids.TryGetValue(new Guid(profiler), out profilerName);
			}
			catch { }

			if (profilerName == null)
				profilerName = profiler.ToString();

			if (!profilerName.Contains("JustMock"))
			{
				locationsBuilder.AppendLine(string.Format("* {0} (from {1})", profilerName, locationDescription));
			}
		}
	}
}

#endif
