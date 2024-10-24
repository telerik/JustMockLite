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

namespace Telerik.JustMock.Core.Castle.Core.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A collection of <see cref="IConfiguration"/> objects.
    /// </summary>
#if FEATURE_SERIALIZATION
    [Serializable]
#endif
    internal class ConfigurationCollection : List<IConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <c>ConfigurationCollection</c>.
        /// </summary>
        public ConfigurationCollection()
        {
        }

        /// <summary>
        /// Creates a new instance of <c>ConfigurationCollection</c>.
        /// </summary>
        public ConfigurationCollection(IEnumerable<IConfiguration> value) : base(value)
        {
        }

        public IConfiguration this[string name]
        {
            get
            {
                foreach(IConfiguration config in this)
                {
                    if (name.Equals(config.Name))
                    {
                        return config;
                    }
                }

                return null;
            }
        }
    }
}