/*
 JustMock Lite
 Copyright © 2010-2015 Telerik AD

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

/*  Auto generated */




using System;
using Telerik.JustMock.Expectations;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Context;

namespace Telerik.JustMock
{
#if VISUALBASIC
	[Obsolete("Telerik.JustMock.VisualBasic.dll is a compatibility library for Visual Basic 2008 and will be removed in a future release. It is recommended that you migrate to a later version of the development environment and reference Telerik.JustMock.dll instead.")]
#endif
	public partial class Mock
	{
		#if VISUALBASIC
		

		/// <summary>
		/// Prepares a mock call with user expectations.
		/// </summary>
		/// <remarks> For use in Visual Basic 3.5 to mock actions without parameters</remarks>.
		/// <param name="action"></param>
		public static ActionExpectation Arrange(Action action)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				return repo.Arrange(repo.ConvertDelegateAndArgumentsToExpression(action, new object[0]), () => new ActionExpectation());
			});
		}


		/// <summary>
		/// Asserts the specific action call.
		/// </summary>
		/// <param name="action">Action defining the set operation</param>
		public static void Assert(Action action)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[0]));
			});
		}

		/// <summary>
		/// Asserts the specific action call.
		/// </summary>
		/// <param name="action">Action defining the set operation</param>
		public static void Assert(Action action, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[0]), null, occurs);
			});
		}


		 
		
		
		/// <summary>
		/// Prepares a mock call with user expectations.
		/// </summary>
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// <param name="action"></param>
		///<param name="arg1">Method argument 1</param>
		public static ActionExpectation Arrange<T1>(Action<T1> action, T1 arg1)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				return repo.Arrange(repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1}), () => new ActionExpectation());
			});
		}
		
		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		public static void Assert<T1>(Action<T1> action, T1 arg1)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1 }));
			});
		}

		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		/// <param name="occurs">Specify number times a call should occur</param>
		public static void Assert<T1>(Action<T1> action, T1 arg1, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1 }), null, occurs);
			});
		}


		
		/// <summary>
		/// Prepares a mock call with user expectations.
		/// </summary>
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// <param name="action"></param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		public static ActionExpectation Arrange<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				return repo.Arrange(repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2}), () => new ActionExpectation());
			});
		}
		
		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		public static void Assert<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2 }));
			});
		}

		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		/// <param name="occurs">Specify number times a call should occur</param>
		public static void Assert<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2 }), null, occurs);
			});
		}


		
		/// <summary>
		/// Prepares a mock call with user expectations.
		/// </summary>
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// <param name="action"></param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		public static ActionExpectation Arrange<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				return repo.Arrange(repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3}), () => new ActionExpectation());
			});
		}
		
		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		public static void Assert<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3 }));
			});
		}

		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		/// <param name="occurs">Specify number times a call should occur</param>
		public static void Assert<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3 }), null, occurs);
			});
		}


		
		/// <summary>
		/// Prepares a mock call with user expectations.
		/// </summary>
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// <param name="action"></param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		public static ActionExpectation Arrange<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				return repo.Arrange(repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4}), () => new ActionExpectation());
			});
		}
		
		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		public static void Assert<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4 }));
			});
		}

		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		/// <param name="occurs">Specify number times a call should occur</param>
		public static void Assert<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4 }), null, occurs);
			});
		}


		
		/// <summary>
		/// Prepares a mock call with user expectations.
		/// </summary>
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// <param name="action"></param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		///<param name="arg5">Method argument 5</param>
		public static ActionExpectation Arrange<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				return repo.Arrange(repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4, arg5}), () => new ActionExpectation());
			});
		}
		
		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		///<param name="arg5">Method argument 5</param>
		public static void Assert<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4, arg5 }));
			});
		}

		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		///<param name="arg5">Method argument 5</param>
		/// <param name="occurs">Specify number times a call should occur</param>
		public static void Assert<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4, arg5 }), null, occurs);
			});
		}


		
		/// <summary>
		/// Prepares a mock call with user expectations.
		/// </summary>
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// <param name="action"></param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		///<param name="arg5">Method argument 5</param>
		///<param name="arg6">Method argument 6</param>
		public static ActionExpectation Arrange<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				return repo.Arrange(repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4, arg5, arg6}), () => new ActionExpectation());
			});
		}
		
		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		///<param name="arg5">Method argument 5</param>
		///<param name="arg6">Method argument 6</param>
		public static void Assert<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4, arg5, arg6 }));
			});
		}

		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		///<param name="arg5">Method argument 5</param>
		///<param name="arg6">Method argument 6</param>
		/// <param name="occurs">Specify number times a call should occur</param>
		public static void Assert<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4, arg5, arg6 }), null, occurs);
			});
		}


		
		/// <summary>
		/// Prepares a mock call with user expectations.
		/// </summary>
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// <param name="action"></param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		///<param name="arg5">Method argument 5</param>
		///<param name="arg6">Method argument 6</param>
		///<param name="arg7">Method argument 7</param>
		public static ActionExpectation Arrange<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				return repo.Arrange(repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4, arg5, arg6, arg7}), () => new ActionExpectation());
			});
		}
		
		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		///<param name="arg5">Method argument 5</param>
		///<param name="arg6">Method argument 6</param>
		///<param name="arg7">Method argument 7</param>
		public static void Assert<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4, arg5, arg6, arg7 }));
			});
		}

		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		///<param name="arg5">Method argument 5</param>
		///<param name="arg6">Method argument 6</param>
		///<param name="arg7">Method argument 7</param>
		/// <param name="occurs">Specify number times a call should occur</param>
		public static void Assert<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4, arg5, arg6, arg7 }), null, occurs);
			});
		}


		
		/// <summary>
		/// Prepares a mock call with user expectations.
		/// </summary>
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// <param name="action"></param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		///<param name="arg5">Method argument 5</param>
		///<param name="arg6">Method argument 6</param>
		///<param name="arg7">Method argument 7</param>
		///<param name="arg8">Method argument 8</param>
		public static ActionExpectation Arrange<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				return repo.Arrange(repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8}), () => new ActionExpectation());
			});
		}
		
		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		///<param name="arg5">Method argument 5</param>
		///<param name="arg6">Method argument 6</param>
		///<param name="arg7">Method argument 7</param>
		///<param name="arg8">Method argument 8</param>
		public static void Assert<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }));
			});
		}

		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		///<param name="arg5">Method argument 5</param>
		///<param name="arg6">Method argument 6</param>
		///<param name="arg7">Method argument 7</param>
		///<param name="arg8">Method argument 8</param>
		/// <param name="occurs">Specify number times a call should occur</param>
		public static void Assert<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }), null, occurs);
			});
		}


		
		/// <summary>
		/// Prepares a mock call with user expectations.
		/// </summary>
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// <param name="action"></param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		///<param name="arg5">Method argument 5</param>
		///<param name="arg6">Method argument 6</param>
		///<param name="arg7">Method argument 7</param>
		///<param name="arg8">Method argument 8</param>
		///<param name="arg9">Method argument 9</param>
		public static ActionExpectation Arrange<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				return repo.Arrange(repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9}), () => new ActionExpectation());
			});
		}
		
		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		///<param name="arg5">Method argument 5</param>
		///<param name="arg6">Method argument 6</param>
		///<param name="arg7">Method argument 7</param>
		///<param name="arg8">Method argument 8</param>
		///<param name="arg9">Method argument 9</param>
		public static void Assert<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 }));
			});
		}

		/// <summary>
		/// Asserts a specific action call on the mock.
		/// <remarks> For use in Visual Basic 3.5 to mock actions with parameters</remarks>.
		/// </summary>
		/// <param name="action">Method to Assert</param>
		///<param name="arg1">Method argument 1</param>
		///<param name="arg2">Method argument 2</param>
		///<param name="arg3">Method argument 3</param>
		///<param name="arg4">Method argument 4</param>
		///<param name="arg5">Method argument 5</param>
		///<param name="arg6">Method argument 6</param>
		///<param name="arg7">Method argument 7</param>
		///<param name="arg8">Method argument 8</param>
		///<param name="arg9">Method argument 9</param>
		/// <param name="occurs">Specify number times a call should occur</param>
		public static void Assert<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Occurs occurs)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				var repo = MockingContext.CurrentRepository;
				repo.Assert(null, repo.ConvertDelegateAndArgumentsToExpression(action, new object[] {arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 }), null, occurs);
			});
		}


		
		#endif
	}
}
