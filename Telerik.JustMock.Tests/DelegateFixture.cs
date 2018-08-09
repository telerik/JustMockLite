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
	public class DelegateFixture
	{
#if !PORTABLE
		[TestMethod, TestCategory("Lite"), TestCategory("DotNetCore"), TestCategory("Mock"), TestCategory("Delegate")]
		public void ShouldCreateMockDelegate()
		{
			var mock = Mock.Create<Action<int, string>>();
			mock(10, null);

			var declTypeName = mock.Method.DeclaringType.Name;
			Assert.True(declTypeName.Contains("Action"));
			Assert.True(declTypeName.Contains("Int32"));
			Assert.True(declTypeName.Contains("String"));
		}
#endif

		[TestMethod, TestCategory("Lite"), TestCategory("Mock"), TestCategory("Delegate")]
		public void ShouldArrangeMockDelegateBehavior()
		{
			var mock = Mock.Create<Action>();
			bool called = false;
			Mock.Arrange(() => mock()).DoInstead(() => called = true);
			mock();
			Assert.True(called);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock"), TestCategory("Delegate")]
		public void ShouldArrangeMockDelegateReturnValue()
		{
			var mock = Mock.Create<Func<int>>();
			Mock.Arrange(() => mock()).Returns(123);
			Assert.Equal(123, mock());
		}

		public interface INode
		{
			INode Child { get; }
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock"), TestCategory("Delegate")]
		public void ShouldArrangeRecursivelyResultOfMockDelegateInvocation()
		{
			var mock = Mock.Create<Func<INode>>();
			var expected = Mock.Create<INode>();
			Mock.Arrange(() => mock().Child.Child).Returns(expected);
			Assert.Same(expected, mock().Child.Child);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock"), TestCategory("Delegate")]
		public void ShouldMatchArgumentsOfMockDelegate()
		{
			var mock = Mock.Create<Func<int, int>>();
			Mock.Arrange(() => mock(5)).Returns(10);
			Assert.Equal(10, mock(5));
			Assert.Equal(0, mock(6));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock"), TestCategory("Delegate")]
		public void ShouldPassMockDelegateArgumentsToImplementationOverride()
		{
			var mock = Mock.Create<Func<int, int>>();
			Mock.Arrange(() => mock(Arg.AnyInt)).Returns((int x) => x * 2);
			Assert.Equal(10, mock(5));
			Assert.Equal(12, mock(6));
		}

		public interface ICallback
		{
			Func<ICallback> CallbackProc { get; }
			string Result { get; }
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock"), TestCategory("Delegate")]
		public void ShouldMockDelegateRecursivelyOnArrange()
		{
			var mock = Mock.Create<ICallback>(Behavior.Loose);
			Mock.Arrange(() => mock.CallbackProc().Result).Returns("result");
			var actual = mock.CallbackProc().Result;
			Assert.Equal("result", actual);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock"), TestCategory("Delegate")]
		public void ShouldMockDelegateRecursivelyThroughRecursiveBehavior()
		{
			var mock = Mock.Create<ICallback>(Behavior.RecursiveLoose);
			var delg = mock.CallbackProc;
			Mock.Arrange(() => delg().Result).Returns("result");
			var actual = mock.CallbackProc().Result;
			Assert.Equal("result", actual);
		}

#if !PORTABLE
		[TestMethod, TestCategory("Lite"), TestCategory("DotNetCore"), TestCategory("Mock"), TestCategory("Delegate")]
		public void ShouldThrowWhenMockingAbstractDelegate()
		{
			Assert.Throws<MockException>(() => Mock.Create<Delegate>());
			Assert.Throws<MockException>(() => Mock.Create<MulticastDelegate>());
		}
#endif

		[TestMethod, TestCategory("Lite"), TestCategory("Mock"), TestCategory("Delegate")]
		public void ShouldAssertDelegateCall()
		{
			var action = Mock.Create<Action>();
			Mock.Arrange(() => action()).MustBeCalled();

			Assert.Throws<AssertionException>(() => Mock.Assert(() => action()));
			Assert.Throws<AssertionException>(() => Mock.Assert(action));

			action();

			Mock.Assert(action);
			Mock.Assert(() => action());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock"), TestCategory("Delegate")]
		public void ShouldAssertDelegateCallWithAssertAll()
		{
			var action = Mock.Create<Action>();
			Mock.Arrange(() => action());

			Assert.Throws<AssertionException>(() => Mock.AssertAll(action));

			action();

			Mock.AssertAll(action);
		}
	}
}
