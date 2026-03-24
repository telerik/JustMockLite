/*
 JustMock Lite
 Copyright © 2010-2015,2019 Progress Software Corporation

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

namespace Telerik.JustMock.Expectations.Abstraction
{
    /// <summary>
    /// Provides methods for arranging exceptions to be thrown when a mocked member is called.
    /// </summary>
    /// <typeparam name="TContainer">Expectation type that continues the arrangement.</typeparam>
    public interface IThrows<TContainer>
    {
        /// <summary>
        /// Throws the specified exception when the arranged call occurs.
        /// </summary>
        /// <param name="exception">Exception instance to throw.</param>
        /// <returns>The current expectation so that you can continue the arrangement.</returns>
        IAssertable Throws(Exception exception);

        /// <summary>
        /// Throws a new exception of the specified type when the arranged call occurs.
        /// </summary>
        /// <returns>The current expectation so that you can continue the arrangement.</returns>
        IAssertable Throws<TException>() where TException : Exception;

        /// <summary>
        /// Throws a new exception of the specified type when the arranged call occurs.
        /// </summary>
        /// <returns>The current expectation so that you can continue the arrangement.</returns>
        IAssertable Throws<TException>(params object[] args) where TException : Exception;

#if !LITE_EDITION
        /// <summary>
        /// Faults the returned task with the specified exception when the arranged async call occurs.
        /// </summary>
        /// <param name="exception">Exception instance to use for the faulted task.</param>
        /// <returns>The current expectation so that you can continue the arrangement.</returns>
        IAssertable ThrowsAsync(Exception exception);

        /// <summary>
        /// Faults the returned task with a new exception of the specified type when the arranged async call occurs.
        /// </summary>
        /// <returns>The current expectation so that you can continue the arrangement.</returns>
        IAssertable ThrowsAsync<TException>() where TException : Exception;

        /// <summary>
        /// Faults the returned task with a new exception of the specified type when the arranged async call occurs.
        /// </summary>
        /// <returns>The current expectation so that you can continue the arrangement.</returns>
        IAssertable ThrowsAsync<TException>(params object[] args) where TException : Exception;
#endif
    }
}
