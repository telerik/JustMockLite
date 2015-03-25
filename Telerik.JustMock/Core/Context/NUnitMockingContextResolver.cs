/*
 JustMock Lite
 Copyright © 2010-2014 Telerik AD

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

namespace Telerik.JustMock.Core.Context
{
	internal class NUnitMockingContextResolver : HierarchicalTestFrameworkContextResolver
	{
		private const string NunitAssertionExceptionName = "NUnit.Framework.AssertionException, nunit.framework";

		public NUnitMockingContextResolver()
			: base(NunitAssertionExceptionName)
		{
			this.SetupStandardHierarchicalTestStructure(
				new[] { "NUnit.Framework.TestAttribute, nunit.framework", "NUnit.Framework.TestCaseAttribute, nunit.framework", "NUnit.Framework.TestCaseSourceAttribute, nunit.framework" },
				new[] { "NUnit.Framework.SetUpAttribute, nunit.framework", "NUnit.Framework.TearDownAttribute, nunit.framework" },
				new[] { "NUnit.Framework.TestFixtureSetUpAttribute, nunit.framework", "NUnit.Framework.TestFixtureTearDownAttribute, nunit.framework" },
				null,
				FixtureConstuctorSemantics.InstanceConstructorCalledOncePerFixture);
		}

		public static bool IsAvailable
		{
			get { return Type.GetType(NunitAssertionExceptionName) != null; }
		}
	}
}
