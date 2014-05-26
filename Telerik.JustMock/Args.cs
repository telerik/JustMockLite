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
	public sealed partial class Args
	{
		/// <summary>
		/// Gets or sets value indicating whether to ignore arguments.
		/// </summary>
		/// <remarks>
		/// Unless explicitly specified, the arguments will be ignored by default if there is a filter present.
		/// </remarks>
		public bool? IsIgnored { get; set; }

		/// <summary>
		/// Gets or sets value indicating whether to ignore instance.
		/// </summary>
		/// <remarks>
		/// Unless explicitly specified, the instance will be ignored by default if there is a filter present
		/// and it takes as a first argument a 'this' argument.
		/// </remarks>
		public bool? IsInstanceIgnored { get; set; }

		/// <summary>
		/// Gets or sets a customized filter on the invocation arguments.
		/// </summary>
		/// <remarks>
		/// If a filter is specified it has to have the same signature as the asserted method,
		/// and may optionally have a first argument of the same type as the one declaring the method
		/// to receive the 'this' argument on which the method was called.
		/// </remarks>
		public Delegate Filter { get; set; }

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
		/// Specifies a condition on the invocation arguments. See <see cref="M:Telerik.JustMock.Args.Filter"/> for usage details.
		/// </summary>
		public static Args Matching(Delegate predicate)
		{
			return ProfilerInterceptor.GuardInternal(() => new Args().AndMatching(predicate));
		}

		/// <summary>
		/// Specifies a condition on the invocation arguments. See <see cref="M:Telerik.JustMock.Args.Filter"/> for usage details.
		/// </summary>
		public Args AndMatching(Delegate predicate)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				this.Filter = predicate;
				return this;
			});
		}

		internal static Args NotSpecified()
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new Args();
			});
		}
	}
}
