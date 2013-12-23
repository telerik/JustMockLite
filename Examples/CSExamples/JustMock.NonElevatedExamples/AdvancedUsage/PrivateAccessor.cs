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

namespace JustMock.NonElevatedExamples.AdvancedUsage.PrivateAccessorNamespace
{
    /// <summary>
    /// The Telerik JustMock PrivateAccessor allows you to call non-public members of the tested code right in your unit tests. 
    /// See http://www.telerik.com/help/justmock/advanced-usage-private-accessor.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class PrivateAccessor_Tests
    {
        [TestMethod]
        public void PrivateAccessor_ShouldCallPrivateMethod()
        {
            // ACT
            // Wrapping the instance holding the private method.
            var inst = new PrivateAccessor(new ClassWithNonPublicMembers());
            // Calling the non-public method by giving its exact name.
            var actual = inst.CallMethod("MePrivate");

            // ASSERT
            Assert.AreEqual(1000, actual);
        }

        [TestMethod]
        public void PrivateAccessor_ShouldCallPrivateStaticMethod()
        {
            // ACT
            // Wrapping the instance holding the private method by type.
            var inst = PrivateAccessor.ForType(typeof(ClassWithNonPublicMembers));
            // Calling the non-public static method by giving its exact name.
            var actual = inst.CallMethod("MeStaticPrivate");

            // ASSERT
            Assert.AreEqual(2000, actual);
        }

        [TestMethod]
        public void PrivateAccessor_ShouldGetSetProperty()
        {
            // ACT
            // Wrapping the instance holding the private property.
            var inst = new PrivateAccessor(new ClassWithNonPublicMembers());
            // Setting the value of the private property.
            inst.SetProperty("Prop", 555);

            // ASSERT - Asserting with getting the value of the private property.
            Assert.AreEqual(555, inst.GetProperty("Prop"));
        } 
    }

    #region SUT
    public class ClassWithNonPublicMembers
    {
        private int Prop { get; set; }

        private int MePrivate()
        {
            return 1000;
        }

        private static int MeStaticPrivate()
        {
            return 2000;
        }
    } 
    #endregion
}
