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
using System.Reflection;
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
        [TestMethod, TestCategory("ConstructorResolution"), TestCategory("Constructor")]
        public void ShouldPreferExactBaseConstructorOverObjectOverload()
        {
            var ctor = ResolveBaseConstructorForTest(typeof(RankedBaseCtorTarget), "value");

            Assert.Equal(typeof(string), ctor.GetParameters()[0].ParameterType);
        }

        [TestMethod, TestCategory("ConstructorResolution"), TestCategory("Constructor")]
        public void ShouldPreferMoreSpecificBaseConstructorForNull()
        {
            var ctor = ResolveBaseConstructorForTest(typeof(RankedBaseCtorTarget), new object[] { null });

            Assert.Equal(typeof(string), ctor.GetParameters()[0].ParameterType);
        }

        [TestMethod, TestCategory("ConstructorResolution"), TestCategory("Constructor")]
        public void ShouldPreferTypedMatcherOverNullPlaceholderSpecificity()
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var objectCtor = typeof(RankedBaseCtorTarget).GetConstructor(flags, null, new[] { typeof(object) }, null);
            var stringCtor = typeof(RankedBaseCtorTarget).GetConstructor(flags, null, new[] { typeof(string) }, null);
            var accessor = PrivateAccessor.ForType(typeof(Mock));

            Assert.True((bool)accessor.CallMethod("IsBetterConstructorMatch",
                objectCtor, stringCtor, new object[] { null }, new[] { typeof(object) }));
            Assert.False((bool)accessor.CallMethod("IsBetterConstructorMatch",
                stringCtor, objectCtor, new object[] { null }, new[] { typeof(object) }));
        }

        [TestMethod, TestCategory("ConstructorResolution"), TestCategory("Constructor")]
        public void ShouldPreferInterfaceBaseConstructorOverObjectOverload()
        {
            var ctor = ResolveBaseConstructorForTest(typeof(InterfaceRankedBaseCtorTarget), "value");

            Assert.Equal(typeof(IComparable), ctor.GetParameters()[0].ParameterType);
        }

        [TestMethod, TestCategory("ConstructorResolution"), TestCategory("Constructor")]
        public void ShouldPreferExactNumericBaseConstructorOverWidening()
        {
            var ctor = ResolveBaseConstructorForTest(typeof(NumericRankedBaseCtorTarget), 42);

            Assert.Equal(typeof(int), ctor.GetParameters()[0].ParameterType);
        }

        [TestMethod, TestCategory("ConstructorResolution"), TestCategory("Constructor")]
        public void ShouldPreferNarrowerCompatibleNumericBaseConstructor()
        {
            var ctor = ResolveBaseConstructorForTest(typeof(NumericWideningBaseCtorTarget), 42);

            Assert.Equal(typeof(long), ctor.GetParameters()[0].ParameterType);
        }

        [TestMethod, TestCategory("ConstructorResolution"), TestCategory("Constructor")]
        public void ShouldKeepUnrelatedInterfaceBaseConstructorsAmbiguous()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                ResolveBaseConstructorForTest(typeof(AmbiguousBaseCtorTarget), "value"));

            Assert.True(exception.InnerException is MockException);
            Assert.True(exception.InnerException.Message.Contains("Ambiguous constructor"));
        }

        [TestMethod, TestCategory("ConstructorResolution"), TestCategory("Constructor")]
        public void ShouldKeepConflictingParameterPreferencesAmbiguous()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                ResolveBaseConstructorForTest(typeof(CrossRankedBaseCtorTarget), "first", "second"));

            Assert.True(exception.InnerException is MockException);
            Assert.True(exception.InnerException.Message.Contains("Ambiguous constructor"));
        }

        private static ConstructorInfo ResolveBaseConstructorForTest(Type type, params object[] args)
        {
            return (ConstructorInfo)PrivateAccessor.ForType(typeof(Mock))
                .CallMethod("ResolveBaseConstructor", type, args);
        }

        public class RankedBaseCtorTarget
        {
            protected RankedBaseCtorTarget(object value) { }
            protected RankedBaseCtorTarget(string value) { }
        }

        public class InterfaceRankedBaseCtorTarget
        {
            protected InterfaceRankedBaseCtorTarget(object value) { }
            protected InterfaceRankedBaseCtorTarget(IComparable value) { }
        }

        public class NumericRankedBaseCtorTarget
        {
            protected NumericRankedBaseCtorTarget(long value) { }
            protected NumericRankedBaseCtorTarget(int value) { }
        }

        public class NumericWideningBaseCtorTarget
        {
            protected NumericWideningBaseCtorTarget(double value) { }
            protected NumericWideningBaseCtorTarget(long value) { }
        }

        public class CrossRankedBaseCtorTarget
        {
            protected CrossRankedBaseCtorTarget(string first, object second) { }
            protected CrossRankedBaseCtorTarget(object first, string second) { }
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldPreferExactBaseConstructorWithTypedMatcher()
        {
            Mock.ArrangeBaseConstructor<RankedBaseCtorTarget>(Arg.IsAny<string>()).OccursOnce();

            new RankedBaseCtorDerived("value");

            Mock.AssertBaseConstructor<RankedBaseCtorTarget>(Arg.IsAny<string>());
        }

        public class RankedBaseCtorDerived : RankedBaseCtorTarget
        {
            public RankedBaseCtorDerived(string value) : base(value) { }
        }

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
        public void ShouldCallOriginalBaseConstructorWhenCallOriginalIsArranged()
        {
            BaseCtorTarget.CallCount = 0;
            Mock.ArrangeBaseConstructor<BaseCtorTarget>().CallOriginal().OccursOnce();

            new BaseCtorDerived();

            Assert.Equal(1, BaseCtorTarget.CallCount);
            Mock.AssertBaseConstructor<BaseCtorTarget>();
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldRunCallbackOnceWhenCallingOriginalBaseConstructor()
        {
            BaseCtorTarget.CallCount = 0;
            var callbackCount = 0;
            Mock.ArrangeBaseConstructor<BaseCtorTarget>()
                .DoInstead(() => callbackCount++)
                .CallOriginal()
                .OccursOnce();

            new BaseCtorDerived();

            Assert.Equal(1, callbackCount);
            Assert.Equal(1, BaseCtorTarget.CallCount);
            Mock.AssertBaseConstructor<BaseCtorTarget>();
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldCallBaseConstructorForEachDerivedConstruction()
        {
            BaseCtorTarget.CallCount = 0;
            Mock.ArrangeBaseConstructor<BaseCtorTarget>().Occurs(2);

            new BaseCtorDerived();
            new BaseCtorDerived();

            Assert.Equal(2, BaseCtorTarget.CallCount);
            Mock.AssertBaseConstructor<BaseCtorTarget>();
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldFailBaseConstructorAssertionWhenOccurrenceIsNotMet()
        {
            Mock.ArrangeBaseConstructor<BaseCtorTarget>().Occurs(2);

            new BaseCtorDerived();

            Assert.Throws<AssertionException>(() => Mock.AssertBaseConstructor<BaseCtorTarget>());
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldReportForbiddenBaseConstructorCall()
        {
            Mock.ArrangeBaseConstructor<BaseCtorTarget>().OccursNever();

            Assert.Throws<AssertionException>(() => new BaseCtorDerived());
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldResolveBaseConstructorUsingReferenceTypeMatcher()
        {
            Mock.ArrangeBaseConstructor<OverloadedBaseCtorTarget>(Arg.IsAny<string>()).OccursOnce();

            new OverloadedBaseCtorDerived("value");

            Mock.AssertBaseConstructor<OverloadedBaseCtorTarget>(Arg.IsAny<string>());
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldResolveOverloadedBaseConstructorWithSpecificMatcherType()
        {
            Mock.ArrangeBaseConstructor<OverloadedBaseCtorTarget>(Arg.IsAny<Uri>()).OccursOnce();

            new OverloadedBaseCtorDerived(new Uri("https://example.com"));

            Mock.AssertBaseConstructor<OverloadedBaseCtorTarget>(Arg.IsAny<Uri>());
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldNotMatchBaseConstructorWithDifferentArgument()
        {
            Mock.ArrangeBaseConstructor<ArgumentBaseCtorTarget>(42).OccursOnce();

            new ArgumentBaseCtorDerived(43);

            Assert.Throws<AssertionException>(() =>
                Mock.AssertBaseConstructor<ArgumentBaseCtorTarget>(42));
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldResolveImplicitNumericConstructorConversion()
        {
            var arrangement = Mock.ArrangeBaseConstructor<NumericBaseCtorTarget>(42);

            Assert.NotNull(arrangement);
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldApplyBaseConstructorArrangementAcrossDerivedTypes()
        {
            Mock.ArrangeBaseConstructor<SharedBaseCtorTarget>().Occurs(2);

            new FirstSharedBaseCtorDerived();
            new SecondSharedBaseCtorDerived();

            Mock.AssertBaseConstructor<SharedBaseCtorTarget>();
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldSuppressReachableConstructorsWhenIntermediateBodyIsSuppressed()
        {
            InheritanceBaseCtorTarget.GrandparentCallCount = 0;
            InheritanceBaseCtorTarget.ParentCallCount = 0;
            Mock.ArrangeBaseConstructor<InheritanceParentCtorTarget>().DoNothing().OccursOnce();

            new InheritanceDerived();

            Assert.Equal(0, InheritanceBaseCtorTarget.GrandparentCallCount);
            Assert.Equal(0, InheritanceBaseCtorTarget.ParentCallCount);
            Mock.AssertBaseConstructor<InheritanceParentCtorTarget>();
        }

        [TestMethod, TestCategory("Elevated"), TestCategory("Constructor")]
        public void ShouldReportWhenBaseConstructorOverloadDoesNotExist()
        {
            var exception = Assert.Throws<MockException>(() =>
                Mock.ArrangeBaseConstructor<CleanupBaseCtorTarget>("wrong"));

            Assert.True(exception.Message.Contains("No constructor"));
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

            public OverloadedBaseCtorDerived(Uri value)
                : base(value)
            {
            }
        }

        public class ArgumentBaseCtorTarget
        {
            protected ArgumentBaseCtorTarget(int value)
            {
            }
        }

        public class ArgumentBaseCtorDerived : ArgumentBaseCtorTarget
        {
            public ArgumentBaseCtorDerived(int value)
                : base(value)
            {
            }
        }

        public class NumericBaseCtorTarget
        {
            protected NumericBaseCtorTarget(long value)
            {
            }
        }

        public class NumericBaseCtorDerived : NumericBaseCtorTarget
        {
            public NumericBaseCtorDerived(long value)
                : base(value)
            {
            }
        }

        public class SharedBaseCtorTarget
        {
            protected SharedBaseCtorTarget()
            {
            }
        }

        public class FirstSharedBaseCtorDerived : SharedBaseCtorTarget
        {
        }

        public class SecondSharedBaseCtorDerived : SharedBaseCtorTarget
        {
        }

        public class InheritanceBaseCtorTarget
        {
            public static int GrandparentCallCount;
            public static int ParentCallCount;

            protected InheritanceBaseCtorTarget()
            {
                GrandparentCallCount++;
            }
        }

        public class InheritanceParentCtorTarget : InheritanceBaseCtorTarget
        {
            protected InheritanceParentCtorTarget()
            {
                ParentCallCount++;
            }
        }

        public class InheritanceDerived : InheritanceParentCtorTarget
        {
        }

        public class AmbiguousBaseCtorTarget
        {
            protected AmbiguousBaseCtorTarget(IComparable value)
            {
            }

            protected AmbiguousBaseCtorTarget(ICloneable value)
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
