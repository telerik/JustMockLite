/*
 JustMock Lite
 Copyright © 2022 Progress Software Corporation

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
using System.Linq;
using Telerik.JustMock.Core;
using Telerik.JustMock.Helpers;
using System.Threading.Tasks;
using System.Security.Policy;
using Telerik.JustMock.AutoMock;


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
    public interface ITaskAsync
    {
        Task<int> GenericTaskWithValueReturnType();
        Task<int> GenericTaskWithValueReturnTypeAndOneParam(int value);
        Task<object> GenericTaskWithObjectReturnType();
        Task<object> GenericTaskWithObjectReturnTypeAndOneParam(object value);
    }

    public class TaskClient
    {
        private ITaskAsync task;

        public TaskClient(ITaskAsync t)
        {
            task = t;
        }

        public async Task<int> TaskUsageWithValue()
        {
            return await task.GenericTaskWithValueReturnTypeAndOneParam(10);
        }
    }


#if NETCORE
    public interface IValueTaskAsync
    {
        ValueTask<int> GenericTaskWithValueReturnType();
        ValueTask<int> GenericTaskWithValueReturnTypeAndOneParam(int value);
        ValueTask<object> GenericTaskWithObjectReturnType();
        ValueTask<object> GenericTaskWithObjectReturnTypeAndOneParam(object value);
    }
#endif

    [TestClass]
    public class ReturnAsyncFixture
    {
        private class Foo { }

        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task ShouldReturnValueWithTastResultInAsyncTest()
        {
            int expected = 10;
            var mock = Mock.Create<ITaskAsync>();
            Mock.Arrange(() => mock.GenericTaskWithValueReturnType()).Returns(Task.FromResult(expected));

            var result = await mock.GenericTaskWithValueReturnType();

            Assert.Equal(expected, result);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task ShouldReturnAsyncValueWithInAsyncTest()
        {
            int expected = 10;
            var mock = Mock.Create<ITaskAsync>();
            Mock.Arrange(() => mock.GenericTaskWithValueReturnType()).ReturnsAsync(expected);

            var result = await mock.GenericTaskWithValueReturnType();

            Assert.Equal(expected, result);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task ShouldReturnAsyncValueForGenericTaskWithOneParam()
        {
            int expected = 10;
            var mock = Mock.Create<ITaskAsync>();
            Mock.Arrange(() => mock.GenericTaskWithValueReturnTypeAndOneParam(Arg.AnyInt)).ReturnsAsync(expected);

            var result = await mock.GenericTaskWithValueReturnTypeAndOneParam(20);

            Assert.Equal(expected, result);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task ShouldReturnAsyncValueForGenericTaskWithObjectReturnTypeAndNullAsReturnValue()
        {
            var mock = Mock.Create<ITaskAsync>();
            Mock.Arrange(() => mock.GenericTaskWithObjectReturnType()).ReturnsAsync(null);

            var result = await mock.GenericTaskWithObjectReturnType();

            Assert.Null(result);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task ShouldReturnAsyncValueForGenericTaskWithObjectReturnType()
        {
            Foo expected = new Foo();
            var mock = Mock.Create<ITaskAsync>();
            Mock.Arrange(() => mock.GenericTaskWithObjectReturnType()).ReturnsAsync(expected);

            var result = await mock.GenericTaskWithObjectReturnType();

            Assert.Same(expected, result);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task ShouldReturnAsyncValueForGenericTaskWithObjectReturnTypeAndOneParamAndNullAsReturnValue()
        {
            var mock = Mock.Create<ITaskAsync>();
            Mock.Arrange(() => mock.GenericTaskWithObjectReturnTypeAndOneParam(Arg.AnyInt)).ReturnsAsync(null);

            var result = await mock.GenericTaskWithObjectReturnTypeAndOneParam(20);

            Assert.Null(result);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task ShouldReturnAsyncValueForGenericTaskWithObjectReturnTypeAndOneParam()
        {
            Foo expected = new Foo();
            var mock = Mock.Create<ITaskAsync>();
            Mock.Arrange(() => mock.GenericTaskWithObjectReturnTypeAndOneParam(Arg.AnyInt)).ReturnsAsync(expected);

            var result = await mock.GenericTaskWithObjectReturnTypeAndOneParam(20);

            Assert.Same(expected, result);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task TestTaskAutomockingReturnsAsyncResult()
        {
            // Arange
            var container = new MockingContainer<TaskClient>();
            var expectedResult = 10;

            container.Arrange<ITaskAsync>(t => t.GenericTaskWithValueReturnTypeAndOneParam(Arg.AnyInt)).ReturnsAsync(expectedResult);

            // Act
            var actualResult = await container.Instance.TaskUsageWithValue();

            // Assert
            Assert.Equals(expectedResult, actualResult);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task TestTaskAutomockingAsyncResultWithIntParameter()
        {
            // Arange
            var container = new MockingContainer<TaskClient>();
            var expectedResult = 10;

            container.Arrange<ITaskAsync, int>(t => t.GenericTaskWithValueReturnTypeAndOneParam(Arg.AnyInt)).ReturnsAsync(expectedResult);

            // Act
            var actualResult = await container.Instance.TaskUsageWithValue();

            // Assert
            Assert.Equals(expectedResult, actualResult);
        }

#if NETCORE
        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task ShouldReturnAsyncValueForGenericValueTaskWithinAsyncTest()
        {
            int expected = 10;
            var mock = Mock.Create<IValueTaskAsync>();
            Mock.Arrange(() => mock.GenericTaskWithValueReturnType()).ReturnsAsync(expected);

            var result = await mock.GenericTaskWithValueReturnType();

            Assert.Equal(expected, result);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task ShouldReturnAsyncValueForGenericValueTaskWithOneParam()
        {
            int expected = 10;
            var mock = Mock.Create<IValueTaskAsync>();
            Mock.Arrange(() => mock.GenericTaskWithValueReturnTypeAndOneParam(Arg.AnyInt)).ReturnsAsync(expected);

            var result = await mock.GenericTaskWithValueReturnTypeAndOneParam(20);

            Assert.Equal(expected, result);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task ShouldReturnAsyncValueForGenericValueTaskWithObjectReturnTypeAndNullAsReturnValue()
        {
            var mock = Mock.Create<IValueTaskAsync>();
            Mock.Arrange(() => mock.GenericTaskWithObjectReturnType()).ReturnsAsync(null);

            var result = await mock.GenericTaskWithObjectReturnType();

            Assert.Null(result);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task ShouldReturnAsyncValueForGenericValueTaskWithObjectReturnType()
        {
            Foo expected = new Foo();
            var mock = Mock.Create<IValueTaskAsync>();
            Mock.Arrange(() => mock.GenericTaskWithObjectReturnType()).ReturnsAsync(expected);

            var result = await mock.GenericTaskWithObjectReturnType();

            Assert.Same(expected, result);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task ShouldReturnAsyncValueForGenericValueTaskWithObjectReturnTypeAndOneParamAndNullAsReturnValue()
        {
            var mock = Mock.Create<IValueTaskAsync>();
            Mock.Arrange(() => mock.GenericTaskWithObjectReturnTypeAndOneParam(Arg.AnyInt)).ReturnsAsync(null);

            var result = await mock.GenericTaskWithObjectReturnTypeAndOneParam(20);

            Assert.Null(result);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task ShouldReturnAsyncValueForGenericValueTaskWithObjectReturnTypeAndOneParam()
        {
            Foo expected = new Foo();
            var mock = Mock.Create<IValueTaskAsync>();
            Mock.Arrange(() => mock.GenericTaskWithObjectReturnTypeAndOneParam(Arg.AnyInt)).ReturnsAsync(expected);

            var result = await mock.GenericTaskWithObjectReturnTypeAndOneParam(20);

            Assert.Same(expected, result);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("ReturnsAsync")]
        public async Task TestValueTaskAutomockingAsyncResultWithIntParameter()
        {
            // Arange
            var container = new MockingContainer<TaskClient>();
            var expectedResult = 10;

            container.Arrange<IValueTaskAsync, int>(t => t.GenericTaskWithValueReturnTypeAndOneParam(Arg.AnyInt)).ReturnsAsync(expectedResult);

            // Act
            var actualResult = await container.Instance.TaskUsageWithValue();

            // Assert
            Assert.Equals(expectedResult, actualResult);
        }
#endif
    }
}
