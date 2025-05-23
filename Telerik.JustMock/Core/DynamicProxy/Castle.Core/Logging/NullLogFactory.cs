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
    /// NullLogFactory used when logging is turned off.
    /// </summary>
#if FEATURE_SERIALIZATION
    [Serializable]
#endif
    internal class NullLogFactory : AbstractLoggerFactory
    {
        /// <summary>
        ///   Creates an instance of ILogger with the specified name.
        /// </summary>
        /// <param name = "name">Name.</param>
        public override ILogger Create(string name)
        {
            return NullLogger.Instance;
        }

        /// <summary>
        ///   Creates an instance of ILogger with the specified name and LoggerLevel.
        /// </summary>
        /// <param name = "name">Name.</param>
        /// <param name = "level">Level.</param>
        public override ILogger Create(string name, LoggerLevel level)
        {
            return NullLogger.Instance;
        }
    }
}