# Isolation

In JustMock you make arrangements by calling the static methods on the Mock class:

``` C#
Mock.Arrange(() => DateTime.Now).Returns(new DateTime(1))
```

A commonly occurring question is – "How long is this arrangement active?" Alternatively stated:
* How are arrangements in different tests isolated from one another?
* Do arrangements made in one test leak to other tests?

In short – no. Arrangements do not leak from one test to another. The long answer is rather complex, so if you’re content in knowing that JustMock automagically manages the lifetime of arrangements for you in the most intuitive possible way, then you may want to stop reading here.

## Testing framework contexts

The lifetime of an arrangement depends on the lifetime of the context in which the arrangement is made. So, what is a context, exactly? Well, "the current context" is derived from one of many circumstances known during the test run. Read on.

### Test methods
The most obvious context is given by the currently executing test method. Check the example below:

``` C#
[TestMethod]
public void Test1()
{
	Mock.Arrange(() => DateTime.Now).Returns(new DateTime());
	Assert.Equal(new DateTime(), DateTime.Now);
}

[TestMethod]
public void Test2()
{
	Assert.NotEqual(new DateTime(), DateTime.Now);
}
```

The arranged value of `DateTime.Now` in `Test1` doesn’t leak to `Test2`. How does JustMock know to prevent the leakage? What it does is analyze the call stack every time that an intercepted method is called. The `Arrange` statement in `Test1` tells JustMock, among other things, to intercept all calls to `DateTime.Now`. When `DateTime.Now` is called afterwards, JustMock intercepts the call and tries to determine the current context in which the intercepted operation is executing. This is done by taking the call stack at the point of the call and traversing it upwards until a method is found that provides meaningful context. In our example, the meaningful context is provided by the `Test1` method, which is found by `MSTestMockingContextResolver` – an internal class dedicated to working with methods decorated with MSTest’s testing attributes.

So, as soon as the call stack traversal hits `Test1` (which is decorated with the `[TestMethod]` attribute) it is dubbed “the current context”, and only arrangements made beforehand in the same context are used to determine the ultimate behavior of the intercepted call. We’ve made arrangements in `Test1`, so those are respected. `Test2` is a different context, so it doesn’t share arrangements with `Test1`.

### Set-up/Teardown

What about set-up/teardown methods? How do arrangements made in a set-up method affect test methods?

Every testing framework defines some hierarchy of methods that are executed one after another, building context for test methods to ultimately execute in. So, in MSTest we have the `AssemblyInitialize`, `ClassInitialize`, `TestInitialize` and `TestMethod` attributes. Each method decorated with one of these attributes is called upon to build additional context for methods decorated with one of the latter attributes in the list. For clarity, let’s say that `AssemblyInitialize` is the highest in the hierarchy and `TestMethod` is the lowest.
JustMock respects this notion by maintaining hierarchical contexts. Within a given context, every arrangement made in that context or contexts above it in the hierarchy are respected. An arrangement made in `AssemblyInitialize` is active whenever any decorated method in the same assembly is the current context. An arrangement made in `ClassInitialize` is active whenever any test method in the same class is the current context. An arrangement made in a `TestInitialize` is active until the next test finishes execution.

Let’s look at an example:

``` C#
[TestClass]
public class TestSuite1
{
	[ClassInitialize]
	public static void Init(TestContext ctx)
	{
		Mock.Arrange(() => DateTime.Now).Returns(new DateTime(1));
	}

	[TestMethod]
	public void Test1()
	{
		Mock.Arrange(() => DateTime.Now).Returns(new DateTime(2));
		Assert.Equal(new DateTime(2), DateTime.Now);
	}

	[TestMethod]
	public void Test2()
	{
		Assert.Equal(new DateTime(1), DateTime.Now);
	}
}
[TestClass]
public class TestSuite2
{
	public void Test3()
	{
		Assert.NotEqual(new DateTime(1), DateTime.Now);
		Assert.NotEqual(new DateTime(2), DateTime.Now);
	}
}
```

In the above example the arrangement made in `TestSuite1.ClassInitialize` is respected in `Test2`. `Test1`, however, overrides the same arrangement with a different value – no problem here. Also, neither of the arrangements leak to tests in `TestSuite2`.

Note that an arrangement made in, e.g. `ClassInitialize` is only active whenever the current context is some method in the same class that is also decorated with a testing framework attribute. If there is no method on the call stack decorated with a testing framework attribute, then the context cannot be resolved, even if some non-decorated method of a test class is on the stack. This is important for, e.g. data-driven NUnit tests, where you can generate test cases in methods that are not decorated by framework attributes. Arrangements made in these generator methods are not associated with the test class that is currently executed.

### Call stacks in async test methods
Some test runners allow you to make async tests. Their test methods are declared with the `async` keyword. These test methods may do `await`s inside them and if they do, then the call stack upon returning from an await operation will no longer contain the test method. In this case JustMock still manages to reconstruct the call stack (and thus, the original context) by inspecting the code generated by the compiler (specifically the compiler-generated `StateMachine` class) and working its way from there.

Even though the code after an await may be executing in a different thread, the current context is always correctly resolved to the test method that started the async operation.

## Contexts unrelated to testing frameworks

The current context cannot always be resolved just by looking at the call stack and trying to find a method decorated with a testing framework attribute. Sometimes a non-decorated method is called by the test runner (e.g. for generating test cases). Other times, the test executes on several threads and, naturally, only one thread has a test method on the call stack.

### Instances
Whenever the context cannot be obtained from the call stack, there are other clues that JustMock looks at. Most importantly, we can look at the instance on which the method was called (when the method is an instance method, not a static one and also when the method is not future-mocked). These instances are usually controlled by the user – they have either created them using `Mock.Create<T>` or they have treated them as partial mocks and made arrangements on them. Thus, every instance associated with some arrangement can remember the last context in which it was used. Whenever a call on a mock instance cannot resolve the current context by looking at the call stack, we can just pretend that we’re in the context that last worked with that instance.

As a corollary, whenever the context of a call on a mock instance can be resolved by looking at the call stack, that instance’s “last context” is set to it.

This fail-safe lookup allows the use of mocks across unrelated call stacks, e.g. when mocks are generated by test case generators, or when mocks are used across threads.

#### Instance sameness
Regular mocks have additional internal fields that are used to store their last use context. Those fields cannot be added to partial mocks, however. Instead they’re stored in look-aside tables, where the mock instance is used as a key. Mind you, they’re not stored in a dictionary, because there is no guarantee that the `GetHashCode` method of a mock instance actually works – it might be arranged to do something else, or it might depend on the object having a valid state, which cannot be guaranteed if the instance had been created with `Constructor.Mocked`. Instead, linear look-up and reference equality is used to locate the additional data regarding the partial mock.

Unless the instance is not of a reference type. The context associated with a value type instance can only be looked up by comparing the instances for equality. The implication of this is that the `Equals` method on value types must be able to work correctly and never throw, regardless of the object’s current state. It also can never be arranged.

### Threads
So what about static methods? When a call to a static method is intercepted, you don’t have an instance with which to associate a context. The same goes for future mocking. If you’re doing future mocking, then the instance you have at hand is not a partial mock and is not associated with any sort of context.

We do have the method itself though. Maybe we can associate the context with the method and use the method as a key, then? Turns out, that’s a bad idea. Unlike mock instances, static methods are used all over the place and the user cannot control who calls these methods when. The result is that mocking static methods (especially mscorlib ones) disregarding what the test runner’s other threads are doing is an easy way to crash the test runner itself.

This is why calls to static methods that have no current context are bound to the context that was active last on the current thread. This is a pretty safe bet – if there is any “last context” for the current thread at all, then it’s a kind of safe to assume that this thread is meant to execute user code in the first place. This is the case with the MSTest runner.

#### Explicit clean-up

When every test is executed on a separate thread, lifetime management for statics is easy, but that’s not always the case. Some runners execute the entire test run on a single thread – that’s the case with, e.g. NUnit. In that case stuff may leak out from one test to the next, especially arrangements on statics. To protect the test runner from side effects due to user arrangements, every context must be explicitly dropped when its usefulness is semantically over. This is done by calling the `Mock.Reset` method at the end of each test method. Never heard of that method, have you? And with good reason – under the hood, a call to that method is injected at the end of every test method by the JustMock profiler during the test run! Pure magic...

Explicit calls to `Mock.Reset` are generally unnecessary. JustMock Lite users never need them, as they can’t mess up the test runner itself. The rare case when that would be necessary is if you’re doing elevated mocking in a testing framework that is unsupported by JustMock. If JustMock doesn’t know about the testing framework, it can’t possibly know where to inject calls to `Mock.Reset`, so you need to do that manually.

A caveat to the above is that the body of every test method is wrapped in a try-finally block by the profiler. This may subtly change the behavior of the test at run time. If you’re testing memory reclamation behavior, i.e. you’re doing stuff, then calling `GC.Collect` and asserting that the objects created during the test run are now gone, then those tests might break. If that happens, put the `[DisableAutomaticRepositoryReset]` attribute on that method. Read the documentation of that attribute for further information.

#### Mocking static calls and future mocking on all threads (cross-thread static and future mocking)

Mocking statics across threads can be dangerous, but it is not always dangerous. If we’re careful, then chances are nothing is going to break. Thus, JustMock allows you to specify that you want an arrangement on a static member to work regardless of thread. This is done by adding the `.OnAllThreads()` modifier to the arrangement:

``` C#
Mock.Arrange(() => DateTime.Now).Returns(new DateTime(1)).OnAllThreads();
```

Now `DateTime.Now` is clobbered on all threads, even those used internally by the test runner. Be careful with this feature. If the test runner starts crashing randomly, then tests using `OnAllThreads` should be prime suspects.

Also, whenever you use this feature, you can no longer safely execute tests in parallel as now arrangements in different tests may interfere with one another.

### When all else fails

So, we’ve intercepted a call to a method. The method is either static, or the instance is not a mock. We’ve looked at the call stack and no test method in sight. The current thread has never been used for mocking. What now?

When there is no place where we can get our context from, we just dub the current method as the context. The current method is now a local context. All method calls made from it will also use it as their context. As a corollary, whenever there’s a method in the call stack that has become a local context, it’s reused by method beneath it in the call stack.

No surprises here.

## Finalizers

The finalizer thread is exempt from any mocking activity. No arrangements work on the finalizer thread. If a mock or a partial mock is created from a class with a finalizer, then that finalizer will not be executed upon garbage collection.

## In Conclusion

Write every test as if it’s the only test that will execute in any test run. Don’t try to “arrange things once”, unless you’re doing it within class/test set-up methods. Don’t try to manually manage the lifetime or scope of arrangements – JustMock has that nailed down pretty well already. Now, go mock something using this newfound knowledge!
