using System;
using System.Threading;

namespace Telerik.JustMock.Core.Context
{
    internal class MSTestV4MockingContextResolver : MSTestBaseMockingContextResolver
    {
        private const string Mstestv4AssemblyName = "MSTest.TestFramework";
        private const string Mstestv4AssertionFailedName = "Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException, " + Mstestv4AssemblyName;

        public MSTestV4MockingContextResolver()
            : base(Mstestv4AssertionFailedName, Mstestv4AssemblyName)
        {
        }

        public static bool IsAvailable
        {
            get { return FindType(Mstestv4AssertionFailedName, false) != null; }
        }
    }
}
