/*
 JustMock Lite
 Copyright © 2010-2023 Progress Software Corporation

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

namespace JustMock.NonElevatedExamples.BasicUsage.SequentialMocking
{
    /// <summary>
    /// Sequential mocking allows you to return different values on the same or different consecutive calls to 
    /// one and the same type. In other words, you can set up expectations for successive calls of the same type. 
    /// See http://www.telerik.com/help/justmock/basic-usage-sequential-mocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class SequentialMocking_Tests
    {
        [TestMethod]
        public void ShouldArrangeAndAssertInASequence()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: Sequence calls to foo.GetIntValue() should return different values.
            Mock.Arrange(() => foo.GetIntValue()).Returns(0).InSequence();
            Mock.Arrange(() => foo.GetIntValue()).Returns(1).InSequence();
            Mock.Arrange(() => foo.GetIntValue()).Returns(2).InSequence();

            // ACT
            int actualFirstCall = foo.GetIntValue();
            int actualSecondCall = foo.GetIntValue();
            int actualThirdCall = foo.GetIntValue();

            // ASSERT
            Assert.AreEqual(0, actualFirstCall);
            Assert.AreEqual(1, actualSecondCall);
            Assert.AreEqual(2, actualThirdCall);
        }

        [TestMethod]
        public void ShouldAssertSequentlyWithAMatchers()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var iFoo = Mock.Create<IFoo>();

            // Arranging: 
            //      When iFoo.Execute() is called with "foo" as an argument, it should return "hello". 
            //      Next iFoo.Execute() calls with any string as an argument should return "bye".
            Mock.Arrange(() => iFoo.Execute("foo")).Returns("hello").InSequence();
            Mock.Arrange(() => iFoo.Execute(Arg.IsAny<string>())).Returns("bye").InSequence();

            // ACT
            string actualFirstCall = iFoo.Execute("foo");
            string actualSecondCall = iFoo.Execute("bar");

            // This will also return "bye" as this is the last arrange in the sequence.
            string actualThirdCall = iFoo.Execute("foobar");

            // ASSERT
            Assert.AreEqual("hello", actualFirstCall);
            Assert.AreEqual("bye", actualSecondCall);
            Assert.AreEqual("bye", actualThirdCall);
        }

        [TestMethod]
        public void ShouldAssertMultipleCallsWithDifferentMatchers()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: First call to foo.Echo() with any integer bigger than 10 as an argument should return 10, 
            //              every next call to foo.Echo() with any integer bigger than 20 as an argument should return 20. 
            Mock.Arrange(() => foo.Echo(Arg.Matches<int>(x => x > 10))).Returns(10).InSequence();
            Mock.Arrange(() => foo.Echo(Arg.Matches<int>(x => x > 20))).Returns(20).InSequence();

            // ACT
            int actualFirstCall = foo.Echo(11);
            int actualSecondCall = foo.Echo(21);

            // ASSERT
            Assert.AreEqual(10, actualFirstCall);
            Assert.AreEqual(20, actualSecondCall);
        }

        [TestMethod]
        public void ShouldArrangeInSequencedReturns()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: First call to foo.Echo() with any integer as an argument should return 10, 
            //              every next call to foo.Echo() (using the same matcher) should return 11. 
            Mock.Arrange(() => foo.Echo(Arg.AnyInt)).Returns(10).Returns(11);

            // ACT
            var ctualFirstCall = foo.Echo(1);
            var actualSecondCall = foo.Echo(2);

            // ASSERT
            Assert.AreEqual(10, ctualFirstCall);
            Assert.AreEqual(11, actualSecondCall);
        }
    }

    #region SUT
    public interface IFoo
    {
        string Execute(string arg);
        int Echo(int arg1);
        int GetIntValue();
    }  
    #endregion
}
