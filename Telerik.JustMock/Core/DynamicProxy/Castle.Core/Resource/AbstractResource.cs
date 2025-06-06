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

    internal abstract class AbstractResource : IResource
    {
#if FEATURE_APPDOMAIN
        protected static readonly string DefaultBasePath = AppDomain.CurrentDomain.BaseDirectory;
#else
        protected static readonly string DefaultBasePath = AppContext.BaseDirectory;
#endif

        public virtual string FileBasePath
        {
            get { return DefaultBasePath; }
        }

        public abstract TextReader GetStreamReader();

        public abstract TextReader GetStreamReader(Encoding encoding);

        public abstract IResource CreateRelative(string relativePath);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
