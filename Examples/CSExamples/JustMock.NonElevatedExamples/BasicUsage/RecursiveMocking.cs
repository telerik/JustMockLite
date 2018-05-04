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

namespace JustMock.NonElevatedExamples.BasicUsage.RecursiveMocking
{
    /// <summary>
    /// Recursive mocks enable you to mock members that are obtained as a result of "chained" calls on a mock. 
    /// For example, recursive mocking is useful in the cases when you test code like this: foo.Bar.Baz.Do("x"). 
    /// See http://www.telerik.com/help/justmock/basic-usage-recursive-mocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class RecursiveMocking_Tests
    {
        [TestMethod]
        public void ShouldAssertNestedVeriables()
        {
            string pingArg = "ping";
            var expected = "test";

            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: When foo.Bar.Do() is called, it should return the expected string. 
            //              This will automatically create mock of foo.Bar and a NullReferenceException will be avoided.
            Mock.Arrange(() => foo.Bar.Do(pingArg)).Returns(expected);

            // ACT
            var actualFooBarDo = foo.Bar.Do(pingArg);

            // ASSERT
            Assert.AreEqual(expected, actualFooBarDo);
        }

        [TestMethod]
        public void ShouldInstantiateFooBar()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // ASSERT - Not arranged members in a RecursiveLoose mocks should not be null.
            Assert.IsNotNull(foo.Bar);
        }

        [TestMethod]
        public void ShouldAssertNestedPropertyGet()
        {
            var expected = 10;

            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: When foo.Bar.Value is called, it should return expected value. 
            //              This will automatically create mock of foo.Bar and a NullReferenceException will be avoided.
            Mock.Arrange(() => foo.Bar.Value).Returns(expected);

            // ACT
            var actual = foo.Bar.Value;

            // ASSERT
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldAssertNestedPropertySet()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: Setting foo.Bar.Value to 5, should do nothing. 
            //              This will automatically create mock of foo.Bar and a NullReferenceException will be avoided.
            Mock.ArrangeSet(() => { foo.Bar.Value = 5; }).DoNothing().MustBeCalled();

            // ACT
            foo.Bar.Value = 5;

            // ASSERT
            Mock.Assert(foo);
        }

        [TestMethod]
        public void NestedPropertyAndMethodCalls()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: When foo.Bar.Do() is called with "x" as an argument, it return "xit". 
            //              This will automatically create mock of foo.Bar and a NullReferenceException will be avoided.
            Mock.Arrange(() => foo.Bar.Do("x")).Returns("xit");

            // Arranging: When foo.Bar.Baz.Do() is called with "y" as an argument, it return "yit". 
            //              This will automatically create mock of foo.Bar and foo.Bar.Baz and a 
            //              NullReferenceException will be avoided.
            Mock.Arrange(() => foo.Bar.Baz.Do("y")).Returns("yit");

            // ACT
            var actualFooBarDo = foo.Bar.Do("x");
            var actualFooBarBazDo = foo.Bar.Baz.Do("y");

            // ASSERT
            Assert.AreEqual("xit", actualFooBarDo);
            Assert.AreEqual("yit", actualFooBarBazDo);
        }
    }

    #region SUT
    public interface IFoo
    {
        IBar Bar { get; set; }
        string Do(string command);
    }

    public interface IBar
    {
        int Value { get; set; }
        string Do(string command);
        IBaz Baz { get; set; }
    }

    public interface IBaz
    {
        string Do(string command);
    }
    #endregion
}
