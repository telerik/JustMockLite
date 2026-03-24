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

namespace Telerik.JustMock.Expectations.Abstraction
{
    /// <summary>
    /// Exposes options for configuring what happens when an arranged void member is called.
    /// </summary>
    public interface IAction<TContainer> : IDoInstead<TContainer>, IThrows<TContainer>, IAssertable
    {
        /// <summary>
        /// Raises the specified event when the arranged call occurs.
        /// </summary>
        /// <param name="eventExpression">Expression that identifies the event to raise.</param>
        /// <param name="args">Arguments to pass to the event handler delegate.</param>
        /// <returns>The current expectation so that you can continue the arrangement.</returns>
        /// <exception cref="InvalidOperationException">Thrown when JustMock cannot resolve the event expression.</exception>
        TContainer Raises(Action eventExpression, params object[] args);

        /// <summary>
        /// Raises the specified event when the arranged call occurs.
        /// </summary>
        /// <param name="eventExpression">Expression that identifies the event to raise.</param>
        /// <param name="args">Event arguments to pass to the handler.</param>
        /// <returns>The current expectation so that you can continue the arrangement.</returns>
        /// <exception cref="InvalidOperationException">Thrown when JustMock cannot resolve the event expression.</exception>
        TContainer Raises(Action eventExpression, EventArgs args);

        /// <summary>
        /// Raises the specified event when the arranged call occurs.
        /// </summary>
        /// <param name="eventExpression">Expression that identifies the event to raise.</param>
        /// <param name="func">Function that creates the event arguments from the arranged call arguments.</param>
        /// <returns>The current expectation so that you can continue the arrangement.</returns>
        /// <exception cref="InvalidOperationException">Thrown when JustMock cannot resolve the event expression.</exception>
        TContainer Raises<T1>(Action eventExpression, Func<T1, EventArgs> func);

        /// <summary>
        /// Raises the specified event when the arranged call occurs.
        /// </summary>
        /// <param name="eventExpression">Expression that identifies the event to raise.</param>
        /// <param name="func">Function that creates the event arguments from the arranged call arguments.</param>
        /// <returns>The current expectation so that you can continue the arrangement.</returns>
        /// <exception cref="InvalidOperationException">Thrown when JustMock cannot resolve the event expression.</exception>
        TContainer Raises<T1, T2>(Action eventExpression, Func<T1, T2, EventArgs> func);

        /// <summary>
        /// Raises the specified event when the arranged call occurs.
        /// </summary>
        /// <param name="eventExpression">Expression that identifies the event to raise.</param>
        /// <param name="func">Function that creates the event arguments from the arranged call arguments.</param>
        /// <returns>The current expectation so that you can continue the arrangement.</returns>
        /// <exception cref="InvalidOperationException">Thrown when JustMock cannot resolve the event expression.</exception>
        TContainer Raises<T1, T2, T3>(Action eventExpression, Func<T1, T2, T3, EventArgs> func);

        /// <summary>
        /// Raises the specified event when the arranged call occurs.
        /// </summary>
        /// <param name="eventExpression">Expression that identifies the event to raise.</param>
        /// <param name="func">Function that creates the event arguments from the arranged call arguments.</param>
        /// <returns>The current expectation so that you can continue the arrangement.</returns>
        /// <exception cref="InvalidOperationException">Thrown when JustMock cannot resolve the event expression.</exception>
        TContainer Raises<T1, T2, T3, T4>(Action eventExpression, Func<T1, T2, T3, T4, EventArgs> func);

        /// <summary>
        /// Leaves the arranged void call without additional behavior.
        /// </summary>
        /// <remarks>
        /// Use this method when you want the call to succeed without throwing or running custom logic. Loose mocks already behave
        /// this way by default.
        /// </remarks>
        /// <returns>The current expectation so that you can continue the arrangement. Reference to <see cref="IAssertable"/></returns>
        IAssertable DoNothing();
    }
}
