using System;
using System.Threading;
using System.Threading.Tasks;

#region JustMock Test Attributes
#if NUNIT
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using AssertionException = NUnit.Framework.AssertionException;
#if NUNIT3
using ClassInitialize = NUnit.Framework.OneTimeSetUpAttribute;
using ClassCleanup = NUnit.Framework.OneTimeTearDownAttribute;
#endif
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
    [TestClass]
    public class MockResetFixture
    {
#if NUNIT3
        private static Foo staticFoo;
        [ClassInitialize]
        public static void ClassInitialize()
        {
            staticFoo = Mock.Create<Foo>();
            Mock.Arrange(() => staticFoo.FooNotImplemented());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Mock.Reset();
            Assert.Throws<NotImplementedException>(() => staticFoo.FooNotImplemented());
        }
#elif !NUNIT && !XUNIT
        private static Foo staticFoo;
        [ClassInitialize]
        public static void ClassInitialize(TestContext ctx){
            staticFoo = Mock.Create<Foo>();
            Mock.Arrange(() => staticFoo.FooNotImplemented());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Mock.Reset();
            Assert.Throws<NotImplementedException>(() => staticFoo.FooNotImplemented());
        }

#endif
        private Foo myFoo;
        [TestInitialize]
        public void TestInit()
        {
            myFoo = Mock.Create<Foo>();
            Mock.Arrange(() => myFoo.FooNotImplemented());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Mock.Reset();
            Assert.Throws<NotImplementedException>(() => myFoo.FooNotImplemented());
        }

        [TestMethod]
        public void ThrowNotImplementedAfterMockReset()
        {
            // Arrange
            var myFoo = Mock.Create<Foo>();

            // Act
            Mock.Reset();

            // Assert
            Assert.Throws<NotImplementedException>(() => myFoo.FooNotImplemented());
        }

        [TestMethod]
        public void DoNotThrowErrorWithMockReset()
        {
            // Act
            Mock.Reset();

            // Assert
            myFoo.FooNotImplemented();

        }

        class Foo
        {
            public void FooNotImplemented()
            {
                throw new NotImplementedException();
            }
        }

    }
}
