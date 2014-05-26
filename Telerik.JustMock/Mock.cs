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
using System.Linq.Expressions;
using Telerik.JustMock.Abstraction;
using Telerik.JustMock.Analytics;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Behaviors;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Expectations;
using Telerik.JustMock.Expectations.Abstraction;
using Telerik.JustMock.Setup;

namespace Telerik.JustMock
{
	/// <summary>
	/// Entry point for setting up and asserting mocks.
	/// </summary>
	public partial class Mock
	{
	
		static Mock()
		{
#if SILVERLIGHT
			if (-1 == typeof(object).Assembly.FullName.IndexOf("PublicKeyToken=7cec85d7bea7798e", StringComparison.InvariantCultureIgnoreCase))
			{
				throw new InvalidOperationException("Telerik.JustMock.Silverlight should only be used inside the Silverlight runtime. For all other runtimes reference Telerik.JustMock instead.");
			}
#endif

#if !LITE_EDITION
			if (!MockingUtil.IsMetro())
			{
				try
				{
					AnalyticsTracker.Instance.CreateDefaultTracker(AnalyticsConfiguration.EnableAnalytics);
				}
				catch { }
			}
#endif
		}

		/// <summary>
		/// Gets a value indicating whether the JustMock profiler is enabled.
		/// </summary>
		/// <returns></returns>
		public static bool IsProfilerEnabled
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() => ProfilerInterceptor.IsProfilerAttached);
			}
		}

		/// <summary>
		/// Setups the target call to act in a specific way.
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <typeparam name="TResult">
		/// Return type for the target setup.
		/// </typeparam>
		/// <param name="func">
		/// Expression delegate to the target call
		/// </param>
		/// <returns>
		/// Reference to <see cref="FuncExpectation{TResult}"/> to setup the mock.
		/// </returns>
		public static FuncExpectation<TResult> Arrange<T, TResult>(Func<TResult> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.EnableInterception(typeof(T));
				return repo.Arrange(() => func(), () => new FuncExpectation<TResult>());
			});
		}

		/// <summary>
		/// Setups the target mock call with user expectation.
		/// </summary>
		/// <typeparam name="TResult">
		/// Return type for the target setup.
		/// </typeparam>
		/// <param name="expression">
		/// Provide the target method call
		/// </param>
		/// <returns>
		/// Reference to <see cref="FuncExpectation{TResult}"/> to setup the mock.
		/// </returns>
		public static FuncExpectation<TResult> Arrange<TResult>(Expression<Func<TResult>> expression)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return MockingContext.CurrentRepository.Arrange(expression, () => new FuncExpectation<TResult>());
			});
		}

		/// <summary>
		/// Setups the target mock call with user expectation.
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <typeparam name="TResult">
		/// Return type for the target setup.
		/// </typeparam>
		/// <param name="obj">
		/// Target instance.
		/// </param>
		/// <param name="func">
		/// Expression delegate to the target call
		/// </param>
		/// <returns>
		/// Reference to <see cref="FuncExpectation{TResult}"/> to setup the mock.
		/// </returns>
		public static FuncExpectation<TResult> Arrange<T, TResult>(T obj, Func<T, TResult> func)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.EnableInterception(typeof(T));
				return repo.Arrange(() => func(obj), () => new FuncExpectation<TResult>());
			});
		}

#if !VISUALBASIC
		/// <summary>
		/// Setups the target call to act in a specific way.
		/// </summary>
		/// <param name="expression">
		/// Target expression
		/// </param>
		/// <returns>
		/// Reference to <see cref="ActionExpectation"/> to setup the mock.
		/// </returns>
		public static ActionExpectation Arrange(Expression<Action> expression)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return MockingContext.CurrentRepository.Arrange(expression, () => new ActionExpectation());
			});
		}
#endif

		/// <summary>
		/// Setups target property set operation to act in a specific way.  
		/// <example>
		/// <code>
		/// Mock.ArrangeSet(() =>; foo.MyValue = 10).Throws(new InvalidOperationException());
		/// </code>
		/// This will throw InvalidOperationException for when foo.MyValue is set with 10.
		/// </example>
		/// </summary>
		/// <param name="action">
		/// Target action
		/// </param>
		/// <returns>
		/// Reference to <see cref="ActionExpectation"/> to setup the mock.
		/// </returns>
		public static ActionExpectation ArrangeSet(Action action)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return MockingContext.CurrentRepository.Arrange(action, () => new ActionExpectation());
			});
		}
	  
		/// <summary>
		/// Returns interface defining non-public expectations.
		/// </summary>
		public static INonPublicExpectation NonPublic
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
				{
					return new NonPublicExpectation();
				});
			}
		}

		/// <summary>
		/// Asserts a specific call from expression.
		/// </summary>
		/// <param name="expression">Target expression</param>
		/// <typeparam name="TReturn">Return type for the assert expression</typeparam>
		public static void Assert<TReturn>(Expression<Func<TReturn>> expression)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression);
			});
		}

		/// <summary>
		/// Asserts a specific call from expression.
		/// </summary>
		/// <param name="expression">Target expression</param>
		/// <typeparam name="TReturn">Return type for the assert expression</typeparam>
		/// <param name="args">Assert argument</param>
		public static void Assert<TReturn>(Expression<Func<TReturn>> expression, Args args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression, args, null);
			});
		}


		/// <summary>
		/// Asserts a specific call from expression.
		/// </summary>
		/// <param name="expression">Target expression</param>
		/// <param name="occurs">Specifies how many times a call has occurred</param>
		/// <typeparam name="TReturn">Return type for the target call</typeparam>
		public static void Assert<TReturn>(Expression<Func<TReturn>> expression, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression, null, occurs);
			});
		}

		/// <summary>
		/// Asserts the specified call from expression.
		/// </summary>
		/// <param name="expression">The action to verify.</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		/// <typeparam name="TReturn">Return type for the target call</typeparam>
		public static void Assert<TReturn>(Expression<Func<TReturn>> expression, Args args, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression, args, occurs);
			});
		}

#if !VISUALBASIC
		/// <summary>
		/// Asserts a specific call from expression.
		/// </summary>
		/// <param name="expression">Action expression defining the action to verify.</param>
		public static void Assert(Expression<Action> expression)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression);
			});
		}
#endif

		/// <summary>
		/// Asserts the specified call from expression.
		/// </summary>
		/// <param name="expression">The action to verify.</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		public static void Assert(Expression<Action> expression, Args args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression, args, null);
			});
		}

		/// <summary>
		/// Asserts the specified call from expression.
		/// </summary>
		/// <param name="expression">The action to verify.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public static void Assert(Expression<Action> expression, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression, null, occurs);
			});
		}

		/// <summary>
		/// Asserts the specified call from expression.
		/// </summary>
		/// <param name="expression">The action to verify.</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public static void Assert(Expression<Action> expression, Args args, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(null, expression, args, occurs);
			});
		}

		/// <summary>
		/// Asserts a specific call from expression.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="func">Contains the target mock call</param>
		/// <typeparam name="T">Target type</typeparam>
		/// <typeparam name="TResult">The type of the return value of the method</typeparam>
		public static void Assert<T, TResult>(T target, Func<T, TResult> func)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AssertAction(() => func(target));
			});
		}

		/// <summary>
		/// Asserts a specific call from expression.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="occurs">Specifies how many times a call has occurred</param>
		/// <param name="func">Contains the target mock call</param>
		/// <typeparam name="T">Target type</typeparam>
		/// <typeparam name="TResult">The type of the return value of the method</typeparam>
		public static void Assert<T, TResult>(T target, Func<T, TResult> func, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AssertAction(() => func(target), null, occurs);
			});
		}
		
		/// <summary>
		/// Asserts the specific property set operation.
		/// </summary>
		/// <param name="action">Action defining the set operation</param>
		public static void AssertSet(Action action)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AssertAction(action);
			});
		}

		/// <summary>
		/// Asserts the specific property set operation.
		/// </summary>
		/// <param name="action">Action defining the set operation</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public static void AssertSet(Action action, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AssertAction(action, null, occurs);
			});
		}

		/// <summary>
		/// Asserts the specific property set operation.
		/// </summary>
		/// <param name="action">Action defining the set operation</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public static void AssertSet(Action action, Args args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AssertAction(action, args, null);
			});
		}

		/// <summary>
		/// Asserts the specific property set operation.
		/// </summary>
		/// <param name="action">Action defining the set operation</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		/// <param name="occurs">Specifies the number of times a mock call should occur.</param>
		public static void AssertSet(Action action, Args args, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AssertAction(action, args, occurs);
			});
		}

		/// <summary>
		/// Asserts all expected calls that are marked as must or
		/// to be occurred a certain number of times.
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <param name="mocked">Target instance</param>
		public static void Assert<T>(T mocked)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.Assert(mocked);
			});
		}

		/// <summary>
		/// Asserts all expected setups.
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <param name="mocked">Target instance</param>
		public static void AssertAll<T>(T mocked)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.AssertAll(mocked);
			});
		}

		/// <summary>
		/// Returns the number of times the specified member was called.
		/// </summary>
		/// <param name="expression">The action to inspect</param>
		/// <returns>Number of calls</returns>
		public static int GetTimesCalled<TReturn>(Expression<Func<TReturn>> expression)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalled(expression, null));
		}

		/// <summary>
		/// Returns the number of times the specified member was called.
		/// </summary>
		/// <param name="expression">The action to inspect</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		/// <returns>Number of calls</returns>
		public static int GetTimesCalled<TReturn>(Expression<Func<TReturn>> expression, Args args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalled(expression, args));
		}

		/// <summary>
		/// Returns the number of times the specified member was called.
		/// </summary>
		/// <param name="expression">The action to inspect</param>
		/// <returns>Number of calls</returns>
		public static int GetTimesCalled(Expression<Action> expression)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalled(expression, null));
		}

		/// <summary>
		/// Returns the number of times the specified member was called.
		/// </summary>
		/// <param name="expression">The action to inspect</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		/// <returns>Number of calls</returns>
		public static int GetTimesCalled(Expression<Action> expression, Args args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalled(expression, args));
		}

		/// <summary>
		/// Returns the number of times the specified setter or event subscription method was called.
		/// </summary>
		/// <param name="action">The setter or event subscription method to inspect</param>
		/// <returns>Number of calls</returns>
		public static int GetTimesSetCalled(Action action)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalledFromAction(action, null));
		}

		/// <summary>
		/// Returns the number of times the specified setter or event subscription method was called.
		/// </summary>
		/// <param name="action">The setter or event subscription method to inspect</param>
		/// <param name="args">Specifies to ignore the instance and/or arguments during assertion.</param>
		/// <returns>Number of calls</returns>
		public static int GetTimesSetCalled(Action action, Args args)
		{
			return ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.GetTimesCalledFromAction(action, args));
		}

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
				MockingContext.CurrentRepository.InterceptStatics(typeof(T), null, false);
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
				MockingContext.CurrentRepository.InterceptStatics(typeof(T), behavior, false);
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
				MockingContext.CurrentRepository.InterceptStatics(staticType, null, staticConstructor == StaticConstructor.Mocked);
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
				MockingContext.CurrentRepository.InterceptStatics(staticType, null, false);
			});
		}

		/// <summary>
		/// Setups the target for mocking all static calls.
		/// </summary>
		/// <param name="targetType">Target type</param>
		/// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
		public static void SetupStatic(Type targetType, Behavior behavior)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.InterceptStatics(targetType, behavior, false);
			});
		}

		/// <summary>
		/// Setups the target for mocking all static calls.
		/// </summary>
		/// <param name="targetType">Target type</param>
		/// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
		/// <param name="staticConstructor">Defines the behavior of the static constructor</param>
		public static void SetupStatic(Type targetType, Behavior behavior, StaticConstructor staticConstructor)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				MockingContext.CurrentRepository.InterceptStatics(targetType, behavior, staticConstructor == StaticConstructor.Mocked);
			});
		}


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

		/// <summary>
		/// Creates a mocked instance of a non-public type with <see cref="Behavior.RecursiveLoose"/> behavior.
		/// </summary>
		/// <param name="type">The type to mock.</param>
		/// <returns>Mock instance</returns>
		public static object Create(Type type)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return MockingContext.CurrentRepository.Create(type, null, null, null, null);
			});
		}

		/// <summary>
		/// Creates a mocked instance of a non-public type.
		/// </summary>
		/// <param name="type">The type to mock.</param>
		/// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
		/// <returns>Mock instance</returns>
		public static object Create(Type type, Behavior behavior)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return MockingContext.CurrentRepository.Create(type, null, behavior, null, null);
			});
		}
#endif

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
		/// <param name="targetType">Target to mock</param>
		/// <param name="constructor">
		/// Specifies whether to call the base constructor
		/// </param>
		/// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
		/// <returns>Mock instance</returns>
		public static object Create(Type targetType, Constructor constructor, Behavior behavior)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return MockingContext.CurrentRepository.Create(targetType, null, behavior, null, constructor == Constructor.Mocked);
			});
		}

		/// <summary>
		/// Creates a mocked instance from a given type.
		/// </summary>
		/// <param name="target">Target type to mock</param>
		/// <param name="settings">Mock settings</param>
		/// <returns>Mock instance</returns>
		public static object Create(Type target, Action<IFluentConfig> settings)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var fluentConfig = new FluentConfig();
				settings(fluentConfig);
				return fluentConfig.CreateMock(target, MockingContext.CurrentRepository);
			});
		}

		/// <summary>
		/// Creates a mock instance from a given type.
		/// </summary>
		/// <param name="target">Mocking type</param>
		/// <param name="behavior">Specifies behavior of the mock. Default is <see cref="Behavior.RecursiveLoose"/></param>
		/// <param name="args">Constructor arguments</param>
		/// <returns>Mock instance</returns>
		public static object Create(Type target, Behavior behavior, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				return MockingContext.CurrentRepository.Create(target, args, behavior, null, null);
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

		/// <summary>
		/// Arranges the return values of properties and methods according to the given functional specification.
		/// </summary>
		/// <typeparam name="T">Mock type.</typeparam>
		/// <param name="mock">The mock on which to make the arrangements. If 'null' then the specification will be applied to all instances.</param>
		/// <param name="functionalSpecification">The functional specification to apply to this mock.</param>
		/// <remarks>
		/// See article "Create Mocks By Example" for further information on how to write functional specifications.
		/// </remarks>
		public static void ArrangeLike<T>(T mock, Expression<Func<T, bool>> functionalSpecification)
		{
			ProfilerInterceptor.GuardInternal(() => FunctionalSpecParser.ApplyFunctionalSpec(mock, functionalSpecification, ReturnArranger.Instance));
		}

		#region Raise Event methods
		
		/// <summary>
		/// Raises the specified event. If the event is not mocked and is declared on a C# or VB class
		/// and has the default implementation for add/remove, then that event can also be raised using this 
		/// method, even with the profiler off. The type on which the event is defined may need to be pre-intercepted
		/// using <see cref="Intercept"/> before calling Raise.
		/// </summary>
		/// <param name="eventExpression">Event expression</param>
		/// <param name="args">Delegate arguments</param>
		public static void Raise(Action eventExpression, params object[] args)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				object instance;
				var evt = MockingContext.CurrentRepository.ParseAddHandlerAction(eventExpression, out instance);
				RaiseEventBehavior.RaiseEventImpl(instance, evt, args);
			});
		}

		#endregion

		/// <summary>
		/// Removes all existing arrangements within the current mocking context (e.g. current test method).
		/// Arrangements made in parent mocking contexts (e.g. in fixture setup method) are preserved.
		/// </summary>
		public static void Reset()
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.RetireRepository());
		}

		/// <summary>
		/// Explicitly enables the interception of the given type by the profiler. Interception is usually enabled
		/// implicitly by calls to <see cref="Create"/> or <see cref="Arrange"/>. This method is rarely needed in cases
		/// where you're trying to arrange setters or raise events on a partial mock.
		/// </summary>
		/// <typeparam name="TTypeToIntercept">The type to intercept</typeparam>
		public static void Intercept<TTypeToIntercept>()
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.EnableInterception(typeof(TTypeToIntercept)));
		}

		/// <summary>
		/// Explicitly enables the interception of the given type by the profiler. Interception is usually enabled
		/// implicitly by calls to <see cref="Create"/> or <see cref="Arrange"/>. This method is rarely needed in cases
		/// where you're trying to arrange setters or raise events on a partial mock.
		/// </summary>
		/// <typeparam name="TTypeToIntercept">The type to intercept</typeparam>
		/// <param name="typeToIntercept">The type to intercept</param>
		public static void Intercept(Type typeToIntercept)
		{
			ProfilerInterceptor.GuardInternal(() => MockingContext.CurrentRepository.EnableInterception(typeToIntercept));
		}
	}
}
