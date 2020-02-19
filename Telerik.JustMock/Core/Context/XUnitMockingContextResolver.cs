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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Telerik.JustMock.Core.Context
{
	internal abstract class XUnitMockingContextResolver : HierarchicalTestFrameworkContextResolver
	{
		private Type exceptionType = typeof(XUnit.AssertFailedException);

		protected XUnitMockingContextResolver(string exceptionName)
			: base(exceptionName)
		{
		}

		protected override Expression<Func<string, Exception, Exception>> CreateExceptionFactory()
		{
			return this.CreateExceptionFactory(this.exceptionType);
		}
	}

	internal class XUnit1xMockingContextResolver : XUnitMockingContextResolver
	{
		private const string XunitAssertionExceptionName = "Xunit.Sdk.AssertActualExpectedException, xunit";

		public static bool IsAvailable
		{
			get { return FindType(XunitAssertionExceptionName, false) != null; }
		}

		public XUnit1xMockingContextResolver()
			: base(XunitAssertionExceptionName)
		{
			SetupStandardHierarchicalTestStructure(
				new[] { "Xunit.FactAttribute, xunit" },
				null, null, null,
				FixtureConstuctorSemantics.InstanceConstructorCalledOncePerTest);
		}
	}

	internal class XUnit2xMockingContextResolver : XUnitMockingContextResolver
	{
		private const string XunitAssertionExceptionName = "Xunit.Sdk.XunitException, xunit.assert";

		public static bool IsAvailable
		{
			get { return FindType(XunitAssertionExceptionName, false, true) != null; }
		}

		public XUnit2xMockingContextResolver()
			: base(XunitAssertionExceptionName)
		{
			SetupStandardHierarchicalTestStructure(
				new[] { "Xunit.FactAttribute, xunit.core", "Xunit.TheoryAttribute, xunit.core" },
				null, null, null,
				FixtureConstuctorSemantics.InstanceConstructorCalledOncePerTest);
		}
	}
}
