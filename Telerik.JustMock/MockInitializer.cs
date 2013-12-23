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
using System.Linq.Expressions;
using Telerik.JustMock.Core;

namespace Telerik.JustMock
{
    /// <summary>
    /// Mock interceptor initializer.
    /// </summary>
    public partial class MockInitializer
    {
        /// <summary>
        /// Initializes the mock interceptor.
        /// </summary>
        /// <param name="expression">Target expression containing the member to initialize.</param>
		[Obsolete("It is no longer needed to call this method to ensure test execution correctness.")]
		public void For(Expression<Action> expression)
        {
        }

        /// <summary>
        /// Setups the mock code interceptor against a specific method.
        /// </summary>
        /// <param name="expression">Target expression containing the member to initialize.</param>
        /// <typeparam name="TRet">Type of the return value of the method that this delegate encapsulates.</typeparam>
		[Obsolete("It is no longer needed to call this method to ensure test execution correctness.")]
		public void For<TRet>(Expression<Func<TRet>> expression)
        {
        }
    }

    /// <summary>
    /// Class defining members for partially setting up mock on a call.
    /// </summary>
    public partial class MockInitializer<T> : MockInitializer
    {
        /// <summary>
        /// Setups the mock code interceptor against a specific method.
        /// </summary>
        /// <param name="expression">Target expression containing the member to initialize.</param>
		[Obsolete("It is no longer needed to call this method to ensure test execution correctness.")]
		public void For(Expression<Action<T>> expression)
        {
        }

        /// <summary>
        /// Setups the mock code interceptor against a specific method.
        /// </summary>
        /// <param name="expression">Target expression containing the member to initialize.</param>
		[Obsolete("It is no longer needed to call this method to ensure test execution correctness.")]
		public void For(Expression<Func<T>> expression)
        {
        }
    }
}
