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
using Telerik.JustMock.Helpers;

namespace JustMock.NonElevatedExamples.BasicUsage.Mock_Returns
{
    /// <summary>
    /// The Returns method is used with non void calls to ignore the actual call and return a custom value.
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-returns.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Mock_Returns_Tests
    {
        [TestMethod]
        public void ShouldAssertPropertyGetCall()
        {
            var expected = 10;

            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: When foo.Bar is called, it should return the expected value.
            Mock.Arrange(() => foo.Bar).Returns(expected);

            // ACT
            var actual = foo.Bar;

            // ASSERT
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldAssertMethodCallWithMatcher1()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: When foo.Echo() is called with any integer as an argument, it should return 1 + that argument.
            Mock.Arrange(() => foo.Echo(Arg.IsAny<int>())).Returns((int i) => ++i);

            // ACT
            var actual = foo.Echo(10);

            // ASSERT
            Assert.AreEqual(11, actual);
        }

        [TestMethod]
        public void ShouldAssertMethodCallWithMatcher2()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: When foo.Echo() is called with an integer argument exactly matching 10, 
            //  it should return that argument.
            Mock.Arrange(() => foo.Echo(Arg.Matches<int>(x => x == 10))).Returns((int i) => i);

            // ACT
            var actual = foo.Echo(10);

            // ASSERT
            Assert.AreEqual(10, actual);
        }

        [TestMethod]
        public void ShouldReturnWhateverSecondArgIs()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: When foo.Execute() is called with any integer arguments, it should return the second argument.
            Mock.Arrange(() => foo.Execute(Arg.IsAny<int>(), Arg.IsAny<int>())).Returns((int id, int i) => i);

            // ACT
            var actual = foo.Execute(100, 10);

            // ASSERT
            Assert.AreEqual(actual, 10);
        }

        [TestMethod]
        public void ShouldReturnInSequence()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            int[] values = new int[3] { 1, 2, 3 };

            // Arranging: When foo.Bar_GET is called number of times, it should return the array values in sequence.
            Mock.Arrange(() => foo.Bar).ReturnsMany(values);

            // ACT
            var first = foo.Bar;
            var second = foo.Bar;
            var third = foo.Bar;

            // ASSERT
            Assert.AreEqual(first, 1);
            Assert.AreEqual(second, 2);
            Assert.AreEqual(third, 3);
        }
    }

    #region SUT
    public interface IFoo
    {
        int Bar { get; set; }
        int Echo(int myInt);
        int Execute(int myInt1, int myInt2);
    }
    #endregion
}
