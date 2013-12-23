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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Telerik.JustMock.Core;

namespace Telerik.JustMock.Analytics
{
	internal static class JustMockInformation
	{
		#region GUIDS
		
		private static readonly string NCOVER3GUID = "9721F7EB-5F92-447c-9F75-79278052B7BA";
		private static readonly string NCOVER4GUID = "7D35BD6E-BC90-44D2-9DC7-4E555B643072";
		private static readonly string NCOVER41GUID = "3E8C918E-5A84-4971-A6AF-20B97F48B6B3";
		
		private static readonly string OPENCOVERGUID = "1542C21D-80C3-45E6-A56C-A9C1E4BEB7B8";
		private static readonly string OPENCOVER64OLDGUID = "A7A1EDD8-D9A9-4D51-85EA-514A8C4A9100";
		
		private static readonly string PARTCOVERGUID = "717FF691-2ADF-4AC0-985F-1DD3C42FDF90";
		
		private static readonly string VSPERF9GUID = "0A56A683-003A-41A1-A0AC-0F94C4913C48";
		private static readonly string VSPERF9ALTGUID = "6468EC6C-94BD-40D3-BD93-4414565DAFBF";
		private static readonly string VSPERF10GUID = "09547427-F119-4a1f-A5B2-3D866632EFE0";
		private static readonly string VSPERF10ALTGUID = "F1216318-0905-4fe8-B2E8-105CEB7CD689";
		private static readonly string VSPERF11GUID = "44A86CAD-F7EE-429C-83EB-F3CDE3B87B70";
		private static readonly string VSPERF11ALTGUID = "D9DB81DB-81FE-4611-815D-144AD522E1B1";
		
		private static readonly string INTELLITRACE2010GUID = "301EC75B-AD5A-459C-A4C4-911C878FA196";
		private static readonly string INTELLITRACE2012GUID = "B19F184A-CC62-4137-9A6F-AF0F91730165";
		
		private static readonly string MIGHTYMOUSEGUID = "36C8D782-F697-45C4-856A-92D05C061A39";
		private static readonly string MIGHTYMOUSE64GUID = "5D789D88-EEE7-46C4-909F-E39D5606544D";
		
		private static readonly string JUSTTRACESAMPLING = "4681113A-86F0-44CE-855B-FED84C0924E5";
		private static readonly string JUSTTRACETRACING = "9180FC24-0BC1-4A0E-ABEC-5F860697BD66";
		private static readonly string JUSTTRACEMEMORY = "06051352-82A1-41DB-BA4F-12D9D3DF4767";
		private static readonly string JUSTTRACEWORKSTATION = "55347B27-3DDF-47FD-ACDC-490C8B04E111";
		
		#endregion
		
		public static bool IsJustMockDllInProgramFiles
		{
			get
			{
				string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Telerik\JustMock\Libraries\Telerik.JustMock.dll");
				return File.Exists(path);
			}
		}
		
		#region JustTrace
		
		public static bool IsJustTraceSamplingInstalled
		{
			get
			{
				return IsProfilerInstalled(JUSTTRACESAMPLING);
			}
		}
		
		public static bool IsJustTraceTracingInstalled
		{
			get
			{
				return IsProfilerInstalled(JUSTTRACETRACING);
			}
		}
		
		public static bool IsJustTraceMemoryInstalled
		{
			get
			{
				return IsProfilerInstalled(JUSTTRACEMEMORY);
			}
		}
		
		public static bool IsJustTraceWorkstationInstalled
		{
			get
			{
				return IsProfilerInstalled(JUSTTRACEWORKSTATION);
			}
		}
		
		#endregion
		
		#region NCover 
		
		public static bool IsNCover3Installed
		{
			get
			{
				return IsProfilerInstalled(NCOVER3GUID);
			}
		}
		
		public static bool IsNCover4Installed
		{
			get
			{
				return IsProfilerInstalled(NCOVER4GUID);
			}
		}
		
		public static bool IsNCover41Installed
		{
			get
			{
				return IsProfilerInstalled(NCOVER41GUID);
			}
		}
		
		#endregion
		
		#region OpenCover
		
		public static bool IsOpenCoverInstalled
		{
			get
			{
				return IsProfilerInstalled(OPENCOVERGUID);
			}
		}
		
		public static bool IsOpenCover64OldInstalled
		{
			get
			{
				return IsProfilerInstalled(OPENCOVER64OLDGUID, true);
			}
		}
		
		#endregion
		
		#region PartCover
		
		public static bool IsPartCoverInstalled
		{
			get
			{
				return IsProfilerInstalled(PARTCOVERGUID);
			}
		}
		
		#endregion
		
		#region VSPerf
		
		public static bool IsVsPerf9Installed
		{
			get
			{
				return IsProfilerInstalled(VSPERF9GUID);
			}
		}
		
		public static bool IsVsPerf9AltInstalled
		{
			get
			{
				return IsProfilerInstalled(VSPERF9ALTGUID);
			}
		}
		
		public static bool IsVsPerf10Installed
		{
			get
			{
				return IsProfilerInstalled(VSPERF10GUID);
			}
		}
		
		public static bool IsVsPerf10AltInstalled
		{
			get
			{
				return IsProfilerInstalled(VSPERF10ALTGUID);
			}
		}
		
		public static bool IsVsPerf11Installed
		{
			get
			{
				return IsProfilerInstalled(VSPERF11GUID);
			}
		}
		
		public static bool IsVsPerf11AltInstalled
		{
			get
			{
				return IsProfilerInstalled(VSPERF11ALTGUID);
			}
		}
		
		#endregion
		
		#region IntelliTrace
		
		public static bool IsIntelliTrace2010Installed
		{
			get
			{
				return IsProfilerInstalled(INTELLITRACE2010GUID);
			}
		}
		
		public static bool IsIntelliTrace2012Installed
		{
			get
			{
				return IsProfilerInstalled(INTELLITRACE2012GUID);
			}
		}
		
		#endregion
		
		#region MightyMouse
		
		public static bool IsMightyMouseInstalled
		{
			get
			{
				return IsProfilerInstalled(MIGHTYMOUSEGUID);
			}
		}
		
		public static bool IsMightyMouse64Installed
		{
			get
			{
				return IsProfilerInstalled(MIGHTYMOUSE64GUID, true);
			}
		}
		
		#endregion
		
		private static bool IsProcess64
		{
			get
			{
				return IntPtr.Size == 8;
			}
		}

		private static bool IsProfilerInstalled(string profilerGuid, bool isGuidOnlyFor64BitsProfiler = false)
		{
			StringBuilder path = new StringBuilder();

			path.Append(@"Software\Classes");
			
			if (IsProcess64 || isGuidOnlyFor64BitsProfiler)
				path.Append(@"\Wow6432Node");
			
			path.Append(@"\CLSID\");
			path.Append(profilerGuid);

			string filePath = SecuredRegistryMethods.GetValue(true, path.ToString(), "InprocServer32");

			if(String.IsNullOrEmpty(filePath))
			{
				filePath = SecuredRegistryMethods.GetValue(false, path.ToString(), "InprocServer32");
			}
			
			return File.Exists(filePath);
		}

#if !SILVERLIGHT

		private static readonly StringDictionary environmentVariablesForCurrentProcess = new StringDictionary();

		static JustMockInformation()
		{
			environmentVariablesForCurrentProcess = GetEnvironmentVariablesForCurrentProcess();
		}

		public static string JustMockVersion
		{
			get
			{
				return typeof(SystemInformation).Assembly.GetName().Version.ToString();
			}
		}

		public static string GetDotNetVersion
		{
			get
			{
				foreach (AssemblyName assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
				{
					if (assemblyName.Name == "mscorlib")
						return assemblyName.Version.ToString();
				}

				return "";
			}
		}

		private static StringDictionary GetEnvironmentVariablesForCurrentProcess()
		{
			Process process = Process.GetCurrentProcess();
			HashSet<int> processes = new HashSet<int>();

			while (process.ProcessName != "MSBuild" && process.ProcessName != "devenv")
			{
				if (!processes.Contains(process.Id))
				{
					processes.Add(process.Id);
					process = ParentProcessUtilities.GetParentProcess();
				}
				else 
				{
					return null;
				}
			}

			return process.StartInfo.EnvironmentVariables;
		}

		public static string GetVisualStudioVersion
		{
			get
			{
				return environmentVariablesForCurrentProcess != null ? environmentVariablesForCurrentProcess["VisualStudioVersion"] : "Not Detected";
			}
		}

		public static string GetVisualStudioEdition
		{
			get
			{
				return environmentVariablesForCurrentProcess != null ? environmentVariablesForCurrentProcess["VisualStudioEdition"] : "Not Detected";
			}
		}

		public static string GetClrVersion
		{
			get
			{
				return Environment.Version.ToString();
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ParentProcessUtilities
		{
			// These members must match PROCESS_BASIC_INFORMATION
			internal IntPtr Reserved1;
			internal IntPtr PebBaseAddress;
			internal IntPtr Reserved2_0;
			internal IntPtr Reserved2_1;
			internal IntPtr UniqueProcessId;
			internal IntPtr InheritedFromUniqueProcessId;

			[DllImport("ntdll.dll")]
			private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);

			/// <summary>
			/// Gets the parent process of the current process.
			/// </summary>
			/// <returns>An instance of the Process class.</returns>
			public static Process GetParentProcess()
			{
				return GetParentProcess(Process.GetCurrentProcess().Handle);
			}

			/// <summary>
			/// Gets the parent process of specified process.
			/// </summary>
			/// <param name="id">The process id.</param>
			/// <returns>An instance of the Process class.</returns>
			public static Process GetParentProcess(int id)
			{
				Process process = Process.GetProcessById(id);
				return GetParentProcess(process.Handle);
			}

			/// <summary>
			/// Gets the parent process of a specified process.
			/// </summary>
			/// <param name="handle">The process handle.</param>
			/// <returns>An instance of the Process class.</returns>
			public static Process GetParentProcess(IntPtr handle)
			{
				ParentProcessUtilities pbi = new ParentProcessUtilities();
				int returnLength;
				int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
				if (status != 0)
					throw new Win32Exception(status);

				try
				{
					return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
				}
				catch (ArgumentException)
				{
					// not found
					return null;
				}
			}
		}
#endif
	}
}
