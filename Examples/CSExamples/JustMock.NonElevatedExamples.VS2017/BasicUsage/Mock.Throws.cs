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

namespace JustMock.NonElevatedExamples.BasicUsage.Mock_Throws
{
    /// <summary>
    /// The Throws method is used to throw an exception when a given call is made.
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-throws.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Mock_Throws_Tests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowExceptionOnMethodCall()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: When foo.Execute() is called with any string as an argument, it should throw an ArgumentException.
            Mock.Arrange(() => foo.Execute(Arg.AnyString)).Throws<ArgumentException>();

            // ACT
            foo.Execute(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowExceptionWithArgumentsOnMethodCall()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: When foo.Execute() is called with an empty string as an argument, 
            //  it should throw an ArgumentException with args: "Argument shouldn't be empty.".
            Mock.Arrange(() => foo.Execute(string.Empty)).Throws<ArgumentException>("Argument shouldn't be empty.");

            // ACT
            foo.Execute(string.Empty);
        }
    }

    #region SUT
    public interface IFoo
    {
        string Execute(string myStr);
    }
    #endregion
}
