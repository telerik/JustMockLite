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

#if NUNIT
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using AssertionException = NUnit.Framework.AssertionException;
#elif VSTEST_PORTABLE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AssertionException = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssertFailedException;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#endif

namespace Telerik.JustMock.Tests
{
	[TestClass]
	public class DynamicFixture
	{
		public class TestBed
		{
			protected virtual int Value
			{
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}

			protected virtual int Get(int x, string y)
			{
				throw new NotImplementedException();
			}

			protected virtual int this[string x]
			{
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}

			protected virtual INode Root
			{
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}
		}

		public interface INode
		{
			string Name { get; }
			INode Left { get; }
			INode Right { get; }
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic"), TestCategory("DynaMock")]
		public void ShouldArrangeNonPublicGetterViaDynaMock()
		{
			var mock = Mock.Create<TestBed>();
			dynamic wrapper = Mock.NonPublic.Wrap(mock);
			Mock.NonPublic.Arrange<int>(wrapper.Value).Returns(123);
			Assert.Equal(123, new PrivateAccessor(mock).GetProperty("Value"));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic"), TestCategory("DynaMock")]
		public void ShouldArrangeNonPublicSetterViaDynaMock()
		{
			var mock = Mock.Create<TestBed>();
			dynamic wrapper = Mock.NonPublic.Wrap(mock);
			dynamic acc = new PrivateAccessor(mock);
			Mock.NonPublic.Arrange(wrapper.Value = 123).MustBeCalled();

			acc.Value = 100;
			Assert.Throws<AssertionException>(() => Mock.Assert(mock));

			acc.Value = 123;
			Mock.Assert(mock);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic"), TestCategory("DynaMock")]
		public void ShouldArrangeNonPublicSetterWithMatchersViaDynaMock()
		{
			var mock = Mock.Create<TestBed>();
			dynamic wrapper = Mock.NonPublic.Wrap(mock);
			Mock.NonPublic.Arrange(wrapper.Value = ArgExpr.IsAny<int>()).MustBeCalled();

			Assert.Throws<AssertionException>(() => Mock.Assert(mock));
			new PrivateAccessor(mock).SetProperty("Value", 77);
			Mock.Assert(mock);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic"), TestCategory("DynaMock")]
		public void ShouldArrangeNonPublicMethodViaDynaMock()
		{
			var mock = Mock.Create<TestBed>();
			dynamic wrapper = Mock.NonPublic.Wrap(mock);
			Mock.NonPublic.Arrange<int>(wrapper.Get(10, "ss")).Returns(123);

			dynamic acc = Mock.NonPublic.MakePrivateAccessor(mock);
			Assert.Equal(0, (int) acc.Get(20, "dd"));
			Assert.Equal(123, (int) acc.Get(10, "ss"));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic"), TestCategory("DynaMock")]
		public void ShouldArrangeNonPublicMethodWithMatchersViaDynaMock()
		{
			var mock = Mock.Create<TestBed>();
			dynamic wrapper = Mock.NonPublic.Wrap(mock);
			Mock.NonPublic.Arrange<int>(wrapper.Get(ArgExpr.Matches<int>(x => x > 40), ArgExpr.IsAny<string>())).Returns(123);

			dynamic acc = Mock.NonPublic.MakePrivateAccessor(mock);
			Assert.Equal(0, (int) acc.Get(20, "ss"));
			Assert.Equal(123, (int) acc.Get(50, "dd"));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic"), TestCategory("DynaMock")]
		public void ShouldArrangeNonPublicIndexerGetterViaDynaMock()
		{
			var mock = Mock.Create<TestBed>();
			dynamic wrapper = Mock.NonPublic.Wrap(mock);

			Mock.NonPublic.Arrange<int>(wrapper["sss"]).Returns(123);

			dynamic acc = Mock.NonPublic.MakePrivateAccessor(mock);
			Assert.Equal(0, (int) acc["ssd"]);
			Assert.Equal(123, (int) acc["sss"]);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic"), TestCategory("DynaMock")]
		public void ShouldArrangeNonPublicIndexerSetterViaDynaMock()
		{
			var mock = Mock.Create<TestBed>();
			dynamic wrapper = Mock.NonPublic.Wrap(mock);
			dynamic acc = Mock.NonPublic.MakePrivateAccessor(mock);

			Mock.NonPublic.Arrange<int>(wrapper["sss"] = 1000).MustBeCalled();

			Assert.Throws<AssertionException>(() => Mock.Assert(mock));
			acc["sss"] = 123;
			Assert.Throws<AssertionException>(() => Mock.Assert(mock));
			acc["aaa"] = 1000;
			Assert.Throws<AssertionException>(() => Mock.Assert(mock));
			acc["sss"] = 1000;
			Mock.Assert(mock);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic"), TestCategory("DynaMock")]
		public void ShouldArrangeNonPublicMemberRecursivelyViaDynaMock()
		{
			var mock = Mock.Create<TestBed>();
			dynamic wrapper = Mock.NonPublic.Wrap(mock);
			dynamic acc = Mock.NonPublic.MakePrivateAccessor(mock);

			Mock.NonPublic.Arrange<string>(wrapper.Root.Left.Left.Right.Name).Returns("abc");

			Assert.Equal("", (string) acc.Root.Left.Name);
			Assert.Equal("abc", (string) acc.Root.Left.Left.Right.Name);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic"), TestCategory("DynaMock")]
		public void ShouldAssertNonPublicMethodViaDynaMock()
		{
			var mock = Mock.Create<TestBed>();
			var wrapper = Mock.NonPublic.Wrap(mock);

			Assert.Throws<AssertionException>(() => Mock.NonPublic.Assert(wrapper.Value = 123, Occurs.Once()));
			Assert.Throws<AssertionException>(() => Mock.NonPublic.Assert(wrapper.Value = ArgExpr.IsAny<int>(), Occurs.Once()));
			new PrivateAccessor(mock).SetProperty("Value", 123);
			Mock.NonPublic.Assert(wrapper.Value = 123, Occurs.Once());
			Mock.NonPublic.Assert(wrapper.Value = ArgExpr.IsAny<int>(), Occurs.Once());
		}
	}
}
