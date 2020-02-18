using System;

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
using AssertionException = Telerik.JustMock.XUnit.AssertFailedException;
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
#if !NETCORE
	[TestClass]
	public class MarshalByRefFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldMockMethodWithRefOutOnMarshalByRefObject()
		{
			var mock = Mock.Create<Marshalled>();
			bool called = false;

			int arb = 20, arc = 30;
			Mock.Arrange(() => mock.Method(10, ref arb, out arc))
				.Returns(() =>
				{
					called = true;
					return 100;
				});

			int b = 0, c;
			var r = mock.Method(10, ref b, out c);
			Assert.True(called);
			Assert.Equal(100, r);
			Assert.Equal(20, b);
			Assert.Equal(30, c);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldArrangeThrowExceptionOnMarshalByRefObject()
		{
			var mock = Mock.Create<Marshalled>();
			Mock.Arrange(() => mock.Nothing()).Throws<ApplicationException>();
			Assert.Throws<ApplicationException>(() => mock.Nothing());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldPassExceptionFromOriginalImplementationOnMarshalByRefObject()
		{
			var mock = Mock.Create<Marshalled>(Behavior.CallOriginal);
			Assert.Throws<ApplicationException>(() => mock.Throw());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldPassResultsFromOriginalImplementationOnMarshalByRefObject()
		{
			var mock = Mock.Create<Marshalled>(Behavior.CallOriginal);

			int b = 40, c;
			var result = mock.Method(5, ref b, out c);

			Assert.Equal(123, result);
			Assert.Equal(50, b);
			Assert.Equal(100, c);
		}

		public class Marshalled : MarshalByRefObject
		{
			public int Method(int a, ref int b, out int c)
			{
				b = a * 10;
				c = a * 20;
				return 123;
			}

			public void Nothing()
			{ }

			public void Throw()
			{
				throw new ApplicationException();
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldPassExceptionFromOriginalImplementationOnSealedMarshalByRefObject()
		{
			var mock = Mock.Create<SealedMarshal>(Behavior.CallOriginal);
			Assert.Throws<ApplicationException>(() => mock.Throw());

			Mock.Arrange(() => mock.Throw()).DoNothing();
			mock.Throw();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldPassResultsFromOriginalImplementationOnSealedMarshalByRefObject()
		{
			var mock = Mock.Create<SealedMarshal>(Behavior.CallOriginal);

			int b = 40, c;
			var result = mock.Method(5, ref b, out c, (x, y) => x * y);

			Assert.Equal(123, result);
			Assert.Equal(50, b);
			Assert.Equal(100, c);
		}

		public sealed class SealedMarshal : MarshalByRefObject
		{
			public T Method<T>(T a, ref T b, out T c, Func<T, T, T> mult)
			{
				b = mult(a, (T)Convert.ChangeType(10, typeof(T)));
				c = mult(a, (T)Convert.ChangeType(20, typeof(T)));
				return (T)Convert.ChangeType(123, typeof(T));
			}

			public void Throw()
			{
				throw new ApplicationException();
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldMockFrameworkMarshalByRefObjectClass()
		{
			var mock = Mock.Create<System.Drawing.Graphics>();
			var called = false;
			Mock.Arrange(() => mock.DrawLine(null, 0, 0, 0, 0)).IgnoreArguments()
				.DoInstead(() => called = true);
			mock.DrawLine(null, 0, 0, 0, 0);
			Assert.True(called);
		}

		public abstract class LikeStream : MarshalByRefObject
		{
			public abstract void Do();

			public void CallDo()
			{
				Do();
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertMarshalByRefMemberOnAbstractType()
		{
			var mock = Mock.Create<LikeStream>();
			mock.Do();
			Mock.Assert(() => mock.Do());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertMarshalByRefMemberOnAbstractTypeCalledFromWithinType()
		{
			var mock = Mock.Create<LikeStream>();
			Mock.Arrange(() => mock.CallDo()).CallOriginal();
			mock.CallDo();
			Mock.Assert(() => mock.Do());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertMarshalByRefMocksSelfEqual()
		{
			var mock = Mock.Create<LikeStream>();
			Assert.True(mock.Equals(mock));

			Mock.Arrange(() => mock.Equals(mock)).Returns(false);
			Assert.False(mock.Equals(mock));
		}
	}
#endif
}
