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

namespace JustMock.NonElevatedExamples.BasicUsage.Generics
{
    /// <summary>
    /// Telerik JustMock allows you to mock generic classes/interfaces/methods in the same way 
    /// as you do it for non-generic ones.
    /// See http://www.telerik.com/help/justmock/basic-usage-generics.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Generics_Tests
    {
        [TestMethod]
        public void ShouldDistinguishVirtualCallsDependingOnArgumentTypes()
        {
            int expextedCallWithInt = 0;
            int expextedCallWithString = 1;

            // ARRANGE
            // Creating a mock instance of the "FooGeneric" class.
            var foo = Mock.Create<FooGeneric>();

            // Arranging: When foo.Get<int>() generic is called, it should return expextedCallWithInt.
            Mock.Arrange(() => foo.Get<int>()).Returns(expextedCallWithInt);
            // Arranging: When foo.Get<string>() generic is called, it should return expextedCallWithString.
            Mock.Arrange(() => foo.Get<string>()).Returns(expextedCallWithString);

            // ACT
            var actualCallWithInt = foo.Get<int>();
            var actualCallWithString = foo.Get<string>();

            // ASSERT
            Assert.AreEqual(expextedCallWithInt, actualCallWithInt);
            Assert.AreEqual(expextedCallWithString, actualCallWithString);
        }

        [TestMethod]
        public void ShouldMockGenericClass()
        {
            int expectedValue = 1;

            // ARRANGE
            // Creating a mock instance of the "FooGeneric<T>" class.
            var foo = Mock.Create<FooGeneric<int>>();

            // Arranging: When foo.Get() is called with any integer as an argument, it should return expectedValue.
            Mock.Arrange(() => foo.Get(Arg.IsAny<int>())).Returns(expectedValue);

            // ACT
            int actualValue = foo.Get(0);

            // ASSERT
            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void ShouldMockGenericMethod()
        {
            var expectedValue = 10;

            // ARRANGE
            // Creating a mock instance of the "FooGeneric<T>" class.
            var genericClass = Mock.Create<FooGeneric<int>>();

            // Arranging: When genericClass.Get() is called with 1, 1 as arguments, it should return expectedValue.
            Mock.Arrange(() => genericClass.Get(1, 1)).Returns(expectedValue);

            // ACT
            var actual = genericClass.Get(1, 1);

            // ASSERT
            Assert.AreEqual(expectedValue, actual);
        }

        [TestMethod]
        public void ShouldMockMethodInGenericClass()
        {
            bool isCalled = false;

            // ARRANGE
            // Creating a mock instance of the "FooGeneric<T>" class.
            var genericClass = Mock.Create<FooGeneric<int>>();

            // Arranging: When genericClass.Execute() is called with 1 as an argument, it should notify for call.
            Mock.Arrange(() => genericClass.Execute(1)).DoInstead(() => isCalled = true);

            // ACT
            genericClass.Execute(1);

            // ASSERT
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public void ShouldMockVirtualGenericMethodInNonGenericClass()
        {
            var expectedValue = 10;

            // ARRANGE
            // Creating a mock instance of the "FooGeneric" class.
            var genericClass = Mock.Create<FooGeneric>();

            // Arranging: When genericClass.Get<int, int>() is called with 1 as an argument, it should return expectedValue.
            Mock.Arrange(() => genericClass.Get<int, int>(1)).Returns(expectedValue);

            // ACT
            var actual = genericClass.Get<int, int>(1);

            // ASSERT
            Assert.AreEqual(expectedValue, actual);
        }
    }

    #region SUT
    public class FooGeneric
    {
        public virtual TRet Get<T, TRet>(T arg1)
        {
            return default(TRet);
        }

        public virtual int Get<T>()
        {
            throw new NotImplementedException();
        }
    }

    public class FooGeneric<T>
    {
        public virtual T Get(T arg)
        {
            throw new NotImplementedException();
        }
        public virtual T Get(T arg, T arg2)
        {
            throw new NotImplementedException();
        }

        public virtual void Execute<T1>(T1 arg)
        {
            throw new Exception();
        }
    }
    #endregion
}
