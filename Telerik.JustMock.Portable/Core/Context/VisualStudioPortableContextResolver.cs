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
				new[] { "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute" },
				new[] { "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute", "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestCleanupAttribute" },
				new[] { "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.ClassInitializeAttribute", "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.ClassCleanupAttribute" },
				new[] { "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssemblyInitializeAttribute", "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssemblyCleanupAttribute" },
				FixtureConstuctorSemantics.InstanceConstructorCalledOncePerFixture);
		}
	}
}
