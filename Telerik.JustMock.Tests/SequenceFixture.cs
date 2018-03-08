/*
 JustMock Lite
 Copyright © 2010-2015 Telerik AD

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
using Telerik.JustMock.Helpers;


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
using AssertionException = Xunit.Sdk.AssertException;
#elif VSTEST_PORTABLE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AssertionException = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssertFailedException;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#endif
#endregion


namespace Telerik.JustMock.Tests
{
	[TestClass]
	public class SequenceFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("Sequence")]
		public void ShouldAssertSquenceWithAnyMatcher()
		{
			var iFoo = Mock.Create<IFoo>();

			Mock.Arrange(() => iFoo.Execute("foo")).Returns("hello").InSequence();
			Mock.Arrange(() => iFoo.Execute(Arg.IsAny<string>())).Throws(new ArgumentException()).InSequence();

			Assert.Equal(iFoo.Execute("foo"), "hello");
			Assert.Throws<ArgumentException>(() => iFoo.Execute("crash"));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Sequence")]
		public void ShouldAssertMulitipleSetupWithSameCall()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.GetIntValue()).Returns(1).InSequence();
			Mock.Arrange(() => foo.GetIntValue()).Returns(2).InSequence();
			Mock.Arrange(() => foo.GetIntValue()).Returns(3).InSequence();

			Assert.Equal(foo.GetIntValue(), 1);
			Assert.Equal(foo.GetIntValue(), 2);
			Assert.Equal(foo.GetIntValue(), 3);

		}

		[TestMethod, TestCategory("Lite"), TestCategory("Sequence")]
		public void ShouldAssertMultipleObjectOfSameType()
		{
			var bar1 = Mock.Create<IBar>();
			var bar2 = Mock.Create<IBar>();

			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.GetBar()).Returns(bar1).InSequence();
			Mock.Arrange(() => foo.GetBar()).Returns(bar2).InSequence();

			Assert.Equal(foo.GetBar().GetHashCode(), bar1.GetHashCode());
			Assert.Equal(foo.GetBar().GetHashCode(), bar2.GetHashCode());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Sequence")]
		public void ShouldMultipleForDifferentMatchers()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo(Arg.Matches<int>(x => x > 10))).Returns(10).InSequence();
			Mock.Arrange(() => foo.Echo(Arg.Matches<int>(x => x > 20))).Returns(20).InSequence();

			Assert.Equal(foo.Echo(11), 10);
			Assert.Equal(foo.Echo(21), 20);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Sequence")]
		public void ShouldBeAbleToChainReturns()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo(Arg.AnyInt))
				.Returns(10)
				.Returns(11);

			Assert.Equal(10, foo.Echo(1));
			Assert.Equal(11, foo.Echo(2));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Sequence")]
		public void ShouldBeAbleToSetMustBeCalledForChainReturn()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo(Arg.AnyInt))
				.Returns(10)
				.Returns(11).MustBeCalled();

			Assert.Equal(10, foo.Echo(1));
			Assert.Equal(11, foo.Echo(2));

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Sequence")]
		public void ShouldBeAbleToCorrectlyArrangeTwoChainReturns()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo(Arg.AnyInt))
				.Returns(10)
				.Returns(11).MustBeCalled();

			Mock.Arrange(() => foo.Echo(Arg.AnyInt))
				.Returns(12)
				.Returns(13).MustBeCalled();

			Assert.Equal(10, foo.Echo(1));
			Assert.Equal(11, foo.Echo(2));
			Assert.Equal(12, foo.Echo(3));
			Assert.Equal(13, foo.Echo(4));

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Sequence")]
		public void Should_Arrange_Calls_In_Sequence()
		{
			var foo = Mock.Create<IFoo2>();
			foo.Arrange(x => x.Add(Arg.AnyInt, Arg.AnyInt)).Returns(3).InSequence();
			foo.Arrange(x => x.Add(Arg.AnyInt, Arg.AnyInt)).Returns(5).InSequence();
			foo.Arrange(x => x.Add(Arg.AnyInt, Arg.AnyInt)).Returns(7).InSequence();
			//The parameters don't matter
			Assert.Equals(3, foo.Add(2, 2));
			Assert.Equals(5, foo.Add(2, 2));
			Assert.Equals(7, foo.Add(2, 2));
			//Anything after the last configured InSequence/Returns follows rule of the last arrange
			Assert.Equals(7, foo.Add(2, 5));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Sequence")]
		public void Should_Arrange_Calls_In_Sequence_Fluently()
		{
			var foo = Mock.Create<IFoo2>();
			foo.Arrange(x => x.Add(Arg.AnyInt, Arg.AnyInt)).Returns(3).Returns(5).Returns(7);
			//The parameters don't matter
			Assert.Equals(3, foo.Add(2, 2));
			Assert.Equals(5, foo.Add(2, 2));
			Assert.Equals(7, foo.Add(2, 2));
			//Anything after the last configured InSequence/Returns follows rule of the last arrange
			Assert.Equals(7, foo.Add(2, 2));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Sequence"), TestCategory("InOrder")]
		public void ShouldAssertInOrderOnSameMethod()
		{
			var mock = Mock.Create<IFoo>();
			Mock.Arrange(() => mock.GetIntValue()).InSequence().InOrder();
			Mock.Arrange(() => mock.GetIntValue()).InSequence().InOrder();

			Assert.Throws<AssertionException>(() => Mock.Assert(mock));

			mock.GetIntValue();
			mock.GetIntValue();

			Mock.Assert(mock);
		}

		public interface IFoo2
		{
			int Add(int addend1, int addend2);
		}


		public interface IFoo
		{
			string Execute(string arg);
			int Echo(int arg1);
			int GetIntValue();
			IBar GetBar();
		}

		public interface IBar
		{

		}
	}
}
