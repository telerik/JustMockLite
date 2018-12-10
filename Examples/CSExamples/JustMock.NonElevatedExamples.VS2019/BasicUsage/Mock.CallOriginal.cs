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

namespace JustMock.NonElevatedExamples.BasicUsage.Mock_CallOriginal
{
    /// <summary>
    /// The CallOriginal method marks a mocked method/property call that should execute the original method/property implementation.
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-call-original.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Mock_CallOriginal_Tests
    {
        [TestMethod]
        public void ReturnSum_CallOriginal_ReturnsExpectedSum()
        {
            // ARRANGE
            // Creating a mock instance of the "Log" class.
            var log = Mock.Create<Log>();

            // Arranging when log.ReturnSum() is called with any integers as arguments it should execute its original implementation.
            Mock.Arrange(() => log.ReturnSum(Arg.AnyInt, Arg.AnyInt)).CallOriginal();

            // ACT - Calling log.ReturnSum with 1 and 2 as arguments.
            var actual = log.ReturnSum(1, 2);

            // We should expect 3 as a return value
            int expected = 3;

            // ASSERT - We are asserting that the expected and the actual values are equal.
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Info_CallOriginal_ThrowException()
        {
            // ARRANGE
            // Creating a mock instance of the "Log" class.
            var log = Mock.Create<Log>(); 
            
            // Arranging when log.Info() is called with any string as an argument it should execute its original implementation.
            Mock.Arrange(() => log.Info(Arg.IsAny<string>())).CallOriginal();

            // ACT
            log.Info("test");

            // ASSERT - We are asserting with the [ExpectedException(typeof(Exception))] test attribute.
        }
    }

    #region SUT
    public class Log
    {
        public virtual int ReturnSum(int firstInt, int secondInt)
        {
            return firstInt + secondInt;
        }

        public virtual void Info(string message)
        {
            throw new Exception(message);
        }
    } 
    #endregion
}
