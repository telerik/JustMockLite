/*
 JustMock Lite
 Copyright © 2010-2015 Telerik EAD

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
using System.Diagnostics;

namespace Telerik.JustMock.Core.Context
{
	internal class XamarinIosNUnitContextResolver : HierarchicalTestFrameworkContextResolver
	{
		private const string NunitAssertionExceptionName = "NUnit.Framework.AssertionException, MonoTouch.NUnitLite";

		public XamarinIosNUnitContextResolver()
			: base(NunitAssertionExceptionName)
		{
			this.SetupStandardHierarchicalTestStructure(
				new[] { "NUnit.Framework.TestAttribute, MonoTouch.NUnitLite", "NUnit.Framework.TestCaseAttribute, MonoTouch.NUnitLite", "NUnit.Framework.TestCaseSourceAttribute, MonoTouch.NUnitLite" },
				new[] { "NUnit.Framework.SetUpAttribute, MonoTouch.NUnitLite", "NUnit.Framework.TearDownAttribute, MonoTouch.NUnitLite" },
				new[] { "NUnit.Framework.TestFixtureSetUpAttribute, MonoTouch.NUnitLite", "NUnit.Framework.TestFixtureTearDownAttribute, MonoTouch.NUnitLite" },
				null,
				FixtureConstuctorSemantics.InstanceConstructorCalledOncePerFixture);
		}

		public static bool IsAvailable
		{
			get
			{
				return Type.GetType(NunitAssertionExceptionName) != null;
			}
		}
	}
}
