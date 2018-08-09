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
using System.Reflection;

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
using Telerik.JustMock.Core;
#endif
#endregion

namespace Telerik.JustMock.Tests
{
    [TestClass]
    public class MiscFixture
    {
        [TestMethod, TestCategory("Lite"), TestCategory("DotNetCore")]
        public void MockingInterface()
        {
            // Arrange
            var mock = Mock.Create<Interface1>();
            Mock.Arrange(mock, m => m.Method(Arg.AnyString)).OccursOnce();

            // Act
            var result = mock.Method("test");

            // Assert
            Assert.Equal(default(bool), result);
            Mock.Assert(mock);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("DotNetCore")]
        public void MockingInterfaceReturns()
        {
            // Arrange
            var mock = Mock.Create<Interface1>();
            Mock.Arrange(mock, m => m.Method(Arg.AnyString)).Returns(true).OccursOnce();

            // Act
            var result = mock.Method("test");

            // Assert
            Assert.True(result);
            Mock.Assert(mock);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("DotNetCore"), ExpectedException(typeof(NotImplementedException))]
        public void MockingInterfaceCallOriginal()
        {
            // Arrange
            var mock = Mock.Create<Interface1>();
            Mock.Arrange(mock, m => m.Method(Arg.AnyString)).CallOriginal().OccursOnce();

            // Act
            var result = mock.Method("test");

            // Assert
            Assert.True(result);
            Mock.Assert(mock);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("DotNetCore")]
        public void MockingInterfaceDoNothing()
        {
            // Arrange
            var mock = Mock.Create<Interface1>();
            Mock.Arrange(mock, m => m.Method(Arg.AnyString)).DoNothing().OccursOnce();

            // Act
            var result = mock.Method("test");

            // Assert
            Assert.Equal(default(bool), result);
            Mock.Assert(mock);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("DotNetCore"), ExpectedException(typeof(MockException))]
        public void MockingInterfaceTrow()
        {
            // Arrange
            var mock = Mock.Create<Interface1>();
            Mock.Arrange(mock, m => m.Method(Arg.AnyString)).Throws(new MockException()).OccursOnce();

            // Act
            var result = mock.Method("test");
        }

        [TestMethod, TestCategory("Lite"), TestCategory("DotNetCore")]
        public void MockingDependenciesUsingFunctions()
        {
            // Arrange
            var mock1 = Mock.Create<Interface1>();
            var mock2 = Mock.Create<Interface2>();
            var sut = new Class1(mock1, mock2);
            Mock.Arrange(mock1, m => m.Method(Arg.AnyString)).Returns(true).OccursOnce();
            Mock.Arrange(mock2, m => m.Method(Arg.AnyString)).Returns(true).OccursOnce();

            // Act
            var result = sut.And("test1", "test2");

            // Assert
            Assert.Equal(true, result);
            Mock.Assert(mock1);
            Mock.Assert(mock2);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("DotNetCore")]
        public void MockingDependenciesUsingProperties()
        {
            // Arrange
            var mock1 = Mock.Create<Interface1>();
            var mock2 = Mock.Create<Interface2>();
            var sut = new Class1(mock1, mock2);
            Mock.Arrange(mock1, m => m.Value).Returns(1).OccursOnce();
            Mock.Arrange(mock2, m => m.Value).Returns(1).OccursOnce();
            
            // Act
            var result = sut.Add();

            // Assert
            Assert.Equal(2, result);
            Mock.Assert(mock1);
            Mock.Assert(mock2);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("DotNetCore"), ExpectedException(typeof(NotImplementedException))]
        public void ArrangementOfInterfaceDerivedClassMockShouldThrow()
        {
            // Arrange
            var mock = Mock.Create<Interface1Impl>();
            Mock.Arrange(mock, m => m.Method(Arg.AnyObject)).OccursNever();
        }

        [TestMethod, TestCategory("Lite"), TestCategory("DotNetCore"), ExpectedException(typeof(NullReferenceException))]
        public void ArrangementOfSimpleClassMockShouldThrow()
        {
            // Arrange
            var mock = Mock.Create<Class1>();
            Mock.Arrange(mock, m => m.Add()).Returns(0).OccursNever();
        }

        [TestMethod, TestCategory("Lite"), TestCategory("DotNetCore"), ExpectedException(typeof(MockException))]
        public void ArrangementOfRealClassShouldThrow()
        {
            // Arrange
            var sut = new Class1(new Interface1Impl(), new Interface1Imp2());
            var mock1 = Mock.Create<Interface1>();
            Mock.Arrange(sut, s => s.Dependency1).Returns(mock1).OccursOnce();
        }

        [TestMethod, TestCategory("Lite"), TestCategory("DotNetCore"), ExpectedException(typeof(NotImplementedException))]
        public void ActionWithInterfaceDerivedClassMockShouldThrow()
        {
            // Arrange
            var mock = Mock.Create<Interface1Impl>();

            // Act
            var result = mock.Method("test");
        }

        [TestMethod, TestCategory("Lite"), TestCategory("DotNetCore"), ExpectedException(typeof(NullReferenceException))]
        public void ActionWithSimpleClassMockShouldThrow()
        {
            // Arrange
            var mock = Mock.Create<Class1>();

            // Act
            var result = mock.Add();
        }

        [TestMethod, TestCategory("Lite"), TestCategory("DotNetCore")]
        public void AssertionOfInterfaceDerivedClassMockShouldNotThrow()
        {
            // Arrange
            var mock = Mock.Create<Interface1Impl>();

            // Assert
            Mock.Assert(mock);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("DotNetCore")]
        public void AssertionWithSimpleClassMockShouldNotThrow()
        {
            // Arrange
            var mock = Mock.Create<Class1>();

            // Assert
            Mock.Assert(mock);
        }

        public interface Interface1
        {
            bool Method(object value);

            int Value { get; set; }
        }

        public interface Interface2
        {
            bool Method(object value);

            int Value { get; set; }
        }

        public class Interface1Impl : Interface1
        {
            public bool Method(object value)
            {
                throw new NotImplementedException();
            }

            public int Value
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }
        }

        public class Interface1Imp2 : Interface2
        {
            public bool Method(object value)
            {
                throw new NotImplementedException();
            }

            public int Value
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }
        }

        public class Class1
        {
            Interface1 dependency1;
            Interface2 dependency2;

            public Class1(Interface1 dependency1, Interface2 dependency2)
            {
                this.dependency1 = dependency1;
                this.dependency2 = dependency2;
            }

            public bool And(object value1, object value2)
            {
                return this.Dependency1.Method(value1) && this.Dependency2.Method(value2);
            }

            public int Add()
            {
                return this.Dependency1.Value + this.Dependency2.Value;
            }

            public Interface1 Dependency1
            {
                get { return this.dependency1; }
                set { this.dependency1 = value; }
            }

            public Interface2 Dependency2
            {
                get { return this.dependency2; }
                set { this.dependency2 = value; }
            }
        }
    }
}
