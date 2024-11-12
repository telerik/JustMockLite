/*
 JustMock Lite
 Copyright © 2010-2015 Progress Software Corporation

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
using System.Threading;

namespace Telerik.JustMock.Core.Context
{
    internal class MSTestV2MockingContextResolver : HierarchicalTestFrameworkContextResolver
    {
        private const string Mstestv2AssemblyName = "Microsoft.VisualStudio.TestPlatform.TestFramework";
        private const string Mstestv2AssertionFailedName = "Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException, " + Mstestv2AssemblyName;

        public MSTestV2MockingContextResolver()
            : base(Mstestv2AssertionFailedName)
        {
            this.SetupStandardHierarchicalTestStructure(
                new[] { "Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute, " + Mstestv2AssemblyName },
                new[] { "Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute, " + Mstestv2AssemblyName, "Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute, " + Mstestv2AssemblyName },
                new[] { "Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute, " + Mstestv2AssemblyName, "Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute, " + Mstestv2AssemblyName },
                new[] { "Microsoft.VisualStudio.TestTools.UnitTesting.AssemblyInitializeAttribute, " + Mstestv2AssemblyName, "Microsoft.VisualStudio.TestTools.UnitTesting.AssemblyCleanupAttribute, " + Mstestv2AssemblyName },
                FixtureConstuctorSemantics.InstanceConstructorCalledOncePerFixture);
        }

        public static bool IsAvailable
        {
            get { return FindType(Mstestv2AssertionFailedName, false) != null; }
        }

        private const int DefaultGcFrequency = 50;
        private int createdRepoCount;
        private int lastGcCount;
        private int gcFrequency = GetGcFrequency();
        private bool synchronousGc = GetSynchronousGc();

        private static int GetGcFrequency()
        {
            var valueStr = SecuredRegistryMethods.GetValue(false, @"Software\Telerik\JustMock", "MSTestGcFrequency");
            int value;
            if (!String.IsNullOrEmpty(valueStr) && int.TryParse(valueStr, out value) && value >= 1)
                return value;

            return DefaultGcFrequency;
        }

        private static bool GetSynchronousGc()
        {
            var valueStr = SecuredRegistryMethods.GetValue(false, @"Software\Telerik\JustMock", "SynchronousGc");
            int value;
            return !String.IsNullOrEmpty(valueStr) && int.TryParse(valueStr, out value) && value == 1;
        }

#if !SILVERLIGHT
        protected override void OnMocksRepositoryCreated(MocksRepository repo)
        {
            // MSTest runs every test in a different thread. We'd like to collect Thread objects often so that their handle is released.
            // At every N created repos (we assume that each test creates a single repo, so the number of repos created is close to
            // the number of threads created) do a garbage collection, but only if it hasn't been already done in this interval.

            createdRepoCount++;

            if (createdRepoCount % gcFrequency == 0)
            {
                var gen2Collections = GC.CollectionCount(GC.MaxGeneration);
                if (gen2Collections == lastGcCount)
                {
                    if (synchronousGc)
                    {
                        GC.Collect();
                    }
                    else
                    {
                        ThreadPool.QueueUserWorkItem(_ => GC.Collect());
                    }

                    gen2Collections++;
                }
                lastGcCount = gen2Collections;
            }
        }
#endif
    }
}
