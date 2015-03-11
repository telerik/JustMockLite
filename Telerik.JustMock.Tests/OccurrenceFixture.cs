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

using System.Collections.Generic;

#if NUNIT
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using AssertionException = NUnit.Framework.AssertionException;
#elif PORTABLE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AssertionException = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssertFailedException;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#endif

namespace Telerik.JustMock.Tests
{
	[TestClass]
	public class OccurrenceFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("Occurrence")]
		public void ShouldAssertOccursNever()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Assert(() => foo.Submit(), Occurs.Never());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Occurrence")]
		public void ShouldAssertOccursNeverOnArrangedMethod()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Submit()).DoNothing();

			Mock.Assert(() => foo.Submit(), Occurs.Never());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Occurrence")]
		public void ShouldThrowWhenAnyCallIsMadeForOccursNever()
		{
			var foo = Mock.Create<IFoo>();

			foo.Submit();

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Submit(), Occurs.Never()));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Occurrence")]
		public void ShouldAssertExpectedOnce()
		{
			var foo = Mock.Create<IFoo>();

			foo.Submit();

			Mock.Assert(() => foo.Submit(), Occurs.Once());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Occurrence")]
		public void ShouldAssertAtLeastOnce()
		{
			var foo = Mock.Create<IFoo>();

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Submit(), Occurs.AtLeastOnce()));

			foo.Submit();

			Mock.Assert(() => foo.Submit(), Occurs.AtLeastOnce());

			foo.Submit();
			foo.Submit();

			Mock.Assert(() => foo.Submit(), Occurs.AtLeastOnce());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Occurrence")]
		public void ShouldThrowWhenCallsAreMoreThanExpectedOnce()
		{
			var foo = Mock.Create<IFoo>();

			foo.Submit();
			foo.Submit();

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Submit(), Occurs.Once()));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Occurrence")]
		public void ShouldThrowWhenExpectedIsLessThanAtleastNumberOfTimes()
		{
			var foo = Mock.Create<IFoo>();

			foo.Submit();
			foo.Submit();

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Submit(), Occurs.AtLeast(3)));

			foo.Submit();

			Mock.Assert(() => foo.Submit(), Occurs.AtLeast(3));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Occurrence")]
		public void ShouldThrowWhenExpectedCallsAreLessThanTheAtMostNumberOfTImes()
		{
			var foo = Mock.Create<IFoo>();

			foo.Submit();
			foo.Submit();

			Mock.Assert(() => foo.Submit(), Occurs.AtMost(2));

			foo.Submit();
			foo.Submit();

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Submit(), Occurs.AtMost(3)));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Occurrence")]
		public void ShouldThrowWhenExpectedCallsAreLessThanExactNumberOfTimes()
		{
			var foo = Mock.Create<IFoo>();

			foo.Submit();
			foo.Submit();

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Submit(), Occurs.Exactly(3)));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Occurrence")]
		public void ShouldThrowWhenExpectedCallsAreMoreThanExactNumberOfTimes()
		{
			var foo = Mock.Create<IFoo>();

			foo.Submit();
			foo.Submit();
			foo.Submit();
			foo.Submit();

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Submit(), Occurs.Exactly(3)));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Occurrence")]
		public void ShouldAssertExpectedFormatReturnedForSpecificOccurence()
		{
			var foo = Mock.Create<IFoo>();
			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Echo(1), Occurs.Exactly(1)));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Occurrence")]
		public void ShouldAssertUnmatchedOccurrenceFromArrangeOfCallPatternWithAny()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt)).OccursOnce();
			Assert.Throws<AssertFailedException>(() => Mock.Assert(foo));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Occurrence")]
		public void ShouldAssertUnexpectedCallForCallPatternWithTypeMatcher()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt)).OccursNever();
			Mock.Assert(foo);

			Assert.Throws<AssertFailedException>(() => foo.Echo(15));
			Assert.Throws<AssertFailedException>(() => Mock.Assert(foo));
		}

		public interface IFoo
		{
			void Submit();
			int Echo(int intArg);
		}

		public class BaseClass
		{
			public void BaseMethod() { }
			public void GenericBaseMethod<T>() { }
		}

		public class DerivedClass : BaseClass
		{ }

		public class GenericBaseClass<T>
		{
			public void GenericBaseMethod<T, V>() { }
		}

		public class GenericDerivedClass<T> : GenericBaseClass<T>
		{ }

		public interface IFooProvider
		{
			IFoo TheFoo { get; }
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Occurrence"), TestCategory("Bug")]
		public void ShouldNotChangeOccurrenceCountDuringRecursiveArrange()
		{
			var mock = Mock.Create<IFooProvider>();
			Mock.Arrange(() => mock.TheFoo.Submit());
			Mock.Assert(() => mock.TheFoo, Occurs.Never());
		}

		public interface IContainerResolver
		{
			T Resolve<T>(Dictionary<string, object> data);
		}

		public class MockDirectoryInfo
		{
			public MockDirectoryInfo(string path)
			{
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Occurrence"), TestCategory("Bug")]
		public void ShouldArrangeAndAssertExpressionInvolvingCollectionInitializerSyntax()
		{
			IContainerResolver containerResolver = Mock.Create<IContainerResolver>();

			Mock.Arrange(() => containerResolver.Resolve<MockDirectoryInfo>(new Dictionary<string, object> { { "path", @"pptestRoot\DrivesData\TestFamily" } })).Returns(new MockDirectoryInfo("ss")).OccursOnce();

			var ex = Assert.Throws<AssertFailedException>(() => Mock.Assert(containerResolver));
			Assert.True(ex.Message.Contains("Occurrence expectation failed."));
		}
	}
}
