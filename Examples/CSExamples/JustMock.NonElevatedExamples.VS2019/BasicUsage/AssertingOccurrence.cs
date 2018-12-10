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
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.NonElevatedExamples.BasicUsage.AssertingOccurrence
{
    /// <summary>
    /// See http://www.telerik.com/help/justmock/basic-usage-asserting-occurrence.html for full documentation of the feature.
    /// Occurrence is used in conjunction with Mock.Assert and Mock.AssertSet to determine how many times a call has occurred.
    /// There are 6 types of occurrence that we can use:
    ///    Occurs.Never() - Specifies that a particular call is never made on a mock.
    ///    Occurs.Once() - Specifies that a call has occurred only once on a mock.
    ///    Occurs.AtLeastOnce() - Specifies that a call has occurred at least once on a mock.
    ///    Occurs.AtLeast(numberOfTimes) - Specifies the number of times at least a call should occur on a mock.
    ///    Occurs.AtMost(numberOfTimes) - Specifies the number of times at most a call should occur on a mock.
    ///    Occurs.Exactly(numberOfTimes) - Specifies exactly the number of times a call should occur on a mock. 
    /// Furthermore, you can set occurrence directly in the arrangement of a method.
    /// You can use one of 5 different constructs of Occur:
    ///    Occurs(numberOfTimes) - Specifies exactly the number of times a call should occur on a mock.
    ///    OccursOnce() - Specifies that a call should occur only once on a mock.
    ///    OccursNever() - Specifies that a particular call should never be made on a mock.
    ///    OccursAtLeast(numberOfTimes) - Specifies that a call should occur at least once on a mock.
    ///    OccursAtMost(numberOfTimes) - Specifies the number of times at most a call should occur on a mock. 
    /// </summary>
    [TestClass]
    public class AssertingOccurrence_Tests
    {
        [TestMethod]
        public void ShouldOccursNever()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // ASSERT - Asserting that foo.Submit() has never occurred during the test method.
            Mock.Assert(() => foo.Submit(), Occurs.Never());
        }

        [TestMethod]
        public void ShouldOccursOnce()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // ACT
            foo.Submit();

            // ASSERT - Asserting that foo.Submit() occurs exactly once during the test method.
            Mock.Assert(() => foo.Submit(), Occurs.Once());
        }

        [TestMethod]
        public void ShouldOccursAtLeastOnce()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // ACT
            foo.Submit();

            // ASSERT - Asserting that foo.Submit() occurs at least once (could be more than once) during the test method.
            Mock.Assert(() => foo.Submit(), Occurs.AtLeastOnce());

            // ACT - Calling foo.Submit() more times.
            foo.Submit();
            foo.Submit();

            // ASSERT - This should pass again.
            Mock.Assert(() => foo.Submit(), Occurs.AtLeastOnce());
        }

        [TestMethod]
        public void ShouldOccursAtLeastCertainNumberOfTimes()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // ACT
            foo.Submit();
            foo.Submit();
            foo.Submit();

            // ASSERT - Asserting that foo.Submit() occurs at least three times during the test method.
            Mock.Assert(() => foo.Submit(), Occurs.AtLeast(3));

            // ACT - Calling foo.Submit() more times.
            foo.Submit();

            // ASSERT - This should pass again.
            Mock.Assert(() => foo.Submit(), Occurs.AtLeast(3));
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void ShouldOccursCertainNumberOfTimesAtMost()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // ACT
            foo.Submit();
            foo.Submit();

            // ASSERT - Asserting that foo.Submit() occurs maximum twice during the test method.
            Mock.Assert(() => foo.Submit(), Occurs.AtMost(2));

            // ACT - Calling foo.Submit() once again - 3 times in total.
            foo.Submit();

            // Assert - This throws an exception.
            Mock.Assert(() => foo.Submit(), Occurs.AtMost(2));
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void ShouldOccursExactly()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // ACT
            foo.Submit();
            foo.Submit();
            foo.Submit();

            // ASSERT - Asserting that foo.Submit() occurs exactly 3 times during the test method.
            Mock.Assert(() => foo.Submit(), Occurs.Exactly(3));

            // ACT - Calling foo.Submit once again - 4 times in total.
            foo.Submit();

            // Assert - This fails because foo.Submit was called more times than specified.
            Mock.Assert(() => foo.Submit(), Occurs.Exactly(3));
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void ShouldFailOnAssertAllWhenExpectionIsNotMet()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();
            
            // Arranging: That foo.Submit() should occur exactly twice during the test method.
            Mock.Arrange(() => foo.Submit()).Occurs(2);

            // ACT - No actions.

            // ASSERT - This will throw an exception as the expectations are not met.
            Mock.Assert(foo);
        }

        [TestMethod]
        public void ShouldArrangeOccursOnce()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: That foo.Submit() should occur exactly once during the test method.
            Mock.Arrange(() => foo.Submit()).OccursOnce();

            // ACT
            foo.Submit();

            // ASSERT
            Mock.Assert(foo);
        }

        [TestMethod]
        public void ShouldArrangeOccursNever()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: That foo.Submit() should never occur during the test method.
            Mock.Arrange(() => foo.Submit()).OccursNever();

            // ACT - No actions.

            // ASSERT
            Mock.Assert(foo);
        }

        [TestMethod]
        public void ShouldArrangeOccursAtLeast()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: That foo.Submit() should occur at least twice during the test method.
            Mock.Arrange(() => foo.Submit()).OccursAtLeast(2);

            // ACT - Calling foo.Submit() 3 times.
            foo.Submit();
            foo.Submit();
            foo.Submit();

            // ASSERT - This passes as foo.Submit() is called at least twice.
            Mock.Assert(foo);
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void ShouldFailWhenInvokedMoreThanRequried()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: That foo.Submit() should occur maximum twice during the test method.
            Mock.Arrange(() => foo.Submit()).OccursAtMost(2);

            // ACT
            foo.Submit();
            foo.Submit();
            foo.Submit(); // This throws an exception because foo.Submit is being called more times than specified.
        }

        [TestMethod]
        public void ShouldBeAbleToAssertOccursUsingMatcherForSimilarCallAtOneShot()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: Different expectations depending on the argument of foo.Echo().
            Mock.Arrange(() => foo.Echo(1)).Returns((int arg) => arg);
            Mock.Arrange(() => foo.Echo(2)).Returns((int arg) => arg);
            Mock.Arrange(() => foo.Echo(3)).Returns((int arg) => arg);

            // ACT
            foo.Echo(1);
            foo.Echo(2);
            foo.Echo(3);

            // ASSERT - This will pass as foo.Echo() has been called exactly 3 times no matter the argument.
            Mock.Assert(() => foo.Echo(Arg.AnyInt), Occurs.Exactly(3));
        }

        [TestMethod]
        public void ShouldVerifyCallsOrder()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: That foo.Submit() should be called before foo.Echo().
            Mock.Arrange(() => foo.Submit()).InOrder();
            Mock.Arrange(() => foo.Echo(Arg.AnyInt)).InOrder();

            // ACT
            foo.Submit();
            foo.Echo(5);

            // ASSERT 
            Mock.Assert(foo);
        }

        [TestMethod]
        public void ShouldAssertInOrderForDifferentInstancesInTestMethodScope()
        {
            string userName = "Bob";
            string password = "Password";
            int userID = 5;
            var cart = new List<string> { "Foo", "Bar" };

            // ARRANGE
            // Creating mocked instances of the "IUserValidationService" and "IShoppingCartService" interfaces.
            var userServiceMock = Mock.Create<IUserValidationService>();
            var shoppingCartServiceMock = Mock.Create<IShoppingCartService>();

            // Arranging: When userServiceMock.ValidateUser(userName, password) is called it should return userID. 
            //  Also this method should occur exactly once in a given order during the test execution. 
            Mock.Arrange(() => userServiceMock.ValidateUser(userName, password)).Returns(userID).InOrder().OccursOnce();
            // Arranging: When shoppingCartServiceMock.LoadCart(userID) is called it should return cart. 
            //  Also this method should occur exactly once in a given order during the test execution. 
            Mock.Arrange(() => shoppingCartServiceMock.LoadCart(userID)).Returns(cart).InOrder().OccursOnce();

            // ACT
            userServiceMock.ValidateUser(userName, password);
            shoppingCartServiceMock.LoadCart(userID);

            // ASSERT - Asserting occurrence and calls order. 
            Mock.Assert(userServiceMock);
            Mock.Assert(shoppingCartServiceMock);
        }
    }

    #region SUT
    public interface IFoo
    {
        void Submit();
        int Echo(int intArg);
    }

    public interface IUserValidationService
    {
        int ValidateUser(string userName, string password);
    }

    public interface IShoppingCartService
    {
        IList<string> LoadCart(int userID);
    }
    #endregion
}
