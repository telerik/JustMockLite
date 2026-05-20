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
using System.Linq.Expressions;
using Telerik.JustMock.Abstraction;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Setup;

namespace Telerik.JustMock
{
    public partial class Mock
    {
        /// <summary>
        /// Create a mocked instance from specified real constructor with <see cref="Behavior.RecursiveLoose"/> behavior.
        /// </summary>
        /// <typeparam name="T">Type to mock.</typeparam>
        /// <param name="expression">Constructor expression that identifies the type and constructor arguments.</param>
        /// <returns>A mock instance that uses <see cref="Behavior.RecursiveLoose"/> behavior.</returns>
        public static T Create<T>(Expression<Func<T>> expression)
        {
            return ProfilerInterceptor.GuardInternal(() => CreateFromNew<T>(expression, null));
        }

        /// <summary>
        /// Creates a mocked instance from specified real constructor.
        /// </summary>
        /// <typeparam name="T">Type to mock.</typeparam>
        /// <param name="expression">Constructor expression that identifies the type and constructor arguments.</param>
        /// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
        /// <returns>A mock instance for the specified constructor expression.</returns>
        public static T Create<T>(Expression<Func<T>> expression, Behavior behavior)
        {
            return ProfilerInterceptor.GuardInternal(() => CreateFromNew<T>(expression, behavior));
        }

        private static T CreateFromNew<T>(Expression<Func<T>> expression, Behavior? behavior)
        {
            try
            {
                var args = expression.GetArgumentsFromConstructorExpression();
                MockCreationSettings settings = MockCreationSettings.GetSettings(args, behavior, null, false);
                return (T)MockingContext.CurrentRepository.Create(typeof(T), settings);
            }
            catch (InvalidCastException e)
            {
                throw new MockException("The constructor expression was not of the correct form. It should be a 'new' expression.", e);
            }
        }

        /// <summary>
        /// Creates a mock by using fluent configuration settings.
        /// </summary>
        /// <typeparam name="T">Type to mock.</typeparam>
        /// <param name="settings">Delegate that configures mock creation.</param>
        /// <returns>A configured mock instance.</returns>
        public static T Create<T>(Action<IFluentConfig<T>> settings)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var fluentConfig = new FluentConfig<T>();
                settings(fluentConfig);
                return (T)fluentConfig.CreateMock(typeof(T), MockingContext.CurrentRepository);
            });
        }

        /// <summary>
        /// Creates a mock by using the specified behavior and constructor arguments.
        /// </summary>
        /// <typeparam name="T">Type to mock.</typeparam>
        /// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
        /// <param name="args">Arguments to pass to the constructor when JustMock creates the instance.</param>
        /// <returns>A configured mock instance.</returns>
        public static T Create<T>(Behavior behavior, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings(args, behavior, null, null);
                return (T)MockingContext.CurrentRepository.Create(typeof(T), settings);
            });
        }

        /// <summary>
        /// Creates a mock by using the specified behavior.
        /// </summary>
        /// <typeparam name="T">Type to mock.</typeparam>
        /// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
        /// <returns>A configured mock instance.</returns>
        public static T Create<T>(Behavior behavior)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings(behavior);
                return (T)MockingContext.CurrentRepository.Create(typeof(T), settings);
            });
        }

        /// <summary>
        /// Creates a mock that uses <see cref="Behavior.RecursiveLoose"/> behavior.
        /// </summary>
        /// <typeparam name="T">Type to mock.</typeparam>
        /// <returns>A mock instance.</returns>
        public static T Create<T>()
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings();
                return (T)MockingContext.CurrentRepository.Create(typeof(T), settings);
            });
        }

        /// <summary>
        /// Creates a mock for the specified runtime type.
        /// </summary>
        /// <param name="target">Type to mock.</param>
        /// <param name="args">Arguments to pass to the constructor when JustMock creates the instance.</param>
        /// <returns>A mock instance that uses <see cref="Behavior.RecursiveLoose"/> behavior.</returns>
        public static object Create(Type target, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings(args, null, null, null);
                return MockingContext.CurrentRepository.Create(target, settings);
            });
        }

        /// <summary>
        /// Creates a mock for the specified runtime type.
        /// </summary>
        /// <param name="target">Type to mock.</param>
        /// <returns>A mock instance that uses <see cref="Behavior.RecursiveLoose"/> behavior.</returns>
        public static object Create(Type target)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings();
                return MockingContext.CurrentRepository.Create(target, settings);
            });
        }

        /// <summary>
        /// Creates a mock and controls whether JustMock calls the instance constructor.
        /// </summary>
        /// <param name="constructor">
        /// Specifies whether JustMock calls the original constructor.
        /// </param>
        /// <typeparam name="T">Type to mock.</typeparam>
        /// <returns>A mock instance that uses <see cref="Behavior.RecursiveLoose"/> behavior.</returns>
        public static T Create<T>(Constructor constructor)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings(null, null, null, constructor == Constructor.Mocked);
                return (T)MockingContext.CurrentRepository.Create(typeof(T), settings);
            });
        }

        /// <summary>
        /// Creates a mock and controls both constructor execution and default behavior.
        /// </summary>
        /// <param name="constructor">
        /// Specifies whether JustMock calls the base constructor.
        /// </param>
        /// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
        /// <returns>A configured mock instance.</returns>
        /// <typeparam name="T">Type to mock.</typeparam>
        public static T Create<T>(Constructor constructor, Behavior behavior)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings(null, behavior, null, constructor == Constructor.Mocked);
                return (T)MockingContext.CurrentRepository.Create(typeof(T), settings);
            });
        }

        /// <summary>
        /// Creates a mock for the specified runtime type and controls constructor execution.
        /// </summary>
        /// <param name="type">Type to mock.</param>
        /// <param name="constructor">
        /// Specifies whether JustMock calls the original constructor.
        /// </param>
        /// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
        /// <returns>A configured mock instance.</returns>
        public static object Create(Type type, Constructor constructor, Behavior behavior)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings(null, behavior, null, constructor == Constructor.Mocked);
                return MockingContext.CurrentRepository.Create(type, settings);
            });
        }

        /// <summary>
        /// Creates a mock for the specified runtime type by using fluent configuration settings.
        /// </summary>
        /// <param name="type">Type to mock.</param>
        /// <param name="settings">Delegate that configures mock creation.</param>
        /// <returns>A configured mock instance.</returns>
        public static object Create(Type type, Action<IFluentConfig> settings)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var fluentConfig = new FluentConfig();
                settings(fluentConfig);
                return fluentConfig.CreateMock(type, MockingContext.CurrentRepository);
            });
        }

        /// <summary>
        /// Creates a mock for the specified runtime type by using the supplied behavior and constructor arguments.
        /// </summary>
        /// <param name="type">Type to mock.</param>
        /// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
        /// <param name="args">Arguments to pass to the constructor when JustMock creates the instance.</param>
        /// <returns>A configured mock instance.</returns>
        public static object Create(Type type, Behavior behavior, params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings(args, behavior, null, null);
                return MockingContext.CurrentRepository.Create(type, settings);
            });
        }

        /// <summary>
        /// Creates a mock for the specified runtime type by using the supplied behavior.
        /// </summary>
        /// <param name="type">Type to mock.</param>
        /// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
        /// <returns>A configured mock instance.</returns>
        public static object Create(Type type, Behavior behavior)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings(behavior);
                return MockingContext.CurrentRepository.Create(type, settings);
            });
        }

        /// <summary>
        /// Creates a mock that uses <see cref="Behavior.RecursiveLoose"/> behavior and the supplied constructor arguments.
        /// </summary>
        /// <typeparam name="T">Type to mock.</typeparam>
        /// <param name="args">Arguments to pass to the constructor when JustMock creates the instance.</param>
        /// <returns>A mock instance.</returns>
        public static T Create<T>(params object[] args)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings(args, null, null, null);
                return (T)MockingContext.CurrentRepository.Create(typeof(T), settings);
            });
        }

        /// <summary>
        /// Creates a mock and arranges it by applying the specified functional specification.
        /// </summary>
        /// <remarks>
        /// Use this overload when you want to describe behavior declaratively instead of arranging members one by one.
        /// </remarks>
        /// <typeparam name="T">Type to mock.</typeparam>
        /// <param name="functionalSpecification">Functional specification to apply to the mock.</param>
        /// <returns>A mock that uses <see cref="Behavior.RecursiveLoose"/> behavior.</returns>
        public static T CreateLike<T>(Expression<Func<T, bool>> functionalSpecification)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var mock = Mock.Create<T>();
                ArrangeLike(mock, functionalSpecification);
                return mock;
            });
        }

#if !LITE_EDITION
        /// <summary>
        /// Creates a mocked instance from a internal class with <see cref="Behavior.RecursiveLoose"/> behavior.
        /// </summary>
        /// <param name="fullName">Fully qualified name of the target type.</param>
        /// <returns>Mock instance</returns>
        public static object Create(string fullName)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings();
                return MockingContext.CurrentRepository.Create(MockingUtil.GetTypeFrom(fullName), settings);
            });
        }

        /// <summary>
        /// Creates a mocked instance from an internal class.
        /// </summary>
        /// <param name="fullName">Fully qualified name of the target type.</param>
        /// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
        /// <returns>Mock instance</returns>
        public static object Create(string fullName, Behavior behavior)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings(behavior);
                return MockingContext.CurrentRepository.Create(MockingUtil.GetTypeFrom(fullName), settings);
            });
        }

        /// <summary>
        /// Creates a mocked instance from an internal class.
        /// </summary>
        /// <param name="fullName">Fully qualified name of the target type.</param>
        /// <param name="settings">Settings for the mock</param>
        /// <returns>Mock instance</returns>
        public static object Create(string fullName, Action<IFluentConfig> settings)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var fluentConfig = new FluentConfig();
                settings(fluentConfig);
                return fluentConfig.CreateMock(MockingUtil.GetTypeFrom(fullName), MockingContext.CurrentRepository);
            });
        }
#endif
    }
}
