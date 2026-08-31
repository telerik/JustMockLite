/*
 JustMock Lite
 Copyright © 2010-2015,2018,2026 Progress Software Corporation

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
        /// <summary>
        /// Sets up future mocking for all instance members of type <typeparamref name="T"/>
        /// with <see cref="Behavior.RecursiveLoose"/> behavior.
        /// All future instances of <typeparamref name="T"/> created during the test will have
        /// their instance methods and properties intercepted with the default behavior.
        /// </summary>
        /// <remarks>
        /// This method requires the JustMock profiler (CodeWeaver). It is not available in JustMock Lite.
        /// Like <see cref="SetupStatic{T}()"/>, this call enables wholesale interception of all members
        /// of <typeparamref name="T"/>, including constructors. When using the default
        /// <see cref="Behavior.RecursiveLoose"/> behavior, constructor bodies do not execute — fields
        /// will not be initialized by the constructor. Use <see cref="Behavior.CallOriginal"/> if
        /// constructor execution is required, or arrange the constructor explicitly with
        /// <c>Mock.Arrange(() => new T()).CallOriginal()</c>.
        /// Per-method arrangements made after this call take precedence over the class-level default.
        /// </remarks>
        /// <typeparam name="T">Target type to future-mock.</typeparam>
        public static void SetupFuture<T>()
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings();
                MockingContext.CurrentRepository.InterceptFuture(typeof(T), settings);
            });
        }

        /// <summary>
        /// Sets up future mocking for all instance members of type <typeparamref name="T"/>
        /// with the specified behavior.
        /// </summary>
        /// <remarks>
        /// This method requires the JustMock profiler (CodeWeaver). It is not available in JustMock Lite.
        /// Like <see cref="SetupStatic{T}()"/>, this call enables wholesale interception of all members
        /// of <typeparamref name="T"/>, including constructors. When using <see cref="Behavior.RecursiveLoose"/>
        /// or <see cref="Behavior.Loose"/> behavior, constructor bodies do not execute — fields will not be
        /// initialized by the constructor. Use <see cref="Behavior.CallOriginal"/> if constructor execution
        /// is required, or arrange the constructor explicitly with <c>Mock.Arrange(() => new T()).CallOriginal()</c>.
        /// Per-method arrangements made after this call take precedence over the class-level default.
        /// </remarks>
        /// <typeparam name="T">Target type to future-mock.</typeparam>
        /// <param name="behavior">
        /// Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/>.
        /// </param>
        public static void SetupFuture<T>(Behavior behavior)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings(behavior);
                MockingContext.CurrentRepository.InterceptFuture(typeof(T), settings);
            });
        }

        /// <summary>
        /// Sets up future mocking for all instance members of the specified type
        /// with <see cref="Behavior.RecursiveLoose"/> behavior.
        /// </summary>
        /// <remarks>
        /// This method requires the JustMock profiler (CodeWeaver). It is not available in JustMock Lite.
        /// Like <see cref="SetupStatic(Type)"/>, this call enables wholesale interception of all members
        /// of <paramref name="type"/>, including constructors. Constructor bodies do not execute under the
        /// default <see cref="Behavior.RecursiveLoose"/> behavior. Use <see cref="Behavior.CallOriginal"/>
        /// if constructor execution is required.
        /// Per-method arrangements made after this call take precedence over the class-level default.
        /// </remarks>
        /// <param name="type">Target type to future-mock.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <c>null</c>.</exception>
        public static void SetupFuture(Type type)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                if (type == null)
                    throw new ArgumentNullException(nameof(type));

                MockCreationSettings settings = MockCreationSettings.GetSettings();
                MockingContext.CurrentRepository.InterceptFuture(type, settings);
            });
        }

        /// <summary>
        /// Sets up future mocking for all instance members of the specified type
        /// with the specified behavior.
        /// </summary>
        /// <remarks>
        /// This method requires the JustMock profiler (CodeWeaver). It is not available in JustMock Lite.
        /// Like <see cref="SetupStatic(Type)"/>, this call enables wholesale interception of all members
        /// of <paramref name="type"/>, including constructors. Constructor bodies do not execute under
        /// <see cref="Behavior.RecursiveLoose"/> or <see cref="Behavior.Loose"/> behavior. Use
        /// <see cref="Behavior.CallOriginal"/> if constructor execution is required.
        /// Per-method arrangements made after this call take precedence over the class-level default.
        /// </remarks>
        /// <param name="type">Target type to future-mock.</param>
        /// <param name="behavior">
        /// Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <c>null</c>.</exception>
        public static void SetupFuture(Type type, Behavior behavior)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                if (type == null)
                    throw new ArgumentNullException(nameof(type));

                MockCreationSettings settings = MockCreationSettings.GetSettings(behavior);
                MockingContext.CurrentRepository.InterceptFuture(type, settings);
            });
        }
#endif
    }
}
