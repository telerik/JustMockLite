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
using Telerik.JustMock.Helpers;

namespace JustMock.NonElevatedExamples.BasicUsage.FluentMocking
{
    /// <summary>
    /// Fluent Assertions allow you to easily follow the Arrange Act Assert pattern in a straigtforward way.
    /// Note that JustMock dynamically checks for any assertion mechanism provided by the underlying test framework 
    /// if such one is available (MSTest, XUnit, NUnit, MbUnit, Silverlight) and uses it, rather than using its own 
    /// MockAssertionException when a mock assertion fails. This functionality extends the JustMock tooling support 
    /// for different test runners. 
    /// See http://www.telerik.com/help/justmock/basic-usage-fluent-mocking.html for full documentation of the feature.
    /// 
    /// Note: To write in a fluent way, you will need to have the Telerik.JustMock.Helpers namespace included. 
    /// </summary>
    [TestClass]
    public class FluentMocking_Tests
    {
        [TestMethod]
        public void ShouldBeAbleToAssertSpecificFuntionForASetup()
        {
            var expected = @"c:\JustMock";

            // ARRANGE
            // Creating a mocked instance of the "IFileReader" interface.
            var fileReader = Mock.Create<IFileReader>();

            // Arranging: When fileReader.Path_GET is called, it should return expected.
            fileReader.Arrange(x => x.Path).Returns(expected).OccursOnce();

            // ACT
            var actual = fileReader.Path;

            // ASSERT
            Assert.AreEqual(expected, actual);
            fileReader.Assert(x => x.Path);
        }
    }

    #region SUT
    public interface IFileReader
    {
        string Path { get; set; }

        void Initialize();
    }
    #endregion
}
