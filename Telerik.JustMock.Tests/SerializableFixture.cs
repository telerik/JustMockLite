/*
 JustMock Lite
 Copyright © 2019 Progress Software Corporation

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
using System.Text;
using Telerik.JustMock.DemoLib;

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
    public class SerializableFixture
    {
        [TestMethod, TestCategory("Lite"), TestCategory("Serializable")]
        public void ShouldMockTypesMarkedWithSerializableAttribute()
        {
            int expected = 10;
            var foo = Mock.Create<FooSerializable>();

            Mock.Arrange(() => foo.Value).Returns(expected);

            var actual = foo.Value;
            Assert.Equal(expected, actual);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("Serializable")]
        public void ShouldMockTypesThatInheritISerializable()
        {
            int expected = 10;
            var foo = Mock.Create<FooInheritISerializable>();

            Mock.Arrange(() => foo.Value).Returns(expected);

            var actual = foo.Value;
            Assert.Equal(expected, actual);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("Serializable")]
        public void ShouldMockTypeThatImplementSerializationAttributeAndInterface()
        {
            int expected = 10;
            var foo = Mock.Create<FooImplementSerializationAttributeAndInterface>();

            Mock.Arrange(() => foo.Value).Returns(expected);

            var actual = foo.Value;
            Assert.Equal(expected, actual);
        }
    }
}
