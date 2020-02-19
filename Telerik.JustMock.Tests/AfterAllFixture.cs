/*
 JustMock Lite
 Copyright © 2019 Progress Software Corporation

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
    public class AfterAllFixture
    {
        [TestMethod, TestCategory("Lite"), TestCategory("AfterAll")]
        public void ShouldAssertAfterAllWithPrerequisites()
        {
            var foo = Mock.Create<IFoo>();

            var init = Mock.Arrange(() => foo.Init());
            Mock.ArrangeSet(() => foo.Value = Arg.AnyInt).AfterAll(init);
            Mock.Arrange(() => foo.Save()).AfterAll(init);

            foo.Init();
            foo.Value = 5;
            foo.Save();

            Mock.AssertAll(foo);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("AfterAll")]
        public void ShouldThrowAfterAllWithoutPrerequisites()
        {
            var foo = Mock.Create<IFoo>();

            var init = Mock.Arrange(() => foo.Init());
            Mock.ArrangeSet(() => foo.Value = Arg.AnyInt).AfterAll(init);
            Mock.Arrange(() => foo.Save()).AfterAll(init);

            foo.Value = 5;
            foo.Save();

            Assert.Throws<AssertionException>(() => Mock.AssertAll(foo));
        }

        [TestMethod, TestCategory("Lite"), TestCategory("AfterAll")]
        public void ShouldAssertAfterAllWithPrerequisitesOrdered()
        {
            var foo = Mock.Create<IFoo>();

            var init = Mock.Arrange(() => foo.Init());
            Mock.ArrangeSet(() => foo.Value = Arg.AnyInt).AfterAll(init).InOrder();
            Mock.Arrange(() => foo.Save()).AfterAll(init).InOrder();

            foo.Init();
            foo.Value = 5;
            foo.Save();

            Mock.AssertAll(foo);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("AfterAll")]
        public void ShouldThrowAfterAllWithoutPrerequisitesOrdered()
        {
            var foo = Mock.Create<IFoo>();

            var init = Mock.Arrange(() => foo.Init());
            Mock.ArrangeSet(() => foo.Value = Arg.AnyInt).AfterAll(init).InOrder();
            Mock.Arrange(() => foo.Save()).AfterAll(init).InOrder();

            foo.Value = 5;
            foo.Save();

            Assert.Throws<AssertionException>(() => Mock.AssertAll(foo));
        }

        

        [TestMethod, TestCategory("Lite"), TestCategory("AfterAll")]
        public void ShouldAssertAfterAllInderectChainedWithPrerequisites()
        {
            var foo = Mock.Create<IFoo>();
            var bar = Mock.Create<IBar>();

            var fooInit = Mock.Arrange(() => foo.Init());
            var barInit = Mock.Arrange(() => bar.Init());
            Mock.ArrangeSet(() => bar.Foo = Arg.IsAny<IFoo>()).AfterAll(fooInit).AfterAll(barInit);

            foo.Init();
            bar.Init();
            bar.Foo = foo;

            Mock.AssertAll(bar);
        }

        [TestMethod, TestCategory("Lite"), TestCategory("AfterAll")]
        public void ShouldThrowAfterAllInderectChainedWithPartialPrerequisites()
        {
            var foo = Mock.Create<IFoo>();
            var bar = Mock.Create<IBar>();

            var fooInit = Mock.Arrange(() => foo.Init());
            var barInit = Mock.Arrange(() => bar.Init());
            Mock.ArrangeSet(() => bar.Foo = Arg.IsAny<IFoo>()).AfterAll(fooInit).AfterAll(barInit);

            bar.Init();
            bar.Foo = foo;

            Assert.Throws<AssertionException>(() => Mock.AssertAll(bar));
        }

        public interface IFoo
        {
            void Init();
            int Value { get; set; }
            void Save();
        }

        public interface IBar
        {
            void Init();
            IFoo Foo { get; set; }
        }

        public interface IFooContainer
        {
            IEnumerable<IFoo> Values { get; }
        }
    }
}
