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
using System.Reflection;
using System.Text;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#else
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using AssertionException = NUnit.Framework.AssertionException;
#endif

namespace Telerik.JustMock.Tests
{
	[TestClass]
	public class NonPublicFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldMockProtectedVirtualMembers()
		{
			var foo = Mock.Create<Foo>(Behavior.CallOriginal);

			Mock.NonPublic.Arrange(foo, "Load").MustBeCalled();

			foo.Init();

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldMockProtectedProperty()
		{
			var foo = Mock.Create<Foo>(Behavior.CallOriginal);

			Mock.NonPublic.Arrange<int>(foo, "IntValue").Returns(10);

			int ret = foo.GetMultipleOfIntValue();

			Assert.Equal(20, ret);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldMockOverloadUsingMatchers()
		{
			var foo = Mock.Create<Foo>(Behavior.CallOriginal);

			bool called = false;

			Mock.NonPublic
				.Arrange(foo, "ExecuteProtected", ArgExpr.IsAny<int>(), ArgExpr.IsNull<Foo>())
				.DoInstead(() => called = true);

			foo.Execute(10, null);

			Assert.True(called);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldThrowArgumentExpectionForNullArguments()
		{
			var foo = Mock.Create<Foo>(Behavior.CallOriginal);
			Assert.Throws<ArgumentException>(() => Mock.NonPublic.Arrange(foo, "ExecuteProtected", 0, null));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldAssertNonPublicActions()
		{
			var foo = Mock.Create<Foo>(Behavior.CallOriginal);

			Mock.NonPublic.Arrange(foo, "ExecuteProtected", 10);

			foo.Execute(10);

			// assert if called as expected.
			Mock.NonPublic.Assert(foo, "ExecuteProtected", 10);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldAssertNonPublicFunctions()
		{
			var foo = Mock.Create<Foo>(Behavior.CallOriginal);

			Mock.NonPublic.Arrange<int>(foo, "IntValue").Returns(10);

			foo.GetMultipleOfIntValue();

			Mock.NonPublic.Assert<int>(foo, "IntValue");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldThrowForAssertingCallsThatWereNotInvoked()
		{
			var foo = Mock.Create<Foo>(Behavior.CallOriginal);

			Mock.NonPublic.Arrange(foo, "ExecuteProtected", 10);

			// assert if called as expected.
			Assert.Throws<AssertionException>(() => Mock.NonPublic.Assert(foo, "ExecuteProtected", 10));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldAssertOccrenceForNonPublicFunction()
		{
			var foo = Mock.Create<Foo>(Behavior.CallOriginal);
			Mock.NonPublic.Assert<int>(foo, "IntValue", Occurs.Never());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldAssertOccurenceForNonPublicAction()
		{
			var foo = Mock.Create<Foo>(Behavior.CallOriginal);

			Mock.NonPublic.Arrange(foo, "ExecuteProtected", 10);

			foo.Execute(10);

			Mock.NonPublic.Assert(foo, "ExecuteProtected", Occurs.Exactly(1), 10);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldThrowArgumentExcpetionForMethodSpecification()
		{
			var foo = Mock.Create<Foo>(Behavior.CallOriginal);
			Assert.Throws<ArgumentException>(() =>  Mock.NonPublic.Arrange(foo, "ExecuteProtected"));
		}
	  
		#if !SILVERLIGHT

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldCreateMockFromClassHavingAbstractInternalMethodInBase()
		{
			var foo = Mock.Create<FooAbstract2>();
			foo.TryCreateToken(string.Empty);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldMockTypeWithInternalCtorWhenInternalVisibleToIsApplied()
		{
			// Provided that InternalsVisibleTo attribute is included in assemblyinfo.cs.
			var foo = Mock.Create<FooInternal>(Behavior.CallOriginal);
			Assert.NotNull(foo.Builder);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldAssertNonPublicMethodFromBase()
		{
			var baz = Mock.Create<Baz>(Behavior.CallOriginal);

			const string targetMethod = "MethodToMock";
			
			Mock.NonPublic.Arrange(baz, targetMethod).DoNothing();

			baz.MethodToTest();
		 
			Mock.NonPublic.Assert(baz, targetMethod);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldAssertNonPublicCallWhenOccurrenceIsApplied()
		{
			var baz = Mock.Create<Bar>(Behavior.CallOriginal);

			const string targetMethod = "MethodToMock";

			Mock.NonPublic.Arrange(baz, targetMethod).OccursOnce();

			baz.GetType().GetMethod(targetMethod, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(baz, null);

			Mock.NonPublic.Assert(baz, targetMethod);
		}

		public class Bar
		{
			protected virtual void MethodToMock()
			{
				throw new ArgumentException("Base method Invoked");
			}
		}

		public class Baz : Bar
		{
			public virtual void MethodToTest()
			{
				MethodToMock();
			}
		}

		internal class FooInternal
		{
			internal FooInternal()
			{
				builder = new StringBuilder();
			}

			public StringBuilder Builder
			{
				get
				{
					return builder;
				}
			}

			private StringBuilder builder; 
		}

		#endif

		internal abstract class FooAbstract
		{
			protected internal abstract bool TryCreateToken(string literal);
		}

		internal abstract class FooAbstract2 : FooAbstract
		{

		}

		public class Foo
		{
			protected virtual void ExecuteProtected(int arg1, Foo foo)
			{
				throw new NotImplementedException();
			}

			protected virtual void ExecuteProtected(Foo foo, int arg1)
			{
				throw new NotImplementedException();
			}

			protected virtual void ExecuteProtected(int arg1)
			{
				throw new NotImplementedException();
			}

			public virtual void Execute(int arg1)
			{
				ExecuteProtected(arg1);
			}

			public virtual void Execute(int arg1, Foo foo)
			{
				ExecuteProtected(arg1, foo);
			}

			protected virtual void Load()
			{
				throw new NotImplementedException();
			}

			protected virtual int IntValue
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public virtual void Init()
			{
				Load();
			}

			public virtual int GetMultipleOfIntValue()
			{
				return IntValue * 2;
			}
		}
	}
}
