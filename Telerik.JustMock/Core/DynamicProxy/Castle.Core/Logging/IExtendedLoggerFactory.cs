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

namespace Telerik.JustMock.Core.Castle.Core.Logging
{
    using System;

    /// <summary>
    ///   Provides a factory that can produce either <see cref = "ILogger" /> or
    ///   <see cref = "IExtendedLogger" /> classes.
    /// </summary>
    internal interface IExtendedLoggerFactory : ILoggerFactory
    {
        /// <summary>
        ///   Creates a new extended logger, getting the logger name from the specified type.
        /// </summary>
        new IExtendedLogger Create(Type type);

        /// <summary>
        ///   Creates a new extended logger.
        /// </summary>
        new IExtendedLogger Create(string name);

        /// <summary>
        ///   Creates a new extended logger, getting the logger name from the specified type.
        /// </summary>
        new IExtendedLogger Create(Type type, LoggerLevel level);

        /// <summary>
        ///   Creates a new extended logger.
        /// </summary>
        new IExtendedLogger Create(string name, LoggerLevel level);
    }
}