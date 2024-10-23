/*
 JustMock Lite
 Copyright © 2010-2015 Progress Software Corporation

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
    /// Defines occurrence for a specific call.
    /// </summary>
    public interface IOccurrence
    {
        /// <summary>
        /// Specifies how many times the call should occur.
        /// </summary>
        /// <param name="numberOfTimes">Specified number of times</param>
        /// <returns>Disposable object that can be used to disable this arrangement.</returns>
        IDisposable Occurs(int numberOfTimes, string message = null);

        /// <summary>
        /// Specifies how many times at least the call should occur.
        /// </summary>
        /// <param name="numberOfTimes">Specified number of times</param>
        /// <returns>Disposable object that can be used to disable this arrangement.</returns>
        IDisposable OccursAtLeast(int numberOfTimes, string message = null);

        /// <summary>
        /// Specifies how many times maximum the call can occur.
        /// </summary>
        /// <param name="numberOfTimes">Specified number of times</param>
        /// <returns>Disposable object that can be used to disable this arrangement.</returns>
        IDisposable OccursAtMost(int numberOfTimes, string message = null);

        /// <summary>
        /// Specifies that the call must occur once.
        /// </summary>
        /// <returns>Disposable object that can be used to disable this arrangement.</returns>
        IDisposable OccursOnce(string message = null);

        /// <summary>
        /// Specifies that the call must never occur.
        /// </summary>
        /// <returns>Disposable object that can be used to disable this arrangement.</returns>
        IDisposable OccursNever(string message = null);
    }
}
