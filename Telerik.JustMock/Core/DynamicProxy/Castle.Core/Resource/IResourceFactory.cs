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

namespace Telerik.JustMock.Core.Castle.Core.Resource
{
    using System;

    /// <summary>
    /// Depicts the contract for resource factories.
    /// </summary>
    internal interface IResourceFactory
    {
        /// <summary>
        /// Used to check whether the resource factory
        /// is able to deal with the given resource
        /// identifier.
        /// </summary>
        /// <remarks>
        /// Implementors should return <c>true</c>
        /// only if the given identifier is supported
        /// by the resource factory
        /// </remarks>
        bool Accept(CustomUri uri);

        /// <summary>
        /// Creates an <see cref="IResource"/> instance
        /// for the given resource identifier
        /// </summary>
        IResource Create(CustomUri uri);

        /// <summary>
        /// Creates an <see cref="IResource"/> instance
        /// for the given resource identifier
        /// </summary>
        IResource Create(CustomUri uri, string basePath);
    }
}