using System;
using Telerik.JustMock.Core;

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
	public class DelegateFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("Mock"), TestCategory("Delegate")]
		public void ShouldCreateMockDelegate()
		{
			var mock = Mock.Create<Action<int, string>>();
			mock(10, null);

			var declTypeName = mock.Method.DeclaringType.Name;
			Assert.True(declTypeName.Contains("Action"));
			Assert.True(declTypeName.Contains("Int32"));
			Assert.True(declTypeName.Contains("String"));
		}

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

		[TestMethod, TestCategory("Lite"), TestCategory("Mock"), TestCategory("Delegate")]
		public void ShouldThrowWhenMockingAbstractDelegate()
		{
			Assert.Throws<MockException>(() => Mock.Create<Delegate>());
			Assert.Throws<MockException>(() => Mock.Create<MulticastDelegate>());
		}
	}
}
