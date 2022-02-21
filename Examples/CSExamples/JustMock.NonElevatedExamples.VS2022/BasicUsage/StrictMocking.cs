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
using Telerik.JustMock.Core;

namespace JustMock.NonElevatedExamples.BasicUsage.StrictMocking
{
    /// <summary>
    /// You may have a case where you want to enable only arranged calls and to reject others. 
    /// In such cases you need to set the mock Behavior to Strict.
    /// See http://www.telerik.com/help/justmock/basic-usage-strict-mocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class StrictMocking_Tests
    {
        [ExpectedException(typeof(StrictMockException))]
        [TestMethod]
        public void ArbitraryCallsShouldGenerateException()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface with Behavior.Strict. 
            // This means, every non-arranged call from this instance will throw MockException.
            var foo = Mock.Create<IFoo>(Behavior.Strict);

            //ACT - As foo.VoidCall() is not arranged, it should throw an exception.
            foo.VoidCall();
        }

        [TestMethod]
        [ExpectedException(typeof(StrictMockException))]
        public void ArbitraryCallsShouldGenerateExpectedException()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface with Behavior.Strict. 
            // This means, every non-arranged call from this instance will throw MockException.
            var foo = Mock.Create<IFoo>(Behavior.Strict);

            //ACT - As foo.VoidCall() is not arranged, it should throw an exception.
            foo.GetGuid();
        }
    }

    #region SUT
    public interface IFoo
    {
        void VoidCall();
        Guid GetGuid();
    }
    #endregion
}
