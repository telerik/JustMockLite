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

namespace JustMock.NonElevatedExamples.BasicUsage.Matchers
{
    /// <summary>
    /// See http://www.telerik.com/help/justmock/basic-usage-matchers.html for full documentation of the feature.
    /// Matchers let you ignore passing actual values as arguments used in mocks. 
    /// Instead, they give you the possibility to pass just an expression that satisfies the 
    /// argument type or the expected value range. There are several types of matchers supported in Telerik JustMock:
    ///     - Defined Matchers:
    ///         Arg.AnyBool
    ///         Arg.AnyDouble
    ///         Arg.AnyFloat
    ///         Arg.AnyGuid
    ///         Arg.AnyInt
    ///         Arg.AnyLong
    ///         Arg.AnyObject
    ///         Arg.AnyShort
    ///         Arg.AnyString
    ///         Arg.NullOrEmpty
    ///     - Arg.IsAny<[Type]>();
    ///     - Arg.IsInRange([FromValue : int], [ToValue : int], [RangeKind])
    ///     - Arg.Matches<T>(Expression<Predicate<T>> expression) 
    /// </summary>
    [TestClass]
    public class Matchers_Tests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UsingMatchersAndSpecializations()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging when foo.Echo() is called with any integer as an argument it should return 10.
            Mock.Arrange(() => foo.Echo(Arg.AnyInt)).Returns(10);
            // Arranging when foo.Echo() is called with integer, bigger than 10 as an argument it should throw ArgumentException.
            Mock.Arrange(() => foo.Echo(Arg.Matches<int>(x => x > 10))).Throws(new ArgumentException());

            // ACT
            int actual = foo.Echo(1);

            // ASSERT
            Assert.AreEqual(10, actual);

            // ACT - This will throw ArgumentException.
            foo.Echo(11);
        }

        [TestMethod]
        public void ShouldUseMatchersInArrange()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging when foo.Echo() is called with arguments: integer equals to 10 and integer equals to 20, 
            //  it should return 30.
            Mock.Arrange(() => foo.Echo(Arg.Matches<int>(x => x == 10), Arg.Matches<int>(x => x == 20))).Returns(30);

            // ACT
            var actual = foo.Echo(10, 20);

            // ASSERT
            Assert.AreEqual(30, actual);
        }

        [TestMethod]
        public void IgnoringAllArgumentsForASpecificExpectation()
        {
            // Arrange
            var foo = Mock.Create<IFoo>();

            Mock.Arrange(() => foo.Echo(0, 0)).IgnoreArguments().Returns(10);

            // Act
            int actual = foo.Echo(10, 200);

            // Assert
            Assert.AreEqual(10, actual);
        }

        [TestMethod]
        public void ShouldUseMatchersInAssert()
        {
            // ARRANGE
            // Creating a mocked instance of the "IPaymentService" interface.
            var paymentService = Mock.Create<IPaymentService>();

            // ACT
            paymentService.ProcessPayment(DateTime.Now, 54.44M);

            // ASSERT - Asserting that paymentService.ProcessPayment() is called with arguments: 
            //              - any DateTime
            //              - decimal equals 54.44M.
            Mock.Assert(() => paymentService.ProcessPayment(
                Arg.IsAny<DateTime>(),
                Arg.Matches<decimal>(paymentAmount => paymentAmount == 54.44M)));
        }

        [TestMethod]
        public void ShouldIgnoreArgumentsWuthMatcherInAssert()
        {
            // ARRANGE
            // Creating a mocked instance of the "IPaymentService" interface.
            var paymentService = Mock.Create<IPaymentService>();

            // ACT
            paymentService.ProcessPayment(DateTime.Now, 54.44M);

            // ASSERT - Asserting that paymentService.ProcessPayment() is called no matter the arguments.
            Mock.Assert(() => paymentService.ProcessPayment(new DateTime(), 0), Args.Ignore());
        }

        [TestMethod]
        public void MatchingCertainRefParameters()
        {
            int myRefArg = 5;

            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging when foo.Bar() is called with ref argument that equals 5, it should return 10.
            Mock.Arrange(() => foo.Bar(ref Arg.Ref(5).Value)).Returns(10);

            // ACT
            int actual = foo.Bar(ref myRefArg);

            // ASSERT
            Assert.AreEqual(10, actual);
            Assert.AreEqual(5, myRefArg); // Asserting that the ref arguments has not been changed.
        }

        [TestMethod]
        public void MatchingRefParametersOfAnyType()
        {
            int myRefArg = 5;

            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging when foo.Bar() is called with any integer ref argument, it should return 10.
            Mock.Arrange(() => foo.Bar(ref Arg.Ref(Arg.AnyInt).Value)).Returns(10);

            // ACT
            int actual = foo.Bar(ref myRefArg);

            // ASSERT
            Assert.AreEqual(10, actual);
            Assert.AreEqual(5, myRefArg); // Asserting that the ref arguments has not been changed.
        }

        [TestMethod]
        public void MatchingRefParametersWithSpecialization()
        {
            int myRefArg = 11;

            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging when foo.Bar() is called with integer ref argument that is bigger than 10, it should return 10.
            Mock.Arrange(() => foo.Bar(ref Arg.Ref(Arg.Matches<int>(x=> x > 10)).Value)).Returns(10);

            // ACT
            int actual = foo.Bar(ref myRefArg);

            // ASSERT
            Assert.AreEqual(10, actual);
            Assert.AreEqual(11, myRefArg); // Asserting that the ref arguments has not been changed.
        }
    }

    #region SUT
    public interface IPaymentService
    {
        void ProcessPayment(DateTime dateTi, decimal deci);
    }

    public interface IFoo
    {
        int Echo(int intArg1);
        int Echo(int intArg1, int intArg2);
        int Bar(ref int intArg1);
    }
    #endregion
}
