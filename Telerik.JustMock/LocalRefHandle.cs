/*
 JustMock Lite
 Copyright © 2010-2018 Telerik EAD

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

using Telerik.JustMock.Core;
using Telerik.JustMock.Expectations;

namespace Telerik.JustMock
{
    /// <summary>
    /// Handles local refs returns used for mocking, used with conjunction with <see cref="LocalRef.WithValue{T}(T)"/>
    /// </summary>
    /// <example>
    /// var refHandle = LocalRef.WithValue(10);
    /// Mock.Arrange(mock, m => m.Echo(ref Arg.Ref(Arg.AnyInt).Value)).Returns(refHandle.Handle).OccursOnce();
    /// </example>
    public abstract class LocalRefHandle<T>
    {
        FuncExpectation<T>.RefDelegate refDelegate;

        /// <summary>
        /// Constructs local ref handle
        /// </summary>
        /// <param name="refDelegate">Mocking delegate</param>
        public LocalRefHandle(FuncExpectation<T>.RefDelegate refDelegate)
        {
            this.refDelegate = refDelegate;
        }

        /// <summary>
        /// Delegate used for mocking ref returns, see <see cref="Expectations.FuncExpectation{TReturn}.Returns(System.Delegate)"/>
        /// </summary>
        public FuncExpectation<T>.RefDelegate Handle
        {
            get { return ProfilerInterceptor.GuardInternal(() => this.refDelegate); }
        }

        /// <summary>
        /// Ref to the internal value of type <see cref="T"/>
        /// </summary>
        public ref T Ref
        {
            get
            {
                return ref ProfilerInterceptor.GuardInternal((target, args) =>
                {
                    return ref this.refDelegate();
                }, null, null);
            }
        }
    }
}
