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
    /// Throws() methods.
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    public interface IThrows<TContainer>
    {
        /// <summary>
        /// Throws a the specified exception for target call.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        IAssertable Throws(Exception exception);

        /// <summary>
        /// Throws a the specified exception for target call.
        /// </summary>
        /// <returns></returns>
        IAssertable Throws<TException>() where TException : Exception;

        /// <summary>
        /// Throws a the specified exception for target call.
        /// </summary>
        /// <returns></returns>
        IAssertable Throws<TException>(params object[] args) where TException : Exception;

#if !LITE_EDITION
        /// <summary>
        /// Throws a the specified exception for the target async call causing returned task to fail.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        IAssertable ThrowsAsync(Exception exception);

        /// <summary>
        /// Throws a the specified exception for the target async call causing returned task to fail.
        /// </summary>
        /// <returns></returns>
        IAssertable ThrowsAsync<TException>() where TException : Exception;

        /// <summary>
        /// Throws a the specified exception for the target async call causing returned task to fail.
        /// </summary>
        /// <returns></returns>
        IAssertable ThrowsAsync<TException>(params object[] args) where TException : Exception;
#endif
    }
}
