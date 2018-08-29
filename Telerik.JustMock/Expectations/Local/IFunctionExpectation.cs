/*
 JustMock Lite
 Copyright © 2018 Telerik EAD

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
using System.Reflection;

namespace Telerik.JustMock.Expectations.Abstraction.Local.Function
{
	public interface IFunctionExpectation
	{
		/// <summary>
		/// Arranges a C# 7.0 local function for mocking.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="methodName">Name of the method where the local function is nestes</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		ActionExpectation Arrange(object target, string methodName, string localFunctionName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function for mocking.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="methodName">Name of the method where the local function is nested</param>
		/// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		ActionExpectation Arrange(object target, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function for mocking.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="method">Metadata for the method where the local function is nestes</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		ActionExpectation Arrange(object target, MethodInfo method, string localMemberName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function for mocking.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="methodName">Name of the method where the local function is nested</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, string localFunctionName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function for mocking.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="methodName">Name of the method where the local function is nested</param>
		/// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function for mocking.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="method">Metadata for the method where the local function is nestes</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		FuncExpectation<TReturn> Arrange<TReturn>(object target, MethodInfo method, string localFunctionName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function inside static method for mocking.
		/// </summary>
		/// <param name="T">The tpye with static method containing local function</param>
		/// <param name="methodName">Name of the method where the local function is nestes</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		ActionExpectation Arrange<T>(string methodName, string localMemberName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function inside static method for mocking.
		/// </summary>
		/// <param name="T">The tpye with static method containing local function</param>
		/// <param name="methodName">Name of the method where the local function is nestes</param>
		/// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		ActionExpectation Arrange<T>(string methodName, Type[] methodParamTypes, string localMemberName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function inside static method for mocking.
		/// </summary>
		/// <param name="T">The tpye with static method containing local function</param>
		/// <param name="method">Metadata of the method where the local function is nestes</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		ActionExpectation Arrange<T>(MethodInfo method, string localMemberName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function inside static method for mocking.
		/// </summary>
		/// <param name="type">The tpye with static method containing local function</param>
		/// <param name="methodName">Name of the method where the local function is nestes</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		ActionExpectation Arrange(Type type, string methodName, string localFunctionName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function inside static method for mocking.
		/// </summary>
		/// <param name="type">The tpye with static method containing local function</param>
		/// <param name="methodName">Name of the method where the local function is nestes</param>
		/// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		ActionExpectation Arrange(Type type, string methodName, Type[] methodParamTypes, string localMemberName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function inside static method for mocking.
		/// </summary>
		/// <param name="type">The tpye with static method containing local function</param>
		/// <param name="method">Metadata of the method where the local function is nestes</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		ActionExpectation Arrange(Type type, MethodInfo method, string localMemberName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function inside static method for mocking.
		/// </summary>
		/// <param name="T">The tpye with static method containing local function</param>
		/// <param name="methodName">Name of the method where the local function is nestes</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		FuncExpectation<TReturn> Arrange<T, TReturn>(string methodName, string localMemberName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function inside static method for mocking.
		/// </summary>
		/// <param name="T">The tpye with static method containing local function</param>
		/// <param name="methodName">Name of the method where the local function is nestes</param>
		/// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		FuncExpectation<TReturn> Arrange<T, TReturn>(string methodName, Type[] methodParamTypes, string localMemberName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function inside static method for mocking.
		/// </summary>
		/// <param name="T">The tpye with static method containing local function</param>
		/// <param name="method">Metadata of the method where the local function is nestes</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		FuncExpectation<TReturn> Arrange<T, TReturn>(MethodInfo method, string localMemberName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function inside static method for mocking.
		/// </summary>
		/// <param name="type">The tpye with static method containing local function</param>
		/// <param name="methodName">Name of the method where the local function is nestes</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		FuncExpectation<TReturn> Arrange<TReturn>(Type type, string methodName, string localFunctionName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function inside static method for mocking.
		/// </summary>
		/// <param name="type">The tpye with static method containing local function</param>
		/// <param name="methodName">Name of the method where the local function is nestes</param>
		/// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		FuncExpectation<TReturn> Arrange<TReturn>(Type type, string methodName, Type[] methodParamTypes, string localMemberName, params object[] args);

		/// <summary>
		/// Arranges a C# 7.0 local function inside static method for mocking.
		/// </summary>
		/// <param name="type">The tpye with static method containing local function</param>
		/// <param name="method">Metadata of the method where the local function is nestes</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		FuncExpectation<TReturn> Arrange<TReturn>(Type type, MethodInfo method, string localMemberName, params object[] args);

		/// <summary>
		/// Calls the specified C# 7.0 local function by name.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="methodName">Name of the method where the local function is nested</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>The value returned by the specified C# 7.0 local function.</returns>
		object Call(object target, string methodName, string localFunctionName, params object[] args);

		/// <summary>
		/// Calls the specified C# 7.0 local function by name.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="methodName">Name of the method where the local function is nested</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>The value returned by the specified C# 7.0 local function.</returns>
		T Call<T>(object target, string methodName, string localFunctionName, params object[] args);

		/// <summary>
		/// Calls the specified C# 7.0 local function by name.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="methodName">Name of the method where the local function is nested</param>
		/// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>The value returned by the specified C# 7.0 local function.</returns>
		object Call(object target, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args);

		/// <summary>
		/// Calls the specified C# 7.0 local function by name.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="methodName">Name of the method where the local function is nested</param>
		/// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Local function arguments</param>
		/// <returns>The value returned by the specified C# 7.0 local function.</returns>
		T Call<T>(object target, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args);

		/// <summary>
		/// Setups a non-public method for mocking.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="method">Method to setup taken from reflection.</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Method arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		object Call(object target, MethodInfo method, string localFunctionName, params object[] args);

		/// <summary>
		/// Setups a non-public method for mocking.
		/// </summary>
		/// <param name="target">Target instance</param>
		/// <param name="method">Method to setup taken from reflection.</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Method arguments</param>
		/// <returns>Reference to setup actions calls</returns>
		T Call<T>(object target, MethodInfo method, string localFunctionName, params object[] args);

		/// <summary>
		/// Asserts the specified C# 7.0 local function that it is called as expected.
		/// </summary>
		/// <param name="target">Target mock</param>
		/// <param name="method">Metadata for the method where the local function is nestes</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Method arguments</param>
		void Assert(object target, MethodInfo method, string localFunctionName, params object[] args);

		/// <summary>
		/// Asserts the specified C# 7.0 local function that it is called as expected.
		/// </summary>
		/// <param name="target">Target mock</param>
		/// <param name="methodName">Name of the  where the local function is nestes</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Method arguments</param>
		void Assert(object target, string methodName, string localFunctionName, params object[] args);

		/// <summary>
		/// Asserts the specified C# 7.0 local function that it is called as expected.
		/// </summary>
		/// <param name="target">Target mock</param>
		/// <param name="methodName">Name of the  where the local function is nestes</param>
		/// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="args">Method arguments</param>
		void Assert(object target, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args);

		/// <summary>
		/// Asserts the specified C# 7.0 local function that it is called as expected.
		/// </summary>
		/// <param name="target">Target mock</param>
		/// <param name="method">Metadata for the method where the local function is nestes</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="occurs">Specifies the number of times a call should occur.</param>
		/// <param name="args">Method arguments</param>
		void Assert(object target, MethodInfo method, string localFunctionName, Occurs occurs, params object[] args);

		/// <summary>
		/// Asserts the specified C# 7.0 local function that it is called as expected.
		/// </summary>
		/// <param name="target">Target mock</param>
		/// <param name="methodName">Name of the  where the local function is nestes</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="occurs">Specifies the number of times a call should occur.</param>
		/// <param name="args">Method arguments</param>
		void Assert(object target, string methodName, string localFunctionName, Occurs occurs, params object[] args);


		/// <summary>
		/// Asserts the specified C# 7.0 local function that it is called as expected.
		/// </summary>
		/// <param name="target">Target mock</param>
		/// <param name="methodName">Name of the  where the local function is nestes</param>
		/// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
		/// <param name="localFunctionName">Name of the nested local function</param>
		/// <param name="occurs">Specifies the number of times a call should occur.</param>
		/// <param name="args">Method arguments</param>
		void Assert(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Occurs occurs, params object[] args);
	}
}