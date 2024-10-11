/*
 JustMock Lite
 Copyright © 2010-2023 Progress Software Corporation

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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.JustMock.Core.Context
{
    internal class VisualStudioPortableContextResolver : HierarchicalTestFrameworkContextResolver
    {
        public const string AssertFailedExceptionTypeName = "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssertFailedException, Microsoft.VisualStudio.TestPlatform.UnitTestFramework";

        public VisualStudioPortableContextResolver()
            : base(AssertFailedExceptionTypeName)
        {
            this.SetupStandardHierarchicalTestStructure(
                new[] { "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute, Microsoft.VisualStudio.TestPlatform.UnitTestFramework" },
                new[] { "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute, Microsoft.VisualStudio.TestPlatform.UnitTestFramework", "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestCleanupAttribute, Microsoft.VisualStudio.TestPlatform.UnitTestFramework" },
                new[] { "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.ClassInitializeAttribute, Microsoft.VisualStudio.TestPlatform.UnitTestFramework", "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.ClassCleanupAttribute, Microsoft.VisualStudio.TestPlatform.UnitTestFramework" },
                new[] { "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssemblyInitializeAttribute, Microsoft.VisualStudio.TestPlatform.UnitTestFramework", "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssemblyCleanupAttribute, Microsoft.VisualStudio.TestPlatform.UnitTestFramework" },
                FixtureConstuctorSemantics.InstanceConstructorCalledOncePerFixture);
        }

        public static bool IsAvailable
        {
            get { return Type.GetType(AssertFailedExceptionTypeName) != null; }
        }
    }
}
