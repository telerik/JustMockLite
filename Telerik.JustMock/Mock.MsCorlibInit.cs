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

namespace Telerik.JustMock
{
	/// <summary>
    /// Mock entry point for settting up expection, creating mock objects and verfiying results.
    /// </summary>
	public partial class Mock
	{
		 
		
        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="T1">Target type containing the second mocking member.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
        public static MsCorlibInitializer Replace<T, T1>(Expression<Action<T, T1>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T1">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="TRet">Type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
		public static MsCorlibInitializer Replace<T1, TRet>(Expression<Func<T1, TRet>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

		
        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="T1">Target type containing the second mocking member.</typeparam>
        /// <typeparam name="T2">Target type containing the third mocking member.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
        public static MsCorlibInitializer Replace<T, T1, T2>(Expression<Action<T, T1, T2>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T1">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="T2">Target type containing the second mocking member.</typeparam>
        /// <typeparam name="TRet">Type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
		public static MsCorlibInitializer Replace<T1, T2, TRet>(Expression<Func<T1, T2, TRet>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

		
        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="T1">Target type containing the second mocking member.</typeparam>
        /// <typeparam name="T2">Target type containing the third mocking member.</typeparam>
        /// <typeparam name="T3">Target type containing the fourth mocking member.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
        public static MsCorlibInitializer Replace<T, T1, T2, T3>(Expression<Action<T, T1, T2, T3>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T1">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="T2">Target type containing the second mocking member.</typeparam>
        /// <typeparam name="T3">Target type containing the third mocking member.</typeparam>
        /// <typeparam name="TRet">Type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
		public static MsCorlibInitializer Replace<T1, T2, T3, TRet>(Expression<Func<T1, T2, T3, TRet>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

		
        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="T1">Target type containing the second mocking member.</typeparam>
        /// <typeparam name="T2">Target type containing the third mocking member.</typeparam>
        /// <typeparam name="T3">Target type containing the fourth mocking member.</typeparam>
        /// <typeparam name="T4">Target type containing the fifth mocking member.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
        public static MsCorlibInitializer Replace<T, T1, T2, T3, T4>(Expression<Action<T, T1, T2, T3, T4>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T1">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="T2">Target type containing the second mocking member.</typeparam>
        /// <typeparam name="T3">Target type containing the third mocking member.</typeparam>
        /// <typeparam name="T4">Target type containing the fourth mocking member.</typeparam>
        /// <typeparam name="TRet">Type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
		public static MsCorlibInitializer Replace<T1, T2, T3, T4, TRet>(Expression<Func<T1, T2, T3, T4, TRet>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

		
        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="T1">Target type containing the second mocking member.</typeparam>
        /// <typeparam name="T2">Target type containing the third mocking member.</typeparam>
        /// <typeparam name="T3">Target type containing the fourth mocking member.</typeparam>
        /// <typeparam name="T4">Target type containing the fifth mocking member.</typeparam>
        /// <typeparam name="T5">Target type containing the sixth mocking member.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
        public static MsCorlibInitializer Replace<T, T1, T2, T3, T4, T5>(Expression<Action<T, T1, T2, T3, T4, T5>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T1">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="T2">Target type containing the second mocking member.</typeparam>
        /// <typeparam name="T3">Target type containing the third mocking member.</typeparam>
        /// <typeparam name="T4">Target type containing the fourth mocking member.</typeparam>
        /// <typeparam name="T5">Target type containing the fifth mocking member.</typeparam>
        /// <typeparam name="TRet">Type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
		public static MsCorlibInitializer Replace<T1, T2, T3, T4, T5, TRet>(Expression<Func<T1, T2, T3, T4, T5, TRet>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

		
        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="T1">Target type containing the second mocking member.</typeparam>
        /// <typeparam name="T2">Target type containing the third mocking member.</typeparam>
        /// <typeparam name="T3">Target type containing the fourth mocking member.</typeparam>
        /// <typeparam name="T4">Target type containing the fifth mocking member.</typeparam>
        /// <typeparam name="T5">Target type containing the sixth mocking member.</typeparam>
        /// <typeparam name="T6">Target type containing the seventh mocking member.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
        public static MsCorlibInitializer Replace<T, T1, T2, T3, T4, T5, T6>(Expression<Action<T, T1, T2, T3, T4, T5, T6>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T1">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="T2">Target type containing the second mocking member.</typeparam>
        /// <typeparam name="T3">Target type containing the third mocking member.</typeparam>
        /// <typeparam name="T4">Target type containing the fourth mocking member.</typeparam>
        /// <typeparam name="T5">Target type containing the fifth mocking member.</typeparam>
        /// <typeparam name="T6">Target type containing the sixth mocking member.</typeparam>
        /// <typeparam name="TRet">Type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
		public static MsCorlibInitializer Replace<T1, T2, T3, T4, T5, T6, TRet>(Expression<Func<T1, T2, T3, T4, T5, T6, TRet>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

		
        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="T1">Target type containing the second mocking member.</typeparam>
        /// <typeparam name="T2">Target type containing the third mocking member.</typeparam>
        /// <typeparam name="T3">Target type containing the fourth mocking member.</typeparam>
        /// <typeparam name="T4">Target type containing the fifth mocking member.</typeparam>
        /// <typeparam name="T5">Target type containing the sixth mocking member.</typeparam>
        /// <typeparam name="T6">Target type containing the seventh mocking member.</typeparam>
        /// <typeparam name="T7">Target type containing the eighth mocking member.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
        public static MsCorlibInitializer Replace<T, T1, T2, T3, T4, T5, T6, T7>(Expression<Action<T, T1, T2, T3, T4, T5, T6, T7>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T1">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="T2">Target type containing the second mocking member.</typeparam>
        /// <typeparam name="T3">Target type containing the third mocking member.</typeparam>
        /// <typeparam name="T4">Target type containing the fourth mocking member.</typeparam>
        /// <typeparam name="T5">Target type containing the fifth mocking member.</typeparam>
        /// <typeparam name="T6">Target type containing the sixth mocking member.</typeparam>
        /// <typeparam name="T7">Target type containing the seventh mocking member.</typeparam>
        /// <typeparam name="TRet">Type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
		public static MsCorlibInitializer Replace<T1, T2, T3, T4, T5, T6, T7, TRet>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, TRet>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

		
        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="T1">Target type containing the second mocking member.</typeparam>
        /// <typeparam name="T2">Target type containing the third mocking member.</typeparam>
        /// <typeparam name="T3">Target type containing the fourth mocking member.</typeparam>
        /// <typeparam name="T4">Target type containing the fifth mocking member.</typeparam>
        /// <typeparam name="T5">Target type containing the sixth mocking member.</typeparam>
        /// <typeparam name="T6">Target type containing the seventh mocking member.</typeparam>
        /// <typeparam name="T7">Target type containing the eighth mocking member.</typeparam>
        /// <typeparam name="T8">Target type containing the ninth mocking member.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
        public static MsCorlibInitializer Replace<T, T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Action<T, T1, T2, T3, T4, T5, T6, T7, T8>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

        /// <summary>
        ///	Replaces the body of target mscorlib member with mock handlers.
        /// </summary>
        /// <param name="expression">Target expression.</param>
        /// <typeparam name="T1">Target type containing the first mocking member.</typeparam>
        /// <typeparam name="T2">Target type containing the second mocking member.</typeparam>
        /// <typeparam name="T3">Target type containing the third mocking member.</typeparam>
        /// <typeparam name="T4">Target type containing the fourth mocking member.</typeparam>
        /// <typeparam name="T5">Target type containing the fifth mocking member.</typeparam>
        /// <typeparam name="T6">Target type containing the sixth mocking member.</typeparam>
        /// <typeparam name="T7">Target type containing the seventh mocking member.</typeparam>
        /// <typeparam name="T8">Target type containing the eighth mocking member.</typeparam>
        /// <typeparam name="TRet">Type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <returns>Initializer</returns>
        [Obsolete("It is no longer needed to call this method to mock mscorlib members.")]
		public static MsCorlibInitializer Replace<T1, T2, T3, T4, T5, T6, T7, T8, TRet>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TRet>> expression)
        {
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return new MsCorlibInitializer();
			});
        }

			
	  
	}
}
