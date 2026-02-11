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
    internal class MSTestV2MockingContextResolver : MSTestBaseMockingContextResolver
    {
        private const string Mstestv2AssemblyName = "Microsoft.VisualStudio.TestPlatform.TestFramework";
        private const string Mstestv2AssertionFailedName = "Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException, " + Mstestv2AssemblyName;

        public MSTestV2MockingContextResolver()
            : base(Mstestv2AssertionFailedName, Mstestv2AssemblyName)
        {
        }

        public static bool IsAvailable
        {
            get { return FindType(Mstestv2AssertionFailedName, false) != null; }
        }
    }
}
