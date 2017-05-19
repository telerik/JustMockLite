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
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.AutoMock;

namespace JustMock.NonElevatedExamples.BasicUsage.Automocking
{
    /// <summary>
    /// Automocking allows the developer to create an instance of a class (the system under test) without having 
    /// to explicitly create each individual dependency as a unique mock. The mocked dependencies are still available 
    /// to the developer if methods or properties need to be arranged as part of the test. 
    /// See http://www.telerik.com/help/justmock/basic-usage-automocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Automocking_Tests
    {
        [TestMethod]
        public void ShouldMockDependenciesWithContainer()
        {
            // ARRANGE
            // Creating a MockingContainer of ClassUnderTest. 
            // To instantiate the system uder test (the container) you should use the Instance property 
            // For example: container.Instance. 
            var container = new MockingContainer<ClassUnderTest>();

            string expectedString = "Test";

            // Arranging: When the GetString() method from the ISecondDependecy interface 
            //              is called from the container, it should return expectedString. 
            container.Arrange<ISecondDependency>(
               secondDep => secondDep.GetString()).Returns(expectedString);

            // ACT - Calling SringMethod() from the mocked instance of ClassUnderTest
            var actualString = container.Instance.StringMethod();

            // ASSERT
            Assert.AreEqual(expectedString, actualString);
        }

        [TestMethod]
        public void ShouldAssertAllContainerArrangments()
        {
            // ARRANGE
            // Creating a MockingContainer of ClassUnderTest. 
            // To instantiate the system uder test (the container) you should use the Instance property 
            // For example: container.Instance. 
            var container = new MockingContainer<ClassUnderTest>();
             
            // Arranging: That the GetString() method from the ISecondDependecy interface 
            //              must be called from the container instance durring the test method. 
            container.Arrange<ISecondDependency>(
               secondDep => secondDep.GetString()).MustBeCalled();

            // ACT - Calling SringMethod() from the mocked instance of ClassUnderTest
            var actualString = container.Instance.StringMethod();

            // ASSERT - Asserting all expectations for the container.
            container.AssertAll();
        }
    }

    #region SUT
    public class ClassUnderTest
    {
        private IFirstDependency firstDep;
        private ISecondDependency secondDep;

        public ClassUnderTest(IFirstDependency first, ISecondDependency second)
        {
            this.firstDep = first;
            this.secondDep = second;
        }

        public IList<object> CollectionMethod()
        {
            var firstCollection = firstDep.GetList();

            return firstCollection;
        }

        public string StringMethod()
        {
            var secondString = secondDep.GetString();

            return secondString;
        }
    }

    public interface IFirstDependency
    {
        IList<object> GetList();
    }

    public interface ISecondDependency
    {
        string GetString();
    }
    #endregion
}
