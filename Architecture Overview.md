# JustMock Developer Overview

JustMock is a compact, yet highly complex library. The following should at least provide some insight about the structure of the library and the some of the design decisions behind it.

Unlike other mocking libraries out there, JustMock must be able to function in the context of the JustMock profiler and the notion that everything, even basic stuff like `Enumerable.FirstOrDefault()` can have its behavior changed. The design of JustMock goes to great lengths to ensure that it can mock everything without pulling the rug from under its feet.

## Licensing
JustMock Lite is licensed under the Apache 2.0 OSS license. New third-party dependencies must not be used in violation of their license given this fact.

## Organization

At the top level JustMock is a single assembly. Having a single assembly for everything simplifies versioning and deployment. The assembly itself is broadly separated into two parts - the public API and the core implementation.

In principle, the core implementation is not tied to a particular frontend, or a particular form of the public API. In principle, any public API, for example, Moq's public API, can be re-implemented in terms of the primitives provided by the core. In practice, this is not possible, because there are always subtle semantic differences among libraries, so it's impossible to use a single core implementation and say that JustMock can be turned into a drop-in replacement for any mocking library just by reimplementing the public API. Yet, the design of JustMock is based on a separation between the core primitives and the form of the public API. There are some abstraction leakages (like Behavior - a public API component, leaking into the core), which could be eliminated with better design and refactoring.

The core implementation is contained in the `Core/` folder. Everything outside the Core folder is the friendly part of the public API of JustMock. The public API has very little functionality in itself and delegates almost everything to the Core.

Even though JustMock is a single assembly, it has several dependencies that arecompiled into it. The biggest are Castle.DynamicProxy and NInject. Essentially, the original projects were forked some time ago, stuffed into internal JustMock namespaces and made part of the JustMock assembly. This decision has some advantages and drawbacks:
* If the test project also depends on Castle or NInject, then there could be no version conflict - both versions of the dependency can work side-by-side. We had a problem where JustMock was tied to Unity 2, but the developer used Unity 3 in their project and the two assemblies couldn't work together.
* Bugfixes from the original project have to be back-ported - it's no longer a simple replacement of the dependent assembly.
* Plugins designed to work with either library no longer work, because the namespaces are changed to avoid conflicts.

## Architecture and Terminology

JustMock's code base takes care of three broad categories - contexts, calls and objects.

### Mocking contexts

Arrangements have a lifetime. In regular mocking libraries the lifetime of an arrangement is usually the same as the lifetime of the instance on which the arrangement is made. The lifetime of the mock instance is managed by the user - they can make a fresh instance for every test, or they can reuse an instance and its arrangements across tests. JustMock has a harder problem to deal with - not every arrangement has an associated instance, or the instance is beyond the user's control. The simplest example is static methods. At what point should the arrangement on `DateTime.Now` expire?

Static and future arrangements have no natural context to tie the lifetime, unlike instance arrangements. Therefore, a new context source has to be devised. The context source that JustMock uses is the unit testing framework itself. Methods and types decorated with attributes of a unit testing framework define the context within which arrangements are active. See the [isolation document](Isolation.md) for more information about the algorithm used to define the lifetime of arrangements and their isolation between tests.

The core type which ties together context management and discovery is `Core.Context.MockingContext`. At any point in the application, accessing the `MockingContext.CurrentRepository` or `MockingContext.ResolveRepository` members will create the necessary stack of mock repositories defining the current behavior of JustMock within the current context. The entry point for the JustMock public API like `Mock.Create` and `Mock.Arrange` is one of the methods on this type.

A "repository" or "mocks repository" is implemented by the `Core.MocksRepository` type. Its job is creating, arranging and asserting mocks. The `MockingContext`'s job is to create and retire mocks repositories as the context changes.

The `MockingContext` class depends on `IMockingContextResolver` and its implementers to resolve the current context. There are `IMockingContextResolver` implementers for different unit testing frameworks, like MSTest and NUnit, each being able to build the mocks repository with the correct accumulated context depending on knowledge about the design of each unit testing framework.

### Objects
In regular mocking libraries, the equivalent of the `Mock.Create<T>` method is the user's entry point into the library. All library operations can only be applied to a mock created by that library. JustMock is different. Its profiler allows it to replace the behavior of any method, on any object, or even when there is no object (static methods).

In the absence of abstract classes and interfaces, JustMock would not even need a `Mock.Create<T>` method. You can just create regular instances and arrange their behavior. In fact, this is exactly what JustMock's `Mock.Create<T>` does for non-abstract types when the profiler is enabled - it simply instantiates the object (optionally calling the object constructor) and intercepts all calls to it. This is exactly how sealed classes are mocked.

Still, JustMock needs to account for the existence of interfaces and abstract classes, and also for the use case where the profiler is not enabled. The non-portable build of JustMock handles those cases using the Castle.DynamicProxy library. The library is interned in `Core/DynamicProxy/`. The library's duty is to build at runtime subclasses or interface implementations of a given type and allow you to intercept all calls to virtual members on that type. The creation is handled by the `Core.DynamicProxyMockFactory` class. Interception is handled by the `Core.DynamicProxyInterceptor` class.

Mocks of `MarshalByRef` objects are handled differently. Their mocks are in fact transparent proxies and are created by `Core.TransparentProxy.MockingProxy`. The use of transparent proxies allows JustMock to mock all public members on a MarshalByRef object, even without the profiler.

Every object touched by JustMock has a corresponding `Core.IMockMixin` that specifies the behavior of that object when calls to it are intercepted. Objects created through a DynamicProxy have a built-in `IMockMixin` which is accessed through a simple cast. Objects that were not created by JustMock, as well as static methods, have a IMockMixin object stored in a look-aside table. The general algorithm for recovering a `IMockMixin` from any object or type is implemented in `MocksRepository.GetMockMixin`. There is also the `Core.Invocation.MockMixin` property based on the previous method which gives you the mock mixin associated with the currently intercepted invocation. It is easier to work with this property whenever code is executing in the context of an invocation.

The general algorithm of creating a mock object is encoded in `MocksRepository.Create`.

### Calls to intercepted methods
At the heart of a mocking library is the ability to specify replacement behavior for certain calls and then being able to intercept those calls to execute the replacement behavior. Interception begins in either `Core.DynamicProxyInterceptor`, `Core.ProfilerInterceptor` or `Core.TransparentProxy.MockingProxy`. Note that transparent proxies delegate to DynamicProxyInterceptor, but use a custom implementation of `Core.Castle.DynamicProxy.IInvocation`. When a call is intercepted by any of the above interceptors, its first job is to determine the current MocksRepository. It can do that by looking up the current repository in the MockingContext or by taking the repository with the `IMockMixin` associated with the current instance or type. If a MocksRepository is found, a `Core.Invocation` object is constructed and `MocksRepository.DispatchInvocation` is called. The `Invocation` object stores all the input state describing what method was called and what arguments were passed, as well as all output state, like what the return value should be and if the original implementation should be skipped. After the dispatch completes, the interceptor examines the output state and returns it back to the calling method or passes to the original implementation.

The specification or replacement behavior in JustMock is called an "arrangement". Every call to `Mock.Arrange` produces a single arrangement in the current context's MocksRepository.

A single arrangement is represented in the core by a `Core.IMethodMock` object and is called a "method mock". `Mock.Arrange` creates instances of `Expectations.ActionExpectation` and `Expectations.FuncExpectation` which implement `Core.IMethodMock`. The object returned by `Mock.Arrange` also implements the API interfaces in `Expectations/Abstractions/`. These interfaces allow for the fluent configuration of arrangements with calls like `.DoInstead()`, `.Returns()` or `.OccursOnce()`. These fluent calls are called "clauses" in general. Each of these clauses attaches a specific "behavior" the method mock.

Not all intercepted calls might be linked to an arrangement. A generic behavior can be specified when creating mocks with `Mock.Create<T>` or `Mock.SetupStatic()`, like Behavior.Loose or Behavior.Strict. This generic behavior specifies the behavior of method calls which are not linked to an arrangement. Whenever a call is intercepted, JustMock first searches for a corresponding method mock, and if one is not found, it delegates to the generic behavior associated with that mock.

#### Behaviors

In both cases behaviors are represented as implementers of `Core.Behaviors.IBehavior`. A method mock has the properties `IMethodMock.Behaviors` and `IMethodMock.OccurrencesBehavior`. A mock mixin has the properties `IMockMixin.FallbackBehaviors` and `IMockMixin.SupplementaryBehaviors`. JustMock searches for a corresponding method mock when processing an intercepted call. If one is found, then its set of behaviors is used to define the behavior of the call. If no method mock is found, the mock mixin for the intercepted object or type is looked up and its `FallbackBehaviors` are executed. If a mock mixin is available, then its `SupplementaryBehaviors` are always executed at the end of an interception. If no mock mixin and no method mock are found, execution continues to the method's original implementation.

`MockBuilder.DissectBehavior` is responsible for turning a value from the `Behavior` enum into a list of supplementary and fallback behaviors to be passed to `MocksRepository.Create`.

All behavior implementations are found in `Core/Behaviors/`. Some behaviors only make sense on a method mock, others only on a mock mixin, still others make sense on both.

Behaviors use the output state of the `Invocation` object passed to `IBehavior.Process` to communicate among themselves if necessary.

#### Call patterns and invocations
Invocation objects always contain concrete instances and concrete arguments. Yet, users can specify arbitrary argument predicates in their arrangements. For example, the might specify that the arrangement should only be executed if an argument has a certain value, or matches a certain predicate, or that an argument's value doesn't matter. These predicates are expressed in terms of matchers. Matchers are implementations of `Core.MatcherTree.IMatcher`. Users specify matchers using the methods on the `Arg` and `ArgExpr` classes.

The entire expression passed to `Mock.Arrange` and its variants is called a "call pattern". Every invocation can be checked to see if it matches a given call pattern. JustMock converts call patterns passed to `Mock.Arrange` into `Core.CallPattern` objects and tests invocations against the, to find corresponding method mocks. The relevant methods are `MocksRepository.ConvertActionToCallPattern`, `MocksRepository.ConvertExpressionToCallPattern` and `MocksRepository.ConvertMethodInfoToCallPattern`, used by `Mock.ArrangeSet`, `Mock.Arrange` and `Mock.NonPublic.Arrange` respectively.

Method mocks and their call patterns are stored in a dictionary of trees - `MocksRepository.arrangementTreeRoots`. The key in the dictionary and the base of each tree is the intercepted member. The Nth level of the tree corresponds to the Nth argument of the method. The tree itself is implemented by `Core.MatcherTree.IMatcherTreeNode` and its implementers. Every non-leaf tree node contains the matcher for the corresponding argument in the call pattern. The leafs are instances of `Core.MatcherTree.MethodMockMatcherTreeNode` and contain the `IMethodMock` whose call pattern was used to construct this path in the matcher tree. Invocation dispatch starts at the base of the tree corresponding to the intercepted method and finishes at a leaf that provides the method mock. The matcher at each level is tested against the corresponding argument in the invocation to determine whether this branch of the tree should be checked further. If no leaf is found, this means that no method mock corresponds to the intercepted invocation and control is passed to the generic behavior specified by the mock mixin.

### Assertions
Arrangement clauses that specify expected behavior are called "expectations" and their associated behavior implements `Core.Behaviors.IAssertableBehavior`. Users call `Mock.Assert` to assert that an expectation is fulfilled. There are several flavors of expectations.

#### Method mock expectations
Expectations given in arrangement clauses are stored in the method mock of that arrangement. These can be asserted by calling `Mock.Assert(object)` and `Mock.Assert(type)`. This form of assertion finds all method mocks for that object or type, i.e. all methods arranged for that object and type and assert all assertable behaviors in that method mock. In addition, all behaviors on the associated mock mixin are asserted and also all behaviors on dependent mocks. Dependent mocks are stored in `IMockMixin` and are added for example by arranging a call on a mock to return another mock. See `IMockMixin.DependentMocks`.

#### Explicit occurrence expectations
Calls to `Mock.Assert(expression, ...)` work differently, depending on whether `expression` matches an existing arrangement and is a bit of a mess. The following description might not be accurate in all possible permutations of arguments passed to `Mock.Assert` and pre-existing arrangements. If `expression` corresponds to an arrangement, then the corresponding method mock is asserted.

If `expression` does not correspond to an arrangement or an `Occurs` is given, then the number of invocations matching the given call pattern are counted in the `MocksRepository.invocationTreeRoots` tree and the number of occurrences is asserted. If an `Occurs` argument is not given, then JustMock checks that at least one invocation matching the call pattern has occurred, otherwise it asserts the count specified by `Occurs`.

JustMock has a very peculiar behavior when the call pattern of a method mock uses non-trivial argument matchers and the call pattern built from `expression` is also non-trivial. The details are implemented around the `Core.MatcherTree.MatchingOptions` enum, the code that uses it, and the logic used to compute whether one matcher matches another.

#### Implicit expectations
A call to `Mock.AssertAll(object)` asserts all expectations just like `Mock.Assert(object)`. In addition, it treats all arrangements on the given object as `.MustBeCalled()` unless an explicit occurrence expectation is specified.

## Profiler isolation
The JustMock profiler allows you to mock almost anything. You could, for example arrange `List<object>.Add(o)` to do nothing for all list instances. Obviously, if JustMock tries to use a method arranged unexpectedly, it could fail spectacularly. On the other hand, JustMock should not limit the user's ability to mock anything, even if JustMock depends on the original implementation.

Isolation from the effects of possibly disrupting arrangements is achieved using the `ProfilerInterceptor.GuardInternal` and `ProfilerInterceptor.GuardExternal` methods. For starters, the profiler never instruments any method in the Telerik.JustMock assembly. Also, every public JustMock method's body is wrapped in a call to `ProfilerInterceptor.GuardInternal`. `GuardInternal` disables all interception for all code executing inside it. Calls to `GuardInternal` can safely be nested. So, even if the user arranges a method used internally by JustMock, all JustMock code executes in `GuardInternal` blocks in which no calls are ever intercepted. Whenever JustMock needs to call back into user code, e.g. call the delegate passed to a `.DoInstead()`, the delegate is executed in a `GuardExternal` block. `GuardExternal` re-enables all interception for all code executing inside it. `GuardInternal` and `GuardExternal` blocks can be freely mixed. If code is executing directly under a `GuardInternal` call, then interception is disabled. If code is executing directly under a `GuardExternal` call, then interception is enabled. When exiting a block, guard state is managed so that interception always behaves with respect only to the inner-most `GuardInternal` or `GuardExternal` on the call stack.

A common source of bugs is calling user code directly, without wrapping it in a `GuardExternal` block. In those cases user code will not see the effect of their existing arrangements. One important thing to note is that JustMock should execute **only** user code in `GuardExternal` block. For example, if JustMock needs to call a method with a dynamic signature, then it cannot use `MethodBase.Invoke` in a `GuardExternal` block, because the behavior of `MethodBase.Invoke` or any of the methods used in its implementation may be arranged. In such cases JustMock relies on `MockingUtil.MakeFuncCaller` to create a dynamic method that takes an array of arguments and can call a delegate with any signature. `MakeFuncCaller` is called in the current `GuardInternal` block and the result delegate can be safely called in a `GuardExternal` block.

There is a test: `ImplementationCorrectnessTests.AssertAllPublicMethodsAreWrappedInGuardInternal`, that asserts that the bodies of all non-trivial public methods in the JustMock API are wrapped in calls to GuardInternal.

## Debugging tests
JustMock has a useful debugging facility implemented by `Telerik.JustMock.DebugView`. This facility is geared towards users, not developers. It provides an insight into JustMock's inner workings and decision making process. Advanced users can depend on it to understand why JustMock behaves the way it does. The class documentation gives a full tutorial how to use it. Occasionally relying on this type is useful when dithering over support tickets. Sometimes, it's also much quicker to read the DebugView output rather than trace JustMock's execution in the debugger.
