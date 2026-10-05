/*
 JustMock Lite
 Copyright © 2010-2015 Progress Software Corporation

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
using Telerik.JustMock.Core;

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
    public class ConstructorFixture
    {
        [TestMethod, TestCategory("Lite"), TestCategory("Constructor")]
        public void ShouldCallBaseCtorWhenNotMocked()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Mock.Create<Foo>(Constructor.NotMocked);
            });
        }

#if LITE_EDITION
        [TestMethod, TestCategory("Lite"), TestCategory("Constructor")]
        public void ShouldNotExposeBaseConstructorApiInLiteEdition()
        {
            var publicMethods = typeof(Mock).GetMethods();

            Assert.False(Array.Exists(publicMethods, method =>
                method.Name == "ArrangeBaseConstructor" || method.Name == "AssertBaseConstructor"));
        }
#endif

#if !(COREFX && LITE_EDITION)
        [TestMethod, TestCategory("Lite"), TestCategory("Constructor")]
#if SILVERLIGHT
        [Ignore, Description("SL instance constructor mocking not implemented")]
#endif
        public void ShouldSkipBaseConstructorWhenMocked()
        {
            Assert.NotNull(Mock.Create<Foo>());
        }

        [TestMethod, TestCategory("Lite"), TestCategory("Constructor")]
#if SILVERLIGHT
        [Ignore, Description("SL instance constructor mocking not implemented")]
#endif
        public void ShouldSkipBaseConstructorOfAbstractClassWhenMocked()
        {
            Assert.NotNull(Mock.Create<AbstractFoo>());
        }
#endif

#if !LITE_EDITION
        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldCreateMockForFrameWorkClassWithInternalCtor()
        {
            var downloadDateCompleted = Mock.Create<System.IO.IsolatedStorage.IsolatedStorageFile>();
            Assert.NotNull(downloadDateCompleted != null);
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldFutureMockConstructorWithArg()
        {
            long? arg = null;
            Mock.Arrange(() => new CtorLongArg(Arg.AnyLong)).DoInstead<long>(x => arg = x);

            new CtorLongArg(100);
            Assert.True(arg.Value == 100);
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldCallBaseConstructorByDefaultWhenOnlyOccurrenceIsArranged()
        {
            BaseCtorTarget.CallCount = 0;
            Mock.ArrangeBaseConstructor<BaseCtorTarget>().OccursOnce();

            new BaseCtorDerived();

            Assert.Equal(1, BaseCtorTarget.CallCount);
            Mock.AssertBaseConstructor<BaseCtorTarget>();
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldSuppressBaseConstructorWhenDoNothingIsArranged()
        {
            BaseCtorTarget.CallCount = 0;
            Mock.ArrangeBaseConstructor<BaseCtorTarget>().DoNothing().OccursOnce();

            new BaseCtorDerived();

            Assert.Equal(0, BaseCtorTarget.CallCount);
            Mock.AssertBaseConstructor<BaseCtorTarget>();
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldResolveBaseConstructorUsingReferenceTypeMatcher()
        {
            Mock.ArrangeBaseConstructor<OverloadedBaseCtorTarget>(Arg.IsAny<string>()).OccursOnce();

            new OverloadedBaseCtorDerived("value");

            Mock.AssertBaseConstructor<OverloadedBaseCtorTarget>(Arg.IsAny<string>());
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldClearMatcherContextAfterBaseConstructorResolutionFails()
        {
            Assert.Throws<MockException>(() =>
                Mock.ArrangeBaseConstructor<AmbiguousBaseCtorTarget>(Arg.IsAny<string>()));

            Mock.ArrangeBaseConstructor<CleanupBaseCtorTarget>(42).OccursOnce();
            new CleanupBaseCtorDerived(42);

            Mock.AssertBaseConstructor<CleanupBaseCtorTarget>(42);
        }

        public class BaseCtorTarget
        {
            public static int CallCount;

            protected BaseCtorTarget()
            {
                CallCount++;
            }
        }

        public class BaseCtorDerived : BaseCtorTarget
        {
        }

        public class OverloadedBaseCtorTarget
        {
            protected OverloadedBaseCtorTarget(string value)
            {
            }

            protected OverloadedBaseCtorTarget(Uri value)
            {
            }
        }

        public class OverloadedBaseCtorDerived : OverloadedBaseCtorTarget
        {
            public OverloadedBaseCtorDerived(string value)
                : base(value)
            {
            }
        }

        public class AmbiguousBaseCtorTarget
        {
            protected AmbiguousBaseCtorTarget(string value)
            {
            }

            protected AmbiguousBaseCtorTarget(object value)
            {
            }
        }

        public class CleanupBaseCtorTarget
        {
            protected CleanupBaseCtorTarget(int value)
            {
            }
        }

        public class CleanupBaseCtorDerived : CleanupBaseCtorTarget
        {
            public CleanupBaseCtorDerived(int value)
                : base(value)
            {
            }
        }

#endif

        public class Foo
        {
            public Foo()
            {
                throw new ArgumentException("Failed");
            }
        }

        public abstract class AbstractFoo
        {
            public AbstractFoo()
            {
                throw new ArgumentException("Failed");
            }

            public abstract int Id { get; set; }
        }

        [TestMethod, TestCategory("Lite"), TestCategory("Constructor")]
        public void ShouldCallConstructorRequiringPrimitiveArgumentConversions()
        {
            Mock.Create<CtorLongArg>(Behavior.CallOriginal, 0);
        }

        public class CtorLongArg
        {
            public CtorLongArg(long l) { }
        }

        public class CallsCtor
        {
            public bool ok;

            public CallsCtor()
            {
                ok = true;
            }
        }

        [TestMethod, TestCategory("Lite"), TestCategory("Constructor")]
        public void ShouldCallDefaultConstructorWhenExplicitlyGivenNoArguments()
        {
            var mock = Mock.Create<CallsCtor>(new object[0]);
            Assert.True(mock.ok);
        }
    }
}
