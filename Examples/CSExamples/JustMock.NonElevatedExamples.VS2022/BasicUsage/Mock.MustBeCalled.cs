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

namespace JustMock.NonElevatedExamples.BasicUsage.Mock_MustBeCalled
{
    /// <summary>
    /// <FEATURE HELP>
    /// The MustBeCalled method is used to assert that a call to a given method or property is made during the execution of a test.
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-must-be-called.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Mock_MustBeCalled_Tests
    {
        [TestMethod]
        public void Value_OnSetTo1_ShouldBeCalled()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: foo.Value should be set to 1 during the test.
            Mock.ArrangeSet<IFoo>(() => { foo.Value = 1; }).MustBeCalled();
            
            // ACT
            foo.Value = 1;

            // ASSERT - Asserting all arrangements on "foo".
            Mock.Assert(foo);
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void Value_NotSet_ShouldNotBeCalled()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: foo.Value should be set to 1 during the test.
            Mock.ArrangeSet<IFoo>(() => { foo.Value = 1; }).MustBeCalled();

            // ACT

            // ASSERT - Asserting all arrangements on "foo".
            Mock.Assert(foo);
        }

        [TestMethod]
        public void Execute_OnArrangedExecuteTestCall_ShouldBeCalled()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: foo.Test() should be called during the test.
            Mock.Arrange(() => foo.Test()).MustBeCalled();
            // Arranging: When foo.ExecuteTest() is called, it should call foo.Test() instead.
            Mock.Arrange(() => foo.ExecuteTest()).DoInstead(() => foo.Test());

            // ACT
            foo.ExecuteTest();

            // ASSERT - Asserting all arrangements on "foo".
            Mock.Assert(foo);
        }
    }

    #region SUT
    public interface IFoo
    {
        int Value { get; set; }
        void ExecuteTest();
        void Test();
    }
    #endregion
}
