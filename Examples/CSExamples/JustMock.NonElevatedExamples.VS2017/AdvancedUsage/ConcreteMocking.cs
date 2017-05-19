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

namespace JustMock.NonElevatedExamples.AdvancedUsage.ConcreteMocking
{
    /// <summary>
    /// Concrete mocking is one of the advanced features supported in Telerik JustMock. Up to this point we have been talking 
    /// mostly about mocking interfaces. This feature allows you to mock the creation of an object. To some extent this is available 
    /// in the free edition and there are more things you can do in the commercial edition of the product.
    /// See http://www.telerik.com/help/justmock/advanced-usage-concrete-mocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class ConcreteMocking_Tests
    {
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ShouldCallOriginalForVirtualMemberWithMockedConstructor()
        {
            // ARRANGE
            // Creating a mocked instance of the "FooVirtual" class.
            //  Telerik JustMock also gives you the ability to explicitly specify whether a constructor should be mocked or not.
            //  By default the constructor is not mocked.
            var foo = Mock.Create<FooVirtual>(Constructor.Mocked);

            // Arranging: When foo.GetList() is called, it should call the original method implementation.
            Mock.Arrange(() => foo.GetList()).CallOriginal();

            // ACT
            foo.GetList();
        }

        [TestMethod]
        public void VoidMethod_OnExcute_ShouldCallGetList()
        {
            // ARRANGE
            // Creating a mocked instance of the "FooVirtual" class.
            //  Telerik JustMock also gives you the ability to explicitly specify whether a constructor should be mocked or not.
            //  By default the constructor is not mocked.
            var foo = Mock.Create<FooVirtual>(Constructor.Mocked);

            // Arranging: When foo.VoidMethod() is called, it should call foo.GetList() instead.
            Mock.Arrange(() => foo.VoidMethod()).DoInstead(() => foo.GetList());
            // Arranging: That foo.GetList() must be called during the test method and it should do nothing.
            Mock.Arrange(() => foo.GetList()).DoNothing().MustBeCalled();

            // ACT
            foo.VoidMethod();

            // ASSERT
            Mock.Assert(foo);
        } 
    }

    #region SUT
    public class FooVirtual
    {
        public FooVirtual()
        {
            throw new NotImplementedException("Constructor");
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual void VoidMethod()
        {
            throw new NotImplementedException();
        }

        public virtual IList<int> GetList()
        {
            throw new NotImplementedException();
        }
    } 
    #endregion
}
