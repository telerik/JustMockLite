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
    /// Adapts a static string content as an <see cref="IResource"/>
    /// </summary>
    internal class StaticContentResource : AbstractResource
    {
        private readonly string contents;

        public StaticContentResource(string contents)
        {
            this.contents = contents;
        }

        public override TextReader GetStreamReader()
        {
            return new StringReader(contents);
        }

        public override TextReader GetStreamReader(Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public override IResource CreateRelative(string relativePath)
        {
            throw new NotImplementedException();
        }
    }
}