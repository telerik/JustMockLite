// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Telerik.JustMock.Core.Castle.DynamicProxy
{
    using System;

    /// <summary>
    ///   Describes the <see cref="IInvocation.Proceed"/> operation for an <see cref="IInvocation"/>
    ///   at a specific point during interception.
    /// </summary>
    public interface IInvocationProceedInfo
    {
        /// <summary>
        ///   Executes the <see cref="IInvocation.Proceed"/> operation described by this instance.
        /// </summary>
        /// <exception cref="NotImplementedException">There is no interceptor, nor a proxy target object, to proceed to.</exception>
        void Invoke();
    }
}
