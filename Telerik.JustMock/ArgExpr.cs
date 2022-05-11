/*
 JustMock Lite
 Copyright © 2010-2015,2022 Progress Software Corporation

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
using System.ComponentModel;
using System.Linq.Expressions;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.MatcherTree;

namespace Telerik.JustMock
{
	/// <summary>
	/// Allows specification of a matching condition for an argument for a non-public method, rather
	/// a specific value.
	/// </summary>
#if !PORTABLE
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
#endif
	public partial class ArgExpr
	{
		internal ArgExpr() { }

		/// <summary>
		/// Matches argument for any value.
		/// </summary>
		/// <typeparam name="T">Type for the argument</typeparam>
		/// <returns>Argument type</returns>
		public static Expression IsAny<T>()
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Expression<Func<T>> expr = () => Arg.IsAny<T>();
				return expr.Body;
			});
		}

		/// <summary>
		/// Matches argument for any value of a given type.
		/// </summary>
		/// <param name="type">Type for the argument</param>
		/// <param name="args">Constructor arguments</param>
		/// <returns>Argument type</returns>
		public static Expression IsAny(Type type, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Expression<Func<object>> expr = () => Arg.IsAny(type, args);
				return expr.Body;
			});
		}

		/// <summary>
		/// Matches argument for the expected condition.
		/// </summary>
		/// <typeparam name="T">
		/// Contains the type of the argument.
		/// </typeparam>
		/// <param name="match">Matcher expression</param>
		/// <returns>Argument type</returns>
		public static Expression Matches<T>(Expression<Predicate<T>> match)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Expression<Func<T>> expr = () => Arg.Matches<T>(match);
				return expr.Body;
			});
		}

		/// <summary>
		/// Matches argument for null value.
		/// </summary>
		/// <typeparam name="T">Type for the argument</typeparam>
		/// <returns>Argument type</returns>
		public static Expression IsNull<T>()
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Expression<Func<T>> expr = () => Arg.IsNull<T>();
				return expr.Body;
			});
		}

		/// <summary>
		/// Returns a value from a ref or out argument
		/// </summary>
		/// <typeparam name="T">Type for the argument</typeparam>
		/// <param name="value">Value to match.</param>
		/// <returns>Argument type</returns>
		public static Expression Out<T>(T value)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Expression<Func<T>> expr = () => OutArg(value);
				return expr.Body;
			});
		}

		[OutArg]
		private static T OutArg<T>(T value)
		{
			return value;
		}
	
		/// <summary>
		/// Matches a value for ref argument
		/// </summary>
		/// <typeparam name="T">Type for the argument</typeparam>
		/// <param name="value">Value to match.</param>
		/// <returns>Argument type</returns>
		public static Expression Ref<T>(T value)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				Expression<Func<T>> expr = () => Arg.Ref<T>(value).Value;
				return expr.Body;
			});
		}
	}
}
