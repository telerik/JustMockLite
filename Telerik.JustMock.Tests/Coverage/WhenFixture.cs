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
using System.Collections.Generic;
using System.Linq;

#region JustMock Test Attributes
#if NUNIT
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using AssertionException = NUnit.Framework.AssertionException;
#elif XUNIT
using Xunit;
using Telerik.JustMock.XUnit.Test.Attributes;
using TestCategory = Telerik.JustMock.XUnit.Test.Attributes.XUnitCategoryAttribute;
using TestClass = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestClassAttribute;
using TestMethod = Xunit.FactAttribute;
using TestInitialize = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestInitializeAttribute;
using TestCleanup = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestCleanupAttribute;
#if XUNIT2
using AssertionException = Xunit.Sdk.XunitException;
#else
using AssertionException = Xunit.Sdk.AssertException;
#endif
#elif VSTEST_PORTABLE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AssertionException = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssertFailedException;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#endif
#endregion


namespace Telerik.JustMock.Tests.Coverage
{
	[TestClass]
	public class WhenFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithOneArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool))
				.When((bool arg1) =>
					{
						called = true;
						return arg1;
					})
				.Returns(1);

			int negative = foo.Echo(false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true);
			Assert.Equal(1, positive);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithTwoArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool, Arg.AnyBool))
				.When((bool arg1, bool arg2) =>
					{
						called = true;
						return arg2 && arg2;
					})
				.Returns(1);

			int negative = foo.Echo(false, false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true, true);
			Assert.Equal(1, positive);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithThreeArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool, Arg.AnyBool, Arg.AnyBool))
				.When((bool arg1, bool arg2, bool arg3) =>
					{
						called = true;
						return arg3 && arg3 && arg3;
					})
				.Returns(1);

			int negative = foo.Echo(false, false, false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true, true, true);
			Assert.Equal(1, positive);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithFourArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool))
				.When((bool arg1, bool arg2, bool arg3, bool arg4) =>
					{
						called = true;
						return arg4 && arg4 && arg4 && arg4;
					})
				.Returns(1);

			int negative = foo.Echo(false, false, false, false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true, true, true, true);
			Assert.Equal(1, positive);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithFiveArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool))
				.When((bool arg1, bool arg2, bool arg3, bool arg4, bool arg5) =>
					{
						called = true;
						return arg5 && arg5 && arg5 && arg5 && arg5;
					})
				.Returns(1);

			int negative = foo.Echo(false, false, false, false, false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true, true, true, true, true);
			Assert.Equal(1, positive);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithSixArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool))
				.When((bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6) =>
					{
						called = true;
						return arg6 && arg6 && arg6 && arg6 && arg6 && arg6;
					})
				.Returns(1);

			int negative = foo.Echo(false, false, false, false, false, false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true, true, true, true, true, true);
			Assert.Equal(1, positive);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithSevenArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool))
				.When((bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7) =>
					{
						called = true;
						return arg7 && arg7 && arg7 && arg7 && arg7 && arg7 && arg7;
					})
				.Returns(1);

			int negative = foo.Echo(false, false, false, false, false, false, false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true, true, true, true, true, true, true);
			Assert.Equal(1, positive);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithEightArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool))
				.When((bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8) =>
					{
						called = true;
						return arg8 && arg8 && arg8 && arg8 && arg8 && arg8 && arg8 && arg8;
					})
				.Returns(1);

			int negative = foo.Echo(false, false, false, false, false, false, false, false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true, true, true, true, true, true, true, true);
			Assert.Equal(1, positive);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithNineArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool))
				.When((bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9) =>
					{
						called = true;
						return arg9 && arg9 && arg9 && arg9 && arg9 && arg9 && arg9 && arg9 && arg9;
					})
				.Returns(1);

			int negative = foo.Echo(false, false, false, false, false, false, false, false, false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true, true, true, true, true, true, true, true, true);
			Assert.Equal(1, positive);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithTenArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool))
				.When((bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10) =>
					{
						called = true;
						return arg10 && arg10 && arg10 && arg10 && arg10 && arg10 && arg10 && arg10 && arg10 && arg10;
					})
				.Returns(1);

			int negative = foo.Echo(false, false, false, false, false, false, false, false, false, false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true, true, true, true, true, true, true, true, true, true);
			Assert.Equal(1, positive);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithElevenArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool))
				.When((bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10, bool arg11) =>
					{
						called = true;
						return arg11 && arg11 && arg11 && arg11 && arg11 && arg11 && arg11 && arg11 && arg11 && arg11 && arg11;
					})
				.Returns(1);

			int negative = foo.Echo(false, false, false, false, false, false, false, false, false, false, false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true, true, true, true, true, true, true, true, true, true, true);
			Assert.Equal(1, positive);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithTwelveArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool))
				.When((bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10, bool arg11, bool arg12) =>
					{
						called = true;
						return arg12 && arg12 && arg12 && arg12 && arg12 && arg12 && arg12 && arg12 && arg12 && arg12 && arg12 && arg12;
					})
				.Returns(1);

			int negative = foo.Echo(false, false, false, false, false, false, false, false, false, false, false, false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true, true, true, true, true, true, true, true, true, true, true, true);
			Assert.Equal(1, positive);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithThirteenArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool))
				.When((bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10, bool arg11, bool arg12, bool arg13) =>
					{
						called = true;
						return arg13 && arg13 && arg13 && arg13 && arg13 && arg13 && arg13 && arg13 && arg13 && arg13 && arg13 && arg13 && arg13;
					})
				.Returns(1);

			int negative = foo.Echo(false, false, false, false, false, false, false, false, false, false, false, false, false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true, true, true, true, true, true, true, true, true, true, true, true, true);
			Assert.Equal(1, positive);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithFourteenArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool))
				.When((bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10, bool arg11, bool arg12, bool arg13, bool arg14) =>
					{
						called = true;
						return arg14 && arg14 && arg14 && arg14 && arg14 && arg14 && arg14 && arg14 && arg14 && arg14 && arg14 && arg14 && arg14 && arg14;
					})
				.Returns(1);

			int negative = foo.Echo(false, false, false, false, false, false, false, false, false, false, false, false, false, false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true, true, true, true, true, true, true, true, true, true, true, true, true, true);
			Assert.Equal(1, positive);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithFifteenArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool))
				.When((bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10, bool arg11, bool arg12, bool arg13, bool arg14, bool arg15) =>
					{
						called = true;
						return arg15 && arg15 && arg15 && arg15 && arg15 && arg15 && arg15 && arg15 && arg15 && arg15 && arg15 && arg15 && arg15 && arg15 && arg15;
					})
				.Returns(1);

			int negative = foo.Echo(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true, true, true, true, true, true, true, true, true, true, true, true, true, true, true);
			Assert.Equal(1, positive);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Matchers")]
		public void ShouldTestConditionalArrangeWithSixteenArgs()
		{
			var foo = Mock.Create<IFoo>();
			bool called = false;
			Mock.Arrange(() => foo.Echo(Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool, Arg.AnyBool))
				.When((bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10, bool arg11, bool arg12, bool arg13, bool arg14, bool arg15, bool arg16) =>
					{
						called = true;
						return arg16 && arg16 && arg16 && arg16 && arg16 && arg16 && arg16 && arg16 && arg16 && arg16 && arg16 && arg16 && arg16 && arg16 && arg16 && arg16;
					})
				.Returns(1);

			int negative = foo.Echo(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
			Assert.Equal(0, negative);
			Assert.True(called);

			int positive = foo.Echo(true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true);
			Assert.Equal(1, positive);
		}

		
		public interface IFoo
		{
			int Echo(bool arg1);
			int Echo(bool arg1, bool arg2);
			int Echo(bool arg1, bool arg2, bool arg3);
			int Echo(bool arg1, bool arg2, bool arg3, bool arg4);
			int Echo(bool arg1, bool arg2, bool arg3, bool arg4, bool arg5);
			int Echo(bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6);
			int Echo(bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7);
			int Echo(bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8);
			int Echo(bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9);
			int Echo(bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10);
			int Echo(bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10, bool arg11);
			int Echo(bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10, bool arg11, bool arg12);
			int Echo(bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10, bool arg11, bool arg12, bool arg13);
			int Echo(bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10, bool arg11, bool arg12, bool arg13, bool arg14);
			int Echo(bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10, bool arg11, bool arg12, bool arg13, bool arg14, bool arg15);
			int Echo(bool arg1, bool arg2, bool arg3, bool arg4, bool arg5, bool arg6, bool arg7, bool arg8, bool arg9, bool arg10, bool arg11, bool arg12, bool arg13, bool arg14, bool arg15, bool arg16);
		}
	}
}
