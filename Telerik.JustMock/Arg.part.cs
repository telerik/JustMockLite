/*
 JustMock Lite
 Copyright © 2010-2015 Telerik AD

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
using Telerik.JustMock.Core;


namespace Telerik.JustMock
{
	/// <summary>
	/// Provides various argument matching shortcuts.
	/// </summary>
	public static partial class Arg
	{
			
		/// <summary>
		/// Gets a value indicating that argument can contain any int value.
		/// </summary>
		[ArgIgnore]
		public static int AnyInt
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<int>();
				});
			}
		}
			
		/// <summary>
		/// Gets a value indicating that argument can contain any float value.
		/// </summary>
		[ArgIgnore]
		public static float AnyFloat
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<float>();
				});
			}
		}
			
		/// <summary>
		/// Gets a value indicating that argument can contain any double value.
		/// </summary>
		[ArgIgnore]
		public static double AnyDouble
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<double>();
				});
			}
		}
			
		/// <summary>
		/// Gets a value indicating that argument can contain any decimal value.
		/// </summary>
		[ArgIgnore]
		public static decimal AnyDecimal
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<decimal>();
				});
			}
		}
			
		/// <summary>
		/// Gets a value indicating that argument can contain any long value.
		/// </summary>
		[ArgIgnore]
		public static long AnyLong
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<long>();
				});
			}
		}
			
		/// <summary>
		/// Gets a value indicating that argument can contain any char value.
		/// </summary>
		[ArgIgnore]
		public static char AnyChar
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<char>();
				});
			}
		}
			
		/// <summary>
		/// Gets a value indicating that argument can contain any string value.
		/// </summary>
		[ArgIgnore]
		public static string AnyString
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<string>();
				});
			}
		}
			
		/// <summary>
		/// Gets a value indicating that argument can contain any object value.
		/// </summary>
		[ArgIgnore]
		public static object AnyObject
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<object>();
				});
			}
		}
			
		/// <summary>
		/// Gets a value indicating that argument can contain any short value.
		/// </summary>
		[ArgIgnore]
		public static short AnyShort
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<short>();
				});
			}
		}
			
		/// <summary>
		/// Gets a value indicating that argument can contain any bool value.
		/// </summary>
		[ArgIgnore]
		public static bool AnyBool
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<bool>();
				});
			}
		}
			
		/// <summary>
		/// Gets a value indicating that argument can contain any Guid value.
		/// </summary>
		[ArgIgnore]
		public static Guid AnyGuid
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<Guid>();
				});
			}
		}
			
		/// <summary>
		/// Gets a value indicating that argument can contain any DateTime value.
		/// </summary>
		[ArgIgnore]
		public static DateTime AnyDateTime
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<DateTime>();
				});
			}
		}
			
		/// <summary>
		/// Gets a value indicating that argument can contain any TimeSpan value.
		/// </summary>
		[ArgIgnore]
		public static TimeSpan AnyTimeSpan
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<TimeSpan>();
				});
			}
		}
			
		/// <summary>
		/// Gets a value indicating that argument can contain any byte value.
		/// </summary>
		[ArgIgnore]
		public static byte AnyByte
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<byte>();
				});
			}
		}
			
		/// <summary>
		/// Gets a value indicating that argument can contain any SByte value.
		/// </summary>
		[ArgIgnore]
		public static SByte AnySByte
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<SByte>();
				});
			}
		}
			
		/// <summary>
		/// Gets a value indicating that argument can contain any Uri value.
		/// </summary>
		[ArgIgnore]
		public static Uri AnyUri
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return Arg.IsAny<Uri>();
				});
			}
		}
		
	}
}
