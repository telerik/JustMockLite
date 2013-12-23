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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace Telerik.JustMock.Analytics
{
	internal static class SystemInformation
	{
		#if !SILVERLIGHT && !VISUALBASIC
		public static string OSVersion
		{
			get
			{
				var result = String.Format("{0} ({1})",
											Environment.OSVersion.VersionString,
											IsWindows64 ? "x64" : "x86");
				return result;
			}
		}

		public static string[] DotNetVersions
		{
			get
			{
				var versions = new List<string>();
				using(RegistryKey dotNetRoot = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP"))
				{
					var subkeyNames = dotNetRoot.GetSubKeyNames().Where(name => name.StartsWith("v"));

					foreach (var subkeyName in subkeyNames)
					{
						var verStr = subkeyName.Substring(1);

						if(IsVersionString(verStr))
						{
							using(var verKey = dotNetRoot.OpenSubKey(subkeyName))
							{
								if(verKey != null && (int)verKey.GetValue("Install", 0, RegistryValueOptions.None) == 1)
								{
									versions.Add(subkeyName);
								}
							}
						}
						else
						{
							using(var verKey = dotNetRoot.OpenSubKey(subkeyName + @"\Client"))
							{
								if(verKey != null && (int)verKey.GetValue("Install", 0, RegistryValueOptions.None) == 1)
								{
									versions.Add(subkeyName);
								}
							}
						}
					}
				}

				return versions.ToArray();
			}
		}

		public static string[] VisualStudioVersions
		{
			get
			{
				var versions = new List<string>();
				using(RegistryKey root = Registry.LocalMachine.OpenSubKey(VSNodePath))
				{
					if(root != null)
					{
						var subKeyNames = root.GetSubKeyNames();

						foreach (var subKeyName in subKeyNames)
						{
							if(IsVersionString(subKeyName))
							{
								using(var verKey = root.OpenSubKey(subKeyName))
								{
									var installDir = (string)verKey.GetValue("InstallDir", null);
									if(installDir != null)
									{
										var devenv = Path.Combine(installDir, "devenv.exe");
										var exeVer = FileVersionInfo.GetVersionInfo(devenv);
										versions.Add(exeVer.ProductVersion);
									}
								}
							}
						}
					}
				}

				if (versions.Count == 0)
					versions.Add("Not Installed");

				return versions.ToArray();
			}
		}

		private static string VSNodePath
		{
			get
			{
				var result = new StringBuilder(@"SOFTWARE\");

				if (IsWindows64)
					result.Append(@"Wow6432Node\");

				result.Append(@"Microsoft\VisualStudio");

				return result.ToString();
			}
		}

		private static bool IsVersionString(string name)
		{
			try
			{
				new Version(name);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool IsWindows64
		{
			get
			{
				var isWindows64Bit = false;

				if (IntPtr.Size == 8)
				{
					isWindows64Bit = true;
				}
				else
				{
					using (var p = Process.GetCurrentProcess())
					{
						try
						{
							if (IsWow64Process(p.Handle, out isWindows64Bit))
								throw new Win32Exception(Marshal.GetLastWin32Error());
						}
						catch (Exception)
						{
							isWindows64Bit = false;
						}
					}
				}

				return isWindows64Bit;
			}
		}

		public static string SilverlightVersion
		{
			get
			{
				try
				{
					var registryCheck = (IntPtr.Size == 8) ? @"SOFTWARE\Wow6432Node\Microsoft\Silverlight" : @"SOFTWARE\Microsoft\Silverlight";
					using (var regKey = Registry.LocalMachine.OpenSubKey(registryCheck, writable: false))
					{
						return (string)regKey.GetValue("Version");
					}
				}
				catch
				{
					return "Not found";
				}
			}
		}

		[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWow64Process([In] IntPtr processHandle, [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);
		#endif
	}
}
