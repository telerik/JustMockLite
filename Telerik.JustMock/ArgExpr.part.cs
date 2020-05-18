/*
 JustMock Lite
 Copyright © 2020 Progress Software Corporation

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
using System.Linq.Expressions;


namespace Telerik.JustMock
{
	/// <summary>
	/// Provides various argument matching shortcuts.
	/// </summary>
	public partial class ArgExpr
	{
			
		/// <summary>
		/// Matches argument can contain any int value.
		/// </summary>
		public static Expression AnyInt
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<int>();
				});
			}
		}
			
		/// <summary>
		/// Matches argument can contain any float value.
		/// </summary>
		public static Expression AnyFloat
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<float>();
				});
			}
		}
			
		/// <summary>
		/// Matches argument can contain any double value.
		/// </summary>
		public static Expression AnyDouble
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<double>();
				});
			}
		}
			
		/// <summary>
		/// Matches argument can contain any decimal value.
		/// </summary>
		public static Expression AnyDecimal
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<decimal>();
				});
			}
		}
			
		/// <summary>
		/// Matches argument can contain any long value.
		/// </summary>
		public static Expression AnyLong
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<long>();
				});
			}
		}
			
		/// <summary>
		/// Matches argument can contain any char value.
		/// </summary>
		public static Expression AnyChar
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<char>();
				});
			}
		}
			
		/// <summary>
		/// Matches argument can contain any string value.
		/// </summary>
		public static Expression AnyString
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<string>();
				});
			}
		}
			
		/// <summary>
		/// Matches argument can contain any object value.
		/// </summary>
		public static Expression AnyObject
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<object>();
				});
			}
		}
			
		/// <summary>
		/// Matches argument can contain any short value.
		/// </summary>
		public static Expression AnyShort
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<short>();
				});
			}
		}
			
		/// <summary>
		/// Matches argument can contain any bool value.
		/// </summary>
		public static Expression AnyBool
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<bool>();
				});
			}
		}
			
		/// <summary>
		/// Matches argument can contain any Guid value.
		/// </summary>
		public static Expression AnyGuid
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<Guid>();
				});
			}
		}
			
		/// <summary>
		/// Matches argument can contain any DateTime value.
		/// </summary>
		public static Expression AnyDateTime
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<DateTime>();
				});
			}
		}
			
		/// <summary>
		/// Matches argument can contain any TimeSpan value.
		/// </summary>
		public static Expression AnyTimeSpan
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<TimeSpan>();
				});
			}
		}
			
		/// <summary>
		/// Matches argument can contain any byte value.
		/// </summary>
		public static Expression AnyByte
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<byte>();
				});
			}
		}
			
		/// <summary>
		/// Matches argument can contain any SByte value.
		/// </summary>
		public static Expression AnySByte
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<SByte>();
				});
			}
		}
			
		/// <summary>
		/// Matches argument can contain any Uri value.
		/// </summary>
		public static Expression AnyUri
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return ArgExpr.IsAny<Uri>();
				});
			}
		}
		
	}
}
