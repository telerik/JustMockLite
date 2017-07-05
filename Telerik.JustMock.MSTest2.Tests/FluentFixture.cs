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
using Telerik.JustMock.Core;

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

using Telerik.JustMock.Helpers;

namespace Telerik.JustMock.MSTest2.Tests
{
	[TestClass]
	public class FluentFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldArrangeAssertMockUsingFluentInterface()
		{
			//Arrange
			const string baseDir = @"C:\Foo\Sub1\Sub2\Sub3\Sub4";
			IFileReader fileReader = Mock.Create<IFileReader>();

			fileReader.Arrange(x => x.GetDirectoryParent(baseDir, 4)).Returns(@"C:\Foo\").Occurs(1);

			//Act
			var handler = new DataFileHandler(fileReader);
			var parent = handler.GetDirectoryParent(baseDir, 4);

			//Assert
			Assert.Equal(@"C:\Foo\", parent);

			fileReader.Assert();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldAssertActionWhenChained()
		{
			IFileReader fileReader = Mock.Create<IFileReader>();

			bool mocked = false;

			fileReader.Arrange(x => x.Delete()).DoInstead(() => mocked = true);

			fileReader.Delete();

			Assert.True(mocked);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldAssertPropertyGetWhenChained()
		{
			IFileReader fileReader = Mock.Create<IFileReader>();

			const string expected = @"c:\JustMock";

			fileReader.Arrange(x => x.Path).Returns(expected);

			Assert.Equal(fileReader.Path, expected);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldAssertPropertySetWhenChained()
		{
			IFileReader fileReader = Mock.Create<IFileReader>(Behavior.Strict);

			const string expected = @"c:\JustMock";

			fileReader.ArrangeSet(x => x.Path = expected);

			fileReader.Path = expected;

			Assert.Throws<MockException>(() => fileReader.Path = "abc");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldBeAbleToAssertSpecificActionForASetup()
		{
			IFileReader fileReader = Mock.Create<IFileReader>();

			fileReader.Arrange(x => x.Delete()).OccursOnce();

			fileReader.Delete();

			fileReader.Assert(x => x.Delete());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldBeAbleToAssertSpecificFuntionForASetup()
		{
			IFileReader fileReader = Mock.Create<IFileReader>();

			const string expected = @"c:\JustMock";

			fileReader.Arrange(x => x.Path).Returns(expected).OccursOnce();

			Assert.Equal(expected, fileReader.Path);

			fileReader.Assert(x => x.Path);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldBeAbleToDoAssertAllForASetup()
		{
			IFileReader fileReader = Mock.Create<IFileReader>();

			const string expected = @"c:\JustMock";

			fileReader.Arrange(x => x.Path).Returns(expected);

			Assert.Equal(expected, fileReader.Path);

			fileReader.AssertAll();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldCallMethodForDefaultEventWhenRaised()
		{
			var foo = Mock.Create<IFileReader>();

			bool raised = false;
			foo.FileDeleted += (sender, args) => raised = true;
			foo.Raise(x => x.FileDeleted += null, EventArgs.Empty);

			Assert.True(raised);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldInvokeMethodForACustomEventWhenRaised()
		{
			var foo = Mock.Create<IFileReader>();

			string actual = string.Empty;
			foo.FileAdded += (string value) => actual = value;
			foo.Raise(x => x.FileAdded += null, "x");

			Assert.Equal("x", actual);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldFailOnAssertIfOccursNeverInvoked()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit()).Occurs(2);

			Assert.Throws<AssertionException>(() => Mock.Assert(foo));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldFailOnAssertIfOccursLessThanExpected()
		{
			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.Submit()).Occurs(10);

			foo.Submit();

			Assert.Throws<AssertionException>(() => Mock.Assert(foo));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldAssertOccursOnce()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit()).OccursOnce();

			foo.Submit();

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldAssertOccursNever()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Submit()).OccursNever();

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldAssertOccursAtLeast()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Submit()).OccursAtLeast(2);

			foo.Submit();
			foo.Submit();
			foo.Submit();

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldFailWhenInvokedMoreThanRequried()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Submit()).OccursAtMost(2);

			foo.Submit();
			foo.Submit();
			Assert.Throws<AssertionException>(() => foo.Submit());
			Assert.Throws<AssertionException>(() => Mock.Assert(foo));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldAssertIndividualCallWithLambda()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Submit()).OccursNever();

			Mock.Assert(() => foo.Submit());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldAsssertOcurrenceWhenAppliedWithCallOriginal()
		{
			var foo = Mock.Create<Foo>(Behavior.CallOriginal);

			Mock.Arrange(() => foo.Submit()).OccursOnce();

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Submit()));
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Fluent"), TestCategory("Occurrence")]
		public void ShouldFluentAssertOccurrenceExpectationSetInArrange()
		{
			const int someValue = 4;
			var target = Mock.Create<IFoo>();
			target.Arrange(x => x.Echo(someValue)).OccursNever();
			target.Assert(x => x.Echo(someValue));
		}

#if !SILVERLIGHT

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldAssertMockingInternalMember()
		{
			var siteOptionsEntity = Mock.Create<HardCodedSiteOptionsEntity>();

			var messageMock = Mock.Create<IMessageOperations>();

			siteOptionsEntity.Arrange(x => x.MessageOperationsHelper).Returns(messageMock);

			Assert.NotNull(siteOptionsEntity.MessageOperationsHelper);
		}

#endif

		public class HardCodedSiteOptionsEntity
		{
			internal virtual IMessageOperations MessageOperationsHelper { get; set; }
		}

		public interface IMessageOperations
		{

		}

		public class Foo
		{
			public virtual void Submit()
			{

			}
		}

		public interface IFoo
		{
			void Submit();
			int Echo(int intArg);
		}

		public interface IFileReader
		{
			string GetDirectoryParent(string directory, int levels);
			void Delete();
			string Path { get; set; }
			event EventHandler<EventArgs> FileDeleted;
			event CustomEvent FileAdded;
		}

		public delegate void CustomEvent(string value);

		public class DataFileHandler
		{
			readonly IFileReader fileReader;

			public DataFileHandler(IFileReader fileReader)
			{
				this.fileReader = fileReader;
			}

			public string GetDirectoryParent(string directory, int levels)
			{
				return fileReader.GetDirectoryParent(directory, levels);
			}
		}

		public interface ILinkedListNode
		{
			ILinkedListNode Tail { get; }
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldMockMethodWithReturnTypeSameAsDeclaringClass()
		{
			var selectionData = Mock.Create<ILinkedListNode>();
			Mock.Arrange(() => selectionData.Tail).Returns(null as ILinkedListNode);
		}

		public interface IDataProcessor
		{
			void Process(string data);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldAssertWithMatcherWhenMatchedArgumentIsArray()
		{
			var mock = Mock.Create<IDataProcessor>();
			Mock.Arrange(() => mock.Process(Arg.AnyString)).DoNothing();

			var data = new string[] { "abc" };
			mock.Process(data[0]);

			Mock.Assert(() => mock.Process(data[0]), Occurs.Once());
		}

		public interface IAction
		{
			void Do();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldFailToChainReturnsCallToVoidMethod()
		{
			var mock = Mock.Create<IAction>();

			Assert.Throws<MockException>(() => Mock.Arrange(() => mock.Do()).Returns(123));
		}

		public interface IGuidResolver
		{
			Guid? GetGuid(string id);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Fluent")]
		public void ShouldFailToChainReturnsCallToActionExpectationFromNonPublicInterface()
		{
			var mock = Mock.Create<IGuidResolver>();
			Assert.Throws<MockException>(() => Mock.NonPublic.Arrange(mock, "GetGuid", ArgExpr.IsNull<string>()).Returns((Guid?)new Guid()));
		}
	}

	[TestClass]
	public class FluentContextFixture
	{
		IDisposable mock;

		[TestInitialize]
		public void TestInit()
		{
			mock = Mock.Create<IDisposable>();
		}

		[TestMethod]
		public void ShouldUpdateContextInFluentAssert()
		{
			Mock.Arrange(() => mock.Dispose());
		}

		[TestCleanup]
		public void TestCleanup()
		{
			mock.AssertAll();
			Mock.AssertAll(mock);
		}
	}
}
