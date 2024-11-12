/*
 JustMock Lite
 Copyright © 2018-2019 Progress Software Corporation

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
    /// <summary>
    /// Interface used to arrange and call local functions.
    /// </summary>
    public interface IFunctionExpectation
    {
        /// <summary>
        /// Arranges a local function for mocking.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange(object target, string methodName, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function for mocking.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function for mocking.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function for mocking.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange(object target, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function for mocking.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function for mocking.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function for mocking.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="method">Metadata for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange(object target, MethodInfo method, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function for mocking.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function for mocking.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function for mocking.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function for mocking.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function for mocking.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function for mocking.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<TReturn>(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function for mocking.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="target">Target instance</param>
        /// <param name="method">Metadata for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<TReturn>(object target, MethodInfo method, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="T">The type with static method containing local function</typeparam>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange<T>(string methodName, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="T">The type with static method containing local function</typeparam>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange<T>(string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="T">The type with static method containing local function</typeparam>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange<T>(string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="T">The type with static method containing local function</typeparam>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange<T>(string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="T">The type with static method containing local function</typeparam>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange<T>(string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="T">The type with static method containing local function</typeparam>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange<T>(string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="T">The type with static method containing local function</typeparam>
        /// <param name="method">Metadata of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange<T>(MethodInfo method, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <param name="type">The type with static method containing local function</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange(Type type, string methodName, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <param name="type">The type with static method containing local function</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        ActionExpectation Arrange(Type type, string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <param name="type">The type with static method containing local function</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        ActionExpectation Arrange(Type type, string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <param name="type">The type with static method containing local function</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange(Type type, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <param name="type">The type with static method containing local function</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange(Type type, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <param name="type">The type with static method containing local function</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange(Type type, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <param name="type">The type with static method containing local function</param>
        /// <param name="method">Metadata of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        ActionExpectation Arrange(Type type, MethodInfo method, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="T">The type with static method containing local function</typeparam>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<T, TReturn>(string methodName, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="T">The type with static method containing local function</typeparam>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<T, TReturn>(string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="T">The type with static method containing local function</typeparam>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<T, TReturn>(string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="T">The type with static method containing local function</typeparam>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<T, TReturn>(string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="T">The type with static method containing local function</typeparam>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<T, TReturn>(string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="T">The type with static method containing local function</typeparam>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<T, TReturn>(string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="T">The type with static method containing local function</typeparam>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="method">Metadata of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<T, TReturn>(MethodInfo method, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="type">The type with static method containing local function</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<TReturn>(Type type, string methodName, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="type">The type with static method containing local function</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<TReturn>(Type type, string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <param name="type">The type with static method containing local function</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<TReturn>(Type type, string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="type">The type with static method containing local function</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<TReturn>(Type type, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="type">The type with static method containing local function</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<TReturn>(Type type, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="type">The type with static method containing local function</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<TReturn>(Type type, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Arranges a local function inside static method for mocking.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return value</typeparam>
        /// <param name="type">The type with static method containing local function</param>
        /// <param name="method">Metadata of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>Reference to setup actions calls</returns>
        FuncExpectation<TReturn> Arrange<TReturn>(Type type, MethodInfo method, string localFunctionName, params object[] args);

        /// <summary>
        /// Calls the specified local function by name.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>The value returned by the specified local function.</returns>
        object Call(object target, string methodName, string localFunctionName, params object[] args);

        /// <summary>
        /// Calls the specified local function by name.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>The value returned by the specified local function.</returns>
        object Call(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Calls the specified local function by name.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>The value returned by the specified local function.</returns>
        object Call(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Calls the specified local function by name.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>The value returned by the specified local function.</returns>
        T Call<T>(object target, string methodName, string localFunctionName, params object[] args);

        /// <summary>
        /// Calls the specified local function by name.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>The value returned by the specified local function.</returns>
        T Call<T>(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Calls the specified local function by name.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>The value returned by the specified local function.</returns>
        T Call<T>(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Calls the specified local function by name.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>The value returned by the specified local function.</returns>
        object Call(object target, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args);

        /// <summary>
        /// Calls the specified local function by name.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>The value returned by the specified local function.</returns>
        object Call(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Calls the specified local function by name.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>The value returned by the specified local function.</returns>
        object Call(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Calls the specified local function by name.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>The value returned by the specified local function.</returns>
        T Call<T>(object target, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args);

        /// <summary>
        /// Calls the specified local function by name.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>The value returned by the specified local function.</returns>
        T Call<T>(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Calls the specified local function by name.
        /// </summary>
        /// <param name="target">Target instance</param>
        /// <param name="methodName">Name of the method where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Local function arguments</param>
        /// <returns>The value returned by the specified local function.</returns>
        T Call<T>(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

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
        /// Asserts the specified local function that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="method">Metadata for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, MethodInfo method, string localFunctionName, params object[] args);

        /// <summary>
        /// Asserts the specified local function that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="methodName">Name of the  where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string methodName, string localFunctionName, params object[] args);

        /// <summary>
        /// Asserts the specified local function that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="methodName">Name of the  where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Asserts the specified local function that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="methodName">Name of the  where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string methodName, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Asserts the specified local function that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="methodName">Name of the  where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string methodName, Type[] methodParamTypes, string localFunctionName, params object[] args);

        /// <summary>
        /// Asserts the specified local function that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="methodName">Name of the  where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Asserts the specified local function that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="methodName">Name of the  where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Asserts the specified local function that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="method">Metadata for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, MethodInfo method, string localFunctionName, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified local function that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="methodName">Name of the  where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string methodName, string localFunctionName, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified local function that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="methodName">Name of the  where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string methodName, string localFunctionName, Occurs occurs, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Asserts the specified local function that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="methodName">Name of the  where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string methodName, string localFunctionName, Occurs occurs, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);

        /// <summary>
        /// Asserts the specified local function that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="methodName">Name of the  where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified local function that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="methodName">Name of the  where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Occurs occurs, Type[] methodGenericTypes, params object[] args);

        /// <summary>
        /// Asserts the specified local function that it is called as expected.
        /// </summary>
        /// <param name="target">Target mock</param>
        /// <param name="methodName">Name of the  where the local function is nested</param>
        /// <param name="methodParamTypes">Types of the parameters for the method where the local function is nested</param>
        /// <param name="localFunctionName">Name of the nested local function</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="methodGenericTypes">Generic types of the method specified by methodName</param>
        /// <param name="localFunctionGenericTypes">Generic types of the local function specified by localFunctionName</param>
        /// <param name="args">Method arguments</param>
        void Assert(object target, string methodName, Type[] methodParamTypes, string localFunctionName, Occurs occurs, Type[] methodGenericTypes, Type[] localFunctionGenericTypes, params object[] args);
    }
}
