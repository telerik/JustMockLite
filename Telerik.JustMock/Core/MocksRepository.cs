/*
 JustMock Lite
 Copyright © 2010-2015,2018-2019 Progress Software Corporation

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
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Telerik.JustMock.Core.Behaviors;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Core.MatcherTree;
using Telerik.JustMock.Core.Recording;
using Telerik.JustMock.Core.TransparentProxy;
using Telerik.JustMock.Diagnostics;
#if !PORTABLE
using Telerik.JustMock.Core.Castle.DynamicProxy.Generators;
#endif

#if NETCORE
using Debug = Telerik.JustMock.Diagnostics.JMDebug;
#else
using Debug = System.Diagnostics.Debug;
#endif

namespace Telerik.JustMock.Core
{
    /// <summary>
    /// An implementation detail. Not intended for external usage.
    /// </summary>
    public sealed class MocksRepository
    {
        private static readonly IMockFactory mockFactory;
        private static readonly List<KeyValuePair<object, IMockMixin>> externalMixinDatabase = new List<KeyValuePair<object, IMockMixin>>();
        private static readonly Dictionary<object, IMockMixin> externalReferenceMixinDatabase = new Dictionary<object, IMockMixin>(ByRefComparer<object>.Instance);

        private static int repositoryCounter;

        private readonly int repositoryId;
        private readonly Thread creatingThread;
        private readonly Dictionary<Type, IMockMixin> staticMixinDatabase = new Dictionary<Type, IMockMixin>();
        private readonly Dictionary<MethodBase, MethodInfoMatcherTreeNode> arrangementTreeRoots = new Dictionary<MethodBase, MethodInfoMatcherTreeNode>();
        private readonly Dictionary<MethodBase, MethodInfoMatcherTreeNode> invocationTreeRoots = new Dictionary<MethodBase, MethodInfoMatcherTreeNode>();
        private readonly Dictionary<KeyValuePair<object, object>, object> valueStore = new Dictionary<KeyValuePair<object, object>, object>();
        private readonly HashSet<Type> arrangedTypes = new HashSet<Type>();
        private readonly HashSet<Type> disabledTypes = new HashSet<Type>();
        private readonly HashSet<MethodBase> globallyInterceptedMethods = new HashSet<MethodBase>();

        private readonly RepositorySharedContext sharedContext;
        private readonly MocksRepository parentRepository;
        private readonly List<WeakReference> controlledMocks = new List<WeakReference>();

        private bool isRetired;

        internal static readonly HashSet<Type> KnownUnmockableTypes = new HashSet<Type>
            {
                typeof(ValueType),
                typeof(Enum),
                typeof(Delegate),
                typeof(MulticastDelegate),
                typeof(Array),
                typeof(String),
                typeof(IntPtr),
                typeof(UIntPtr),
                typeof(void),
#if !PORTABLE
                typeof(AppDomain),
                typeof(TypedReference),
                typeof(RuntimeArgumentHandle),
                Type.GetType("System.ContextBoundObject"),
                Type.GetType("System.ArgIterator"),
#endif
#if !SILVERLIGHT
                Type.GetType("System.__ComObject"),
#endif
#if SILVERLIGHT
                typeof(WeakReference),
#endif
            };

        internal IRecorder Recorder
        {
            get { return this.sharedContext.Recorder; }
        }

        internal bool IsRetired
        {
            get
            {
                return this.isRetired
                    || (this.parentRepository != null && this.parentRepository.IsRetired)
#if !PORTABLE
 || !this.creatingThread.IsAlive
#endif
;
            }
            set
            {
                this.isRetired = value;
            }
        }

        internal bool IsParentToAnotherRepository { get; private set; }

        internal MethodBase Method { get; private set; }

        internal DynamicProxyInterceptor Interceptor { get; private set; }

        internal List<IMatcher> MatchersInContext { get; private set; }

        static MocksRepository()
        {
#if !COREFX
            var badApples = new[]
            {
                typeof(System.Security.Permissions.SecurityAttribute),
                typeof(System.Runtime.InteropServices.MarshalAsAttribute),
                typeof(object).Assembly.GetType("System.Runtime.InteropServices.TypeIdentifierAttribute"),
            };

            foreach (var unmockableAttr in badApples.Where(t => t != null))
                AttributesToAvoidReplicating.Add(unmockableAttr);
#endif

#if !PORTABLE
            mockFactory = new DynamicProxyMockFactory();
#else
            mockFactory = new StaticProxy.StaticProxyMockFactory();
#endif

            ProfilerInterceptor.Initialize();
        }

        internal MocksRepository(MocksRepository parentRepository, MethodBase method)
        {
            this.repositoryId = ++repositoryCounter;
            this.Method = method;
            this.creatingThread = Thread.CurrentThread;
            this.Interceptor = new DynamicProxyInterceptor(this);
            this.MatchersInContext = new List<IMatcher>();
            if (parentRepository != null)
            {
                this.parentRepository = parentRepository;
                this.sharedContext = parentRepository.sharedContext;
                parentRepository.IsParentToAnotherRepository = true;
                CopyConfigurationFromParent();
            }
            else
            {
                this.sharedContext = new RepositorySharedContext();
            }

            ProfilerInterceptor.IsInterceptionEnabled = true;
            DebugView.TraceEvent(IndentLevel.Configuration, () => String.Format("Created mocks repository #{0} for {1}", this.repositoryId, this.Method));
        }

        internal static IMockMixin GetMockMixin(object obj, Type objType)
        {
            IMockMixin asMixin = GetMockMixinFromAnyMock(obj);
            if (asMixin != null)
            {
                return asMixin;
            }

            if (obj != null && objType == null)
            {
                objType = obj.GetType();
            }

            if (obj != null)
            {
                asMixin = GetMixinFromExternalDatabase(obj, objType);
            }
            else if (objType != null)
            {
                MocksRepository repo = MockingContext.ResolveRepository(UnresolvedContextBehavior.CreateNewContextual);
                if (repo != null)
                {
                    lock (repo.staticMixinDatabase)
                        repo.staticMixinDatabase.TryGetValue(objType, out asMixin);
                }
            }
            return asMixin;
        }

        private static IMockMixin GetMockMixinFromAnyMock(object mock)
        {
            var asMixin = MockingProxy.GetMockMixin(mock);
            if (asMixin != null)
                return asMixin;

            return mock as IMockMixin;
        }

        private static IMockMixin GetMixinFromExternalDatabase(object obj, Type type)
        {
            bool isValueType = type.IsValueType;
            lock (externalMixinDatabase)
            {
                if (isValueType)
                {
                    foreach (var entry in externalMixinDatabase)
                    {
                        object otherObj = entry.Key;
                        if (AreTypesInRegistrySame(type, otherObj.GetType()))
                        {
                            bool equal = false;
                            try
                            {
                                equal = AreValueTypeObjectsInRegistryEqual(obj, otherObj);
                            }
                            catch
                            {
                                throw new MockException("Implementation of method Equals in value types must not throw for mocked instances.");
                            }

                            if (equal)
                                return entry.Value;
                        }
                    }
                }
                else
                {
                    IMockMixin result;
                    externalReferenceMixinDatabase.TryGetValue(obj, out result);
                    return result;
                }
            }

            return null;
        }

        private static bool AreTypesInRegistrySame(Type queryType, Type typeInRegistry)
        {
            if (queryType == typeInRegistry)
                return true;

            return false;
        }

        private static bool AreValueTypeObjectsInRegistryEqual(object queryObj, object objInRegistry)
        {
            return Object.Equals(queryObj, objInRegistry); // no guard - Object.Equals on a value type can't get intercepted
        }

        private MethodInfoMatcherTreeNode DeepCopy(MethodInfoMatcherTreeNode root)
        {
            var newRoot = (MethodInfoMatcherTreeNode)root.Clone();
            Queue<MatcherNodeAndParent> queue = new Queue<MatcherNodeAndParent>();
            foreach (var child in root.Children)
                queue.Enqueue(new MatcherNodeAndParent(child, newRoot));

            while (queue.Count > 0)
            {
                MatcherNodeAndParent current = queue.Dequeue();
                IMatcherTreeNode newCurrent = current.Node.Clone();
                foreach (IMatcherTreeNode node in current.Node.Children)
                {
                    queue.Enqueue(new MatcherNodeAndParent(node, newCurrent));
                }

                current.Parent.Children.Add(newCurrent);
                newCurrent.Parent = current.Parent;
            }

            return newRoot;
        }

        private void CopyConfigurationFromParent()
        {
            this.arrangementTreeRoots.Clear();
            this.invocationTreeRoots.Clear();
            this.valueStore.Clear();
            this.controlledMocks.Clear();

            if (parentRepository != null)
            {
                foreach (var root in parentRepository.arrangementTreeRoots)
                {
                    this.arrangementTreeRoots.Add(root.Key, DeepCopy(root.Value));
                }

                foreach (var root in parentRepository.invocationTreeRoots)
                {
                    this.invocationTreeRoots.Add(root.Key, DeepCopy(root.Value));
                }

                foreach (var kvp in parentRepository.valueStore)
                {
                    this.valueStore.Add(kvp.Key, kvp.Value);
                }

                foreach (WeakReference mockRef in parentRepository.controlledMocks)
                {
                    IMockMixin mixin = GetMockMixinFromAnyMock(mockRef.Target);
                    if (mixin != null)
                    {
                        mixin.Repository = this;
                        this.controlledMocks.Add(mockRef);
                    }
                }
            }
        }

        internal void Retire()
        {
            this.IsRetired = true;
            this.Reset();
        }

        internal void InterceptGlobally(MethodBase method)
        {
            if (this.globallyInterceptedMethods.Add(method))
            {
                ProfilerInterceptor.RegisterGlobalInterceptor(method, this);
            }
        }

        internal void Reset()
        {
            DebugView.TraceEvent(IndentLevel.Configuration, () => String.Format("Resetting mock repository related to {0}.", this.Method));

            foreach (var type in this.arrangedTypes)
            {
                ProfilerInterceptor.EnableInterception(type, false, this);
            }

            this.arrangedTypes.Clear();
            this.staticMixinDatabase.Clear();

            foreach (var method in this.globallyInterceptedMethods)
            {
                ProfilerInterceptor.UnregisterGlobalInterceptor(method, this);
            }
            this.globallyInterceptedMethods.Clear();

            lock (externalMixinDatabase)
            {
                foreach (WeakReference mockRef in this.controlledMocks)
                {
                    IMockMixin mock = GetMockMixinFromAnyMock(mockRef.Target);
                    if (mock != null && mock.ExternalizedMock != null && mock.Originator == this)
                    {
                        externalMixinDatabase.RemoveAll(kvp => kvp.Value == mock);
                        externalReferenceMixinDatabase.Remove(mock.ExternalizedMock);
                    }
                }
            }

            this.controlledMocks.Clear();

            this.CopyConfigurationFromParent();
        }

        internal void DispatchInvocation(Invocation invocation)
        {
            DebugView.TraceEvent(IndentLevel.DispatchResult, () => String.Format("Handling dispatch in repo #{0} servicing {1}", this.repositoryId, this.Method));

            if (this.disabledTypes.Contains(invocation.Method.DeclaringType))
            {
                invocation.CallOriginal = true;
                return;
            }

            invocation.InArrange = this.sharedContext.InArrange;
            invocation.InArrangeArgMatching = this.sharedContext.InArrangeArgMatching;
            invocation.InAssertSet = this.sharedContext.InAssertSet;
            invocation.Recording = this.Recorder != null;
            invocation.RetainBehaviorDuringRecording = this.sharedContext.DispatchToMethodMocks;
            invocation.Repository = this;

            bool methodMockProcessed = false;
            if (invocation.Recording)
            {
                this.Recorder.Record(invocation);
            }
            if (!invocation.Recording || invocation.RetainBehaviorDuringRecording)
            {
                methodMockProcessed = DispatchInvocationToMethodMocks(invocation);
            }

            invocation.ThrowExceptionIfNecessary();

            if (!methodMockProcessed)
            {
                // We have to be careful for the potential exception throwing in the assertion context,
                // so skip CallOriginalBehavior processing
                var mock = invocation.MockMixin;
                if (mock != null)
                {
                    var fallbackBehaviorsToExecute =
                        mock.FallbackBehaviors
                            .Where(behavior => !invocation.InAssertSet || !(behavior is CallOriginalBehavior))
                            .ToList();
                    foreach (var fallbackBehavior in fallbackBehaviorsToExecute)
                    {
                        fallbackBehavior.Process(invocation);
                    }
                }
                else
                {
                    invocation.CallOriginal = CallOriginalBehavior.ShouldCallOriginal(invocation) && !invocation.InAssertSet;
                }
            }

            if (!invocation.CallOriginal && !invocation.IsReturnValueSet && invocation.Method.GetReturnType() != typeof(void))
                invocation.ReturnValue = invocation.Method.GetReturnType().GetDefaultValue();
        }

        internal T GetValue<T>(object owner, object key, T dflt)
        {
            object value;
            if (valueStore.TryGetValue(new KeyValuePair<object, object>(owner, key), out value))
            {
                return (T)value;
            }
            else
            {
                return dflt;
            }
        }

        internal void StoreValue<T>(object owner, object key, T value)
        {
            valueStore[new KeyValuePair<object, object>(owner, key)] = value;
        }

        internal IDisposable StartRecording(IRecorder recorder, bool dispatchToMethodMocks)
        {
            return this.sharedContext.StartRecording(recorder, dispatchToMethodMocks);
        }

        internal IDisposable StartArrangeArgMathing()
        {
            return this.sharedContext.StartArrangeArgMathing();
        }

        internal void AddMatcherInContext(IMatcher matcher)
        {
            if (!this.sharedContext.InArrange || this.sharedContext.Recorder != null)
            {
                this.MatchersInContext.Add(matcher);
            }
        }

        internal object Create(Type type, MockCreationSettings settings)
        {
            object delegateResult;
            if (TryCreateDelegate(type, settings, out delegateResult))
            {
                return delegateResult;
            }

            bool isSafeMock = settings.FallbackBehaviors.OfType<CallOriginalBehavior>().Any();
            this.CheckIfCanMock(type, !isSafeMock);

            this.EnableInterception(type);

            bool canCreateProxy = !type.IsSealed;

            MockMixin mockMixinImpl = this.CreateMockMixin(type, settings);

            ConstructorInfo[] ctors = type.GetConstructors();
            bool isCoclass = ctors.Any(ctor => ctor.IsExtern());

            bool hasAdditionalInterfaces = settings.AdditionalMockedInterfaces != null && settings.AdditionalMockedInterfaces.Length > 0;
            bool hasAdditionalProxyTypeAttributes = settings.AdditionalProxyTypeAttributes != null && settings.AdditionalProxyTypeAttributes.Any();

            bool shouldCreateProxy = settings.MustCreateProxy
                || hasAdditionalInterfaces
                || isCoclass
                || type.IsAbstract || type.IsInterface
                || !settings.MockConstructorCall
                || hasAdditionalProxyTypeAttributes
                || !ProfilerInterceptor.IsProfilerAttached
                || !ProfilerInterceptor.TypeSupportsInstrumentation(type);

            bool createTransparentProxy = MockingProxy.CanCreate(type) && !ProfilerInterceptor.IsProfilerAttached;

            Exception proxyFailure = null;
            object instance = null;
            if (canCreateProxy && shouldCreateProxy)
            {
                try
                {
                    instance = mockFactory.Create(type, this, mockMixinImpl, settings, createTransparentProxy);
                }
                catch (ProxyFailureException ex)
                {
                    proxyFailure = ex.InnerException;
                }
            }

            IMockMixin mockMixin = instance as IMockMixin;

            if (instance == null)
            {
                if (type.IsInterface || type.IsAbstract)
                    throw new MockException(String.Format("Abstract type '{0}' is not accessible for inheritance.", type));

                if (hasAdditionalInterfaces)
                    throw new MockException(String.Format("Type '{0}' is not accessible for inheritance. Cannot create mock object implementing the specified additional interfaces.", type));

                if (!ProfilerInterceptor.IsProfilerAttached && !createTransparentProxy)
                    ProfilerInterceptor.ThrowElevatedMockingException(type);

                if (settings.MockConstructorCall && type.IsValueType)
                {
                    settings.MockConstructorCall = false;
                    settings.Args = null;
                }

                if (!settings.MockConstructorCall)
                {
                    try
                    {
                        instance = type.CreateObject(settings.Args);
                    }
                    catch (MissingMethodException)
                    {
                        settings.MockConstructorCall = true;
                    }
                }

                if (settings.MockConstructorCall)
                {
                    instance = MockingUtil.GetUninitializedObject(type);
                }

                if (!createTransparentProxy)
                {
                    mockMixin = this.CreateExternalMockMixin(type, instance, settings);
                }
            }
            else
            {
                this.controlledMocks.Add(new WeakReference(instance));
            }

            if (type.IsClass)
                GC.SuppressFinalize(instance);

            if (createTransparentProxy)
            {
                if (mockMixin == null)
                {
                    mockMixin = mockMixinImpl;
                }
                instance = MockingProxy.CreateProxy(instance, this, mockMixin);
            }

            mockMixin.IsInstanceConstructorMocked = settings.MockConstructorCall;

            return instance;
        }

        internal IMockMixin CreateExternalMockMixin(Type mockObjectType, object mockObject, MockCreationSettings settings)
        {
            if (mockObjectType == null)
            {
                if (mockObject == null)
                    throw new ArgumentNullException("mockObject");
                mockObjectType = mockObject.GetType();
            }

            this.EnableInterception(mockObjectType);

            if (mockObject == null)
                throw new MockException(String.Format("Failed to create instance of type '{0}'", mockObjectType));

            MockMixin mockMixin = this.CreateMockMixin(mockObjectType, settings, false);
            IMockMixin compoundMockMixin = mockFactory.CreateExternalMockMixin(mockMixin, settings.Mixins);
            lock (externalMixinDatabase)
            {
                if (mockObjectType.IsValueType())
                {
                    externalMixinDatabase.RemoveAll(kvp => kvp.Key.Equals(mockObject));
                    externalMixinDatabase.Add(new KeyValuePair<object, IMockMixin>(mockObject, compoundMockMixin));
                }
                else
                {
                    externalReferenceMixinDatabase.Add(mockObject, compoundMockMixin);
                }

                compoundMockMixin.ExternalizedMock = mockObject;
                this.controlledMocks.Add(new WeakReference(compoundMockMixin));
            }

            return compoundMockMixin;
        }

        internal ProxyTypeInfo CreateClassProxyType(Type classToProxy, MockCreationSettings settings)
        {
            MockMixin mockMixinImpl = this.CreateMockMixin(classToProxy, settings);
            return mockFactory.CreateClassProxyType(classToProxy, this, settings, mockMixinImpl);
        }

        private void CheckIfCanMock(Type type, bool checkSafety)
        {
            if (KnownUnmockableTypes.Contains(type)
                || typeof(Delegate).IsAssignableFrom(type))
                throw new MockException("Cannot create mock for type due to CLR limitations.");

            if (checkSafety)
                ProfilerInterceptor.CheckIfSafeToInterceptWholesale(type);
        }

        internal void InterceptStatics(Type type, MockCreationSettings settings, bool mockStaticConstructor)
        {
            if (!ProfilerInterceptor.IsProfilerAttached)
                ProfilerInterceptor.ThrowElevatedMockingException(type);

            if (!settings.FallbackBehaviors.OfType<CallOriginalBehavior>().Any())
                ProfilerInterceptor.CheckIfSafeToInterceptWholesale(type);

            var mockMixin = (IMockMixin)Create(typeof(ExternalMockMixin),
                new MockCreationSettings
                {
                    Mixins = settings.Mixins,
                    SupplementaryBehaviors = settings.SupplementaryBehaviors,
                    FallbackBehaviors = settings.FallbackBehaviors,
                    MustCreateProxy = true,
                });

            mockMixin.IsStaticConstructorMocked = mockStaticConstructor;
            lock (staticMixinDatabase)
                staticMixinDatabase[type] = mockMixin;

            this.EnableInterception(type);
        }

        private MockMixin CreateMockMixin(Type declaringType, MockCreationSettings settings)
        {
            return CreateMockMixin(declaringType, settings, settings.MockConstructorCall);
        }

        private MockMixin CreateMockMixin(Type declaringType, MockCreationSettings settings, bool mockConstructorCall)
        {
            var mockMixin = new MockMixin
            {
                Repository = this,
                DeclaringType = declaringType,
                IsInstanceConstructorMocked = mockConstructorCall,
            };

            foreach (var behavior in settings.SupplementaryBehaviors)
                mockMixin.SupplementaryBehaviors.Add(behavior);

            foreach (var behavior in settings.FallbackBehaviors)
                mockMixin.FallbackBehaviors.Add(behavior);

            return mockMixin;
        }

        [ArrangeMethod]
        internal TMethodMock Arrange<TMethodMock>(Expression expression, Func<TMethodMock> methodMockFactory)
            where TMethodMock : IMethodMock
        {
            TMethodMock result = methodMockFactory();

            using (this.sharedContext.StartArrange())
            {
                result.Repository = this;
                result.ArrangementExpression = ExpressionUtil.ConvertMockExpressionToString(expression);
                result.CallPattern = CallPatternCreator.FromExpression(this, expression);

                AddArrange(result);
            }

#if !PORTABLE
            var createInstanceLambda = ActivatorCreateInstanceTBehavior.TryCreateArrangementExpression(result.CallPattern.Method);
            if (createInstanceLambda != null)
            {
                var createInstanceMethodMock = Arrange(createInstanceLambda, methodMockFactory);
                ActivatorCreateInstanceTBehavior.Attach(result, createInstanceMethodMock);
            }
#endif

            return result;
        }

        [ArrangeMethod]
        internal TMethodMock Arrange<TMethodMock>(Action memberAction, Func<TMethodMock> methodMockFactory)
            where TMethodMock : IMethodMock
        {
            using (this.sharedContext.StartArrange())
            {
                var result = methodMockFactory();
                result.Repository = this;
                result.CallPattern = CallPatternCreator.FromAction(this, memberAction);
                AddArrange(result);
                return result;
            }
        }

        [ArrangeMethod]
        internal TMethodMock Arrange<TMethodMock>(object instance, MethodBase method, object[] arguments, Func<TMethodMock> methodMockFactory)
            where TMethodMock : IMethodMock
        {
            using (this.sharedContext.StartArrange())
            {
                var result = methodMockFactory();
                result.Repository = this;
                result.CallPattern = CallPatternCreator.FromMethodBase(this, instance, method, arguments);

                AddArrange(result);
                return result;
            }
        }

        [ArrangeMethod]
        internal TMethodMock Arrange<TMethodMock>(CallPattern callPattern, Func<TMethodMock> methodMockFactory)
            where TMethodMock : IMethodMock
        {
            using (this.sharedContext.StartArrange())
            {
                var result = methodMockFactory();
                result.Repository = this;
                result.CallPattern = callPattern;
                AddArrange(result);
                return result;
            }
        }

        private void AssertBehaviorsForMocks(IEnumerable<IMethodMock> mocks, bool ignoreMethodMockOccurrences)
        {
            foreach (var methodMock in mocks)
            {
                bool occurrenceAsserted = ignoreMethodMockOccurrences;
                if (!ignoreMethodMockOccurrences
                    && !methodMock.OccurencesBehavior.LowerBound.HasValue
                    && !methodMock.OccurencesBehavior.UpperBound.HasValue)
                {
                    methodMock.OccurencesBehavior.Assert(1, null);
                    occurrenceAsserted = true;
                }

                foreach (var behavior in methodMock.Behaviors.OfType<IAssertableBehavior>())
                    if (!occurrenceAsserted || behavior != methodMock.OccurencesBehavior)
                        behavior.Assert();
            }
        }

        internal void AssertAll(string message, object mock)
        {
            using (MockingContext.BeginFailureAggregation(message))
            {
                var mocks = GetMethodMocksFromObject(mock);
                AssertBehaviorsForMocks(mocks.Select(m => m.MethodMock), false);
            }
        }

        internal void Assert(string message, object mock, Expression expr = null, Args args = null, Occurs occurs = null)
        {
            using (MockingContext.BeginFailureAggregation(message))
            {
                if (expr == null)
                {
                    List<IMethodMock> mocks = new List<IMethodMock>();
                    mocks.AddRange(GetMethodMocksFromObject(mock).Select(m => m.MethodMock));
                    foreach (var methodMock in mocks)
                    {
                        foreach (var behavior in methodMock.Behaviors.OfType<IAssertableBehavior>())
                            behavior.Assert();
                    }

                    MockingUtil.UnwrapDelegateTarget(ref mock);
                    var mockMixin = GetMockMixin(mock, null);
                    if (mockMixin != null)
                    {
                        foreach (var behavior in mockMixin.FallbackBehaviors.OfType<IAssertableBehavior>()
                            .Concat(mockMixin.SupplementaryBehaviors.OfType<IAssertableBehavior>()))
                            behavior.Assert();
                    }
                }
                else
                {
                    CallPattern callPattern = CallPatternCreator.FromExpression(this, expr);
                    AssertForCallPattern(callPattern, args, occurs);
                }
            }
        }

        internal void AssertAction(string message, Action memberAction, Args args = null, Occurs occurs = null)
        {
            using (MockingContext.BeginFailureAggregation(message))
            {
                CallPattern callPattern = CallPatternCreator.FromAction(this, memberAction);
                AssertForCallPattern(callPattern, args, occurs);
            }
        }

        internal void AssertSetAction(string message, Action memberAction, Args args = null, Occurs occurs = null)
        {
            using (MockingContext.BeginFailureAggregation(message))
            {
                using (this.sharedContext.StartAssertSet())
                {
                    CallPattern callPattern = CallPatternCreator.FromAction(this, memberAction, true);
                    AssertForCallPattern(callPattern, args, occurs);
                }
            }
        }

        internal void AssertMethodInfo(string message, object instance, MethodBase method, object[] arguments, Occurs occurs)
        {
            using (MockingContext.BeginFailureAggregation(message))
            {
                CallPattern callPattern = CallPatternCreator.FromMethodBase(instance, method, arguments);
                AssertForCallPattern(callPattern, null, occurs);
            }
        }

        internal void AssertIgnoreInstance(string message, Type type, bool ignoreMethodMockOccurrences)
        {
            using (MockingContext.BeginFailureAggregation(message))
            {
                var methodMocks = GetMethodMocksFromObject(null, type);
                AssertBehaviorsForMocks(methodMocks.Select(m => m.MethodMock), ignoreMethodMockOccurrences);
            }
        }

        internal int GetTimesCalled(Expression expression, Args args)
        {
            CallPattern callPattern = CallPatternCreator.FromExpression(this, expression);

            int callsCount;
            CountMethodMockInvocations(callPattern, args, out callsCount);
            return callsCount;
        }

        internal int GetTimesCalledFromAction(Action action, Args args)
        {
            var callPattern = CallPatternCreator.FromAction(this, action);
            int callsCount;
            CountMethodMockInvocations(callPattern, args, out callsCount);
            return callsCount;
        }

        internal int GetTimesCalledFromMethodInfo(object instance, MethodBase method, object[] arguments)
        {
            var callPattern = CallPatternCreator.FromMethodBase(instance, method, arguments);
            int callsCount;
            CountMethodMockInvocations(callPattern, null, out callsCount);
            return callsCount;
        }

        private HashSet<IMethodMock> CountMethodMockInvocations(CallPattern callPattern, Args args, out int callsCount)
        {
            if (callPattern.IsDerivedFromObjectEquals)
                throw new MockException("Cannot assert calls to methods derived from Object.Equals");

            PreserveRefOutValuesBehavior.ReplaceRefOutArgsWithAnyMatcher(callPattern);

            if (args != null)
            {
                if (args.Filter != null)
                {
                    if (args.IsIgnored == null)
                    {
                        args.IsIgnored = true;
                    }
                    if (!callPattern.Method.IsStatic
                        && args.IsInstanceIgnored == null
                        && args.Filter.Method.GetParameters().Length == callPattern.Method.GetParameters().Length + 1)
                    {
                        args.IsInstanceIgnored = true;
                    }
                }

                if (args.IsIgnored == true)
                {
                    for (int i = 0; i < callPattern.ArgumentMatchers.Count; i++)
                        callPattern.ArgumentMatchers[i] = new AnyMatcher();
                }

                if (args.IsInstanceIgnored == true)
                {
                    callPattern.InstanceMatcher = new AnyMatcher();
                }

                callPattern.Filter = args.Filter;
            }

            MethodInfoMatcherTreeNode root;
            callsCount = 0;
            var mocks = new HashSet<IMethodMock>();
            var method = callPattern.Method;
            if (invocationTreeRoots.TryGetValue(method, out root))
            {
                var occurences = root.GetOccurences(callPattern);
                callsCount = occurences.Select(x => x.Calls).Sum();
                foreach (var mock in occurences.SelectMany(x => x.Mocks))
                {
                    mocks.Add(mock);
                }
            }
            return mocks;
        }

        private void AssertForCallPattern(CallPattern callPattern, Args args, Occurs occurs)
        {
            int callsCount;
            var mocks = CountMethodMockInvocations(callPattern, args, out callsCount);
            if (occurs != null)
            {
                InvocationOccurrenceBehavior.Assert(occurs.LowerBound, occurs.UpperBound, callsCount, null, null);
            }

            if (mocks.Count == 0)
            {
                MethodInfoMatcherTreeNode funcRoot;
                if (arrangementTreeRoots.TryGetValue(callPattern.Method, out funcRoot))
                {
                    var arranges = funcRoot.GetAllMethodMocks(callPattern);
                    foreach (var arrange in arranges)
                    {
                        mocks.Add(arrange.MethodMock);
                    }
                }
            }
            if (occurs == null && mocks.Count == 0)
            {
                InvocationOccurrenceBehavior.Assert(Occurs.AtLeastOnce().LowerBound, Occurs.AtLeastOnce().UpperBound, callsCount, null, null);
            }
            else
            {
                AssertBehaviorsForMocks(mocks, occurs != null);
            }
        }

        internal MethodBase GetMethodFromCallPattern(CallPattern callPattern)
        {
            var method = callPattern.Method as MethodInfo;
            if (method == null || !method.IsVirtual)
                return callPattern.Method;

            method = method.NormalizeComInterfaceMethod();
            var valueMatcher = callPattern.InstanceMatcher as IValueMatcher;
            if (valueMatcher != null && valueMatcher.Value != null)
            {
                var valueType = valueMatcher.Value.GetType();
                var mockMixin = GetMockMixin(valueMatcher.Value, null);

                var type = mockMixin != null ? mockMixin.DeclaringType : valueType;
                if (!type.IsInterface && method.DeclaringType.IsAssignableFrom(type))
                {
                    var concreteMethod = MockingUtil.GetConcreteImplementer(method, type);
                    if (!concreteMethod.IsInheritable() && !ProfilerInterceptor.IsProfilerAttached)
                    {
                        var reimplementedInterfaceMethod = (MethodInfo)method.GetInheritanceChain().Last();
                        if (reimplementedInterfaceMethod.DeclaringType.IsInterface
                            && mockFactory.IsAccessible(reimplementedInterfaceMethod.DeclaringType))
                        {
                            concreteMethod = reimplementedInterfaceMethod;
                        }
                    }
                    method = concreteMethod;
                }
            }

            if (method.DeclaringType != method.ReflectedType)
                method = (MethodInfo)MethodBase.GetMethodFromHandle(method.MethodHandle, method.DeclaringType.TypeHandle);

            return method;
        }

        /// <summary>
        /// Converts the given object to a matcher as follows. This method is most useful for
        /// creating a matcher out of an argument expression.
        /// 
        /// It works as follows:
        /// If the object is not an expression, then a value matcher for that object is returned.
        /// If the object is an expression then:
        /// * if the top of the expression is a method call expression and the member
        ///     has the ArgMatcherAttribute then the specific matcher type is instantiaded
        ///     with the parameters passed to the method call expression and returned.
        ///     If the matcher type is generic, then it is defined with the type of the expression.
        /// * if the top expression is a member or method call and the member
        ///     has the ArgIgnoreAttribute, then a TypeMatcher is returned
        /// * otherwise, the expression is evaluated and a ValueMatcher is returned
        /// </summary>
        /// <param name="argumentObj"></param>
        /// <returns></returns>
        internal static IMatcher CreateMatcherForArgument(object argumentObj)
        {
            var argExpr = argumentObj as Expression;
            if (argExpr == null)
            {
                return new ValueMatcher(argumentObj);
            }
            else
            {
                var argMatcher = TryCreateMatcherFromArgMember(argExpr);
                if (argMatcher != null)
                {
                    return argMatcher;
                }
                else
                {
                    //no matcher, evaluate the original expression
                    var argValue = argExpr.EvaluateExpression();
                    return new ValueMatcher(argValue);
                }
            }
        }

        internal static IMatcher TryCreateMatcherFromArgMember(Expression argExpr)
        {
            // The expression may end with a conversion which erases the type of the required matcher.
            // Remove the conversion before working with matchers
            while (argExpr.NodeType == ExpressionType.Convert)
                argExpr = ((UnaryExpression)argExpr).Operand;

            ArgMatcherAttribute argAttribute = null;
            Expression[] matcherArguments = null;
            if (argExpr is MethodCallExpression)
            {
                var methodCall = (MethodCallExpression)argExpr;
                argAttribute = (ArgMatcherAttribute)Attribute.GetCustomAttribute(methodCall.Method, typeof(ArgMatcherAttribute));
                matcherArguments = methodCall.Arguments.ToArray();
            }
            else if (argExpr is MemberExpression)
            {
                var memberExpr = (MemberExpression)argExpr;
                argAttribute = (ArgMatcherAttribute)Attribute.GetCustomAttribute(memberExpr.Member, typeof(ArgMatcherAttribute));
            }

            if (argAttribute != null)
            {
                if (argAttribute.GetType() == typeof(ArgIgnoreAttribute))
                {
                    return new TypeMatcher(argExpr.Type);
                }
                else if (argAttribute.GetType() == typeof(RefArgAttribute) || argAttribute.GetType() == typeof(OutArgAttribute))
                {
                    var asMemberExpr = argExpr as MemberExpression;
                    if (asMemberExpr != null)
                    {
                        argExpr = asMemberExpr.Expression;
                    }
                    var refCall = (MethodCallExpression)argExpr;
                    var actualArg = refCall.Arguments[0];
                    var memberExpr = actualArg as MemberExpression;
                    if (memberExpr != null && typeof(Expression).IsAssignableFrom(memberExpr.Type) && memberExpr.Expression.Type.DeclaringType == typeof(ArgExpr))
                    {
                        actualArg = (Expression)actualArg.EvaluateExpression();
                    }
                    var argMatcher = CreateMatcherForArgument(actualArg);
                    argMatcher.ProtectRefOut = argAttribute.GetType() == typeof(RefArgAttribute);
                    return argMatcher;
                }
                else
                {
                    var matcherType = argAttribute.Matcher;
                    var matcherArgs = argAttribute.MatcherArgs;

                    if (matcherType.IsGenericTypeDefinition)
                        matcherType = matcherType.MakeGenericType(argExpr.Type);

                    if (matcherArgs == null && matcherArguments != null)
                        matcherArgs = matcherArguments.Select(matcherArgExpr => matcherArgExpr.EvaluateExpression()).ToArray();

                    var matcher = (IMatcher)MockingUtil.CreateInstance(matcherType, matcherArgs);
                    return matcher;
                }
            }
            else
            {
                return null;
            }
        }

        private void AddArrange(IMethodMock methodMock)
        {
            var method = methodMock.CallPattern.Method;

            if (methodMock.CallPattern.IsDerivedFromObjectEquals && method.ReflectedType.IsValueType())
                throw new MockException("Cannot mock Equals method because JustMock depends on it. Also, when Equals is called internally by JustMock, all methods called by it will not be intercepted and will have only their original implementations called.");

            CheckMethodInterceptorAvailable(methodMock.CallPattern.InstanceMatcher, method);

            Type declaringType = method.DeclaringType;

            // If we're arranging a call to a mock object, overwrite its repository.
            // This will ensure correct behavior when a mock object is arranged
            // in multiple contexts.
            var refMatcher = methodMock.CallPattern.InstanceMatcher as ReferenceMatcher;
            if (refMatcher != null)
            {
                var value = refMatcher.Value;
                var mock = GetMockMixin(value, declaringType);
                if (mock != null)
                {
                    methodMock.Mock = mock;
                    mock.Repository = this;
                }

                if (value != null)
                    EnableInterception(value.GetType());
            }

            EnableInterception(declaringType);

            PreserveRefOutValuesBehavior.Attach(methodMock);
            ConstructorMockBehavior.Attach(methodMock);

            MethodInfoMatcherTreeNode funcRoot;
            if (!arrangementTreeRoots.TryGetValue(method, out funcRoot))
            {
                funcRoot = new MethodInfoMatcherTreeNode(method);
                arrangementTreeRoots.Add(method, funcRoot);
            }

            funcRoot.AddChild(methodMock.CallPattern, methodMock, this.sharedContext.GetNextArrangeId());
        }

        private void CheckMethodInterceptorAvailable(IMatcher instanceMatcher, MethodBase method)
        {
            if (ProfilerInterceptor.IsProfilerAttached)
                return;

            var valueMatcher = instanceMatcher as IValueMatcher;
            if (valueMatcher == null)
                return;

            var instance = valueMatcher.Value;
            if (instance == null)
                return;
            if (MockingProxy.CanIntercept(instance, method))
                return;

            if (!(instance is IMockMixin) || !method.IsInheritable())
                ProfilerInterceptor.ThrowElevatedMockingException(method);
        }

        internal void EnableInterception(Type typeToIntercept)
        {
            if (ProfilerInterceptor.IsProfilerAttached)
            {
                for (var type = typeToIntercept; type != null;)
                {
                    if (!ProfilerInterceptor.TypeSupportsInstrumentation(type))
                        DebugView.TraceEvent(IndentLevel.Warning, () => String.Format("Elevated mocking for type {0} will not be available due to limitations in CLR", type));

                    if (this.arrangedTypes.Add(type))
                        ProfilerInterceptor.EnableInterception(type, true, this);

                    type = type.BaseType;

                    if (type == typeof(object) || type == typeof(ValueType) || type == typeof(Enum))
                        break;
                }

                var handle = typeToIntercept.TypeHandle;
                this.disabledTypes.Add(typeof(RuntimeHelpers));
                ProfilerInterceptor.RunClassConstructor(handle);
                this.disabledTypes.Remove(typeof(RuntimeHelpers));
            }
        }

        internal void DisableInterception(Type typeToIntercept)
        {
            if (ProfilerInterceptor.IsProfilerAttached)
            {
                if (this.arrangedTypes.Remove(typeToIntercept))
                {
                    ProfilerInterceptor.EnableInterception(typeToIntercept, false, this);
                }

                this.disabledTypes.Add(typeToIntercept);
            }
        }

        private List<MethodMockMatcherTreeNode> GetMethodMocksFromObject(object mock, Type mockType = null)
        {
            MockingUtil.UnwrapDelegateTarget(ref mock);
            var methodMocks = new List<MethodMockMatcherTreeNode>();
            var visitedMocks = new List<object>(); // can't be HashSet because we can't depend on GetHashCode being implemented properly
            GetMethodMocksFromObjectInternal(mock, mockType, methodMocks, visitedMocks);
            return methodMocks;
        }

        private void GetMethodMocksFromObjectInternal(object mock, Type mockType, List<MethodMockMatcherTreeNode> methodMocks, List<object> visitedMocks)
        {
            if (visitedMocks.Contains(mock))
                return;
            visitedMocks.Add(mock);

            IMatcher instanceMatcher;
            Func<MethodInfoMatcherTreeNode, bool> rootMatcher;
            if (mockType != null)
            {
                instanceMatcher = new AnyMatcher();
                rootMatcher = node => mockType.IsAssignableFrom(node.MethodInfo.DeclaringType);
            }
            else
            {
                instanceMatcher = new ValueMatcher(mock);
                rootMatcher = node =>
                {
                    foreach (var child in node.Children)
                    {
                        if (child.Matcher.Matches(instanceMatcher))
                        {
                            return true;
                        }
                    }
                    return false;
                };
            }

            foreach (var funcRoot in arrangementTreeRoots.Values.Where(rootMatcher))
            {
                var callPattern = CallPattern.CreateUniversalCallPattern(funcRoot.MethodInfo);
                callPattern.InstanceMatcher = instanceMatcher;

                var results = funcRoot.GetAllMethodMocks(callPattern);
                methodMocks.AddRange(results);
            }

            if (mock != null)
            {
                var mockMixin = GetMockMixin(mock, null);
                if (mockMixin != null)
                {
                    foreach (var dependentMock in mockMixin.DependentMocks)
                        GetMethodMocksFromObjectInternal(dependentMock, null, methodMocks, visitedMocks);
                }
            }
        }

        private bool DispatchInvocationToMethodMocks(Invocation invocation)
        {
            CallPattern callPattern = CallPatternCreator.FromInvocation(invocation);

            MethodInfoMatcherTreeNode funcRoot = null;
            if (!invocation.InArrange && !invocation.InAssertSet)
            {
                if (!invocationTreeRoots.TryGetValue(callPattern.Method, out funcRoot))
                {
                    funcRoot = new MethodInfoMatcherTreeNode(callPattern.Method);
                    invocationTreeRoots.Add(callPattern.Method, funcRoot);
                }
            }

            var methodMock = DispatchInvocationToArrangements(callPattern, invocation);

            if (!invocation.InArrange && !invocation.InAssertSet)
            {
                funcRoot.AddOrUpdateOccurence(callPattern, methodMock);
            }

            return methodMock != null;
        }

        private IMethodMock DispatchInvocationToArrangements(CallPattern callPattern, Invocation invocation)
        {
            MethodInfoMatcherTreeNode arrangeFuncRoot;
            var methodMockNodes = new List<MethodMockMatcherTreeNode>();

            var allMethods = new[] { callPattern.Method }
                .Concat(callPattern.Method.GetInheritanceChain().Where(m => m.DeclaringType.IsInterface));
            foreach (var method in allMethods)
            {
                DebugView.TraceEvent(IndentLevel.MethodMatch, () => String.Format("Inspect arrangements on {0} on {1}", method, method.DeclaringType));
                if (!arrangementTreeRoots.TryGetValue(method, out arrangeFuncRoot))
                    continue;

                var results = arrangeFuncRoot.GetMethodMock(callPattern);
                methodMockNodes.AddRange(results);
            }

            var methodMock = GetMethodMockFromNodes(methodMockNodes, invocation);
            if (methodMock == null)
            {
                DebugView.TraceEvent(IndentLevel.MethodMatch, () => "No arrangement chosen");
                return null;
            }

            DebugView.TraceEvent(IndentLevel.MethodMatch, () => String.Format("Chosen arrangement (id={0}) {1}",
                methodMockNodes.First(node => node.MethodMock == methodMock).Id, methodMock.ArrangementExpression));

            methodMock.IsUsed = true; //used to correctly determine inSequence arranges

            var behaviorsToProcess = GetBehaviorsToProcess(invocation, methodMock);
            foreach (var behavior in behaviorsToProcess)
            {
                behavior.Process(invocation);
            }

            return methodMock;
        }

        private static List<IBehavior> GetBehaviorsToProcess(Invocation invocation, IMethodMock methodMock)
        {
            var behaviorsToExecute = new List<IBehavior>();

            var behaviorTypesToSkip = GetBehaviorTypesToSkip(invocation);
            behaviorsToExecute.AddRange(
                methodMock.Behaviors.Where(behavior => !behaviorTypesToSkip.Contains(behavior.GetType())));

            var mock = invocation.MockMixin;
            if (mock != null)
            {
                behaviorsToExecute.AddRange(mock.SupplementaryBehaviors);

#if !PORTABLE
                // explicitly add recursive mocking behavior for ref returns in order to set invocation result
                if (invocation.Method.GetReturnType().IsByRef)
                {
                    behaviorsToExecute.AddRange(
                        mock.FallbackBehaviors.Where(
                            behavior =>
                                behavior is CallOriginalBehavior
                                || (behavior is RecursiveMockingBehavior && ((RecursiveMockingBehavior)behavior).Type != RecursiveMockingBehaviorType.OnlyDuringAnalysis)));
                }
#endif
            }

            return behaviorsToExecute;
        }

        private static List<Type> GetBehaviorTypesToSkip(Invocation invocation)
        {
            var behaviorTypesToSkip = new List<Type>();

            if (invocation.InAssertSet)
            {
                behaviorTypesToSkip.Add(typeof(InvocationOccurrenceBehavior));
            }

            return behaviorTypesToSkip;
        }

        private bool TryCreateDelegate(Type type, MockCreationSettings settings, out object delegateResult)
        {
            delegateResult = null;
            if (!typeof(Delegate).IsAssignableFrom(type) || type == typeof(Delegate) || type == typeof(MulticastDelegate))
                return false;

            var backendType = mockFactory.CreateDelegateBackend(type);
            var backend = Create(backendType, settings);

            delegateResult = Delegate.CreateDelegate(type, backend, backendType.GetMethod("Invoke"));
            return true;
        }

        private IMethodMock GetMethodMockFromNodes(List<MethodMockMatcherTreeNode> methodMockNodes, Invocation invocation)
        {
            if (methodMockNodes.Count == 0)
                return null;

            var resultsQ =
                from node in methodMockNodes
                orderby node.Id
                select new
                {
                    node.MethodMock,
                    Acceptable = new Lazy<bool>(() => node.MethodMock.AcceptCondition == null || (bool)node.MethodMock.AcceptCondition.CallOverride(invocation))
                };

            var resultList = resultsQ.ToList();

            var nonSequential = resultList.Where(x => !x.MethodMock.IsSequential && x.Acceptable).LastOrDefault();
            if (nonSequential != null)
                return nonSequential.MethodMock;

            var sequential = resultList.Where(x => x.MethodMock.IsSequential && !x.MethodMock.IsUsed && x.Acceptable).FirstOrDefault();
            if (sequential != null)
                return sequential.MethodMock;

            return resultList.Where(x => x.MethodMock.IsSequential && x.Acceptable).Select(x => x.MethodMock).LastOrDefault();
        }

        internal string GetDebugView(object mock = null)
        {
            IEnumerable<MethodMockMatcherTreeNode> nodes;
            if (mock != null)
            {
                nodes = GetMethodMocksFromObject(mock);
            }
            else
            {
                nodes = from kvp in this.arrangementTreeRoots
                        let universalCP = CallPattern.CreateUniversalCallPattern(kvp.Key)
                        from node in kvp.Value.GetAllMethodMocks(universalCP)
                        select node;
            }

            var sb = new StringBuilder();
            sb.AppendFormat("Elevated mocking: {0}\n", ProfilerInterceptor.IsProfilerAttached ? "enabled" : "disabled");

            sb.AppendLine("\nArrangements and expectations:");
            bool addedStuff = false;
            foreach (var node in nodes)
            {
                addedStuff = true;
                var methodMock = node.MethodMock;
                sb.AppendFormat("    Arrangement (id={1}) {0}:\n", methodMock.ArrangementExpression, node.Id);
                foreach (var behavior in methodMock.Behaviors.OfType<IAssertableBehavior>())
                {
                    var debugView = behavior.DebugView;
                    if (debugView != null)
                        sb.AppendFormat("        {0}\n", debugView);
                }
            }
            if (!addedStuff)
                sb.AppendLine("    --none--");

            sb.AppendLine("\nInvocations:");
            var invocations =
                (from kvp in this.invocationTreeRoots
                 let universalCP = CallPattern.CreateUniversalCallPattern(kvp.Key)
                 from node in kvp.Value.GetOccurences(universalCP)
                 select node).ToList();

            addedStuff = false;
            foreach (var str in invocations.Select(inv => inv.GetDebugView()).OrderBy(_ => _))
            {
                addedStuff = true;
                sb.AppendFormat("    {0}\n", str);
            }
            if (!addedStuff)
                sb.AppendLine("    --none--");

            return sb.ToString();
        }

        /// <summary>
        /// An implementation detail. Not intended for external usage.
        /// Use this class for creating baseless proxies instead of typeof(object)
        /// so that you don't accidentally enable the interception of Object which kills performance
        /// </summary>
        public abstract class ExternalMockMixin
        { }
    }
}
