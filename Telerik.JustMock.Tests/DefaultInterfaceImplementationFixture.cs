#if NETCORE
using System;

#if NUNIT
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestCategory = NUnit.Framework.CategoryAttribute;
#elif XUNIT
using TestClass = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestClassAttribute;
using TestMethod = Xunit.FactAttribute;
using TestCategory = Telerik.JustMock.XUnit.Test.Attributes.XUnitCategoryAttribute;
#else
using TestClass = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestMethod = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
using TestCategory = Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute;
#endif

namespace Telerik.JustMock.Tests
{
    [TestClass]
    public class DefaultInterfaceImplementationFixture
    {
        public interface IDefault
        {
            int GetValue() => 17;
            int Value => 19;
            int Read();
            void Write(int value);
            int Forwarded { get => Read(); set => Write(value); }
            int ReadTwice() => Read() + Read();
            int Sum(int remaining) => remaining == 0 ? 0 : Read() + Sum(remaining - 1);
            T Echo<T>(T value) where T : class => value;
            int Change(ref int value, out int copy)
            {
                value += 5;
                copy = value * 2;
                return copy + 1;
            }
            void Throw(Exception exception) => throw exception;
            void ChangeAndThrow(ref int value, Exception exception)
            {
                value = 29;
                throw exception;
            }
        }

        public interface IInherited : IDefault
        {
        }

        public interface IAnotherInherited : IDefault
        {
        }

        public interface IGenericDefault<T>
        {
            T Echo(T value) => value;
        }

        public class ClassOverride : IDefault
        {
            public virtual int GetValue() => 42;
            public virtual int Read() => 23;
            public virtual void Write(int value) { }
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldCallOriginalDefaultMethod()
        {
            var mock = Mock.Create<IDefault>(Behavior.Loose);
            Mock.Arrange(() => mock.GetValue()).CallOriginal();

            Assert.Equal(17, mock.GetValue());
            Mock.Assert(() => mock.GetValue(), Occurs.Once());
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldCallOriginalDefaultGetter()
        {
            var mock = Mock.Create<IDefault>(Behavior.Loose);
            Mock.Arrange(() => mock.Value).CallOriginal();

            Assert.Equal(19, mock.Value);
            Mock.Assert(() => mock.Value, Occurs.Once());
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldCallOriginalInheritedDefaultGetter()
        {
            IDefault mock = Mock.Create<IInherited>(Behavior.Loose);
            Mock.Arrange(() => mock.Value).CallOriginal();

            Assert.Equal(19, mock.Value);
            Mock.Assert(() => mock.Value, Occurs.Once());
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldUseCallOriginalBehaviorForDefaultMembers()
        {
            var mock = Mock.Create<IInherited>(Behavior.CallOriginal);

            Assert.Equal(17, mock.GetValue());
            Assert.Equal(19, mock.Value);
            Mock.Assert(() => mock.GetValue(), Occurs.Once());
            Mock.Assert(() => mock.Value, Occurs.Once());
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldKeepLooseDefaultsAndArrangedReturns()
        {
            var mock = Mock.Create<IDefault>(Behavior.Loose);
            Assert.Equal(0, mock.GetValue());
            Assert.Equal(0, mock.Value);

            Mock.Arrange(() => mock.GetValue()).Returns(101);
            Mock.Arrange(() => mock.Value).Returns(102);
            Assert.Equal(101, mock.GetValue());
            Assert.Equal(102, mock.Value);
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldNotShareCallbacksBetweenDifferentProxyTypes()
        {
            IDefault first = Mock.Create<IDefault>(Behavior.CallOriginal);
            IDefault second = Mock.Create<IInherited>(Behavior.CallOriginal);
            IDefault third = Mock.Create<IAnotherInherited>(Behavior.CallOriginal);
            IDefault fourth = Mock.Create<IDefault>(Behavior.CallOriginal);

            Assert.Equal(17, first.GetValue());
            Assert.Equal(19, second.Value);
            Assert.Equal(17, third.GetValue());
            Assert.Equal(19, fourth.Value);
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldCallGenericDefaultMethod()
        {
            var mock = Mock.Create<IDefault>(Behavior.Loose);
            var expected = new object();
            Mock.Arrange(() => mock.Echo(expected)).CallOriginal();
            Mock.Arrange(() => mock.Echo("text")).CallOriginal();

            Assert.Same(expected, mock.Echo(expected));
            Assert.Equal("text", mock.Echo("text"));
            Mock.Assert(() => mock.Echo(expected), Occurs.Once());
            Mock.Assert(() => mock.Echo("text"), Occurs.Once());
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldCallDefaultMethodOnClosedGenericInterfaces()
        {
            var first = Mock.Create<IGenericDefault<int>>(Behavior.CallOriginal);
            var second = Mock.Create<IGenericDefault<string>>(Behavior.CallOriginal);

            Assert.Equal(31, first.Echo(31));
            Assert.Equal("value", second.Echo("value"));
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldCopyDefaultMethodRefAndOutArguments()
        {
            var mock = Mock.Create<IDefault>(Behavior.CallOriginal);
            int value = 7;
            int copy;

            Assert.Equal(25, mock.Change(ref value, out copy));
            Assert.Equal(12, value);
            Assert.Equal(24, copy);
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldCopyRefArgumentWhenDefaultBodyThrows()
        {
            var mock = Mock.Create<IDefault>(Behavior.CallOriginal);
            var expected = new InvalidOperationException("default body");
            int value = 7;

            Assert.Same(expected, Assert.Throws<InvalidOperationException>(() => mock.ChangeAndThrow(ref value, expected)));
            Assert.Equal(29, value);
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldInterceptNestedCallsFromDefaultBody()
        {
            var mock = Mock.Create<IDefault>(Behavior.Loose);
            Mock.Arrange(() => mock.ReadTwice()).CallOriginal();
            Mock.Arrange(() => mock.Read()).Returns(11);

            Assert.Equal(22, mock.ReadTwice());
            Mock.Assert(() => mock.ReadTwice(), Occurs.Once());
            Mock.Assert(() => mock.Read(), Occurs.Exactly(2));
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldCallOriginalDefaultSetter()
        {
            var mock = Mock.Create<IDefault>(Behavior.Loose);
            Mock.ArrangeSet(() => mock.Forwarded = 23).CallOriginal();

            mock.Forwarded = 23;

            Mock.AssertSet(() => mock.Forwarded = 23, Occurs.Once());
            Mock.Assert(() => mock.Write(23), Occurs.Once());
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldDispatchRecursiveDefaultBodyCallsOnceEach()
        {
            var mock = Mock.Create<IDefault>(Behavior.CallOriginal);
            Mock.Arrange(() => mock.Read()).Returns(7);

            Assert.Equal(14, mock.Sum(2));
            Mock.Assert(() => mock.Sum(2), Occurs.Once());
            Mock.Assert(() => mock.Sum(1), Occurs.Once());
            Mock.Assert(() => mock.Sum(0), Occurs.Once());
            Mock.Assert(() => mock.Read(), Occurs.Exactly(2));
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldPreserveExceptionFromDefaultBody()
        {
            var mock = Mock.Create<IDefault>(Behavior.CallOriginal);
            var expected = new InvalidOperationException("default body");

            Assert.Same(expected, Assert.Throws<InvalidOperationException>(() => mock.Throw(expected)));
            Mock.Assert(() => mock.Throw(expected), Occurs.Once());
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldPreserveNotImplementedExceptionFromDefaultBody()
        {
            var mock = Mock.Create<IDefault>(Behavior.CallOriginal);
            var expected = new NotImplementedException("real default body");

            Assert.Same(expected, Assert.Throws<NotImplementedException>(() => mock.Throw(expected)));
            Mock.Assert(() => mock.Throw(expected), Occurs.Once());
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldKeepMissingOriginalDiagnosticForAbstractInterfaceMethod()
        {
            var mock = Mock.Create<IDefault>(Behavior.Loose);
            Mock.Arrange(() => mock.Read()).CallOriginal();

            var exception = Assert.Throws<NotImplementedException>(() => mock.Read());
            Assert.Equal("You can't call the original implementation of a method that does not have one (abstract or interface method).",
                exception.Message);
        }

        [TestMethod, TestCategory("Lite")]
        public void ShouldPreferClassOverrideToDefaultBody()
        {
            IDefault mock = Mock.Create<ClassOverride>(Behavior.CallOriginal);

            Assert.Equal(42, mock.GetValue());
            Mock.Arrange(() => mock.GetValue()).Returns(104);
            Assert.Equal(104, mock.GetValue());
        }
    }
}
#endif
