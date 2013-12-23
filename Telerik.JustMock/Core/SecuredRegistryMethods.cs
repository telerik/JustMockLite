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

using System.Text;
using Microsoft.Win32;

namespace Telerik.JustMock.Core
{
	public delegate bool GetValueImplDelegate(bool currentUser, string keyName, string valueName, StringBuilder outValue, int outValueByteCount);
	public delegate bool SetValueImplDelegate(bool currentUser, string keyName, string valueName, string value);

	internal static class SecuredRegistryMethods
	{
		private static readonly GetValueImplDelegate getValue;
		private static readonly SetValueImplDelegate setValue;

		static SecuredRegistryMethods()
		{
			if (ProfilerInterceptor.IsProfilerAttached)
			{
				ProfilerInterceptor.WrapCallToDelegate("SetRegistryValue", out setValue);
				ProfilerInterceptor.WrapCallToDelegate("GetRegistryValue", out getValue);
			}
			else
			{
				getValue = (currentUser, keyName, valueName, outValue, outValueByteCount) => GetRegistryValue(currentUser, keyName, valueName, outValue);
				setValue = (currentUser, keyName, valueName, value) => SetRegistryValue(currentUser, keyName, valueName, value);
			}
		}

		public static string GetValue(bool currentUser, string keyName, string valueName)
		{
			StringBuilder outValue = new StringBuilder(256);
			if (!getValue(currentUser, keyName, valueName, outValue, outValue.Capacity * sizeof(char)))
				return null;
			return outValue.ToString();
		}

		public static bool SetValue(bool currentUser, string keyName, string valueName, string value)
		{
			return setValue(currentUser, keyName, valueName, value);
		}

		private static bool GetRegistryValue(bool currentUser, string keyName, string valueName, StringBuilder outValue)
		{
#if !SILVERLIGHT
			using (RegistryKey registryKey = currentUser ? Registry.CurrentUser : Registry.LocalMachine)
			{
				using (var key = registryKey.OpenSubKey(keyName))
				{
					if (key != null)
					{
						var filePath = key.GetValue(valueName);
						outValue.Append(filePath);

						return true;
					}
				}
			}
#endif
			return false;
		}

		private static bool SetRegistryValue(bool currentUser, string keyName, string valueName, string value)
		{
#if !SILVERLIGHT
			using (RegistryKey registryKey = currentUser ? Registry.CurrentUser : Registry.LocalMachine)
			{
				using (var key = registryKey.OpenSubKey(keyName, true))
				{
					if (key != null)
					{
						key.SetValue(valueName, value);
						return true;
					}
				}
			}
#endif
			return false;
		}
	}
}
