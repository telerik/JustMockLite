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
using System.Collections.Generic;
using Telerik.JustMock.Core;


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


namespace Telerik.JustMock.Tests
{
	[TestClass]
	public class PropertiesFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShouldBeAbleToReturnForProperty()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Value).Returns(25);

			Assert.Equal(25, foo.Value);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShouldAbleToReturnForIndexer()
		{
			var indexedFoo = Mock.Create<IIndexedFoo>();

			Mock.Arrange(() => indexedFoo[0]).Returns("ping");

			Assert.Equal(indexedFoo[0], "ping");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShouldReturnDifferentForDifferentIndex()
		{
			var indexedFoo = Mock.Create<IIndexedFoo>();

			Mock.Arrange(() => indexedFoo[0]).Returns("ping");
			Mock.Arrange(() => indexedFoo[1]).Returns("pong");

			Assert.Equal(indexedFoo[0], "ping");
			Assert.Equal(indexedFoo[1], "pong");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShouldAssertPropertySet()
		{
			var foo = Mock.Create<IFoo>(Behavior.Strict);

			Mock.ArrangeSet(() => { foo.Value = 3; });

			foo.Value = 3;

			Assert.Throws<MockException>(() => foo.Value = 2);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void SHouldAssertPropertySetUsingMatcher()
		{
			var foo = Mock.Create<IFoo>(Behavior.Strict);

			Mock.ArrangeSet(() => foo.Value = Arg.Matches<int>(x => x > 3));

			foo.Value = 4;
			foo.Value = 5;

			Assert.Throws<MockException>(() => foo.Value = 3);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShouldAssertIndexedSet()
		{
			var foo = Mock.Create<IIndexedFoo>(Behavior.Strict);

			Mock.ArrangeSet(() => { foo[0] = "foo"; });

			foo[0] = "foo";

			Assert.Throws<MockException>(() => foo[0] = "fxx");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void SHouldAssertIndexedSetWithMatcher()
		{
			var foo = Mock.Create<IIndexedFoo>(Behavior.Strict);

			Mock.ArrangeSet(() => { foo[0] = Arg.Matches<string>(x => x.Equals("ping")); });
			Mock.ArrangeSet(() => { foo[1] = Arg.IsAny<string>(); });

			foo[0] = "ping";
			foo[1] = "pong";
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShouldAssertThrowOnProperty()
		{
			var foo = Mock.Create<IFoo>(Behavior.Strict);

			Mock.ArrangeSet(() => foo.Value = 1).Throws(new ArgumentException());

			Assert.Throws<ArgumentException>(() => foo.Value = 1);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShouldAssertVoidCalls()
		{
			var foo = Mock.Create<IFoo>(Behavior.Strict);
			Mock.Arrange(() => foo.Call(Arg.Matches<int>(x => x == 1), Arg.Matches<int>(x => x > 1)));

			foo.Call(1, 2);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShouldAssertDoInsteadOnProperySet()
		{
			bool expected = false;
			var foo = Mock.Create<IFoo>(Behavior.Strict);
			Mock.ArrangeSet(() => { foo.Value = 1; }).DoInstead(() => expected = true);

			foo.Value = 1;

			Assert.True(expected);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShouldAssertCallOriginalForPropertySet()
		{
			var foo = Mock.Create<FooAbstract>(Behavior.Strict);
			Mock.ArrangeSet(() => { foo.Value = 1; }).CallOriginal();
			Assert.Throws<NotImplementedException>(() => { foo.Value = 1; });
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShouldThrowExpectionForASpecificSet()
		{
			var foo = Mock.Create<Foo>();
			Mock.ArrangeSet(() => foo.MyProperty = 10).Throws(new ArgumentException());

			// should not throw any expection.
			foo.MyProperty = 1;

			Assert.Throws<ArgumentException>(() => { foo.MyProperty = 10; });
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShouldAutomaticallyArrangeGetSetWhenNoneSpecifiedForStub()
		{
			var foo = Mock.Create<IFoo>();

			foo.Name = "Spike";

			Assert.Equal("Spike", foo.Name);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShouldBeAbleToSetPropertiesMultipleTimes()
		{
			var foo = Mock.Create<IFoo>();

			foo.Name = "Dude";
			foo.Name += "s";

			Assert.Equal("Dudes", foo.Name);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShouldDoGetOrSetForIndexedProperty()
		{
			var indexed = Mock.Create<IIndexedFoo>();

			indexed[1] = "hello";

			Assert.Equal("hello", indexed[1]);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShoudlAssertSetOnlyProperty()
		{
			var foo = Mock.Create<IFoo>();

			foo.Track = true;

			Mock.AssertSet(() => foo.Track = true);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShouldAutoSetGetWhenAlreadyInvokedButNotArranged()
		{
			var project = Mock.Create<IProject>();

			if (project.Parent == null)
				project.Parent = new Foo();

			Assert.NotNull(project.Parent);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Properties")]
		public void ShouldSetStringPropertyToNull()
		{
			var mock = Mock.Create<IAutomockedProperties>();
			mock.Name = null;
			mock.Array = null;
			mock.Items = null;
			Assert.Null(mock.Name);
			Assert.Null(mock.Array);
			Assert.Null(mock.Items);
		}

		public interface IAutomockedProperties
		{
			string Name { get; set; }
			int[] Array { get; set; }
			IEnumerable<int> Items { get; set; }
		}

		public class Foo
		{
			public virtual int MyProperty { get; set; }
		}

		public abstract class FooAbstract
		{
			public virtual int Value
			{
				set
				{
					throw new NotImplementedException();
				}
			}
		}

		public interface ISolutionItem
		{
			object Parent { get; set; }
		}

		public interface IProject : ISolutionItem
		{

		}

		public interface IIndexedFoo
		{
			string this[int key] { get; set; }
		}

		public interface IFoo
		{
			bool Track { set; }
			string Name { get; set; }
			string Execute(string arg1);
			int Value { get; set; }
			int ReadOnlyValue { get; }
			void Call(int arg1, int arg2);
		}
	}
}
