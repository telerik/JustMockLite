/*
 JustMock Lite
 Copyright © 2010-2015,2018 Progress Software Corporation

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
using Telerik.JustMock.Core.Context;

namespace Telerik.JustMock
{
	public partial class Mock
	{
#if !LITE_EDITION
		///
		/// Warning, the method SetupStatic is used from Telerik.JustMock.VS.Implementation project to determine if JustMock is free version or comercial.
		///

		/// <summary>
		/// Setups the target for mocking all static calls with <see cref="Behavior.RecursiveLoose"/> behavior.
		/// </summary>
		/// <remarks>
		/// Considers all public members of the class. To mock private member,
		/// please use Mock.NonPublic 
		/// </remarks>
		/// <typeparam name="T">Target type</typeparam>
		public static void SetupStatic<T>()
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
                MockCreationSettings settings = MockCreationSettings.GetSettings();
                MockingContext.CurrentRepository.InterceptStatics(typeof(T), settings, false);
            });
		}

		/// <summary>
		/// Setups the target for mocking all static calls.
		/// </summary>
		/// <remarks>
		/// Considers all public members of the class. To mock private member,
		/// please use the private interface Mock.NonPublic
		/// </remarks>
		/// <param name="behavior">
		/// Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/>
		/// </param>
		/// <typeparam name="T">
		/// Target type
		/// </typeparam>
		public static void SetupStatic<T>(Behavior behavior)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
                MockCreationSettings settings = MockCreationSettings.GetSettings(behavior);
                MockingContext.CurrentRepository.InterceptStatics(typeof(T), settings, false);
            });
		}

		/// <summary>
		/// Setups the target for mocking all static calls.
		/// </summary>
		/// <param name="staticType">Static type</param>
		/// <param name="staticConstructor">Defines the behavior of the static constructor</param>
		public static void SetupStatic(Type staticType, StaticConstructor staticConstructor)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
                MockCreationSettings settings = MockCreationSettings.GetSettings();
                MockingContext.CurrentRepository.InterceptStatics(staticType, settings, staticConstructor == StaticConstructor.Mocked);
            });
		}

		/// <summary>
		/// Setups the target for mocking all static calls.
		/// </summary>
		/// <param name="staticType">Static type</param>
		public static void SetupStatic(Type staticType)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
                MockCreationSettings settings = MockCreationSettings.GetSettings();
                MockingContext.CurrentRepository.InterceptStatics(staticType, settings, false);
            });
		}

        /// <summary>
        /// Setups the target for mocking all static calls.
        /// </summary>
        /// <param name="staticType">Static type</param>
        /// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
        public static void SetupStatic(Type staticType, Behavior behavior)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
                MockCreationSettings settings = MockCreationSettings.GetSettings(behavior);
                MockingContext.CurrentRepository.InterceptStatics(staticType, settings, false);
			});
		}

        /// <summary>
        /// Setups the target for mocking all static calls.
        /// </summary>
        /// <param name="staticType">Static type</param>
        /// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
        /// <param name="staticConstructor">Defines the behavior of the static constructor</param>
        public static void SetupStatic(Type staticType, Behavior behavior, StaticConstructor staticConstructor)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
                MockCreationSettings settings = MockCreationSettings.GetSettings(behavior);
                MockingContext.CurrentRepository.InterceptStatics(staticType, settings, staticConstructor == StaticConstructor.Mocked);
			});
		}
#endif
	}
}
