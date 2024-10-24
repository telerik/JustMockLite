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
    using System.IO;
    using System.Text;

    /// <summary>
    /// Represents a 'streamable' resource. Can
    /// be a file, a resource in an assembly.
    /// </summary>
    internal interface IResource : IDisposable
    {
        /// <remarks>
        /// Only valid for resources that
        /// can be obtained through relative paths
        /// </remarks>
        string FileBasePath { get; }

        /// <summary>
        /// Returns a reader for the stream
        /// </summary>
        /// <remarks>
        /// It's up to the caller to dispose the reader.
        /// </remarks>
        TextReader GetStreamReader();

        /// <summary>
        /// Returns a reader for the stream
        /// </summary>
        /// <remarks>
        /// It's up to the caller to dispose the reader.
        /// </remarks>
        TextReader GetStreamReader(Encoding encoding);

        /// <summary>
        /// Returns an instance of <see cref="IResource"/>
        /// created according to the <c>relativePath</c>
        /// using itself as the root.
        /// </summary>
        IResource CreateRelative(string relativePath);
    }
}