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
using Telerik.JustMock.Core;
using Telerik.JustMock.Expectations.Abstraction;

namespace Telerik.JustMock.Expectations
{
	/// <summary>
	/// Implements common expecations.
	/// </summary>
	public partial class FuncExpectation<TReturn>
	{
				
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1>(Func<T1, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
			
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1, T2>(Func<T1, T2, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
			
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1, T2, T3>(Func<T1, T2, T3, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
			
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
			
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
			
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
			
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
			
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
			
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
			
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T10">Type of the tenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
			
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T10">Type of the tenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T11">Type of the eleventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
			
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T10">Type of the tenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T11">Type of the eleventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T12">Type of the twelveth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
			
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T10">Type of the tenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T11">Type of the eleventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T12">Type of the twelveth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T13">Type of the thirteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
			
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T10">Type of the tenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T11">Type of the eleventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T12">Type of the twelveth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T13">Type of the thirteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T14">Type of the fourteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
			
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T10">Type of the tenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T11">Type of the eleventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T12">Type of the twelveth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T13">Type of the thirteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T14">Type of the fourteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T15">Type of the fifteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
			
		/// <summary>
		/// Specifies the delegate that will execute and return the value for the expected member.
		/// </summary>
		/// <typeparam name="T1">Type of the first parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T2">Type of the second parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T3">Type of the third parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T4">Type of the fourth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T5">Type of the fifth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T6">Type of the sixth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T7">Type of the seventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T8">Type of the eighth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T9">Type of the ninth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T10">Type of the tenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T11">Type of the eleventh parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T12">Type of the twelveth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T13">Type of the thirteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T14">Type of the fourteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T15">Type of the fifteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <typeparam name="T16">Type of the sixteenth parameter of the anonymous method that this delegate encapsulates</typeparam>
		/// <returns>Reference to <see cref="IAssertable"/> interface.</returns>
		public IAssertable Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					this.ProcessDoInstead(func, false);
					return this;
				});
		}
	}
}
