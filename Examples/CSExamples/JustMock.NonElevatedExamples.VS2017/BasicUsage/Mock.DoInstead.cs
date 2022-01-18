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

namespace JustMock.NonElevatedExamples.BasicUsage.Mock_DoInstead
{
    /// <summary>
    /// The DoInstead method is used to replace the actual implementation of a method with a mocked one.
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-do-instead.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Mock_DoInstead_Tests
    {
        [TestMethod]
        public void Execute_OnExecuteWithAnyStringArg_ShouldNotifyAndReturnThatArg()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            bool called = false;

            // Arranging: When foo.Execute() is called with any string as an argument it should change "called" to true and also return that argument.
            Mock.Arrange(() => foo.Execute(Arg.IsAny<string>()))
                      .DoInstead(() => { called = true; })
                      .Returns((string s) => s);

            // ACT 
            var actual = foo.Execute("bar");

            // ASSERT
            Assert.AreEqual("bar", actual);
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void Submit_OnExecuteWitAnyIntArgs_ShouldAssignTheirSumToVariable()
        {
            // Arrange
            int expected = 0;

            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            // Arranging: When foo.Submit() is called with any integers as an arguments it should assign their sum to the "expected" variable.
            Mock.Arrange(() => foo.Submit(Arg.IsAny<int>(), Arg.IsAny<int>(), Arg.IsAny<int>(), Arg.IsAny<int>()))
                .DoInstead((int arg1, int arg2, int arg3, int arg4) => { expected = arg1 + arg2 + arg3 + arg4; });

            // Act
            foo.Submit(10, 10, 10, 10);

            // Assert
            Assert.AreEqual(40, expected);
        }

        [TestMethod]
        public void Bar_OnSetTo1_ShouldNotify()
        {
            // Arrange
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            bool isSetTo1 = false;

            // Arranging: When foo.Bar is set to 1 it should change "isSetTo1" to true.
            Mock.ArrangeSet<IFoo>(() => { foo.Bar = 1; }).DoInstead(() => isSetTo1 = true);

            // Act
            foo.Bar = 1;

            // Assert
            Assert.IsTrue(isSetTo1);
        }

        [TestMethod]
        public void AddTo_OnCertainCall_ShouldSumTheArgsAndAssignInsidedTheRefArg()
        {
            // Arrange
            int refArg = 1;

            // Creating a mocked instance of the "DoInsteadWithCustomDelegate" class.
            var mock = Mock.Create<DoInsteadWithCustomDelegate>();

            // Arranging: When mock.AddTo is called with 10 and "refArg" it should assign their sum to the second argument (refArg).
            Mock.Arrange(() => mock.AddTo(10, ref refArg)).DoInstead(new RefAction<int, int>((int arg1, ref int arg2) =>
            {
                arg2 += arg1;
            }));

            // Act
            mock.AddTo(10, ref refArg);

            // Assert
            Assert.AreEqual(11, refArg);
        }
    }

    #region SUT
    public interface IFoo
    {
        int Bar { get; set; }
        string Execute(string str);
        int Submit(int arg1, int arg2, int arg3, int arg4);
    }

    public delegate void RefAction<T1, T2>(T1 arg1, ref T2 arg2);

    public class DoInsteadWithCustomDelegate
    {
        public virtual void AddTo(int arg1, ref int arg2)
        {
        }
    }
    #endregion
}
