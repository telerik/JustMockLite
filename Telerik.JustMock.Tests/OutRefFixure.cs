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



#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
#endif


namespace Telerik.JustMock.Tests
{
	[TestClass]
	public class OutRefFixure
	{
		[TestMethod, TestCategory("Lite"), TestCategory("OutRef")]
		public void ShouldExpectOutArgumets()
		{
			string expected = "ack";

			var iFoo = Mock.Create<IFoo>();

			Mock.Arrange(() => iFoo.Execute("ping", out expected)).Returns(true);

			string original;

			Assert.True(iFoo.Execute("ping", out original));
			Assert.Equal(original, expected);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("OutRef")]
		public void ShouldAssertRefArguments()
		{
			string strToReturn = "abc";

			var iFoo = Mock.Create<IFoo>();

			Mock.Arrange(() => iFoo.Execute(ref strToReturn)).DoNothing();

			string original = string.Empty;

			iFoo.Execute(ref original);

			Assert.Equal(strToReturn, original);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("OutRef")]
		public void ShouldAssertGuidAsOutArgument()
		{
			var iFoo = Mock.Create<IFoo>();

			Guid expected = Guid.NewGuid();

			Mock.Arrange(() => iFoo.GuidMethod(out expected)).Returns(true);

			Guid original;

			Assert.True(iFoo.GuidMethod(out original));
			Assert.Equal(original, expected);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("OutRef")]
		public void ShouldAssertIntRefArgument()
		{
			var foo = Mock.Create<IFoo>();

			var expected = 10;

			Mock.Arrange(() => foo.IntMethod(ref expected)).Returns(true);

			var original = 0;

			Assert.True(foo.IntMethod(ref original));
			Assert.Equal(original, 10);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("OutRef")]
		public void ShouldAssertWhenExpectedOutIsNull()
		{
			var foo = Mock.Create<Foo>(Behavior.CallOriginal);

			Token expected;

			Mock.Arrange(() => foo.Execute(Arg.AnyString, out expected)).Returns(true);

			Assert.True(foo.Execute("xmas", out expected));
		}

		public class Token
		{

		}

		public class Foo
		{
			public virtual bool Execute(string arg, out Token token)
			{
				token = new Token();
				return false;
			}
		}

		public interface IFoo
		{
			bool Execute(string arg1, out string arg2);
			void Execute(ref string arg1);
			bool GuidMethod(out Guid id);
			bool IntMethod(ref int argument);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("OutRef")]
		public void ShouldArrangeByRefValueMatcher()
		{
			var mock = Mock.Create<IFoo>();
			Mock.Arrange(() => mock.IntMethod(ref Arg.Ref(100).Value)).Returns(true);

			int value = 100;
			var actual = mock.IntMethod(ref value);
			Assert.Equal(true, actual);

			value = 0;
			actual = mock.IntMethod(ref value);
			Assert.Equal(false, actual);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("OutRef")]
		public void ShouldArrangeByRefPredicatedMatcher()
		{
			var mock = Mock.Create<IFoo>();
			Mock.Arrange(() => mock.IntMethod(ref Arg.Ref(Arg.IsInRange(0, 1000, RangeKind.Inclusive)).Value)).Returns(true);

			int value = 100;
			var actual = mock.IntMethod(ref value);
			Assert.Equal(true, actual);

			value = -100;
			actual = mock.IntMethod(ref value);
			Assert.Equal(false, actual);

			value = 10000;
			actual = mock.IntMethod(ref value);
			Assert.Equal(false, actual);
		}

		public interface IMixRefs
		{
			int Do(ref int a, ref int b);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("OutRef")]
		public void ShouldMatcherRefsCoexistWithReturnRefs()
		{
			var mock = Mock.Create<IMixRefs>();

			int arrange_a = 100;
			Mock.Arrange(() => mock.Do(ref arrange_a, ref Arg.Ref(500).Value)).Returns(10);

			int a = 0;
			int b = 500;
			mock.Do(ref a, ref b);

			Assert.Equal(100, a);
			Assert.Equal(500, b);

			int a2 = 0;
			int c = 100;
			mock.Do(ref a2, ref c);
			Assert.Equal(0, a2);
			Assert.Equal(100, c);
		}
	}
}
