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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.NonElevatedExamples.BasicUsage.MockingProperties
{
    /// <summary>
    /// Mocking properties is similar to mocking methods, but there are a few cases that need special attention 
    /// like mocking indexers and particular set operations. 
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-properties.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class MockingProperties_Tests
    {
        [TestMethod]
        public void ShouldFakePropertyGet()
        {
            var expectedValue = 25;

            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: When foo.Value_GET is called, it should return expectedValue.
            Mock.Arrange(() => foo.Value).Returns(expectedValue);

            // ACT
            var actual = foo.Value;

            // ASSERT
            Assert.AreEqual(expectedValue, actual);
        }

        [TestMethod]
        public void ShouldAssertPropertySet()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: That foo.Value must be set to 1 during the test method.
            Mock.ArrangeSet(() => foo.Value = 1).MustBeCalled();

            // ACT
            foo.Value = 1;

            // ASSERT - Asserting the expected foo.Value_SET.
            Mock.AssertSet(() => foo.Value = 1);
        }

        [TestMethod]
        [ExpectedException(typeof(StrictMockException))]
        public void ShouldThrowExceptionOnTheThirdPropertySetCall()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface with Behavior.Strict.
            var foo = Mock.Create<IFoo>(Behavior.Strict);

            // Arranging: That foo.Value should be set to an integer bigger than 3.
            Mock.ArrangeSet(() => foo.Value = Arg.Matches<int>(x => x > 3));

            // ACT - These lines will not trigger the Strict behavior, because they satisfy the expectations.
            foo.Value = 4;
            foo.Value = 5;

            // This throws MockException because matching criteria is not met.
            foo.Value = 3;
        }

        [TestMethod]
        public void MockIndexers()
        {
            // ARRANGE
            // Creating a mocked instance of the "IIndexedFoo" interface.
            var indexedFoo = Mock.Create<IIndexedFoo>();

            // Arranging: That the [0] element of indexedFoo should return "ping".
            Mock.Arrange(() => indexedFoo[0]).Returns("ping");
            // Arranging: That the [1] element of indexedFoo should return "pong".
            Mock.Arrange(() => indexedFoo[1]).Returns("pong");

            // ACT
            string actualFirst = indexedFoo[0];
            string actualSecond = indexedFoo[1];

            // ASSERT
            Assert.AreEqual("ping", actualFirst);
            Assert.AreEqual("pong", actualSecond);
        }

        [TestMethod]
        [ExpectedException(typeof(StrictMockException))]
        public void ShouldThrowExceptionForNotArrangedPropertySet()
        {
            // ARRANGE
            // Creating a mocked instance of the "IIndexedFoo" interface with Behavior.Strict.
            var foo = Mock.Create<IIndexedFoo>(Behavior.Strict);

            // Arranging: That the [0] element of foo should be set to "foo".
            Mock.ArrangeSet(() => { foo[0] = "foo"; });

            // ACT - This meets the expectations.
            foo[0] = "foo";

            // This throws StrictMockException because matching criteria is not met.
            foo[0] = "bar";
        }

        [TestMethod]
        [ExpectedException(typeof(StrictMockException))]
        public void ShouldAssertIndexedSetWithMatcher()
        {
            // ARRANGE
            // Creating a mocked instance of the "IIndexedFoo" interface with Behavior.Strict.
            var foo = Mock.Create<IIndexedFoo>(Behavior.Strict);

            // Arranging: That the [0] element of foo should match a string "ping".
            Mock.ArrangeSet(() => { foo[0] = Arg.Matches<string>(x => x.Equals("ping")); });
            // Arranging: That the [1] element of foo should be any string.
            Mock.ArrangeSet(() => { foo[1] = Arg.IsAny<string>(); });

            // ACT - These lines will not trigger the Strict behavior, because they satisfy the expectations.
            foo[0] = "ping";
            foo[1] = "pong";

            // This line does not satisfy the matching criteria and throws a MockException.
            foo[0] = "bar";
        }
    }

    #region SUT
    public interface IIndexedFoo
    {
        string this[int key] { get; set; }
    }

    public interface IFoo
    {
        int Value { get; set; }
    }
    #endregion
}
