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

namespace JustMock.NonElevatedExamples.BasicUsage.Mock_Raises
{
    /// <summary>
    /// The Raises method is used to fire an event once a method is called.
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-raises.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Mock_Raises_Tests
    {
        [TestMethod]
        public void ShouldRaiseCustomEventOnMethodCall()
        {
            var expected = "ping";
            string actual = string.Empty;

            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: When foo.RaiseMethod() is called, it should raise foo.CustomEvent with expected args.
            Mock.Arrange(() => foo.RaiseMethod()).Raises(() => foo.CustomEvent += null, expected);
            foo.CustomEvent += (s) => { actual = s; };

            // ACT
            foo.RaiseMethod();

            // ASSERT
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldRaiseCustomEventForFuncCalls()
        {
            bool isRaised = false;

            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: When foo.Echo() is called with expected arguments, it should raise foo.EchoEvent with arg: true.
            Mock.Arrange(() => foo.Echo("string")).Raises(() => foo.EchoEvent += null, true);
            foo.EchoEvent += (c) => { isRaised = c; };

            // ACT
            var actual = foo.Echo("string");

            // ASSERT
            Assert.IsTrue(isRaised);
        }

        [TestMethod]
        public void ShouldAssertMultipleEventSubscription()
        {
            bool isRaisedForFirstSubscr = false;
            bool isRaisedForSecondSubscr = false;

            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: When foo.Execute() is called, it should raise foo.EchoEvent with arg: true.
            Mock.Arrange(() => foo.Execute()).Raises(() => foo.EchoEvent += null, true);

            // Subscribing for the event
            foo.EchoEvent += c => { isRaisedForFirstSubscr = c; };
            foo.EchoEvent += c => { isRaisedForSecondSubscr = c; };
            
            // ACT
            foo.Execute();

            // ASSERT
            Assert.IsTrue(isRaisedForFirstSubscr);
            Assert.IsTrue(isRaisedForSecondSubscr);
        }

    }

    #region SUT
    public delegate void CustomEvent(string value);
    
    public delegate void EchoEvent(bool echoed); 

    public interface IFoo
    {
        event CustomEvent CustomEvent;
        event EchoEvent EchoEvent;
        void RaiseMethod();
        string Echo(string arg);
        void Execute();
    }
    #endregion
}
