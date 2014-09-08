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
using System.Linq;

namespace Telerik.JustMock.Core.Context
{
	internal class NUnitMockingContextResolver : HierarchicalTestFrameworkContextResolver
	{
		private const string NunitAssemblyName = "nunit.framework";

		public NUnitMockingContextResolver()
			: base("NUnit.Framework.AssertionException", NunitAssemblyName)
		{
			this.SetupStandardHierarchicalTestStructure(
				new[] { "NUnit.Framework.TestAttribute", "NUnit.Framework.TestCaseAttribute", "NUnit.Framework.TestCaseSourceAttribute" },
				new[]{"NUnit.Framework.SetUpAttribute","NUnit.Framework.TearDownAttribute"},
				new[]{"NUnit.Framework.TestFixtureSetUpAttribute","NUnit.Framework.TestFixtureTearDownAttribute"},
				null,
				FixtureConstuctorSemantics.InstanceConstructorCalledOncePerFixture);
		}

		public static bool IsAvailable
		{
			get { return IsAssemblyLoaded(NunitAssemblyName); }
		}
	}
}
