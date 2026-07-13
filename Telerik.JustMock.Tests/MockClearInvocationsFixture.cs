/*
 JustMock Lite
 Copyright (C) 2010-2024 Progress Software Corporation

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

#region JustMock Test Attributes
#if NUNIT
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using AssertionException = NUnit.Framework.AssertionException;
#elif XUNIT
using Xunit;
using Telerik.JustMock.XUnit.Test.Attributes;
using TestCategory = Telerik.JustMock.XUnit.Test.Attributes.XUnitCategoryAttribute;
using TestClass = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestClassAttribute;
using TestMethod = Xunit.FactAttribute;
using TestInitialize = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestInitializeAttribute;
using TestCleanup = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestCleanupAttribute;
using AssertionException = Telerik.JustMock.XUnit.AssertFailedException;
#elif VSTEST_PORTABLE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AssertionException = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssertFailedException;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#endif
#endregion

namespace Telerik.JustMock.Tests
{
    [TestClass]
    public class MockClearInvocationsFixture
    {
        public interface IService
        {
            void Execute();
            int GetValue();
            void Process(string input);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ClearInvocations")]
        public void ClearInvocations_ResetsCounterAndArrangementRemains()
        {
            var mock = Mock.Create<IService>();
            Mock.Arrange(() => mock.GetValue()).Returns(42);

            Assert.Equal(42, mock.GetValue());
            Mock.Assert(() => mock.GetValue(), Occurs.Once());

            Mock.ClearInvocations(mock);

            // Invocation counter reset — Occurs.Never() should now pass
            Mock.Assert(() => mock.GetValue(), Occurs.Never());
            // Arrangement still active — returns the arranged value
            Assert.Equal(42, mock.GetValue());
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ClearInvocations")]
        public void ClearInvocations_MultipleArrangementsAllReset()
        {
            var mock = Mock.Create<IService>();
            Mock.Arrange(() => mock.Execute()).DoNothing();
            Mock.Arrange(() => mock.GetValue()).Returns(7);

            mock.Execute();
            mock.Execute();
            mock.GetValue();

            Mock.ClearInvocations(mock);

            Mock.Assert(() => mock.Execute(), Occurs.Never());
            Mock.Assert(() => mock.GetValue(), Occurs.Never());
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ClearInvocations")]
        public void ClearInvocations_SiblingMockUnaffected()
        {
            var mockA = Mock.Create<IService>();
            var mockB = Mock.Create<IService>();

            Mock.Arrange(() => mockA.Execute()).DoNothing();
            Mock.Arrange(() => mockB.Execute()).DoNothing();

            mockA.Execute();
            mockB.Execute();
            mockB.Execute();

            Mock.ClearInvocations(mockA);

            // mockA reset
            Mock.Assert(() => mockA.Execute(), Occurs.Never());
            // mockB untouched
            Mock.Assert(() => mockB.Execute(), Occurs.Exactly(2));
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ClearInvocations")]
        public void ClearInvocations_GetTimesCalledReturnsZeroAfterClear()
        {
            var mock = Mock.Create<IService>();
            Mock.Arrange(() => mock.Execute()).DoNothing();

            mock.Execute();
            mock.Execute();
            Assert.Equal(2, Mock.GetTimesCalled(() => mock.Execute()));

            Mock.ClearInvocations(mock);

            Assert.Equal(0, Mock.GetTimesCalled(() => mock.Execute()));
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ClearInvocations")]
        public void ClearInvocations_SharedMockPhaseIsolation()
        {
            var mock = Mock.Create<IService>();
            Mock.Arrange(() => mock.Execute()).DoNothing();

            // Phase 1
            mock.Execute();
            Mock.Assert(() => mock.Execute(), Occurs.Once());

            Mock.ClearInvocations(mock);

            // Phase 2 — counter starts from zero again; arrangement still active
            Mock.Assert(() => mock.Execute(), Occurs.Never());
            mock.Execute();
            mock.Execute();
            Mock.Assert(() => mock.Execute(), Occurs.Exactly(2));
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ClearInvocations")]
        public void ClearInvocations_NullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Mock.ClearInvocations(null));
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ClearInvocations")]
        public void ClearInvocations_NonMockThrowsArgumentException()
        {
            var realObject = new object();
            Assert.Throws<ArgumentException>(() => Mock.ClearInvocations(realObject));
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ClearInvocations")]
        public void ClearInvocations_InOrderResetsSequenceTracking()
        {
            var mock = Mock.Create<IService>();
            Mock.Arrange(() => mock.Execute()).InOrder();
            Mock.Arrange(() => mock.GetValue()).InOrder();

            // Phase 1 — call in correct order
            mock.Execute();
            mock.GetValue();
            Mock.Assert(mock);

            Mock.ClearInvocations(mock);

            // Phase 2 — same order again should pass (sequence reset)
            mock.Execute();
            mock.GetValue();
            Mock.Assert(mock);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ClearInvocations")]
        public void ClearInvocations_InOrderWrongSequenceAfterClearFails()
        {
            var mock = Mock.Create<IService>();
            Mock.Arrange(() => mock.Execute()).InOrder();
            Mock.Arrange(() => mock.GetValue()).InOrder();

            mock.Execute();
            mock.GetValue();
            Mock.Assert(mock);

            Mock.ClearInvocations(mock);

            // Call in wrong order after clear
            mock.GetValue();
            mock.Execute();
            Assert.Throws<AssertionException>(() => Mock.Assert(mock));
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ClearInvocations")]
        public void ClearInvocations_InOrderPartialSequenceAfterClear()
        {
            var mock = Mock.Create<IService>();
            Mock.Arrange(() => mock.Execute()).InOrder().OccursOnce();
            Mock.Arrange(() => mock.GetValue()).InOrder().OccursOnce();
            Mock.Arrange(() => mock.Process(Arg.AnyString)).InOrder().OccursOnce();

            mock.Execute();
            mock.GetValue();
            mock.Process("test");
            Mock.Assert(mock);

            Mock.ClearInvocations(mock);

            // After clear, only call first two — third should fail occurs check
            mock.Execute();
            mock.GetValue();
            Assert.Throws<AssertionException>(() => Mock.Assert(mock));
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ClearInvocations")]
        public void ClearInvocations_InOrderMultipleClearCycles()
        {
            var mock = Mock.Create<IService>();
            Mock.Arrange(() => mock.Execute()).InOrder();
            Mock.Arrange(() => mock.GetValue()).InOrder();

            for (int i = 0; i < 3; i++)
            {
                mock.Execute();
                mock.GetValue();
                Mock.Assert(mock);
                Mock.ClearInvocations(mock);
            }

            // After final clear, no invocations
            Mock.Assert(() => mock.Execute(), Occurs.Never());
            Mock.Assert(() => mock.GetValue(), Occurs.Never());
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ClearInvocations")]
        public void ClearInvocations_InOrderSiblingMockSequenceUnaffected()
        {
            var mockA = Mock.Create<IService>();
            var mockB = Mock.Create<IService>();

            Mock.Arrange(() => mockA.Execute()).InOrder();
            Mock.Arrange(() => mockA.GetValue()).InOrder();

            Mock.Arrange(() => mockB.Execute()).InOrder();
            Mock.Arrange(() => mockB.GetValue()).InOrder();

            mockA.Execute();
            mockA.GetValue();
            mockB.Execute();
            mockB.GetValue();

            Mock.ClearInvocations(mockA);

            // mockA sequence reset
            Mock.Assert(() => mockA.Execute(), Occurs.Never());
            // mockB sequence intact
            Mock.Assert(mockB);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ResetInstance")]
        public void ResetInstance_RemovesArrangementsFromMock()
        {
            var mock = Mock.Create<IService>();
            Mock.Arrange(() => mock.GetValue()).Returns(99);

            Assert.Equal(99, mock.GetValue());

            Mock.Reset(mock);

            // Arrangement gone — default value returned
            Assert.Equal(0, mock.GetValue());
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ResetInstance")]
        public void ResetInstance_ClearsInvocationsAlongWithArrangements()
        {
            var mock = Mock.Create<IService>();
            Mock.Arrange(() => mock.Execute()).DoNothing();

            mock.Execute();

            Mock.Reset(mock);

            Mock.Assert(() => mock.Execute(), Occurs.Never());
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ResetInstance")]
        public void ResetInstance_SiblingMockArrangementsUnaffected()
        {
            var mockA = Mock.Create<IService>();
            var mockB = Mock.Create<IService>();

            Mock.Arrange(() => mockA.GetValue()).Returns(1);
            Mock.Arrange(() => mockB.GetValue()).Returns(2);

            Mock.Reset(mockA);

            // mockA arrangement gone
            Assert.Equal(0, mockA.GetValue());
            // mockB still arranged
            Assert.Equal(2, mockB.GetValue());
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ResetInstance")]
        public void ResetInstance_MockRemainsUsableAfterReset()
        {
            var mock = Mock.Create<IService>();
            Mock.Arrange(() => mock.GetValue()).Returns(5);

            Mock.Reset(mock);

            // Can re-arrange after reset
            Mock.Arrange(() => mock.GetValue()).Returns(10);
            Assert.Equal(10, mock.GetValue());
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ResetInstance")]
        public void ResetInstance_NullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Mock.Reset((object)null));
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ResetInstance")]
        public void ResetInstance_NonMockThrowsArgumentException()
        {
            var realObject = new object();
            Assert.Throws<ArgumentException>(() => Mock.Reset(realObject));
        }
    }
}
