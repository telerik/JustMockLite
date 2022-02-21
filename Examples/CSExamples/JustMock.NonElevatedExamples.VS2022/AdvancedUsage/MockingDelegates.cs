/*
 JustMock Lite
 Copyright © 2010-2014 Telerik EAD

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

namespace JustMock.NonElevatedExamples.AdvancedUsage.MockingDelegates
{
    /// <summary>
    /// With Telerik JustMock you can mock delegates and additionally apply all mock capabilities on them. For example, you can 
    ///  assert against their invocation, arrange certain expectations and then pass them in the system under test.
    /// See http://www.telerik.com/help/justmock/advanced-usage-mocking-delegates.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class MockingDelegates_Tests
    {
        [TestMethod]
        public void ShouldArrangeReturnExpectation()
        {
            // ARRANGE
            // Creating a mock instance of the Func<int, int> delegate.
            var delegateMock = Mock.Create<Func<int, int>>();
            
            // Arranging: When the mock is called with 10 as an integer argument, it should return 20.
            Mock.Arrange(() => delegateMock(10)).Returns(20);

            // ACT
            var mySUT = new Foo();
            // Assigning the mock to the dependent property.
            mySUT.FuncDelegate = delegateMock;
            var actual = mySUT.GetInteger(10);
            
            // ASSERT
            Assert.AreEqual(20, actual);
        }

        [TestMethod]
        public void ShouldArrangeOccurrenceExpectation()
        {
            // ARRANGE
            // Creating a mock instance of the Func<int, int> delegate.
            var delegateMock = Mock.Create<Func<int, int>>();

            // Arranging: That the mock should be called with any integer values during the test execution.
            Mock.Arrange(() => delegateMock(Arg.AnyInt)).MustBeCalled();

            // ACT
            var mySUT = new Foo();
            mySUT.FuncDelegate = delegateMock;
            // Assigning the mock to the dependent property.
            var actual = mySUT.GetInteger(123);

            // ASSERT - asserting the mock.
            Mock.Assert(delegateMock);
        }

        [TestMethod]
        public void ShouldPassPrearrangedDelegateMockAsArgument()
        {
            // ARRANGE
            // Creating a mock instance of the Func<string> delegate.
            var delegateMock = Mock.Create<Func<string>>();

            // Arranging: When the mock is called, it should return "Success".
            Mock.Arrange(() => delegateMock()).Returns("Success");

            // ACT
            var testInstance = new DataRepository();
            // Passing the mock into our system under test.
            var actual = testInstance.GetCurrentUserId(delegateMock);

            // ASSERT
            Assert.AreEqual("Success", actual);
        }

        [TestMethod]
        public void ShouldPassDelegateMockAsArgumentAndAssertItsOccurrence()
        {
            bool isCalled = false;

            // ARRANGE
            // Creating a mock instance of the Action<int> delegate.
            var delegateMock = Mock.Create<Action<int>>();

            // Arranging: When the mock is called with any integer value as an argument, it should assign true to isCalled instead.
            Mock.Arrange(() => delegateMock(Arg.AnyInt)).DoInstead(() => isCalled = true);

            // ACT
            var testInstance = new DataRepository();
            // Passing the mock into our system under test.
            testInstance.ApproveCredentials(delegateMock);

            // ASSERT
            Assert.IsTrue(isCalled);
        }
    }

    #region SUT
    public class DataRepository
    {
        public string GetCurrentUserId(Func<string> callback)
        {
            return callback();
        }

        public void ApproveCredentials(Action<int> callback)
        {
            // Some logic here...

            callback(1);
        }
    }

    public class Foo
    {
        public Func<int, int> FuncDelegate { get; set; }

        public int GetInteger(int toThisInt)
        {
            return FuncDelegate(toThisInt);
        }
    }
    #endregion
}
