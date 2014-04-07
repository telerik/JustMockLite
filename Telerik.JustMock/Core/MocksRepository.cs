/*
 JustMock Lite
 Copyright © 2010-2014 Telerik AD

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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Telerik.JustMock.Core.Behaviors;
using Telerik.JustMock.Core.Castle.DynamicProxy;
using Telerik.JustMock.Core.Castle.DynamicProxy.Generators;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Core.Expressions;
using Telerik.JustMock.Core.MatcherTree;
using Telerik.JustMock.Core.Recording;
using Telerik.JustMock.Diagnostics;

namespace Telerik.JustMock.Core
{
	internal class MockCreationSettings
	{
		public object[] Args;
		public IEnumerable<object> Mixins;
		public IEnumerable<IBehavior> SupplementaryBehaviors;
		public IEnumerable<IBehavior> FallbackBehaviors;
		public Type[] AdditionalMockedInterfaces;
		public bool MockConstructorCall;
		public bool MustCreateProxy;
		public IEnumerable<CustomAttributeBuilder> AdditionalProxyTypeAttributes;
		public Expression<Predicate<MethodInfo>> InterceptorFilter;
	}

	public sealed class MocksRepository
	{
		private static readonly List<KeyValuePair<object, IMockMixin>> externalMixinDatabase = new List<KeyValuePair<object,IMockMixin>>();
		private static readonly Dictionary<object, IMockMixin> externalReferenceMixinDatabase = new Dictionary<object, IMockMixin>(ByRefComparer<object>.Instance);
		
		private readonly Dictionary<Type, IMockMixin> staticMixinDatabase = new Dictionary<Type, IMockMixin>();

		internal static readonly HashSet<Type> KnownUnmockableTypes = new HashSet<Type>
			{
				typeof(ValueType),
				typeof(Enum),
				typeof(Delegate),
				typeof(MulticastDelegate),
				typeof(Array),
				typeof(String),
				typeof(AppDomain),
				typeof(IntPtr),
				typeof(UIntPtr),
				typeof(TypedReference),
				typeof(void),
#if !SILVERLIGHT
				Type.GetType("System.__ComObject"),
#endif
				Type.GetType("System.ContextBoundObject"),
				Type.GetType("System.ArgIterator"),
				typeof(RuntimeArgumentHandle),
#if SILVERLIGHT
				typeof(WeakReference),
#endif
			};

		internal static IMockMixin GetMockMixin(object obj, Type objType)
		{
			var asMixin = obj as IMockMixin;
			if (asMixin != null)
				return asMixin;

			if (obj != null && objType == null)
				objType = obj.GetType();

			if (obj != null)
			{
				asMixin = GetMixinFromExternalDatabase(obj, objType);
			}
			else if (objType != null)
			{
				var repo = MockingContext.ResolveRepository(UnresolvedContextBehavior.CreateNewContextual);
				if (repo != null)
				{
					lock (repo.staticMixinDatabase)
						repo.staticMixinDatabase.TryGetValue(objType, out asMixin);
				}
			}
			return asMixin;
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
  
		internal static IMockMixin GetMockMixinFromInvocation(Invocation invocation)
		{
			return GetMockMixin(invocation.Instance, invocation.Method.DeclaringType);
		}

		private static readonly ProxyGenerator generator;
		private readonly Dictionary<MethodBase, MethodInfoMatcherTreeNode> arrangementTreeRoots = new Dictionary<MethodBase, MethodInfoMatcherTreeNode>();
		private readonly Dictionary<MethodBase, MethodInfoMatcherTreeNode> invocationTreeRoots = new Dictionary<MethodBase, MethodInfoMatcherTreeNode>();
		private readonly Dictionary<KeyValuePair<object, object>, object> valueStore = new Dictionary<KeyValuePair<object, object>, object>();
		private readonly HashSet<Type> arrangedTypes = new HashSet<Type>();
		private readonly HashSet<Type> disabledTypes = new HashSet<Type>();
		private readonly HashSet<MethodBase> globallyInterceptedMethods = new HashSet<MethodBase>();

		private readonly List<IMatcher> matchersInContext = new List<IMatcher>();

		private readonly RepositorySharedContext sharedContext;

		private readonly MocksRepository parentRepository;

		private readonly MethodBase method;

		private readonly List<WeakReference> controlledMocks = new List<WeakReference>();

		private readonly Thread creatingThread;

		internal IRecorder Recorder
		{
			get { return this.sharedContext.Recorder; }
		}

		private bool isRetired;
		internal bool IsRetired
		{
			get
			{
				return this.isRetired 
						|| !this.creatingThread.IsAlive
						|| (this.parentRepository != null && this.parentRepository.IsRetired);
			}
			set
			{
				this.isRetired = value;
			}
		}

		internal bool IsParentToAnotherRepository { get; private set; }

		internal MethodBase Method { get { return this.method; } }

		static MocksRepository()
		{
#if !SILVERLIGHT
			var badApples = new[]
			{
				typeof(System.Security.Permissions.SecurityAttribute),
				typeof(System.Runtime.InteropServices.MarshalAsAttribute),
				typeof(object).Assembly.GetType("System.Runtime.InteropServices.TypeIdentifierAttribute"),
			};

			foreach (var unmockableAttr in badApples.Where(t => t != null))
				AttributesToAvoidReplicating.Add(unmockableAttr);
#endif

			generator = new ProxyGenerator();

			ProfilerInterceptor.Initialize();
		}

		internal MocksRepository(MocksRepository parentRepository, MethodBase method)
		{
			this.method = method;
			this.creatingThread = Thread.CurrentThread;
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
		}
  
		private MethodInfoMatcherTreeNode DeepCopy(MethodInfoMatcherTreeNode root)
		{
			var newRoot = (MethodInfoMatcherTreeNode) root.Clone();
			Queue<MatcherNodeAndParent> queue = new Queue<MatcherNodeAndParent>();
			foreach(var child in root.Children)
				queue.Enqueue(new MatcherNodeAndParent(child, newRoot));

			while (queue.Count > 0)
			{
				var current = queue.Dequeue();
				var newCurrent = current.Node.Clone();
				foreach (var node in current.Node.Children)
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
					this.arrangementTreeRoots.Add(root.Key, DeepCopy(root.Value));
				foreach (var root in parentRepository.invocationTreeRoots)
					this.invocationTreeRoots.Add(root.Key, DeepCopy(root.Value));
				foreach (var kvp in parentRepository.valueStore)
					this.valueStore.Add(kvp.Key, kvp.Value);

				foreach (var mockRef in parentRepository.controlledMocks)
				{
					var mixin = mockRef.Target as IMockMixin;
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
			if (globallyInterceptedMethods.Add(method))
			{
				ProfilerInterceptor.RegisterGlobalInterceptor(method, this);
			}
		}

		internal void Reset()
		{
			foreach (var type in this.arrangedTypes)
				ProfilerInterceptor.EnableInterception(type, false, this);
			this.arrangedTypes.Clear();
			this.staticMixinDatabase.Clear();
			foreach (var method in this.globallyInterceptedMethods)
				ProfilerInterceptor.UnregisterGlobalInterceptor(method, this);
			this.globallyInterceptedMethods.Clear();

			lock (externalMixinDatabase)
			{
				foreach (var mockRef in this.controlledMocks)
				{
					var mock = mockRef.Target as IMockMixin;
					if (mock != null && mock.ExternalizedMock != null && mock.Originator == this)
					{
						externalMixinDatabase.RemoveAll(kvp => kvp.Value == mock);
						externalReferenceMixinDatabase.Remove(mock.ExternalizedMock);
					}
				}
			}
			this.controlledMocks.Clear();

			CopyConfigurationFromParent();
		}

		internal void DispatchInvocation(Invocation invocation)
		{
			if (this.disabledTypes.Contains(invocation.Method.DeclaringType))
			{
				invocation.CallOriginal = true;
				return;
			}

			invocation.InArrange = this.sharedContext.InArrange;
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
				var mock = GetMockMixinFromInvocation(invocation);
				if (mock != null)
				{
					foreach (var behavior in mock.FallbackBehaviors)
						behavior.Process(invocation);
				}
				else
				{
					invocation.CallOriginal = true;
				}
			}

			if (!invocation.CallOriginal && !invocation.IsReturnValueSet && invocation.Method.GetReturnType() != typeof(void))
				invocation.ReturnValue = invocation.Method.GetReturnType().GetDefaultValue();
		}

		internal T GetValue<T>(object owner, object key, T dflt)
		{
			object value;
			if (valueStore.TryGetValue(new KeyValuePair<object, object>(owner, key), out value))
				return (T)value;
			else return dflt;
		}

		internal void StoreValue<T>(object owner, object key, T value)
		{
			valueStore[new KeyValuePair<object, object>(owner, key)] = value;
		}

		internal IDisposable StartRecording(IRecorder recorder, bool dispatchToMethodMocks)
		{
			return this.sharedContext.StartRecording(recorder, dispatchToMethodMocks);
		}

		internal void AddMatcherInContext(IMatcher matcher)
		{
			if (!this.sharedContext.InArrange || this.sharedContext.Recorder != null)
				this.matchersInContext.Add(matcher);
		}

		internal object Create(Type type, MockCreationSettings settings)
		{
			object delegateResult;
			if (TryCreateDelegate(type, settings, out delegateResult))
				return delegateResult;

			using (this.sharedContext.StartCreate())
			{
				bool isSafeMock = settings.FallbackBehaviors.OfType<CallOriginalBehavior>().Any();
				CheckIfCanMock(type, !isSafeMock);

				EnableInterception(type);

				var mockMixinImpl = CreateMockMixin(type, settings.SupplementaryBehaviors, settings.FallbackBehaviors, settings.MockConstructorCall);

				var options = new ProxyGenerationOptions();
				options.AddMixinInstance(mockMixinImpl);
				foreach (var mixin in settings.Mixins)
					options.AddMixinInstance(mixin);

				bool hasAdditionalProxyTypeAttributes = false;
				if (settings.AdditionalProxyTypeAttributes != null)
				{
					foreach (var attr in settings.AdditionalProxyTypeAttributes)
					{
						options.AdditionalAttributes.Add(attr);
						hasAdditionalProxyTypeAttributes = true;
					}
				}

				var ctors = type.GetConstructors();
				bool isCoclass = ctors.Any(ctor => ctor.IsExtern());

				bool hasAdditionalInterfaces = settings.AdditionalMockedInterfaces != null && settings.AdditionalMockedInterfaces.Length > 0;

				bool shouldCreateProxy = settings.MustCreateProxy
					|| hasAdditionalInterfaces
					|| isCoclass
					|| type.IsAbstract || type.IsInterface
					|| !ProfilerInterceptor.IsProfilerAttached || !ProfilerInterceptor.TypeSupportsInstrumentation(type)
					|| !settings.MockConstructorCall
					|| hasAdditionalProxyTypeAttributes;

				bool canCreateProxy = !type.IsSealed;
				Exception proxyFailure = null;
				object instance = null;
				if (canCreateProxy && shouldCreateProxy)
				{
					var interceptor = new DynamicProxyInterceptor(this);
#if SILVERLIGHT
					options.Hook = new ProxyGenerationHook(false, settings.InterceptorFilter);
#else
					options.Hook = new ProxyGenerationHook(settings.MockConstructorCall, settings.InterceptorFilter);
#endif

					if (type.IsInterface)
					{
						if (settings.Args != null && settings.Args.Length > 0)
							throw new ArgumentException("Do not supply contructor arguments when mocking an interface or delegate.");
						try
						{
							instance = generator.CreateInterfaceProxyWithoutTarget(type, settings.AdditionalMockedInterfaces, options, interceptor);
						}
						catch (TypeLoadException ex)
						{
							proxyFailure = ex;
						}
						catch (GeneratorException ex)
						{
							proxyFailure = ex;
						}
					}
					else
					{
						try
						{
#if SILVERLIGHT
							if (settings.Args == null || settings.Args.Length == 0)
							{
								ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

								if (!constructors.Any(constr => constr.GetParameters().Length == 0))
								{
									var constructorToCall = constructors.FirstOrDefault();
									if (constructorToCall != null)
									{
										var @params = constructorToCall.GetParameters();
										settings.Args = new object[@params.Length];

										for (int i = 0; i < @params.Length; ++i)
										{
											var p = @params[i];
											settings.Args[i] = Convert.IsDBNull(p.DefaultValue)
												? p.ParameterType.GetDefaultValue()
												: p.DefaultValue;
										}
									}
								}
							}
#endif


							instance = generator.CreateClassProxy(type, settings.AdditionalMockedInterfaces, options, settings.Args, interceptor);
						}
						catch (TypeLoadException ex)
						{
							proxyFailure = ex;
						}
						catch (GeneratorException ex)
						{
							proxyFailure = ex;
						}
						catch (InvalidProxyConstructorArgumentsException ex)
						{
							proxyFailure = ex;
							if (!settings.MockConstructorCall)
								throw new MockException(ex.Message);
						}
					}
				}
				var mockMixin = instance as IMockMixin;
				
				if (instance == null)
				{
					if (type.IsInterface || type.IsAbstract)
						throw new MockException(String.Format("Abstract type '{0}' is not accessible for inheritance.", type));

					if (hasAdditionalInterfaces)
						throw new MockException(String.Format("Type '{0}' is not accessible for inheritance. Cannot create mock object implementing the specified additional interfaces.", type));

					if (!ProfilerInterceptor.IsProfilerAttached)
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

					mockMixin = CreateExternalMockMixin(type, instance, settings.Mixins, settings.SupplementaryBehaviors, settings.FallbackBehaviors);
				}
				else
				{
					this.controlledMocks.Add(new WeakReference(instance));
				}

				if (type.IsClass)
					GC.SuppressFinalize(instance);

				mockMixin.IsInstanceConstructorMocked = settings.MockConstructorCall;

				return instance;
			}
		}

		internal IMockMixin CreateExternalMockMixin(Type mockObjectType, object mockObject, IEnumerable<object> mixins,
			IEnumerable<IBehavior> supplementaryBehaviors, IEnumerable<IBehavior> fallbackBehaviors)
		{
			if (mockObjectType == null)
			{
				if (mockObject == null)
					throw new ArgumentNullException("mockObject");
				mockObjectType = mockObject.GetType();
			}

			EnableInterception(mockObjectType);

			if (mockObject == null)
				throw new MockException(String.Format("Failed to create instance of type '{0}'", mockObjectType));

			var mockMixin = CreateMockMixin(mockObjectType, supplementaryBehaviors, fallbackBehaviors, false);

			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(mockMixin);
			foreach (var mixin in mixins)
				options.AddMixinInstance(mixin);

			var compoundMockMixin = (IMockMixin)generator.CreateClassProxy(typeof(ExternalMockMixin), options);
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

		private void CheckIfCanMock(Type type, bool checkSafety)
		{
			if (KnownUnmockableTypes.Contains(type)
				|| typeof(Delegate).IsAssignableFrom(type))
				throw new MockException("Cannot create mock for type due to CLR limitations.");

			if (checkSafety)
				ProfilerInterceptor.CheckIfSafeToInterceptWholesale(type);
		}

		internal void InterceptStatics(Type type, IEnumerable<object> mixins, IEnumerable<IBehavior> supplementaryBehaviors,
			IEnumerable<IBehavior> fallbackBehaviors, bool mockStaticConstructor)
		{
			if (!ProfilerInterceptor.IsProfilerAttached)
				ProfilerInterceptor.ThrowElevatedMockingException(type);

			if (!fallbackBehaviors.OfType<CallOriginalBehavior>().Any())
				ProfilerInterceptor.CheckIfSafeToInterceptWholesale(type);

			var mockMixin = (IMockMixin)Create(typeof(ExternalMockMixin),
				new MockCreationSettings
				{
					Mixins = mixins,
					SupplementaryBehaviors = supplementaryBehaviors,
					FallbackBehaviors = fallbackBehaviors,
					MustCreateProxy = true,
				});

			mockMixin.IsStaticConstructorMocked = mockStaticConstructor;
			lock (staticMixinDatabase)
				staticMixinDatabase[type] = mockMixin;

			EnableInterception(type);
		}
  
		private MockMixin CreateMockMixin(Type declaringType, IEnumerable<IBehavior> supplementaryBehaviors, IEnumerable<IBehavior> fallbackBehaviors, bool mockConstructorCall)
		{
			var mockMixin = new MockMixin
			{
				Repository = this,
				DeclaringType = declaringType,
				IsInstanceConstructorMocked = mockConstructorCall,
			};

			foreach (var behavior in supplementaryBehaviors)
				mockMixin.SupplementaryBehaviors.Add(behavior);
			foreach (var behavior in fallbackBehaviors)
				mockMixin.FallbackBehaviors.Add(behavior);

			return mockMixin;
		}

		[ArrangeMethod]
		internal TMethodMock Arrange<TMethodMock>(Expression expr, Func<TMethodMock> methodMockFactory)
			where TMethodMock : IMethodMock
		{
			using (this.sharedContext.StartArrange())
			{
				var result = methodMockFactory();
				result.Repository = this;
				result.ArrangementExpression = ExpressionUtil.ConvertMockExpressionToString(expr);

				var callPattern = new CallPattern();
				result.CallPattern = callPattern;
				ConvertExpressionToCallPattern(expr, callPattern);

				AddArrange(result);
				return result;
			}
		}

		[ArrangeMethod]
		internal TMethodMock Arrange<TMethodMock>(Action memberAction, Func<TMethodMock> methodMockFactory)
			where TMethodMock : IMethodMock
		{
			using (this.sharedContext.StartArrange())
			{
				var result = methodMockFactory();
				result.Repository = this;
				result.CallPattern = ConvertActionToCallPattern(memberAction);
				AddArrange(result);
				return result;
			}
		}

		[ArrangeMethod]
		internal TMethodMock Arrange<TMethodMock>(object instance, MethodInfo method, object[] arguments, Func<TMethodMock> methodMockFactory)
			where TMethodMock : IMethodMock
		{
			using (this.sharedContext.StartArrange())
			{
				var result = methodMockFactory();
				result.Repository = this;
				result.CallPattern = ConvertMethodInfoToCallPattern(instance, method, arguments);

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

		internal void AssertAll(object mock)
		{
			using (MockingContext.BeginFailureAggregation())
			{
				var mocks = GetMethodMocksFromObject(mock);
				AssertBehaviorsForMocks(mocks.Select(m => m.MethodMock), false);
			}
		}

		internal void Assert(object mock, Expression expr = null, Args args = null, Occurs occurs = null)
		{
			using (MockingContext.BeginFailureAggregation())
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
				}
				else
				{
					var callPattern = new CallPattern();
					ConvertExpressionToCallPattern(expr, callPattern);
					AssertForCallPattern(callPattern, args, occurs);
				}
			}
		}

		internal void AssertAction(Action memberAction, Args args = null, Occurs occurs = null)
		{
			using (MockingContext.BeginFailureAggregation())
			{
				var callPattern = ConvertActionToCallPattern(memberAction);
				AssertForCallPattern(callPattern, args, occurs);
			}
		}

		internal void AssertMethodInfo(object instance, MethodInfo method, object[] arguments, Occurs occurs)
		{
			using (MockingContext.BeginFailureAggregation())
			{
				var callPattern = ConvertMethodInfoToCallPattern(instance, method, arguments);
				AssertForCallPattern(callPattern, null, occurs);
			}
		}

#if VISUALBASIC
		internal Expression<Action> ConvertDelegateAndArgumentsToExpression(Delegate delg, object[] args)
		{
			var method = delg.Method;
			var declaringType = method.DeclaringType;
			if (declaringType.IsProxy())
				method = (MethodInfo)method.GetInheritanceChain().ElementAt(1);

			if (method.IsCompilerGenerated())
				throw new ArgumentException("The argument appears to be an anonymous delegate or lambda instead of a reference to the intended method. Use AddressOf instead of Sub() to make a reference to the method intended. If you're using Visual Studio 2010 or later then reference and use Telerik.JustMock.dll instead of Telerik.JustJustMock.VisualBasic.dll");

			var matchers = this.matchersInContext;
			if (matchers.Count != 0 && matchers.Count != args.Length)
				throw new ArgumentException("When arranging or asserting using delegates instead of expressions, use either only matchers or only values as formal arguments.");

			var arrangementParameters = new Expression[args.Length];
			var methodParameters = method.GetParameters();
			for (int i = 0; i < args.Length; i++)
			{
				var arg = args[i];
				Type type = (arg != null) ? arg.GetType() : methodParameters[i].ParameterType;
				arrangementParameters[i] = matchers.Count == 0
					? Expression.Constant(arg, type)
					: matchers[i].ToExpression(type);
			}

			MethodCallExpression expression = delg.Target == null
				? Expression.Call(method, arrangementParameters)
				: Expression.Call(Expression.Constant(delg.Target), method, arrangementParameters);

			matchers.Clear();

			var action = (Expression<Action>)Expression.Lambda(expression);
			return action;
		}
#endif

		private void AssertForCallPattern(CallPattern callPattern, Args args, Occurs occurs)
		{
			if (callPattern.IsDerivedFromObjectEquals)
				throw new MockException("Cannot assert calls to methods derived from Object.Equals");

			PreserveRefOutValuesBehavior.ReplaceRefOutArgsWithAnyMatcher(callPattern);
			if (args != null)
			{
				if (args.IsIgnored)
				{
					for (int i = 0; i < callPattern.ArgumentMatchers.Count; i++)
						callPattern.ArgumentMatchers[i] = new AnyMatcher();
				}

				if (args.IsInstanceIgnored)
				{
					callPattern.InstanceMatcher = new AnyMatcher();
				}
			}

			MethodInfoMatcherTreeNode root;
			int callsCount = 0;
			var mocks = new HashSet<IMethodMock>();
			var method = GetMethodFromCallPattern(callPattern);
			if (invocationTreeRoots.TryGetValue(method, out root))
			{
				var occurences = root.GetOccurences(callPattern);
				callsCount = occurences.Select(x => x.Calls).Sum();
				foreach (var mock in occurences.SelectMany(x => x.Mocks))
				{
					mocks.Add(mock);
				}
			}

			if (occurs != null)
			{
				InvocationOccurrenceBehavior.Assert(occurs.LowerBound, occurs.UpperBound, callsCount, null);
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
				InvocationOccurrenceBehavior.Assert(Occurs.AtLeastOnce().LowerBound, Occurs.AtLeastOnce().UpperBound, callsCount, null);
			}
			else
			{
				AssertBehaviorsForMocks(mocks, occurs != null);
			}
		}
  
		private MethodBase GetMethodFromCallPattern(CallPattern callPattern)
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
					method = MockingUtil.GetConcreteImplementer(method, type);
				}
			}

			if (method.DeclaringType != method.ReflectedType)
				method = (MethodInfo) MethodBase.GetMethodFromHandle(method.MethodHandle, method.DeclaringType.TypeHandle);

			return method;
		}

		private void ConvertInvocationToCallPattern(Invocation invocation, CallPattern callPattern)
		{
			callPattern.Method = invocation.Method;
			callPattern.InstanceMatcher = new ReferenceMatcher(invocation.Instance);

			var args = invocation.Args;
			foreach (var argument in args)
			{
				callPattern.ArgumentMatchers.Add(new ValueMatcher(argument));
			}

			callPattern.AdjustForExtensionMethod();
		}

		private CallPattern ConvertActionToCallPattern(Action memberAction)
		{
			var callPattern = new CallPattern();

			Invocation lastInvocation = null;

			var recorder = new DelegatingRecorder();
			recorder.Record += invocation => lastInvocation = invocation;
			using (this.StartRecording(recorder, false))
			{
				memberAction();
			}

			if (lastInvocation == null)
				throw new MockException("The specified action did not call a mocked method.");

			callPattern.Method = lastInvocation.Method;
			callPattern.InstanceMatcher = new ReferenceMatcher(lastInvocation.Instance);

			var matchers = this.matchersInContext;
			// Because it's impossible to distinguish between a literal value passed as an argument and
			// one coming from a matcher, it is impossible to tell exactly which arguments are literal and which are matchers.
			// So, we assume that the user always first specifies some literal values, and then some matchers.
			// We assume that the user will never pass a literal after a matcher.
			for (int i = 0; i < lastInvocation.Args.Length; ++i)
			{
				var indexInMatchers = i - (lastInvocation.Args.Length - matchers.Count);
				var matcher = indexInMatchers >= 0 ? matchers[indexInMatchers] : new ValueMatcher(lastInvocation.Args[i]);
				callPattern.ArgumentMatchers.Add(matcher);
			}
			matchers.Clear();

			callPattern.AdjustForExtensionMethod();

			return callPattern;
		}

		internal void ConvertExpressionToCallPattern(Expression expr, CallPattern callPattern)
		{
			expr = ((LambdaExpression)expr).Body;

			// the expression may end with a boxing conversion, remove that
			while (expr.NodeType == ExpressionType.Convert)
				expr = ((UnaryExpression)expr).Operand;

			Expression target;
			MethodBase method = null;
			Expression[] args;

			// We're parsing either a property/field expression or a method call.
			// parse the top of the expression tree and extract the expressions
			// that will turn into the constituents of the call pattern.
			if (expr is MemberExpression)
			{
				var memberExpr = (MemberExpression)expr;
				if (!(memberExpr.Member is PropertyInfo))
					throw new MockException("Fields cannot be mocked, only properties.");

				var property = (PropertyInfo)memberExpr.Member;
				target = memberExpr.Expression;
				method = property.GetGetMethod(true);
				args = null;
			}
			else if (expr is MethodCallExpression)
			{
				var methodCall = (MethodCallExpression)expr;

				method = methodCall.Method;
				target = methodCall.Object;
				args = methodCall.Arguments.ToArray();

				if (target != null && !target.Type.IsInterface && !target.Type.IsProxy() && target.Type != method.DeclaringType)
					method = MockingUtil.GetConcreteImplementer((MethodInfo)method, target.Type);
			}
			else if (expr is NewExpression)
			{
				var newExpr = (NewExpression)expr;

				method = newExpr.Constructor;
				target = null;
				args = newExpr.Arguments.ToArray();
			}
			else if (expr is InvocationExpression)
			{
				var invocation = (InvocationExpression)expr;
				target = invocation.Expression;
				args = invocation.Arguments.ToArray();
			}
			else throw new MockException("The expression does not represent a method call, property access, new expression or a delegate invocation.");

			// Create the matcher for the instance part of the call pattern.
			// If the base of the target expression is a new expression (new T()),
			// or null (e.g. (null as T) or ((T) null)), then use AnyMatcher for the instance part,
			// otherwise evaluate the instance expression and use a value matcher with the evaluated result.
			var rootTarget = expr;
			Expression prevToRoot = null;
			while (true)
			{
				var memberExpr = rootTarget as MemberExpression;
				if (memberExpr != null && memberExpr.Expression != null && memberExpr.Member is PropertyInfo)
				{
					prevToRoot = rootTarget;
					rootTarget = memberExpr.Expression;
					continue;
				}

				var callExpr = rootTarget as MethodCallExpression;
				if (callExpr != null && callExpr.Object != null)
				{
					prevToRoot = rootTarget;
					rootTarget = callExpr.Object;
					continue;
				}

				if (rootTarget != null && (rootTarget.NodeType == ExpressionType.Convert || rootTarget.NodeType == ExpressionType.TypeAs))
				{
					rootTarget = ((UnaryExpression)rootTarget).Operand;
					continue;
				}

				if (rootTarget is InvocationExpression)
				{
					prevToRoot = rootTarget;
					rootTarget = ((InvocationExpression)rootTarget).Expression;
					continue;
				}

				break;
			}

			object targetMockObject = null;
			Type targetMockType = null;
			bool isStatic = false;
			var rootMatcher = TryCreateMatcherFromArgMember(rootTarget);

			if (rootMatcher != null)
			{
				callPattern.InstanceMatcher = rootMatcher;
			}
			else if (rootTarget is MemberExpression)
			{
				var memberExpr = (MemberExpression)rootTarget;
				targetMockObject = memberExpr.Member is FieldInfo ? memberExpr.EvaluateExpression()
					: memberExpr.Expression != null ? memberExpr.Expression.EvaluateExpression()
					: null;
				targetMockType = memberExpr.Member is FieldInfo ? memberExpr.Type : memberExpr.Member.DeclaringType;

				var asPropertyInfo = memberExpr.Member as PropertyInfo;
				isStatic = asPropertyInfo != null
					? (asPropertyInfo.GetGetMethod(true) ?? asPropertyInfo.GetSetMethod(true)).IsStatic
					: false;
			}
			else if (rootTarget is MethodCallExpression)
			{
				var methodCallExpr = (MethodCallExpression)rootTarget;
				targetMockObject = methodCallExpr.Object != null ? methodCallExpr.Object.EvaluateExpression() : null;
				targetMockType = methodCallExpr.Method.DeclaringType;
				isStatic = methodCallExpr.Method.IsStatic;
			}
			else if (rootTarget is NewExpression)
			{
				callPattern.InstanceMatcher = new AnyMatcher();
			}
			else if (rootTarget is ConstantExpression)
			{
				var constant = (ConstantExpression)rootTarget;
				if (constant.Value == null)
					callPattern.InstanceMatcher = new AnyMatcher();
				else
				{
					if (constant.Type.IsCompilerGenerated() && prevToRoot != null && prevToRoot.Type != typeof(void))
					{
						targetMockObject = prevToRoot.EvaluateExpression();
						targetMockType = prevToRoot.Type;
					}
					else
					{
						targetMockObject = constant.Value;
						targetMockType = constant.Type;
					}
				}
			}

			if (callPattern.InstanceMatcher != null && prevToRoot != expr && prevToRoot != null)
			{
				throw new MockException("Using a matcher for the root member together with recursive mocking is not supported. Arrange the property or method of the root member in a separate statement.");
			}

			if (callPattern.InstanceMatcher == null)
			{
				// TODO: implicit creation of mock mixins shouldn't explicitly refer to behaviors, but
				// should get them from some configuration made outside the Core.
				Debug.Assert(targetMockObject != null || targetMockType != null);
				UnwrapDelegateTarget(ref targetMockObject);
				var mixin = GetMockMixin(targetMockObject, targetMockType);
				if (mixin == null)
				{
					if (isStatic)
					{
						this.InterceptStatics(targetMockType, Behavior.CallOriginal, false);
					}
					else if (targetMockObject != null)
					{
						this.CreateExternalMockMixin(targetMockType, targetMockObject, Behavior.CallOriginal);
					}
				}

				var targetValue = target != null ? target.EvaluateExpression() : null;

				var delgMethod = UnwrapDelegateTarget(ref targetValue);
				if (delgMethod != null)
					method = delgMethod.GetInheritanceChain().First(m => !m.DeclaringType.IsProxy());

				callPattern.InstanceMatcher = new ReferenceMatcher(targetValue);
			}

			// now we have the method part of the call pattern
			Debug.Assert(method != null);
			callPattern.Method = method;

			//Finally, construct the arguments part of the call pattern.
			bool hasParams = false;
			bool hasSingleValueInParams = false;
			if (args != null && args.Length > 0)
			{
				var lastParameter = method.GetParameters().Last();
				if (Attribute.IsDefined(lastParameter, typeof(ParamArrayAttribute)) && args.Last() is NewArrayExpression)
				{
					hasParams = true;
					var paramsArg = (NewArrayExpression) args.Last();
					args = args.Take(args.Length - 1).Concat(paramsArg.Expressions).ToArray();
					if (paramsArg.Expressions.Count == 1)
						hasSingleValueInParams = true;
				}
			
				foreach (var argument in args)
				{
					callPattern.ArgumentMatchers.Add(CreateMatcherForArgument(argument));
				}

				if (hasParams)
				{
					int paramsCount = method.GetParameters().Count();
					if (hasSingleValueInParams)
					{
						var matcher = callPattern.ArgumentMatchers[paramsCount - 1];
						var typeMatcher = matcher as ITypedMatcher;
						if (typeMatcher != null && typeMatcher.Type != method.GetParameters().Last().ParameterType)
						{
							callPattern.ArgumentMatchers[paramsCount - 1] = new ParamsMatcher(new IMatcher[] { matcher });
						}
					}
					else
					{
						var paramMatchers = callPattern.ArgumentMatchers.Skip(paramsCount - 1).Take(callPattern.ArgumentMatchers.Count - paramsCount+1);
						callPattern.ArgumentMatchers = callPattern.ArgumentMatchers.Take(paramsCount - 1).ToList();
						callPattern.ArgumentMatchers.Add(new ParamsMatcher(paramMatchers.ToArray()));
					}
				}
			}

			callPattern.AdjustForExtensionMethod();
		}

		private static MethodInfo UnwrapDelegateTarget(ref object obj)
		{
			var delg = obj as Delegate;
			obj = delg != null ? delg.Target : obj;
			return delg != null ? delg.Method : null;
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
		private static IMatcher CreateMatcherForArgument(object argumentObj)
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

		private static IMatcher TryCreateMatcherFromArgMember(Expression argExpr)
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
				else if (argAttribute.GetType() == typeof(RefArgAttribute))
				{
					var refCall = (MethodCallExpression)((MemberExpression)argExpr).Expression;
					var actualArg = refCall.Arguments[0];
					var argMatcher = CreateMatcherForArgument(actualArg);
					argMatcher.ProtectRefOut = true;
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

		private CallPattern ConvertMethodInfoToCallPattern(object instance, MethodInfo method, object[] arguments)
		{
			var callPattern = new CallPattern
			{
				InstanceMatcher =
					method.IsStatic ? new ReferenceMatcher(null)
					: instance == null ? (IMatcher)new AnyMatcher()
					: new ReferenceMatcher(instance),
				Method = method,
			};
			foreach (var arg in arguments)
				callPattern.ArgumentMatchers.Add(CreateMatcherForArgument(arg));

			callPattern.AdjustForExtensionMethod();

			return callPattern;
		}

		private void AddArrange(IMethodMock methodMock)
		{
			var method = GetMethodFromCallPattern(methodMock.CallPattern);

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
			bool instanceIsntProxy = valueMatcher != null && !(valueMatcher.Value is IMockMixin);

			if (!method.IsInheritable() || instanceIsntProxy)
				ProfilerInterceptor.ThrowElevatedMockingException(method);
		}

		internal void EnableInterception(Type typeToIntercept)
		{
			if (ProfilerInterceptor.IsProfilerAttached)
			{
				for (var type = typeToIntercept; type != null; )
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
				ProfilerInterceptor.GuardExternal(() => ProfilerInterceptor.RunClassConstructor(handle));
				this.disabledTypes.Remove(typeof(RuntimeHelpers));
			}
		}

		private List<MethodMockMatcherTreeNode> GetMethodMocksFromObject(object mock)
		{
			var methodMocks = new List<MethodMockMatcherTreeNode>();
			var visitedMocks = new List<object>(); // can't be HashSet because we can't depend on GetHashCode being implemented properly
			GetMethodMocksFromObjectInternal(mock, methodMocks, visitedMocks);
			return methodMocks;
		}

		private void GetMethodMocksFromObjectInternal(object mock, List<MethodMockMatcherTreeNode> methodMocks, List<object> visitedMocks)
		{
			if (visitedMocks.Contains(mock))
				return;
			visitedMocks.Add(mock);

			var instanceMatcher = new ValueMatcher(mock);
			foreach (var funcRoot in arrangementTreeRoots.Values.Where(x => x.Children[0].Matcher.Matches(instanceMatcher)))
			{
				var callPattern = CallPattern.CreateUniversalCallPattern(funcRoot.MethodInfo);
				callPattern.InstanceMatcher = instanceMatcher;

				var results = funcRoot.GetAllMethodMocks(callPattern);
				methodMocks.AddRange(results);
			}

			var mockMixin = GetMockMixin(mock, null);
			if (mockMixin != null)
			{
				foreach (var dependentMock in mockMixin.DependentMocks)
					GetMethodMocksFromObjectInternal(dependentMock, methodMocks, visitedMocks);
			}
		}

		private bool DispatchInvocationToMethodMocks(Invocation invocation)
		{
			var callPattern = new CallPattern();
			ConvertInvocationToCallPattern(invocation, callPattern);

			MethodInfoMatcherTreeNode funcRoot = null;
			if (!invocation.InArrange)
			{
			if (!invocationTreeRoots.TryGetValue(callPattern.Method, out funcRoot))
			{
				funcRoot = new MethodInfoMatcherTreeNode(callPattern.Method);
				invocationTreeRoots.Add(callPattern.Method, funcRoot);
			}
			}

			var methodMock = DispatchInvocationToArrangements(callPattern, invocation);

			if (!invocation.InArrange)
			{
			funcRoot.AddOrUpdateOccurence(callPattern, methodMock);
			}

			return methodMock != null;
		}

		private IMethodMock DispatchInvocationToArrangements(CallPattern callPattern, Invocation invocation)
		{
			MethodInfoMatcherTreeNode arrangeFuncRoot;
			var allMethods = callPattern.Method.GetInheritanceChain().ToList();
			var methodMockNodes = new List<MethodMockMatcherTreeNode>();

			foreach (var method in allMethods)
			{
				DebugView.TraceEvent(IndentLevel.MethodMatch, () => String.Format("Inspect arrangements on {0} on {1}", method, method.DeclaringType));
				if (!arrangementTreeRoots.TryGetValue(method, out arrangeFuncRoot))
					continue;

				var results = arrangeFuncRoot.GetMethodMock(callPattern);
				methodMockNodes.AddRange(results);
			}

			var methodMock = GetMethodMockFromNodes(methodMockNodes);
			if (methodMock == null)
			{
				DebugView.TraceEvent(IndentLevel.MethodMatch, () => "No arrangement chosen");
				return null;
			}

			DebugView.TraceEvent(IndentLevel.MethodMatch, () => String.Format("Chosen arrangement (id={0}) {1}",
				methodMockNodes.First(node => node.MethodMock == methodMock).Id, methodMock.ArrangementExpression));

			methodMock.IsUsed = true; //used to correctly determine inSequence arranges

			foreach (var behavior in methodMock.Behaviors)
				behavior.Process(invocation);

			var mock = GetMockMixinFromInvocation(invocation);
			if (mock != null)
			{
				foreach (var behavior in mock.SupplementaryBehaviors)
					behavior.Process(invocation);
			}
			return methodMock;
		}

		private bool TryCreateDelegate(Type type, MockCreationSettings settings, out object delegateResult)
		{
			delegateResult = null;
			if (!typeof(Delegate).IsAssignableFrom(type) || type == typeof(Delegate) || type == typeof(MulticastDelegate))
				return false;

			var moduleScope = generator.ProxyBuilder.ModuleScope;
			var moduleBuilder = moduleScope.ObtainDynamicModuleWithStrongName();

			var targetIntfName =
				"Castle.Proxies.Delegates." +
				type.ToString()
				.Replace('.', '_')
				.Replace(',', '`')
				.Replace("+", "__")
				.Replace("[", "``")
				.Replace("]", "``");

			var typeName = moduleScope.NamingScope.GetUniqueName(targetIntfName);
			var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Interface);

			var delegateInvoke = type.GetMethod("Invoke");
			typeBuilder.DefineMethod("Invoke", MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual,
				delegateInvoke.ReturnType, delegateInvoke.GetParameters().Select(p => p.ParameterType).ToArray());

			var backendType = typeBuilder.CreateType();
			var backend = Create(backendType, settings);

			delegateResult = Delegate.CreateDelegate(type, backend, backendType.GetMethod("Invoke"));
			return true;
		}

		private IMethodMock GetMethodMockFromNodes(List<MethodMockMatcherTreeNode> methodMockNodes)
		{
			if (methodMockNodes.Count == 0)
				return null;

			var resultsQ =
				from node in methodMockNodes
				orderby node.Id                           
				select new { node.MethodMock, Acceptable = new Lazy<bool>(node.MethodMock.AcceptCondition) };

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

		private class ProxyGenerationHook : IProxyGenerationHook, IConstructorGenerationHook
		{
			private readonly bool myMockConstructors;
			private readonly Expression<Predicate<MethodInfo>> myInterceptorFilter;
			private readonly Predicate<MethodInfo> myInterceptorFilterImpl;

			public ProxyGenerationHook(bool mockConstructors, Expression<Predicate<MethodInfo>> interceptorFilter)
			{
				myMockConstructors = mockConstructors;
				if (interceptorFilter != null)
				{
					myInterceptorFilter = interceptorFilter;
					myInterceptorFilterImpl = myInterceptorFilter.Compile();
				}
			}

			public void MethodsInspected()
			{
			}

			public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
			{
			}

			public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
			{
				if (Attribute.IsDefined(methodInfo.DeclaringType, typeof(MixinAttribute)))
					return false;

				bool profilerCannotIntercept = methodInfo.IsAbstract || methodInfo.IsExtern() || !ProfilerInterceptor.TypeSupportsInstrumentation(methodInfo.DeclaringType);

				if (ProfilerInterceptor.IsProfilerAttached && !profilerCannotIntercept)
					return false;

				return myInterceptorFilterImpl != null ? myInterceptorFilterImpl(methodInfo) : true;
			}

			public ProxyConstructorImplementation GetConstructorImplementation(ConstructorInfo constructorInfo, ConstructorImplementationAnalysis analysis)
			{
				return myMockConstructors ? ProxyConstructorImplementation.DoNotCallBase
					: analysis.IsBaseVisible ? ProxyConstructorImplementation.CallBase
					: ProxyConstructorImplementation.SkipConstructor;
			}

			public ProxyConstructorImplementation DefaultConstructorImplementation
			{
				get
				{
#if SILVERLIGHT
					return ProxyConstructorImplementation.SkipConstructor;
#else
					return myMockConstructors ? ProxyConstructorImplementation.DoNotCallBase : ProxyConstructorImplementation.SkipConstructor;
#endif
				}
			}

			#region Equality members

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;

				var other = (ProxyGenerationHook) obj;
				return this.myMockConstructors == other.myMockConstructors
					&& ((this.myInterceptorFilter == null && other.myInterceptorFilter == null)
						|| ExpressionComparer.AreEqual(this.myInterceptorFilter, other.myInterceptorFilter));
			}

			public override int GetHashCode()
			{
				return this.myMockConstructors.GetHashCode();
			}

			#endregion
		}

		// use this class for creating baseless proxies instead of typeof(object)
		// so that you don't accidentally enable the interception of Object which kills performance
		public abstract class ExternalMockMixin
		{ }
	}
}
