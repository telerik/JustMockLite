/*
 JustMock Lite
 Copyright © 2010-2015 Telerik EAD

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
		/// <typeparam name="T">Target type for the mocked instance</typeparam>
		/// <param name="expression">Target expression for specifying the new type.</param>
		/// <returns>Mock instance</returns>
		public static T Create<T>(Expression<Func<T>> expression)
		{
			return ProfilerInterceptor.GuardInternal(() => CreateFromNew<T>(expression, null));
		}

		/// <summary>
		/// Creates a mocked instance from specified real constructor.
		/// </summary>
		/// <typeparam name="T">Target type for the mocked instance</typeparam>
		/// <param name="expression">Target expression for specifying the new type.</param>
		/// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
		/// <returns>Mock instance</returns>
		public static T Create<T>(Expression<Func<T>> expression, Behavior behavior)
		{
			return ProfilerInterceptor.GuardInternal(() => CreateFromNew<T>(expression, behavior));
		}

		private static T CreateFromNew<T>(Expression<Func<T>> expression, Behavior? behavior)
		{
			try
			{
				var args = expression.GetArgumentsFromConstructorExpression();
				return (T)MockingContext.CurrentRepository.Create(typeof(T), args, behavior, null, false);
			}
			catch (InvalidCastException e)
			{
				throw new MockException("The constructor expression was not of the correct form. It should be a 'new' expression.", e);
			}
		}

		/// <summary>
		/// Creates a mocked instance from settings specified in the lambda.
		/// </summary>
		/// <typeparam name="T">Type of the mock</typeparam>
		/// <param name="settings">Specifies mock settings</param>
		/// <returns>Mock instance</returns>
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
		/// Creates a mocked instance from a given type.
		/// </summary>
		/// <typeparam name="T">Type of the mock</typeparam>
		/// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
		/// <param name="args">Constructor arguments</param>
		/// <returns>Mock instance</returns>
		public static T Create<T>(Behavior behavior, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return (T)MockingContext.CurrentRepository.Create(typeof(T), args, behavior, null, null);
			});
		}

		/// <summary>
		/// Creates a mocked instance from a given type.
		/// </summary>
		/// <typeparam name="T">Type of the mock</typeparam>
		/// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
		/// <returns>Mock instance</returns>
		public static T Create<T>(Behavior behavior)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return (T)MockingContext.CurrentRepository.Create(typeof(T), null, behavior, null, null);
			});
		}

		/// <summary>
		/// Creates a mocked instance from a given type with <see cref="Behavior.RecursiveLoose"/> behavior.
		/// </summary>
		/// <typeparam name="T">Type of the mock</typeparam>
		/// <returns>Mock instance</returns>
		public static T Create<T>()
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return (T)MockingContext.CurrentRepository.Create(typeof(T), null, null, null, null);
			});
		}

		/// <summary>
		/// Creates a mocked instance from a given type with <see cref="Behavior.RecursiveLoose"/> behavior.
		/// </summary>
		/// <param name="target">Target to mock</param>
		/// <param name="args">Constructor arguments</param>
		/// <returns>Mock instance</returns>
		public static object Create(Type target, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return MockingContext.CurrentRepository.Create(target, args, null, null, null);
			});
		}

		/// <summary>
		/// Creates a mocked instance from a given type with <see cref="Behavior.RecursiveLoose"/> behavior.
		/// </summary>
		/// <param name="target">Target to mock</param>
		/// <returns>Mock instance</returns>
		public static object Create(Type target)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return MockingContext.CurrentRepository.Create(target, null, null, null, null);
			});
		}

		/// <summary>
		/// Creates a mocked instance from a given type with <see cref="Behavior.RecursiveLoose"/> behavior.
		/// </summary>
		/// <param name="constructor">
		/// Specifies whether to call the base constructor
		/// </param>
		/// <typeparam name="T">Target type</typeparam>
		/// <returns>Mock instance</returns>
		public static T Create<T>(Constructor constructor)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return (T)MockingContext.CurrentRepository.Create(typeof(T), null, null, null, constructor == Constructor.Mocked);
			});
		}

		/// <summary>
		/// Creates a mocked instance from a given type.
		/// </summary>
		/// <param name="constructor">
		/// Specifies whether to call the base constructor
		/// </param>
		/// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
		/// <returns>Mock instance</returns>
		/// <typeparam name="T">Target type</typeparam>
		public static T Create<T>(Constructor constructor, Behavior behavior)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return (T)MockingContext.CurrentRepository.Create(typeof(T), null, behavior, null, constructor == Constructor.Mocked);
			});
		}

		/// <summary>
		/// Creates a mocked instance from a given type.
		/// </summary>
		/// <param name="type">Target to mock</param>
		/// <param name="constructor">
		/// Specifies whether to call the base constructor
		/// </param>
		/// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
		/// <returns>Mock instance</returns>
		public static object Create(Type type, Constructor constructor, Behavior behavior)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return MockingContext.CurrentRepository.Create(type, null, behavior, null, constructor == Constructor.Mocked);
			});
		}

		/// <summary>
		/// Creates a mocked instance from a given type.
		/// </summary>
		/// <param name="type">Target type to mock</param>
		/// <param name="settings">Mock settings</param>
		/// <returns>Mock instance</returns>
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
		/// Creates a mock instance from a given type.
		/// </summary>
		/// <param name="type">Mocking type</param>
		/// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
		/// <param name="args">Constructor arguments</param>
		/// <returns>Mock instance</returns>
		public static object Create(Type type, Behavior behavior, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return MockingContext.CurrentRepository.Create(type, args, behavior, null, null);
			});
		}

		/// <summary>
		/// Creates a mock instance from a given type.
		/// </summary>
		/// <param name="type">Mocking type</param>
		/// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
		/// <returns>Mock instance</returns>
		public static object Create(Type type, Behavior behavior)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return MockingContext.CurrentRepository.Create(type, null, behavior, null, null);
			});
		}

		/// <summary>
		/// Creates a mocked instance from a given type with <see cref="Behavior.RecursiveLoose"/> behavior.
		/// </summary>
		/// <typeparam name="T">Mocked object type.</typeparam>
		/// <param name="args">Constructor arguments</param>
		/// <returns>Mock instance</returns>
		public static T Create<T>(params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return (T)MockingContext.CurrentRepository.Create(typeof(T), args, null, null, null);
			});
		}

		/// <summary>
		/// Creates a mock with <see cref="Behavior.RecursiveLoose"/> behavior by parsing the given functional specification.
		/// </summary>
		/// <remarks>
		/// See article "Create Mocks By Example" for further information on how to write functional specifications.
		/// </remarks>
		/// <typeparam name="T">Type for the argument.</typeparam>
		/// <param name="functionalSpecification">The functional specification to apply to the mock object.</param>
		/// <returns>A mock with the given functional specification.</returns>
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
				return MockingContext.CurrentRepository.Create(MockingUtil.GetTypeFrom(fullName), null, null, null, null);
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
				return MockingContext.CurrentRepository.Create(MockingUtil.GetTypeFrom(fullName), null, behavior, null, null);
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
