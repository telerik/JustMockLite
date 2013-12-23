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
using Telerik.JustMock.Core;

namespace Telerik.JustMock
{
	/// <summary>
	/// Specifies Mock.Assert to ignore any specific arguments.
	/// </summary>
	public sealed class Args
	{
		/// <summary>
		/// Gets or sets value indicating whether to ignore arguments.
		/// </summary>
		public bool IsIgnored { get; set; }

		/// <summary>
		/// Gets or sets value indicating whether to ignore instance.
		/// </summary>
		public bool IsInstanceIgnored { get; set; }

		/// <summary>
		/// Marks that Mock.Assert should ignore any argument match.
		/// </summary>
		/// <returns>Returns Args configuration.</returns>
		public static Args Ignore()
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new Args { IsIgnored = true };
			});
		}

		/// <summary>
		/// Marks that Mock.Assert should ignore the instance match.
		/// </summary>
		/// <returns>Returns Args configuration.</returns>
		public static Args IgnoreInstance()
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new Args { IsInstanceIgnored = true };
			});
		}

		/// <summary>
		/// Marks that Mock.Assert should ignore any argument match.
		/// </summary>
		/// <returns>Returns Args configuration.</returns>
		public Args AndIgnoreArguments()
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				this.IsIgnored = true;
				return this;
			});
		}

		/// <summary>
		/// Marks that Mock.Assert should ignore the instance match.
		/// </summary>
		/// <returns>Returns Args configuration.</returns>
		public Args AndIgnoreInstance()
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				this.IsInstanceIgnored = true;
				return this;
			});
		}

		/// <summary>
		/// Default behavior
		/// </summary>
		internal static Args NotSpecified()
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new Args();
			});
		}
	}
}
