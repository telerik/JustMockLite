/*
 JustMock Lite
 Copyright © 2018 Progress Software Corporation

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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.JustMock.Expectations.Abstraction
{
    /// <summary>
    /// Defines methods to mock non-public ref return members.
    /// </summary>
    /// <remarks>
    /// Overloads required generic arguments which are converted internally
    /// to references. For more information about arranging non public expectations
    /// see <see cref="INonPublicExpectation"/> interface.
    /// </remarks>
    public interface INonPublicRefReturnExpectation
    {
        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <typeparam name="TRefReturn">Ref return type</typeparam>
        /// <param name="target">Target instance.</param>
        /// <param name="memberName">Target member name</param>
        /// <param name="args">Method arguments</param>
        FuncExpectation<TRefReturn> Arrange<TRefReturn>(object target, string memberName, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="TRefReturn">Ref return type</typeparam>
        /// <param name="target">Target mock</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="args">Method arguments</param>
        void Assert<TRefReturn>(object target, string memberName, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="TRefReturn">Ref return type</typeparam>
        /// <param name="target">Target mock</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert<TRefReturn>(object target, string memberName, Occurs occurs, params object[] args);

        /// <summary>
        /// Returns the number of times the specified member was called.
        /// </summary>
        /// <typeparam name="TRefReturn">Ref return type</typeparam>
        /// <param name="target">Target mock</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="args">Method arguments</param>
        /// <returns>Number of calls.</returns>
        int GetTimesCalled<TRefReturn>(object target, string memberName, params object[] args);

#if !LITE_EDITION
        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <typeparam name="TRefReturn">Ref return type</typeparam>
        /// <param name="memberName">Target member name</param>
        /// <param name="args">Method arguments</param>
        FuncExpectation<TRefReturn> Arrange<T, TRefReturn>(string memberName, params object[] args);

        /// <summary>
        /// Arranges a method for mocking.
        /// </summary>
        /// <typeparam name="TRefReturn">Ref return type</typeparam>
        /// <param name="targetType">Target type</param>
        /// <param name="memberName">Target member name</param>
        /// <param name="args">Method arguments</param>
        FuncExpectation<TRefReturn> Arrange<TRefReturn>(Type targetType, string memberName, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="T">Specify the target type</typeparam>
        /// <typeparam name="TRefReturn">Ref return type</typeparam>
        /// <param name="memberName">Name of the member</param>
        /// <param name="args">Method arguments</param>
        void Assert<T, TRefReturn>(string memberName, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="TRefReturn">Ref return type</typeparam>
        /// <param name="targetType">Type of the target</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="args">Method arguments</param>
        void Assert<TRefReturn>(Type targetType, string memberName, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="T">Specify the target type</typeparam>
        /// <typeparam name="TRefReturn">Ref return type</typeparam>
        /// <param name="memberName">Name of the member</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert<T, TRefReturn>(string memberName, Occurs occurs, params object[] args);

        /// <summary>
        /// Asserts the specified member that it is called as expected.
        /// </summary>
        /// <typeparam name="TRefReturn">Ref return type</typeparam>
        /// <param name="targetType">Type of the target</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="occurs">Specifies the number of times a call should occur.</param>
        /// <param name="args">Method arguments</param>
        void Assert<TRefReturn>(Type targetType, string memberName, Occurs occurs, params object[] args);

        /// <summary>
        /// Returns the number of times the specified member was called.
        /// </summary>
        /// <typeparam name="T">Specify the target type</typeparam>
        /// <param name="memberName">Name of the member</param>
        /// <param name="args">Method arguments</param>
        /// <returns>Number of calls.</returns>
        int GetTimesCalled<T, TRefReturn>(string memberName, params object[] args);

        /// <summary>
        /// Returns the number of times the specified member was called.
        /// </summary>
        /// <param name="targetType">Type of the target</param>
        /// <param name="memberName">Name of the member</param>
        /// <param name="args">Method arguments</param>
        /// <returns>Number of calls.</returns>
        int GetTimesCalled<TRefReturn>(Type targetType, string memberName, params object[] args);
#endif
    }
}
