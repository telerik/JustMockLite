using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Behaviors;

#region JustMock Test Attributes
#if NUNIT
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif XUNIT
using Xunit;
using Telerik.JustMock.XUnit.Test.Attributes;
using TestCategory = Telerik.JustMock.XUnit.Test.Attributes.XUnitCategoryAttribute;
using TestClass = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestClassAttribute;
using TestMethod = Xunit.FactAttribute;
#elif VSTEST_PORTABLE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
#endregion

namespace Telerik.JustMock.Tests
{
#if !LITE_EDITION && !PORTABLE
    [TestClass]
    public class SetupFutureFixture
    {
        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldAssociateFutureMixinOnlyWithConstructorInvocations()
        {
            var existing = new FutureTarget();
            var repository = CurrentRepository();
            Register(repository, typeof(FutureTarget), Behavior.Loose);

            var oldCall = CreateInvocation(existing, typeof(FutureTarget).GetMethod("GetValue"));
            Assert.Null(Mixin(oldCall));
            Assert.Null(Lookup(existing));

            var created = new FutureTarget();
            var constructor = CreateInvocation(created, typeof(FutureTarget).GetConstructor(Type.EmptyTypes));
            Assert.NotNull(Mixin(constructor));
            Assert.True(ReferenceEquals(Mixin(constructor), Lookup(created)));
            Assert.Null(Lookup(existing));
        }

        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldDispatchFutureNewobjCallbackWithoutRunningTheConstructor()
        {
            Register(CurrentRepository(), typeof(FutureTarget), Behavior.RecursiveLoose);
            var constructor = typeof(FutureTarget).GetConstructor(Type.EmptyTypes);
            FutureTarget.ConstructorCalls = 0;

            var future = (FutureTarget)InvokeProfiler("InterceptNewobj", typeof(FutureTarget).TypeHandle,
                constructor.MethodHandle, new object[0]);

            Assert.NotNull(future);
            Assert.Equal(0, FutureTarget.ConstructorCalls);
            Assert.Equal(0, future.ConstructorValue);
            Assert.NotNull(Lookup(future));
            Assert.Equal(0, ReturnValue(Dispatch(CurrentRepository(), future, typeof(FutureTarget).GetMethod("GetValue"))));
        }

        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldBindTheActualObjectWhenOriginalConstructorInterceptionIsSkipped()
        {
            Register(CurrentRepository(), typeof(FutureTarget), Behavior.CallOriginal);
            var constructor = typeof(FutureTarget).GetConstructor(Type.EmptyTypes);
            var profiler = typeof(Mock).Assembly.GetType("Telerik.JustMock.Core.ProfilerInterceptor", true);
            try
            {
                Assert.Null(InvokeProfiler("InterceptNewobj", typeof(FutureTarget).TypeHandle,
                    constructor.MethodHandle, new object[0]));
                var actual = (FutureTarget)FormatterServices.GetUninitializedObject(typeof(FutureTarget));
                Assert.Null(Lookup(actual));

                var callOriginal = (bool)InvokeProfiler("InterceptCall", typeof(FutureTarget).TypeHandle,
                    constructor.MethodHandle, new object[] { actual, null });

                Assert.True(callOriginal);
                Assert.NotNull(Lookup(actual));
                Assert.False(Lookup(actual).IsInstanceConstructorMocked);
                Assert.Equal(typeof(FutureTarget), Lookup(actual).DeclaringType);
                Assert.True(CallOriginal(Dispatch(CurrentRepository(), actual, typeof(FutureTarget).GetMethod("GetValue"))));
            }
            finally
            {
                PrivateAccessor.ForType(profiler).SetField("skipMethodInterceptionOnce", null);
            }
        }

        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldCreateIndependentBehaviorsAndPreserveRuntimeType()
        {
            var repository = CurrentRepository();
            Register(repository, typeof(FutureTarget), Behavior.Loose);
            var first = Bind(repository, new EqualFutureTarget());
            var second = Bind(repository, new EqualFutureTarget());

            Assert.False(ReferenceEquals(first, second));
            Assert.False(ReferenceEquals(first.FallbackBehaviors[0], second.FallbackBehaviors[0]));
            Assert.Equal(typeof(EqualFutureTarget), first.DeclaringType);
            Assert.Equal(typeof(EqualFutureTarget), second.DeclaringType);
            Assert.True(first.IsInstanceConstructorMocked);
        }

        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldIsolatePropertyAndIndexerStateForEqualInstances()
        {
            var repository = CurrentRepository();
            Register(repository, typeof(EqualFutureTarget), Behavior.Loose);
            var first = new EqualFutureTarget();
            var second = new EqualFutureTarget();
            Assert.True(first.Equals(second));
            Assert.False(ReferenceEquals(first, second));
            Bind(repository, first);
            Bind(repository, second);

            var property = typeof(FutureTarget).GetProperty("Value");
            var indexer = typeof(FutureTarget).GetProperty("Item");
            for (int value = 1; value <= 3; value++)
            {
                Dispatch(repository, first, property.GetSetMethod(), value);
                Dispatch(repository, second, property.GetSetMethod(), value + 10);
                Dispatch(repository, first, indexer.GetSetMethod(), 7, value + 20);
                Dispatch(repository, second, indexer.GetSetMethod(), 7, value + 30);

                Assert.Equal(value, ReturnValue(Dispatch(repository, first, property.GetGetMethod())));
                Assert.Equal(value + 10, ReturnValue(Dispatch(repository, second, property.GetGetMethod())));
                Assert.Equal(value + 20, ReturnValue(Dispatch(repository, first, indexer.GetGetMethod(), 7)));
                Assert.Equal(value + 30, ReturnValue(Dispatch(repository, second, indexer.GetGetMethod(), 7)));
                Assert.Equal(0, ReturnValue(Dispatch(repository, first, indexer.GetGetMethod(), 8)));
            }
        }

        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldIsolateEventAdditionRemovalAndRaising()
        {
            var repository = CurrentRepository();
            Register(repository, typeof(EqualFutureTarget), Behavior.Loose);
            var first = new EqualFutureTarget();
            var second = new EqualFutureTarget();
            var firstMixin = Bind(repository, first);
            var secondMixin = Bind(repository, second);
            var evt = typeof(FutureTarget).GetEvent("Changed");
            int calls = 0;
            EventHandler handler = (sender, args) => calls++;

            Dispatch(repository, first, evt.GetAddMethod(), handler);
            ((IEventsMixin)secondMixin).RaiseEvent(evt, new object[] { second, EventArgs.Empty });
            Assert.Equal(0, calls);
            Dispatch(repository, second, evt.GetRemoveMethod(), handler);
            ((IEventsMixin)firstMixin).RaiseEvent(evt, new object[] { first, EventArgs.Empty });
            Assert.Equal(1, calls);

            Dispatch(repository, second, evt.GetAddMethod(), handler);
            Dispatch(repository, first, evt.GetRemoveMethod(), handler);
            ((IEventsMixin)firstMixin).RaiseEvent(evt, new object[] { first, EventArgs.Empty });
            Assert.Equal(1, calls);
            ((IEventsMixin)secondMixin).RaiseEvent(evt, new object[] { second, EventArgs.Empty });
            Assert.Equal(2, calls);
        }

        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldRespectConstructorBehaviorDuringDispatch()
        {
            var repository = CurrentRepository();
            foreach (var behavior in new[] { Behavior.RecursiveLoose, Behavior.Loose, Behavior.Strict, Behavior.CallOriginal })
            {
                Register(repository, typeof(FutureTarget), behavior);
                var target = new FutureTarget();
                var invocation = CreateInvocation(target, typeof(FutureTarget).GetConstructor(Type.EmptyTypes));
                Invoke(repository, "DispatchInvocation", invocation);

                Assert.Equal(behavior == Behavior.CallOriginal, CallOriginal(invocation));
                Assert.Equal(behavior != Behavior.CallOriginal, Mixin(invocation).IsInstanceConstructorMocked);
            }
        }

        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldPreserveStrictAndCallOriginalMethodBehavior()
        {
            var repository = CurrentRepository();
            Register(repository, typeof(FutureTarget), Behavior.Strict);
            var strict = new FutureTarget();
            Bind(repository, strict);
            Assert.Throws<StrictMockException>(() =>
                Dispatch(repository, strict, typeof(FutureTarget).GetMethod("GetValue")));

            Register(repository, typeof(FutureTarget), Behavior.CallOriginal);
            var original = new FutureTarget();
            Bind(repository, original);
            Assert.True(CallOriginal(Dispatch(repository, original, typeof(FutureTarget).GetMethod("GetValue"))));
        }

        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldPreferNearestRegistrationAndKeepExistingInstanceBehavior()
        {
            var repository = CurrentRepository();
            Register(repository, typeof(FutureTarget), Behavior.Loose);
            var inherited = Bind(repository, new EqualFutureTarget());
            Register(repository, typeof(EqualFutureTarget), Behavior.CallOriginal);
            var overridden = Bind(repository, new EqualFutureTarget());

            Assert.True(inherited.IsInstanceConstructorMocked);
            Assert.False(overridden.IsInstanceConstructorMocked);
            Assert.Equal(typeof(EqualFutureTarget), inherited.DeclaringType);
            Assert.Equal(typeof(EqualFutureTarget), overridden.DeclaringType);
        }

        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldPreserveExplicitMocksDuringConstructionLookup()
        {
            var explicitMock = Mock.Create<VirtualFutureTarget>();
            var existingMixin = Lookup(explicitMock);
            Register(CurrentRepository(), typeof(VirtualFutureTarget), Behavior.CallOriginal);
            var constructor = CreateInvocation(explicitMock, typeof(VirtualFutureTarget).GetConstructor(Type.EmptyTypes));

            Assert.True(ReferenceEquals(existingMixin, Mixin(constructor)));
        }

        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldInheritRegistrationsAndRestoreParentDefaultsOnChildReset()
        {
            var parent = CreateRepository(null);
            MocksRepository child = null;
            try
            {
                Register(parent, typeof(FutureTarget), Behavior.Loose);
                child = CreateRepository(parent);
                Assert.Equal(Behavior.Loose, Registrations(child)[typeof(FutureTarget)]);
                Register(child, typeof(FutureTarget), Behavior.CallOriginal);
                Assert.Equal(Behavior.Loose, Registrations(parent)[typeof(FutureTarget)]);
                Assert.False(Bind(child, new FutureTarget()).IsInstanceConstructorMocked);
                Assert.True(Bind(parent, new FutureTarget()).IsInstanceConstructorMocked);

                Invoke(child, "Reset");

                Assert.Equal(Behavior.Loose, Registrations(child)[typeof(FutureTarget)]);
                Assert.True(Bind(child, new FutureTarget()).IsInstanceConstructorMocked);
                Assert.Equal(Behavior.Loose, Registrations(parent)[typeof(FutureTarget)]);
            }
            finally
            {
                if (child != null)
                    Invoke(child, "Reset");
                Invoke(parent, "Reset");
            }
        }

        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldRemoveFutureRegistrationsAndInstanceAssociationsOnReset()
        {
            var repository = CreateRepository(null);
            var target = new FutureTarget();
            try
            {
                Register(repository, typeof(FutureTarget), Behavior.Loose);
                Bind(repository, target);
                Assert.NotNull(Lookup(target));

                Invoke(repository, "Reset");

                Assert.Equal(0, Registrations(repository).Count);
                Assert.Null(Lookup(target));
                Assert.Null(Invoke(repository, "CreateFutureMockMixin", new FutureTarget()));
            }
            finally
            {
                Invoke(repository, "Reset");
            }
        }

        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldNotReactivateInheritedFutureSetupsWhenChildIsRetired()
        {
            var parent = CreateRepository(null);
            MocksRepository child = null;
            try
            {
                Register(parent, typeof(FutureTarget), Behavior.Loose);
                child = CreateRepository(parent);

                Invoke(child, "Retire");

                Assert.Equal(0, Registrations(child).Count);
                Assert.Equal(Behavior.Loose, Registrations(parent)[typeof(FutureTarget)]);
            }
            finally
            {
                if (child != null)
                    Invoke(child, "Retire");
                Invoke(parent, "Reset");
            }
        }

        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldSynchronizeResetWithTheRegistrationLock()
        {
            var repository = CreateRepository(null);
            var registrations = Registrations(repository);
            using (var started = new ManualResetEventSlim())
            {
                Task reset = null;
                try
                {
                    lock (registrations)
                    {
                        Register(repository, typeof(FutureTarget), Behavior.Loose);
                        reset = Task.Run(() =>
                        {
                            started.Set();
                            Invoke(repository, "Reset");
                        });
                        Assert.True(started.Wait(TimeSpan.FromSeconds(5)));
                        Assert.False(reset.Wait(TimeSpan.FromMilliseconds(100)));
                        Assert.Equal(1, registrations.Count);
                    }

                    Assert.True(reset.Wait(TimeSpan.FromSeconds(5)));
                    Assert.Equal(0, registrations.Count);
                }
                finally
                {
                    if (reset != null)
                        reset.GetAwaiter().GetResult();
                    Invoke(repository, "Reset");
                }
            }
        }

        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldValidateTypeOverloadNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => Mock.SetupFuture((Type)null));
            Assert.Throws<ArgumentNullException>(() => Mock.SetupFuture(null, Behavior.Loose));
        }

        [TestMethod, TestCategory("SetupFutureManaged")]
        public void ShouldRejectUnsupportedTargetsBeforeEnablingInterception()
        {
            foreach (var type in new[] { typeof(IDisposable), typeof(int), typeof(Action), typeof(List<>), typeof(Math), typeof(string) })
            {
                var exception = Assert.Throws<MockException>(() => Mock.SetupFuture(type));
                Assert.True(exception.Message.Contains("supported closed instance class"));
            }

            var genericException = Assert.Throws<MockException>(() => Mock.SetupFuture<int>());
            Assert.True(genericException.Message.Contains("supported closed instance class"));
        }

        [TestMethod, TestCategory("SetupFuture")]
        public void ShouldLeaveExistingInstancesUnchangedAndMockFutureConstructors()
        {
            var existing = new FutureTarget();
            FutureTarget.ConstructorCalls = 0;
            Mock.SetupFuture<FutureTarget>();
            var future = new FutureTarget();

            Assert.Equal(0, FutureTarget.ConstructorCalls);
            Assert.Equal(42, existing.GetValue());
            Assert.Equal(0, future.GetValue());
        }

        [TestMethod, TestCategory("SetupFuture")]
        public void ShouldSupportDefaultTypeOverload()
        {
            Mock.SetupFuture(typeof(FutureTarget));
            var future = new FutureTarget();

            Assert.Equal(0, future.GetValue());
            Assert.Equal(0, future.ConstructorValue);
        }

        [TestMethod, TestCategory("SetupFuture")]
        public void ShouldSupportExplicitLooseGenericOverloadAndArrangementPrecedence()
        {
            Mock.SetupFuture<FutureTarget>(Behavior.Loose);
            var future = new FutureTarget();
            Assert.Equal(0, future.GetValue());

            Mock.Arrange(() => future.GetValue()).Returns(17);

            Assert.Equal(17, future.GetValue());
            Assert.Equal(0, new FutureTarget().GetValue());
        }

        [TestMethod, TestCategory("SetupFuture")]
        public void ShouldSupportExplicitStrictTypeOverload()
        {
            Mock.SetupFuture(typeof(FutureTarget), Behavior.Strict);
            var future = new FutureTarget();

            Assert.Equal(0, future.ConstructorValue);
            Assert.Throws<StrictMockException>(() => future.GetValue());
        }

        [TestMethod, TestCategory("SetupFuture")]
        public void ShouldCallOriginalConstructorAndRetainFutureInstanceAssociation()
        {
            FutureTarget.ConstructorCalls = 0;
            Mock.SetupFuture<FutureTarget>(Behavior.CallOriginal);
            var future = new FutureTarget();

            Assert.Equal(1, FutureTarget.ConstructorCalls);
            Assert.Equal(42, future.ConstructorValue);
            Assert.Equal(42, future.GetValue());
            Assert.NotNull(Lookup(future));
            Assert.Equal(typeof(FutureTarget), Lookup(future).DeclaringType);
        }

        [TestMethod, TestCategory("SetupFuture")]
        public void ShouldHonorExplicitConstructorCallOriginal()
        {
            Mock.SetupFuture<FutureTarget>();
            Mock.Arrange(() => new FutureTarget()).CallOriginal();
            FutureTarget.ConstructorCalls = 0;

            var future = new FutureTarget();

            Assert.Equal(1, FutureTarget.ConstructorCalls);
            Assert.Equal(42, future.ConstructorValue);
            Assert.Equal(0, future.GetValue());
            Assert.NotNull(Lookup(future));
        }

        [TestMethod, TestCategory("SetupFuture")]
        public void ShouldKeepFuturePropertiesIndexersAndEventsIndependent()
        {
            Mock.SetupFuture<EqualFutureTarget>();
            var first = new EqualFutureTarget();
            var second = new EqualFutureTarget();
            first.Value = 11;
            second.Value = 22;
            first[3] = 33;
            second[3] = 44;
            Assert.Equal(11, first.Value);
            Assert.Equal(22, second.Value);
            Assert.Equal(33, first[3]);
            Assert.Equal(44, second[3]);
            first.Value = 55;
            Assert.Equal(55, first.Value);
            Assert.Equal(22, second.Value);

            int calls = 0;
            EventHandler handler = (sender, args) => calls++;
            first.Changed += handler;
            Mock.Raise(() => second.Changed += null, second, EventArgs.Empty);
            Assert.Equal(0, calls);
            second.Changed -= handler;
            Mock.Raise(() => first.Changed += null, first, EventArgs.Empty);
            Assert.Equal(1, calls);
            second.Changed += handler;
            first.Changed -= handler;
            Mock.Raise(() => first.Changed += null, first, EventArgs.Empty);
            Assert.Equal(1, calls);
            Mock.Raise(() => second.Changed += null, second, EventArgs.Empty);
            Assert.Equal(2, calls);
        }

        [TestMethod, TestCategory("SetupFuture")]
        public void ShouldResolveInheritedAndNonPublicMembersThroughRuntimeType()
        {
            Mock.SetupFuture<FutureTarget>();
            var future = new EqualFutureTarget();
            Assert.Equal(0, future.GetValue());
            Mock.NonPublic.Arrange<int>(future, "GetHiddenValue").Returns(19);

            Assert.Equal(19, future.ReadHiddenValue());
            Assert.Equal(typeof(EqualFutureTarget), Lookup(future).DeclaringType);
        }

        [TestMethod, TestCategory("SetupFuture")]
        public void ShouldRestoreRealConstructionAndCallsAfterReset()
        {
            Mock.SetupFuture<FutureTarget>();
            var mocked = new FutureTarget();
            Assert.Equal(0, mocked.GetValue());

            Mock.Reset();
            var real = new FutureTarget();

            Assert.Equal(42, real.ConstructorValue);
            Assert.Equal(42, real.GetValue());
            Assert.Equal(42, mocked.GetValue());
            Assert.Null(Lookup(mocked));
        }

        private static MocksRepository CurrentRepository()
        {
            var context = typeof(Mock).Assembly.GetType("Telerik.JustMock.Core.Context.MockingContext", true);
            return (MocksRepository)context.GetProperty("CurrentRepository").GetValue(null, null);
        }

        private static MocksRepository CreateRepository(MocksRepository parent)
        {
            return (MocksRepository)Activator.CreateInstance(typeof(MocksRepository),
                BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { parent, null }, null);
        }

        private static Dictionary<Type, Behavior> Registrations(MocksRepository repository)
        {
            return (Dictionary<Type, Behavior>)new PrivateAccessor(repository).GetField("futureMixinDatabase");
        }

        private static void Register(MocksRepository repository, Type type, Behavior behavior)
        {
            // Managed tests exercise dispatch without pretending to provide native profiler hooks.
            var registrations = Registrations(repository);
            lock (registrations)
                registrations[type] = behavior;
        }

        private static IMockMixin Bind(MocksRepository repository, object instance)
        {
            return (IMockMixin)Invoke(repository, "CreateFutureMockMixin", instance);
        }

        private static IMockMixin Lookup(object instance)
        {
            return (IMockMixin)PrivateAccessor.ForType(typeof(MocksRepository))
                .CallMethod("GetMockMixin", instance, instance.GetType(), false);
        }

        private static Invocation CreateInvocation(object instance, MethodBase method, params object[] args)
        {
            return (Invocation)Activator.CreateInstance(typeof(Invocation), BindingFlags.Instance | BindingFlags.NonPublic,
                null, new object[] { instance, method, args }, null);
        }

        private static Invocation Dispatch(MocksRepository repository, object instance, MethodInfo method, params object[] args)
        {
            var invocation = CreateInvocation(instance, method, args);
            Invoke(repository, "DispatchInvocation", invocation);
            return invocation;
        }

        private static IMockMixin Mixin(Invocation invocation)
        {
            return (IMockMixin)new PrivateAccessor(invocation).GetProperty("MockMixin");
        }

        private static object ReturnValue(Invocation invocation)
        {
            return new PrivateAccessor(invocation).GetProperty("ReturnValue");
        }

        private static bool CallOriginal(Invocation invocation)
        {
            return (bool)new PrivateAccessor(invocation).GetProperty("CallOriginal");
        }

        private static object Invoke(MocksRepository repository, string method, params object[] args)
        {
            return new PrivateAccessor(repository) { RethrowOriginalOnCallMethod = true }.CallMethod(method, args);
        }

        private static object InvokeProfiler(string method, params object[] args)
        {
            var profiler = typeof(Mock).Assembly.GetType("Telerik.JustMock.Core.ProfilerInterceptor", true);
            var accessor = PrivateAccessor.ForType(profiler);
            accessor.RethrowOriginalOnCallMethod = true;
            return accessor.CallMethod(method, args);
        }

        public class FutureTarget
        {
            private readonly Dictionary<int, int> values = new Dictionary<int, int>();
            public static int ConstructorCalls;
            public int ConstructorValue;

            public FutureTarget()
            {
                ConstructorCalls++;
                ConstructorValue = 42;
            }

            public int Value { get; set; }

            public int this[int index]
            {
                get { return values[index]; }
                set { values[index] = value; }
            }

            public event EventHandler Changed;

            public int GetValue()
            {
                return 42;
            }

            public void RaiseChanged()
            {
                if (Changed != null)
                    Changed(this, EventArgs.Empty);
            }
        }

        public class EqualFutureTarget : FutureTarget
        {
            public override bool Equals(object obj)
            {
                return obj is EqualFutureTarget;
            }

            public override int GetHashCode()
            {
                return 1;
            }

            private int GetHiddenValue()
            {
                return 42;
            }

            public int ReadHiddenValue()
            {
                return GetHiddenValue();
            }
        }

        public class VirtualFutureTarget
        {
            public virtual int GetValue()
            {
                return 42;
            }
        }
    }
#endif
}
