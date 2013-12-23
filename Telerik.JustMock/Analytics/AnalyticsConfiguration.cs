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
using System.Text;
using Microsoft.Win32;
using Telerik.JustMock.Core;

namespace Telerik.JustMock.Analytics
{
	internal enum AnalyticsAccountType
	{
		JustMockApi,
		JustMockTesting,
		JustMockPackage,
	}

	internal static class AnalyticsConfiguration
	{
		public static readonly string RegistryEnableAnalyticsValueName = "EnableAnalytics";
		public static readonly string RegistryPathToJustMock = @"SOFTWARE\Telerik\JustMock";

		public static AnalyticsAccountType AccountType
		{
			get
			{
#if DEBUG
				var accountType = AnalyticsAccountType.JustMockTesting;
#else
				var accountType = AnalyticsAccountType.JustMockApi;
#endif
				object registryValue = SecuredRegistryMethods.GetValue(false, @"SOFTWARE\Telerik\JustMock", "UseTestingBIAccount");

				int? value = registryValue as int?;

				if (value != null && value == 1)
					accountType = AnalyticsAccountType.JustMockTesting;

				return accountType;
			}
		}

		public static bool IsAnalyticsGloballyDisabled()
		{
			// Analytics is disabled if the registry value [HKLM\Software\Telerik]DisableAnalytics
			// or the value [HKLM\Software\Wow6432Node\Telerik]DisableAnalytics (64-bit process only)
			// exists, is convertible to an integer and is different from "0"
			const string ValueName = "DisableAnalytics";

			string value = SecuredRegistryMethods.GetValue(false, @"SOFTWARE\Telerik", ValueName);

			if (value != null && IsValueTrue(value))
				return true;

			if (IntPtr.Size == 8)
			{
				// if this is a 64-bit process then check the 32-bit Wow64 key
				string otherValue = SecuredRegistryMethods.GetValue(false, @"SOFTWARE\Wow6432Node\Telerik", ValueName);

				if (otherValue != null && IsValueTrue(otherValue))
					return true;
			}

			return false;
		}

		private static bool IsValueTrue(object value)
		{
			// if the value is of a string type, then parse it as an int and see if it's different from 0
			var asString = value as string;
			if (asString != null)
			{
				int intValue;
				return int.TryParse(asString, out intValue) && intValue != 0;
			}

			// it may be REG_DWORD, cast it and see if it's different from 0
			var asInt = value as int?;
			if (asInt != null)
				return asInt != 0;

			// it may be REG_QWORD, cast it and see if it's different from 0
			var asLong = value as long?;
			if (asLong != null)
				return asLong != 0;

			// the value exists but is of an unsupported type. Assume this means 'true'
			return true;
		}

		public static bool EnableAnalytics 
		{
			get
			{
				string registryPathToJustMock = RegistryPathToJustMock;
				var value = SecuredRegistryMethods.GetValue(true, registryPathToJustMock, RegistryEnableAnalyticsValueName);	
	
				return (value == "True");
			}
		}
	}
}
