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
using System.Linq.Expressions;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Core.MatcherTree;

namespace Telerik.JustMock
{
	/// <summary>
	/// Allows specification of a matching condition for an argument, rather
	/// a specific value.
	/// </summary>
	public static partial class Arg
	{
		/// <summary>
		/// Matches argument for the expected condition.
		/// </summary>
		/// <typeparam name="T">
		/// Contains the type of the argument.
		/// </typeparam>
		/// <param name="match">Matcher expression</param>
		/// <returns>Argument type</returns>
		[ArgMatcher(Matcher = typeof(PredicateMatcher<>))]
		public static T Matches<T>(Expression<Predicate<T>> match)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AddMatcherInContext(new PredicateMatcher<T>(match));
				return default(T);
			});
		}

		/// <summary>
		/// Matches argument for the specified range.
		/// </summary>
		/// <typeparam name="T">Type of the argument.</typeparam>
		/// <param name="from">starting value.</param>
		/// <param name="to">ending value.</param>
		/// <param name="kind">Kind of Range</param>
		/// <returns>Argument type</returns>
		[ArgMatcher(Matcher = typeof(RangeMatcher<>))]
		public static T IsInRange<T>(T from, T to, RangeKind kind) where T : IComparable
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AddMatcherInContext(new RangeMatcher<T>(from, to, kind));
				return default(T);
			});
		}

		/// <summary>
		/// Matches argument for any value.
		/// </summary>
		/// <typeparam name="T">Type for the argument</typeparam>
		/// <returns>Argument type</returns>
		[ArgIgnore]
		public static T IsAny<T>()
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AddMatcherInContext(new TypeMatcher(typeof(T)));
				return default(T);
			});
		}

		/// <summary>
		/// Matches argument for null value.
		/// </summary>
		/// <typeparam name="T">Type for the argument</typeparam>
		/// <returns>Argument type</returns>
		[ArgMatcher(Matcher = typeof(ValueMatcher), MatcherArgs = new object[] { null })]
		public static T IsNull<T>()
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AddMatcherInContext(new ValueMatcher(null));
				return default(T);
			});
		}
	  
		/// <summary>
		/// Matches argument for null or empty value.
		/// </summary>
		/// <returns>Null</returns>
		[ArgMatcher(Matcher = typeof(StringNullOrEmptyMatcher))]
		public static string NullOrEmpty
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					MockingContext.CurrentRepository.AddMatcherInContext(new StringNullOrEmptyMatcher());
					return String.Empty;
				});
			}
		}

		/// <summary>
		/// Matches the specified value. Useful for mingling concrete values and more general matchers
		/// in the same expression when using delegate-based overloads.
		/// </summary>
		/// <typeparam name="T">Type for the argument</typeparam>
		/// <param name="value">Value to match</param>
		/// <returns>Argument type</returns>
		[ArgMatcher(Matcher = typeof(ValueMatcher))]
		public static T Is<T>(T value)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AddMatcherInContext(new ValueMatcher(value));
				return default(T);
			});
		}

		/// <summary>
		/// An implementation detail that allows passing ref arguments in C#
		/// </summary>
		/// <typeparam name="T">Type for the argument</typeparam>
		public sealed class OutRefResult<T>
		{
			/// <summary>
			/// Pass this member as a ref argument in C#
			/// </summary>
			[RefArg]
			public T Value;
		}

		/// <summary>
		/// Applies a matcher to a 'ref' parameter.
		/// 
		/// By default, 'ref' parameters work like implicitly
		/// arranged return values. In other words, you arrange a method to return a given value
		/// through its 'ref' and 'out' parameters. Use this method to specify that the
		/// argument should have a matcher applied just like regular arguments.
		/// </summary>
		/// <typeparam name="T">Type for the argument</typeparam>
		/// <example>
		/// interface IHasRef
		/// {
		///     int PassRef(ref int a);
		/// }
		/// 
		/// var mock = Mock.Create&lt;IHasRef&gt;()
		/// Mock.Arrange(() => mock.PassRef(ref Arg.Ref(100).Value).Returns(200);
		/// 
		/// The above example arranges PassRef to return 200 whenever its argument is 100.
		/// </example>
		/// <param name="value">A matcher or a value.</param>
		/// <returns>A special value with member 'Value' that must be passed by ref.</returns>
		public static OutRefResult<T> Ref<T>(T value)
		{
			return null;
		}
	}
}
