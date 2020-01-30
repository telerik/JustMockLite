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
using System.Linq;
using System.Reflection;
using System.Text;


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
				.Arrange(foo, "ExecuteProtected", Arg.Expr.IsAny<int>(), Arg.Expr.IsNull<Foo>())
				.DoInstead(() => called = true);

			foo.Execute(10, null);

			Assert.True(called);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldMockOverloadUsingConcreteValues()
		{
			var foo = Mock.Create<Foo>(Behavior.CallOriginal);

			bool called = false, called2 = false;

			Mock.NonPublic
				.Arrange(foo, "ExecuteProtected", 10, Arg.Expr.IsNull<FooDerived>())
				.DoInstead(() => called = true);

			Mock.NonPublic
				.Arrange(foo, "ExecuteProtected", Arg.Expr.IsNull<FooDerived>(), 10)
				.DoInstead(() => called2 = true);

			foo.Execute(10, null);
			foo.Execute(null, 10);

			Assert.True(called);
			Assert.True(called2);
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
		public void ShouldThrowMissingMethodExceptionForMethodSpecification()
		{
			var foo = Mock.Create<Foo>(Behavior.CallOriginal);
			Assert.Throws<MissingMemberException>(() => Mock.NonPublic.Arrange(foo, "ExecuteProtected"));
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

#if !PORTABLE
		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldAssertNonPublicCallWhenOccurrenceIsApplied()
		{
			var baz = Mock.Create<Bar>(Behavior.CallOriginal);

			const string targetMethod = "MethodToMock";

			Mock.NonPublic.Arrange(baz, targetMethod).OccursOnce();

			baz.GetType().GetMethod(targetMethod, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(baz, null);

			Mock.NonPublic.Assert(baz, targetMethod);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic"), TestCategory("Assertion")]
		public void ShouldGetTimesCalledOfNonPublicMethod()
		{
			var mock = Mock.Create<Bar>();
			Mock.NonPublic.MakePrivateAccessor(mock).CallMethod("MethodToMock");

			Assert.Equal(1, Mock.NonPublic.GetTimesCalled(mock, "MethodToMock"));
			Assert.Equal(1, Mock.NonPublic.GetTimesCalled(mock, typeof(Bar).GetMethod("MethodToMock", BindingFlags.NonPublic | BindingFlags.Instance)));
		}
#endif

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
			protected virtual void ExecuteProtected(Foo foo, int arg1)
			{
				throw new NotImplementedException();
			}

			protected virtual void ExecuteProtected(int arg1, Foo foo)
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

			public virtual void Execute(Foo foo, int arg1)
			{
				ExecuteProtected(foo, arg1);
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

		public class FooDerived : Foo
		{

		}

		public class RefTest
		{
			protected virtual void Test(string arg1, ref string asd)
			{

			}

			public void ExecuteTest(ref string asd)
			{
				this.Test("test1", ref asd);
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldArrangeNonPublicUsingByRefArgumentWithMatcher()
		{
			var foo = Mock.Create<RefTest>(Behavior.CallOriginal);
			Mock.NonPublic.Arrange(foo, "Test", Arg.Expr.IsAny<string>(), Arg.Expr.Ref(Arg.Expr.IsAny<string>())).MustBeCalled();
			string asd = "asd";
			foo.ExecuteTest(ref asd);
			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldArrangeNonPublicUsingByRefArgumentWithConstant()
		{
			int call = 1;
			int callA = 0, callB = 0;

			var foo = Mock.Create<RefTest>(Behavior.CallOriginal);
			Mock.NonPublic.Arrange(foo, "Test", Arg.Expr.IsAny<string>(), Arg.Expr.Ref(Arg.Expr.IsAny<string>())).DoInstead(() => callB = call++);
			Mock.NonPublic.Arrange(foo, "Test", Arg.Expr.IsAny<string>(), Arg.Expr.Ref("asd")).DoInstead(() => callA = call++);

			string input = "asd";
			foo.ExecuteTest(ref input);
			input = "foo";
			foo.ExecuteTest(ref input);

			Assert.Equal(1, callA);
			Assert.Equal(2, callB);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldArrangeNonPublicUsingByRefArgumentAsOutputParameter()
		{
			var foo = Mock.Create<RefTest>(Behavior.CallOriginal);
			Mock.NonPublic.Arrange(foo, "Test", Arg.Expr.IsAny<string>(), Arg.Expr.Out("asd"));

			string input = "";
			foo.ExecuteTest(ref input);
			Assert.Equal("asd", input);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldNotArrangeNonPublicUsingConstantArgumentWhereByRefIsExpected()
		{
			var foo = Mock.Create<RefTest>(Behavior.CallOriginal);
			Assert.Throws<MissingMemberException>(() => Mock.NonPublic.Arrange(foo, "Test", Arg.Expr.IsAny<string>(), "asd"));
		}

		public abstract class WeirdSignature
		{
			protected abstract int Do(int a, string b, ref object c, IEnumerable<int> d);
			protected abstract void Do(bool b);
			protected abstract DateTime Do(DateTime dateTime);
			protected static void Do(int d) { }
			protected static int Do(char e) { return 0; }
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldProvideHelpfulExceptionMessageWhenNonPublicMethodIsMissing()
		{
			var foo = Mock.Create<WeirdSignature>();
			var exception = Assert.Throws<MissingMemberException>(() => Mock.NonPublic.Arrange(foo, "Do"));
			var message = exception.Message;
			Assert.Equal(("Method 'Do' with the given signature was not found on type Telerik.JustMock.Tests.NonPublicFixture+WeirdSignature\r\n" +
"Review the available methods in the message below and optionally paste the appropriate arrangement snippet.\r\n" +
"----------\r\n" +
"Method 1: Int32 Do(Int32, System.String, System.Object ByRef, System.Collections.Generic.IEnumerable`1[System.Int32])\r\n" +
"C#: Mock.NonPublic.Arrange<int>(mock, \"Do\", Arg.Expr.IsAny<int>(), Arg.Expr.IsAny<string>(), Arg.Expr.Ref(Arg.Expr.IsAny<object>()), Arg.Expr.IsAny<IEnumerable<int>>());\r\n" +
"VB: Mock.NonPublic.Arrange(Of Integer)(mock, \"Do\", Arg.Expr.IsAny(Of Integer)(), Arg.Expr.IsAny(Of String)(), Arg.Expr.Ref(Arg.Expr.IsAny(Of Object)()), Arg.Expr.IsAny(Of IEnumerable(Of Integer))())\r\n" +
"----------\r\n" +
"Method 2: Void Do(Boolean)\r\n" +
"C#: Mock.NonPublic.Arrange(mock, \"Do\", Arg.Expr.IsAny<bool>());\r\n" +
"VB: Mock.NonPublic.Arrange(mock, \"Do\", Arg.Expr.IsAny(Of Boolean)())\r\n" +
"----------\r\n" +
"Method 3: System.DateTime Do(System.DateTime)\r\n" +
"C#: Mock.NonPublic.Arrange<DateTime>(mock, \"Do\", Arg.Expr.IsAny<DateTime>());\r\n" +
"VB: Mock.NonPublic.Arrange(Of Date)(mock, \"Do\", Arg.Expr.IsAny(Of Date)())\r\n" +
"----------\r\n" +
"Method 4: Void Do(Int32)\r\n" +
"C#: Mock.NonPublic.Arrange(\"Do\", Arg.Expr.IsAny<int>());\r\n" +
"VB: Mock.NonPublic.Arrange(\"Do\", Arg.Expr.IsAny(Of Integer)())\r\n" +
"----------\r\n" +
"Method 5: Int32 Do(Char)\r\n" +
"C#: Mock.NonPublic.Arrange<int>(\"Do\", Arg.Expr.IsAny<char>());\r\n" +
"VB: Mock.NonPublic.Arrange(Of Integer)(\"Do\", Arg.Expr.IsAny(Of Char)())\r\n").Replace("\r\n", Environment.NewLine), message);

			var exception2 = Assert.Throws<MissingMemberException>(() => Mock.NonPublic.Arrange(foo, "Dont"));
			var message2 = exception2.Message;
			Assert.Equal(("Method 'Dont' with the given signature was not found on type Telerik.JustMock.Tests.NonPublicFixture+WeirdSignature\r\n" +
				"No methods or properties found with the given name.\r\n").Replace("\r\n", Environment.NewLine), message2);
		}

		public abstract class NonPublicOverloads
		{
			protected abstract int NotOverloaded(int a, out object b);

			protected abstract int Overloaded(int a);
			protected abstract int Overloaded(string a);

			protected abstract int Prop { set; }

			public int CallNotOverloaded(int a, out object b)
			{
				return NotOverloaded(a, out b);
			}

			public int SetProp
			{
				set { Prop = value; }
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldQuickArrangeNonPublicNonOverloadedMethod()
		{
			var mock = Mock.Create<NonPublicOverloads>(Behavior.CallOriginal);

			Mock.NonPublic.Arrange<int>(mock, "NotOverloaded").Returns(5);

			object b;
			var result = mock.CallNotOverloaded(5, out b);

			Assert.Equal(5, result);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldQuickArrangeNonPublicSetter()
		{
			var mock = Mock.Create<NonPublicOverloads>(Behavior.CallOriginal);
			bool called = false;
			Mock.NonPublic.Arrange(mock, "Prop").DoInstead(() => called = true);

			mock.SetProp = 5;

			Assert.True(called);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldFailToQuickArrangeNonPublicOverloadedMethods()
		{
			var mock = Mock.Create<NonPublicOverloads>();
			Assert.Throws<MissingMemberException>(() => Mock.NonPublic.Arrange<int>(mock, "Overloaded"));
		}

		public abstract class GenericTest
		{
			protected abstract T Do<T>(T x);

			protected abstract IEnumerable<T> Enumerate<T>();

			public int TestDo()
			{
				return Do(10);
			}

			public IEnumerable<int> TestEnumerate()
			{
				return Enumerate<int>();
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldArrangeNonPublicMethodReturningGenericValue()
		{
			var mock = Mock.Create<GenericTest>(Behavior.CallOriginal);
			Mock.NonPublic.Arrange<int>(mock, "Do", Arg.Expr.IsAny<int>()).Returns(123);
			Assert.Equal(123, mock.TestDo());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("NonPublic")]
		public void ShouldArrangeNonPublicMethodReturningGenericValueComplexType()
		{
			var mock = Mock.Create<GenericTest>(Behavior.CallOriginal);
			Mock.NonPublic.Arrange<IEnumerable<int>>(mock, "Enumerate").Returns(new[] { 123 });
			var actual = mock.TestEnumerate().ToArray();
			Assert.Equal(1, actual.Length);
			Assert.Equal(123, actual[0]);
		}
	}
}
