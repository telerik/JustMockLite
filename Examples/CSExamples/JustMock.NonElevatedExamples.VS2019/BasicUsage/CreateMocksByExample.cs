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

namespace JustMock.NonElevatedExamples.BasicUsage.CreateMocksByExample
{
    /// <summary>
    /// The built-in feature for creating mocks by example saves time when it comes to tiresome set up of arrangements. 
    /// This functionality allows you to create mocks of a certain class (the system under test) 
    ///  and in the same time to arrange their behavior.
    /// For simple tests with few arrangements, this provides only marginal benefit. 
    ///  The real benefit comes with complex tests with multiple arrangements
    /// See http://www.telerik.com/help/justmock/basic-usage-create-mocks-by-example.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class CreateMocksByExample_Tests
    {
        [TestMethod]
        public void ShouldUseMatchers()
        {
            // Create a mock and arrange the Equals method, when called with any arguments, 
            //  to forward the call to Object.Equals with the given arguments.
            Mock.CreateLike<IEqualityComparer>(
                cmp => cmp.Equals(Arg.AnyObject, Arg.AnyObject) == Object.Equals(Param._1, Param._2));
        }

        [TestMethod]
        public void SampleTest()
        {
            // Create a mock and arrange: 
            //  - the Driver property, should return "MSSQL" when called, 
            //  - the Open() method, when called with any string argument should return true.
            var conn = Mock.CreateLike<IConnection>(
                me => me.Driver == "MSSQL" && me.Open(Arg.AnyString) == true);
            
            // ASSERT
            Assert.AreEqual("MSSQL", conn.Driver);
            Assert.IsTrue(conn.Open(@".\SQLEXPRESS"));
        }

        [TestMethod]
        public void ShouldUseCreateLikeAlongWithStandartArrangements()
        {
            // Create a mock and arrange: 
            //  - the Driver property, should return "MSSQL" when called.
            var conn = Mock.CreateLike<IConnection>(me => me.Driver == "MSSQL");

            // Arranging: The Open() method must be called with any string argument and it should return true.
            Mock.Arrange(() => conn.Open(Arg.AnyString)).Returns(true).MustBeCalled();

            // ASSERT
            Assert.AreEqual("MSSQL", conn.Driver);
            Assert.IsTrue(conn.Open(@".\SQLEXPRESS"));
            Mock.Assert(conn);
        }
    }

    #region SUT
    public interface IEqualityComparer
    {
        bool Equals(object a, object b);
    }

    public interface IConnection
    {
        string Driver { get; }
        bool Open(string parameters);
    } 
    #endregion
}
