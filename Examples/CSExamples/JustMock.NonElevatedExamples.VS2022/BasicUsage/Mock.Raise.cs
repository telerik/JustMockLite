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

namespace JustMock.NonElevatedExamples.BasicUsage.Mock_Raise
{
    /// <summary>
    /// The Raise method is used for raising mocked events. You can use custom or standard events.
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-raise.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Mock_Raise_Tests
    {
        [TestMethod]
        public void ShouldInvokeMethodForACustomEventWhenRaised()
        {
            // ARRANGE
            // Creating a mocked instance of the "IFoo" interface.
            var foo = Mock.Create<IFoo>();

            string expected = "ping";
            string actual = string.Empty;

            foo.CustomEvent += delegate(string exp)
            {
                actual = exp;
            };

            // ACT
            Mock.Raise(() => foo.CustomEvent += null, expected);

            // ASSERT
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldRaiseEventWithStandardEventArgs()
        {
            // ARRANGE
            // Creating a mocked instance of the "IExecutor" interface.
            var executor = Mock.Create<IExecutor<int>>();

            string acutal = null;
            string expected = "ping";

            executor.Done += delegate(object sender, FooArgs args)
            {
                acutal = args.Value;
            };

            // ACT - Raising the event with the expected args.
            Mock.Raise(() => executor.Done += null, new FooArgs(expected));

            // ASSERT
            Assert.AreEqual(expected, acutal);
        }

        [TestMethod]
        public void Submit_OnIsChangedRaised_ShouldBeCalled()
        {
            // ARRANGE
            // Creating the necessary mocked instances.
            var activeDocument = Mock.Create<IDocument>();
            var activeView = Mock.Create<IDocumentView>();

            // Attaching the Submit method to the IsChanged event. 
            activeDocument.IsChanged += new EventHandler(activeDocument.Submit);

            // Arranging: activeDocument.Submit should be called during the test method with any arguments.
            Mock.Arrange(() => activeDocument.Submit(Arg.IsAny<object>(), Arg.IsAny<EventArgs>())).MustBeCalled();
            // Arranging: activeView.Document should return activeDocument when called.
            Mock.Arrange(() => activeView.Document).Returns(activeDocument);

            // ACT
            Mock.Raise(() => activeView.Document.IsChanged += null, EventArgs.Empty);

            // ASSERT - Asserting all arrangements on "foo".
            Mock.Assert(activeDocument);
        }
    }

    #region SUT
    public interface IFoo
    {
        event CustomEvent CustomEvent;
    }

    public delegate void CustomEvent(string value);

    public interface IExecutor<T>
    {
        event EventHandler<FooArgs> Done;
    }

    public class FooArgs : EventArgs
    {
        public FooArgs()
        {
        }

        public FooArgs(string value)
        {
            this.Value = value;
        }

        public string Value { get; set; }
    }

    public interface IDocumentView
    {
        IDocument Document { get; }
    }

    public interface IDocument
    {
        event EventHandler IsChanged;
        void Submit(object sender, EventArgs e);
    }
    #endregion
}
