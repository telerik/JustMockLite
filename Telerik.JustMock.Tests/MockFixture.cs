/*
 JustMock Lite
 Copyright © 2010-2015 Telerik AD

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

#if NUNIT
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using AssertionException = NUnit.Framework.AssertionException;
#elif VSTEST_PORTABLE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AssertionException = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssertFailedException;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#endif

#if LITE_EDITION
#if !COREFX
using Telerik.JustMock.DemoLibSigned;
#endif
#endif

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Telerik.JustMock.Core;

namespace Telerik.JustMock.Tests
{
	/// <summary>
	/// Validates Mock capabilities
	/// </summary>
	[TestClass]
	public class MockFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldMockInterface()
		{
			var iCloneable = Mock.Create<ICloneable>();
			Assert.NotNull(iCloneable);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldMockInterfaceContainingProperty()
		{
			var bar = Mock.Create<IBar>();
			Assert.NotNull(bar);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldReturnServiceFromServiceProvider()
		{
			var provider = Mock.Create<IServiceProvider>();

			Mock.Arrange(() => provider.GetService(typeof(IFooService))).Returns(new FooService());

			Assert.True(provider.GetService(typeof(IFooService)) is FooService);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInstead()
		{
			var foo = Mock.Create<IFoo>();

			bool called = false;

			Mock.Arrange(() => foo.Execute(Arg.IsAny<string>()))
								  .DoInstead(() => { called = true; })
								  .Returns((string s) => s);

			Assert.Equal(foo.Execute("ping"), "ping");
			Assert.True(called);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadForOneArgFromMethod()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.IsAny<int>()))
				.DoInstead((int arg1) => { expected = arg1; });

			foo.Submit(10);

			Assert.Equal(10, expected);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadForTwoArgsFromMethod()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.IsAny<int>(), Arg.IsAny<int>()))
				.DoInstead((int arg1, int arg2) => { expected = arg1 + arg2; });

			foo.Submit(10, 10);

			Assert.Equal(20, expected);
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadForThreeArgsFromMethod()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.IsAny<int>(), Arg.IsAny<int>(), Arg.IsAny<int>()))
				.DoInstead((int arg1, int arg2, int arg3) => { expected = arg1 + arg2 + arg3; });

			foo.Submit(10, 10, 10);

			Assert.Equal(30, expected);
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadForFourArgsFromMethod()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Submit(Arg.IsAny<int>(), Arg.IsAny<int>(), Arg.IsAny<int>(), Arg.IsAny<int>()))
				.DoInstead((int arg1, int arg2, int arg3, int arg4) => { expected = arg1 + arg2 + arg3 + arg4; });

			foo.Submit(10, 10, 10, 10);

			Assert.Equal(40, expected);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadForOneArgFromFunc()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.IsAny<int>()))
				.DoInstead((int arg1) => { expected = arg1; })
				.Returns(() => expected);

			Assert.Equal(foo.Echo(10), expected);
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadForTwoArgsFromFunc()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.IsAny<int>(), Arg.IsAny<int>()))
				.DoInstead((int arg1, int arg2) => { expected = arg1 + arg2; })
				.Returns(() => expected);

			Assert.Equal(foo.Echo(10), expected);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadForThreeArgsFromFunc()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.IsAny<int>(), Arg.IsAny<int>(), Arg.IsAny<int>()))
				.DoInstead((int arg1, int arg2, int arg3) => { expected = arg1 + arg2 + arg3; })
				.Returns(() => expected);

			Assert.Equal(foo.Echo(10), expected);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDoInsteadForFourArgsFromFunc()
		{
			int expected = 0;

			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.IsAny<int>(), Arg.IsAny<int>(), Arg.IsAny<int>(), Arg.IsAny<int>()))
				.DoInstead((int arg1, int arg2, int arg3, int arg4) => { expected = arg1 + arg2 + arg3 + arg4; })
				.Returns(() => expected);

			Assert.Equal(foo.Echo(10), expected);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldThrowIfArgumentsPassedForInterface()
		{
			Assert.Throws<Exception>(() => Mock.Create<IFoo>(25, true));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertCallOriginal()
		{
			var foo = Mock.Create<FooBase>();

			Mock.Arrange(() => foo.GetString("x")).CallOriginal();
			Mock.Arrange(() => foo.GetString("y")).Returns("z");

			Assert.Equal("x", foo.GetString("x"));
			Assert.Equal("z", foo.GetString("y"));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertGuidParam()
		{
			var foo = Mock.Create<IFoo>();

			Guid guid = Guid.NewGuid();

			bool called = false;

			Mock.Arrange(() => foo.Execute(guid)).DoInstead(() => called = true);

			foo.Execute(guid);

			Assert.True(called);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertCallOriginalForVoid()
		{
			var log = Mock.Create<Log>();
			Mock.Arrange(() => log.Info(Arg.IsAny<string>())).CallOriginal();
			Assert.Throws<Exception>(() => log.Info("test"));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldPassArgumentToOriginal()
		{
			var log = Mock.Create<Log>();

			Mock.Arrange(() => log.Info("x")).CallOriginal();

			Assert.Equal(Assert.Throws<Exception>(() => log.Info("x")).Message, "x");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void MockObjectShouldBeAssignableToMockedInterface()
		{
			var iFoo = Mock.Create<IFoo>();
			Assert.True(iFoo is IFoo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertVoidCall()
		{
			var iFoo = Mock.Create<IFoo>();

			Mock.Arrange(() => iFoo.JustCall()).DoNothing();

			iFoo.JustCall();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertCallsThatAreMethodCalls()
		{
			const int arg = 2;
			var iBar = Mock.Create<IBar>();
			Mock.Arrange(() => iBar.Echo(Arg.IsAny<int>())).Returns(1);

			Assert.Equal(iBar.Echo(arg + 1), 1);
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertGenericThrowsCall()
		{
			var iFoo = Mock.Create<IFoo>();

			Mock.Arrange(() => iFoo.Execute(Arg.IsAny<string>())).Throws<FormatException>();
			Assert.Throws<FormatException>(() => iFoo.Execute("crash"));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertThrowsHavingArguments()
		{
			var iFoo = Mock.Create<IFoo>();

			Mock.Arrange(() => iFoo.Execute(Arg.IsAny<string>())).Throws<CustomExepction>("test", true);
			Assert.Throws<CustomExepction>(() => iFoo.Execute("crash"));
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertEqualityRefereces()
		{
			var iFoo1 = Mock.Create<IFoo>();
			var iFoo2 = Mock.Create<IFoo>();

			Assert.True(iFoo1.Equals(iFoo1));
			Assert.False(iFoo1.Equals(iFoo2));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void TwoMockedObjectsShouldHavdDifferentHashCode()
		{
			var iFoo1 = Mock.Create<IFoo>();
			var iFoo2 = Mock.Create<IFoo>();
			Assert.NotEqual(iFoo1.GetHashCode(), iFoo2.GetHashCode());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ToStringShouldNotbeNullOrEmpty()
		{
			var foo = Mock.Create<IFoo>();
			Assert.False(String.IsNullOrEmpty(foo.ToString()));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldMockClassWithNonDefaultConstructor()
		{
			var nonDefaultClass = Mock.Create<ClassWithNonDefaultConstructor>("ping", 1);
			Assert.NotNull(nonDefaultClass);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldMockClassWithNonDefaultConstructorWithoutPassingAnything()
		{
			var nonDefaultClass = Mock.Create<ClassWithNonDefaultConstructor>();
			Assert.NotNull(nonDefaultClass);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldMockClassWithNoDefaultConstructorWithNull()
		{
			var nonDefaultClass = Mock.Create<ClassWithNonDefaultConstructor>(null, 1);
			Assert.NotNull(nonDefaultClass);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShoulsMockClassWithMultipleNonDefaultConstructor()
		{
			var nonDefaultClass = Mock.Create<ClassWithNonDefaultConstructor>(null, 1, true);
			Assert.NotNull(nonDefaultClass);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertGuidNonDefaultCtorWithDefaultIfNotSpecified()
		{
			var nonDefaultGuidClass = Mock.Create<ClassNonDefaultGuidConstructor>();
			Assert.Equal(nonDefaultGuidClass.guidValue, default(Guid));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertBaseCallWithGuid()
		{
			var foo = Mock.Create<FooBase>();

			Mock.Arrange(() => foo.GetGuid()).CallOriginal();

			Assert.Equal(foo.GetGuid(), default(Guid));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertImplementedInterface()
		{
			var implemented = Mock.Create<IFooImplemted>();
			Assert.NotNull(implemented);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertBaseForImplemetedInterface()
		{
			var implemented = Mock.Create<IFooImplemted>();
			Assert.True(implemented is IFoo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void SHouldAssertCallsFromBaseInImplemented()
		{
			var implemented = Mock.Create<IFooImplemted>();

			Mock.Arrange(() => implemented.Execute("hello")).Returns("world");

			Assert.Equal(implemented.Execute("hello"), "world");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldMockObject_GetHashCodeMethod()
		{
			var foo = Mock.Create<FooBase>();

			Mock.Arrange(() => foo.GetHashCode()).Returns(1);

			Assert.Equal(1, foo.GetHashCode());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldMockObject_ToStringMethod()
		{
			var foo = Mock.Create<FooBase>();
			Mock.Arrange(() => foo.ToString()).Returns("foo");
			Assert.Equal("foo", foo.ToString());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldMockObject_EqualsMethod()
		{
			var foo = Mock.Create<FooBase>();
			Mock.Arrange(() => foo.Equals(Arg.IsAny<object>())).Returns(true);
			Assert.True(foo.Equals(new object()));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldCallUnderlyingEquals()
		{
			var foo1 = Mock.Create<FooOverridesEquals>("foo");
			var foo2 = Mock.Create<FooOverridesEquals>("foo");
			Assert.True(foo1.Equals(foo2));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldNotCallBaseByDefault()
		{
			var foo = Mock.Create<FooBase>();
			// this will not throw exception.
			foo.ThrowException();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldTakeLatestSetup()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute("ping")).Returns("pong");
			Mock.Arrange(() => foo.Execute(Arg.IsAny<string>())).Returns("pong");

			Assert.Equal(foo.Execute("nothing"), "pong");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldOverrideBehaviorFromBaseClass()
		{
			var foo = Mock.Create<FooBase>();

			Mock.Arrange(() => foo.GetString("pong")).CallOriginal().InSequence();
			Mock.Arrange(() => foo.GetString(Arg.IsAny<string>())).Returns("ping").InSequence();

			Assert.Equal(foo.GetString("pong"), "pong");
			Assert.Equal(foo.GetString("it"), "ping");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDateTimeAsRef()
		{
			var foo = Mock.Create<IFoo>();

			DateTime expected = new DateTime(2009, 11, 26);

			Mock.Arrange(() => foo.Execute(out expected)).DoNothing();

			DateTime acutal = DateTime.Now;

			foo.Execute(out acutal);

			Assert.Equal(expected, acutal);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertOutputParamPassedViaNested()
		{
			var nested = new Nested();
			nested.expected = 10;

			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(out nested.expected));

			int actual = 0;

			foo.Execute(out actual);

			Assert.Equal(actual, 10);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldNotInvokeOriginalCallWhenInitiatedFromCtor()
		{
			var foo = Mock.Create<FooAbstractCall>(false);
			Assert.NotNull(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldNotInvokeOriginalActionWhenInitiatedFromCtor()
		{
			var foo = Mock.Create<FooAbstractAction>();
			Assert.NotNull(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldNotInitalizeAsArgMatcherWhenProcessingMemberAccessArgument()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(false, BadGuid)).OccursOnce();

			foo.Execute(false, Guid.Empty);

			Mock.Assert(() => foo.Execute(Arg.IsAny<bool>(), Guid.Empty));
		}

		public static readonly Guid BadGuid = Guid.Empty;

		public abstract class FooAbstractCall
		{
			public FooAbstractCall(bool flag)
			{
				// invoke base will throw exception here.
				Initailize();
			}

			public abstract bool Initailize();
		}

		public abstract class FooAbstractAction
		{
			public FooAbstractAction()
			{
				// invoke base will throw exception here.
				Initailize();
			}

			public abstract void Initailize();
		}


#if !COREFX
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldCreateMockClassWithInternalConstructor()
		{
			var foo = Mock.Create<FooWithInternalConstruct>();
			Assert.NotNull(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertArrangeWithInternalConstructor()
		{
			var foo = Mock.Create<FooWithInternalConstruct>();

			bool called = false;

			Mock.Arrange(() => foo.Execute()).DoInstead(() => called = true);

			foo.Execute();

			Assert.True(called);
		}
#endif

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertGenericFuncCalls()
		{
			var genericClass = Mock.Create<FooGeneric<int>>();

			Mock.Arrange(() => genericClass.Get(1, 1)).Returns(10);

			Assert.Equal(genericClass.Get(1, 1), 10);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertGenericVoidCalls()
		{
			var genericClass = Mock.Create<FooGeneric<int>>();

			bool called = false;

			Mock.Arrange(() => genericClass.Execute(1)).DoInstead(() => called = true);

			genericClass.Execute(1);

			Assert.True(called);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertGenricMockWithNoGenericClass()
		{
			var genericClass = Mock.Create<FooGeneric>();

			Mock.Arrange(() => genericClass.Get<int, int>(1)).Returns(10);

			Assert.Equal(genericClass.Get<int, int>(1), 10);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertFuncWithOccurrence()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute("x")).Returns("x");

			foo.Execute("x");

			Mock.Assert(() => foo.Execute("x"), Occurs.Exactly(1));
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertOutputGenericArgument()
		{
			var fooGen = Mock.Create<FooGeneric>();

			int result = 0;

			fooGen.Execute<int, int>(out result);

			Assert.Equal(result, 0);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertArrangeForGenricOutArgument()
		{
			var fooGen = Mock.Create<FooGeneric>();

			int expected = 10;

			Mock.Arrange(() => fooGen.Execute<int, int>(out expected)).Returns(0);

			int actual = 0;

			fooGen.Execute<int, int>(out actual);

			Assert.Equal(expected, actual);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertParamArrayMatchArugments()
		{
			string expected = "bar";
			string argument = "foo";

			var target = Mock.Create<IParams>();

			Mock.Arrange(() => target.ExecuteParams(argument, "baz")).Returns(expected);
			string ret = target.ExecuteParams(argument, "baz");
			Assert.Equal(expected, ret);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldDistinguiseMethodWithDifferentGenericArgument()
		{
			var foo = Mock.Create<FooGeneric>();

			Mock.Arrange(() => foo.Get<int>()).Returns(10);
			Mock.Arrange(() => foo.Get<string>()).Returns(12);

			Assert.Equal(foo.Get<string>(), 12);
			Assert.Equal(foo.Get<int>(), 10);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertParamArrayWithMatcherAndConcreteValue()
		{
			string expected = "bar";
			string argument = "foo";

			var target = Mock.Create<IParams>();

			Mock.Arrange(() => target.ExecuteByName(Arg.IsAny<int>(), argument)).Returns(expected);

			string ret = target.ExecuteByName(0, argument);

			Assert.Equal(expected, ret);
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertCallWithMultipleMathers()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo(Arg.Matches<int>(x => x == 10), Arg.Matches<int>(x => x == 10))).Returns(20);

			int ret = foo.Echo(10, 10);

			Assert.Equal(20, ret);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertArgumentMatherArgumentSetup()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(10, Arg.Matches<int>(x => x > 10 && x < 20), 21)).Returns(20);

			int ret = foo.Echo(10, 11, 21);

			Assert.Equal(20, ret);
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertCallWithDefaultValues()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(0, 0)).Returns(2);

			int ret = foo.Echo(0, 0);

			Assert.Equal(2, ret);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldCreateMockFromRealObject()
		{
			var realItem = Mock.Create(() => new RealItem());
			Assert.NotNull(realItem);
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldCreateMockFromRealObjectForNonDefaultConstructor()
		{
			var realItem = Mock.Create(() => new RealItem(10));
			Assert.NotNull(realItem);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldCreateMockFromRealObjectThatHasOnlyNonDefaultCtor()
		{
			var realItem = Mock.Create(() => new RealItem2(10, string.Empty));
			Assert.NotNull(realItem);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldCreateMockFromRealCtorWithParams()
		{
			// the following line should not throw any argument exception.
			var realItem = Mock.Create(() => new RealItem("hello", 10, 20),
				Behavior.CallOriginal);

			Assert.Equal("hello", realItem.Text);
			Assert.Equal(2, realItem.Args.Length);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertMixins()
		{
			var realItem = Mock.Create<RealItem>(x =>
			{
				x.Implements<IDisposable>();
				x.CallConstructor(() => new RealItem(0));
			});
			var iDispose = realItem as IDisposable;

			iDispose.Dispose();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertMixinsWithClosure()
		{
			int a = 5;
			var realItem = Mock.Create<RealItem>(x =>
			{
				x.Implements<IDisposable>();
				x.CallConstructor(() => new RealItem(a));
			});
			var iDispose = realItem as IDisposable;

			iDispose.Dispose();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldImplementDependentInterfacesWhenTopIsSpecified()
		{
			var realItem = Mock.Create<RealItem>(x =>
			{
				x.Implements<IFooImplemted>();
				x.CallConstructor(() => new RealItem(0));
			});

			Assert.NotNull(realItem as IFoo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldCloneWhenItemImplementsICloneableAndOneOtherInterface()
		{
			var myMock = Mock.Create<IDisposable>(x => x.Implements<ICloneable>());
			var myMockAsClonable = myMock as ICloneable;
			bool isCloned = false;

			Mock.Arrange(() => myMockAsClonable.Clone()).DoInstead(() => isCloned = true);

			myMockAsClonable.Clone();

			Assert.True(isCloned);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldCallOriginalMethodForCallsOriginal()
		{
			var foo = Mock.Create<FooBase>(Behavior.CallOriginal);
			//// should call the original.
			Assert.Throws<InvalidOperationException>(() => foo.ThrowException());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldNotCallOriginalMethodIfThereisASetupForCallsOriginal()
		{
			var foo = Mock.Create<FooBase>(Behavior.CallOriginal);

			Guid expected = Guid.NewGuid();

			Mock.Arrange(() => foo.GetGuid()).Returns(expected);

			Assert.Equal(expected, foo.GetGuid());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldCallOriginalIfThereIsNoSetupForSimilarTypeForCallsOiginal()
		{
			var foo = Mock.Create<FooBase>(Behavior.CallOriginal);

			Mock.Arrange(() => foo.Echo(1)).Returns(2);

			Assert.Equal(2, foo.Echo(1));
			Assert.Equal(2, foo.Echo(2));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldBeAbleToIgnoreArgumentsIfSpecified()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo(10, 9)).IgnoreArguments().Returns(11);

			Assert.Equal(11, foo.Echo(1, 1));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldInterceptVirtualsFromBaseClass()
		{
			var foo = Mock.Create<FooChild>();

			Mock.Arrange(() => foo.ThrowException()).Throws<ArgumentException>();

			Assert.Throws<ArgumentException>(() => foo.ThrowException());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertSetupWithArgAnyMatcherForArray()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Submit(Arg.IsAny<byte[]>())).MustBeCalled();

			foo.Submit(new byte[10]);

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertDictionaryArgumentForIsAny()
		{
			var param = Mock.Create<IParams>();

			Mock.Arrange(() =>
				param.ExecuteArrayWithString(Arg.AnyString, Arg.IsAny<Dictionary<string, object>>()))
				.MustBeCalled();

			param.ExecuteArrayWithString("xxx", new Dictionary<string, object>());

			Mock.Assert(param);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertSetupWithCallHavingParamsAndPassedWithMatcher()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.FindOne(Arg.IsAny<ICriteria>())).Returns(true);

			var criteria = Mock.Create<ICriteria>();

			Assert.True(foo.FindOne(criteria));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldThrowNotImplementedExceptionForBaseInovocationOnAbstract()
		{
			var node = Mock.Create<ExpressionNode>(Behavior.CallOriginal);
			Assert.Throws<NotImplementedException>(() => { var expected = node.NodeType; });
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Behavior"), TestCategory("CallOriginal")]
		public void CallOriginalClause_AbstractMethod_ThrowsNotImplemented()
		{
			var mock = Mock.Create<IFoo>();
			Mock.Arrange(() => mock.JustCall()).CallOriginal();
			Assert.Throws<NotImplementedException>(() => mock.JustCall());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldSetupMockWithParamsWhenNoParamIsPassed()
		{
			var fooParam = Mock.Create<FooParam>();

			var expected = "hello";

			Mock.Arrange(() => fooParam.FormatWith(expected)).Returns(expected);

			Assert.Equal(expected, fooParam.FormatWith(expected));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldBeAbleToPassSingleArgIsAnyForParamsTypeArgument()
		{
			var fooParam = Mock.Create<FooParam>();

			Mock.Arrange(() => fooParam.GetDevicesInLocations(0, false, Arg.IsAny<MesssageBox>())).Returns(10);

			int result = fooParam.GetDevicesInLocations(0, false, new MesssageBox());

			Assert.Equal(10, result);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldBeAbleToPassMultipleArgIsAnyForParamsTypeArgument()
		{
			var fooParam = Mock.Create<FooParam>();

			Mock.Arrange(() => fooParam
				.GetDevicesInLocations(0, false, Arg.IsAny<MesssageBox>(), Arg.IsAny<MesssageBox>()))
				.Returns(10);

			var box = new MesssageBox();

			int result = fooParam.GetDevicesInLocations(0, false, box, box);

			Assert.Equal(10, result);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldCreateMockFromInterfaceWithSimilarGenericOverloads()
		{
			var session = Mock.Create<ISession>();
			Assert.NotNull(session);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertExpectedWithDynamicQuery()
		{
			var bookRepo = Mock.Create<IBookRepository>();

			var expected = new Book();

			Mock.Arrange(() => bookRepo.GetWhere(book => book.Id > 1))
					.Returns(expected)
					.MustBeCalled();

			var actual = bookRepo.GetWhere(book => book.Id > 1);

			Assert.Equal(expected, actual);

			Mock.Assert(bookRepo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertMockExpressionDeleteArgumentForCompoundQuery()
		{
			var bookRepo = Mock.Create<IBookRepository>();

			var expected = new Book();

			string expectedTitle = "Telerik";

			Mock.Arrange(() => bookRepo.GetWhere(book => book.Id > 1 && book.Title == expectedTitle))
					.Returns(expected)
					.MustBeCalled();

			var actual = bookRepo.GetWhere(book => book.Id > 1 && book.Title == expectedTitle);

			Assert.Equal(expected, actual);

			Mock.Assert(bookRepo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertMockForDynamicQueryWhenComparedUsingAVariable()
		{
			var repository = Mock.Create<IBookRepository>();
			var expected = new Book { Title = "Adventures" };
			var service = new BookService(repository);

			Mock.Arrange(() => repository.GetWhere(book => book.Id == 1))
				.Returns(expected)
				.MustBeCalled();

			var actual = service.GetSingleBook(1);

			Assert.Equal(actual.Title, expected.Title);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertWithIntValueUsedForLongValuedArgument()
		{
			int number = 5;
			int expectedResult = 42;

			var myClass = Mock.Create<ClassWithLongMethod>();

			Mock.Arrange(() => myClass.AddOne(number)).Returns(expectedResult);

			// Act
			var result = myClass.AddOne(number);

			// Assert
			Assert.Equal(expectedResult, result);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAutoArrangePropertySetInConstructor()
		{
			var expected = "name";
			var item = Mock.Create<Item>(() => new Item(expected));

			Assert.Equal(expected, item.Name);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldTakeOutValueFromDoInteadWhenDefinedWithCustomDelegate()
		{
			int outArg = 1;
			var mock = Mock.Create<DoInsteadWithCustomDelegate>();
			Mock.Arrange(() => mock.Do(0, ref outArg)).DoInstead(new RefAction<int, int>((int i, ref int arg2) => { arg2 = 2; }));

			mock.Do(0, ref outArg);

			Assert.Equal(2, outArg);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldCheckMethodOverloadsWhenResolvingInterfaceInheritance()
		{
			var project = Mock.Create<IProject>();
			Assert.NotNull(project);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void PropertySetShouldThrowExceptionWhenNameHasSet_Literal()
		{
			var b_object = Mock.Create<B>();

			Mock.ArrangeSet(() => b_object.b_string_set_get = string.Empty).DoNothing().MustBeCalled();

			b_object.b_string_set_get = string.Empty;

			Mock.Assert(b_object);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldNotAffectAssertionForInvalidAsserts()
		{
			var foo = Mock.Create<IFoo>();

			Guid goodGuid = Guid.NewGuid();
			Guid badGuid = Guid.NewGuid();

			Mock.Arrange(() => foo.CallMeOnce(true, goodGuid)).OccursOnce();

			foo.CallMeOnce(true, goodGuid);

			Mock.Assert(() => foo.CallMeOnce(true, badGuid), Occurs.Never());
			Mock.Assert(() => foo.CallMeOnce(true, Guid.Empty), Occurs.Never());

			Mock.Assert(() => foo.CallMeOnce(true, goodGuid), Occurs.Once());
			Mock.Assert(() => foo.CallMeOnce(false, badGuid), Args.Ignore(), Occurs.Once());

			Mock.Assert(() => foo.CallMeOnce(Arg.AnyBool, badGuid), Occurs.Never());
			Mock.Assert(() => foo.CallMeOnce(Arg.IsAny<bool>(), badGuid), Occurs.Never());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldEqualityCheckForMockFromAnInterfaceThatHasEquals()
		{
			IRule mockRule1 = Mock.Create<IRule>();

			List<IRule> ruleList = new List<IRule>();

			Assert.False(ruleList.Contains(mockRule1));

			ruleList.Add(mockRule1);

			Assert.True(ruleList.Contains(mockRule1));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertMethodWithKeyValuePairTypeArgument()
		{
			var presenter = Mock.Create<InteractiveKioskPresenter>(Behavior.CallOriginal);

			var key = Mock.Create<IKioskPart>();
			var val = Mock.Create<IKioskWellInfo>();

			presenter.ShowControl(new KeyValuePair<IKioskPart, IKioskWellInfo>(key, val));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertMethodWithStructTypeArgument()
		{
			var presenter = Mock.Create<InteractiveKioskPresenter>(Behavior.CallOriginal);

			Size size = new Size();

			presenter.DrawRect(size);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldParsePrimitiveParamsArrayCorrectly()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.SubmitWithParams(Arg.AnyInt)).MustBeCalled();

			foo.SubmitWithParams(10);

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertCorrectMethodWhenDifferentArgumentsPassedForParamSetup()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.SubmitWithParams(10)).OccursOnce();

			foo.SubmitWithParams(10);
			foo.SubmitWithParams(10, 11);

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertOccursForIndexedPropertyWithDifferentArguments()
		{
			var foo = Mock.Create<Foo>();

			string expected = "string";

			Mock.Arrange(() => foo.Baz["Test"]).Returns(expected);
			Mock.Arrange(() => foo.Baz["TestName"]).Returns(expected);

			Assert.Equal(expected, foo.Baz["Test"]);
			Assert.Equal(expected, foo.Baz["TestName"]);

			Mock.Assert(() => foo.Baz["Test"], Occurs.Once());
			Mock.Assert(() => foo.Baz["TestName"], Occurs.Once());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldNotSkipBaseInterfaceWhenSomeMembersAreSame()
		{
			var loanString = Mock.Create<ILoanStringField>();
			Assert.NotNull(loanString);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertParamsArrayAsArrayBasedOnArgument()
		{
			string value1 = "Hello";
			string value2 = "World";

			var session = Mock.Create<IMockable>();

			Mock.Arrange(() => session.Get<string>(Arg.Matches<string[]>(v => v.Contains("Lol") &&
													 v.Contains("cakes"))))
				.Returns(new[]
				 {
					 value1,
					 value2,
				 });

			var testValues = new[]{
						 "Lol",
						 "cakes"
					 };

			var result = session.Get<string>(testValues);

			Assert.Equal(value1, result[0]);
			Assert.Equal(value2, result[1]);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldNotInitRescursiveMockingWithProfilerForProperyThatReturnsMock()
		{
			WorkerHelper helper = new WorkerHelper();

			helper.Arrange();

			helper.Worker.Echo("hello");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertMockWithEnumArgumentWithUnderlyingTypeOtherThanInt()
		{
			var subdivisionTypeCode = SubdivisionTypeCode.City;
			var subdivisionTypeRepository = Mock.Create<ISubdivisionTypeRepository>();

			Mock.Arrange(() => subdivisionTypeRepository.Get(subdivisionTypeCode)).Returns((SubdivisionTypeCode subDivision) =>
			{
				return subDivision.ToString();
			});

			var result = subdivisionTypeRepository.Get(subdivisionTypeCode);

			Assert.Equal(result, subdivisionTypeCode.ToString());

			Mock.AssertAll(subdivisionTypeRepository);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertMockWithNullableValueTypeArg()
		{
			FooNullable foo = Mock.Create<FooNullable>();

			var now = DateTime.Now;

			Mock.Arrange(() => foo.ValideDate(now)).MustBeCalled();

			foo.ValideDate(now);

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertMockWithNullForNullableValueTypeArg()
		{
			FooNullable foo = Mock.Create<FooNullable>();

			Mock.Arrange(() => foo.ValideDate(null)).MustBeCalled();

			foo.ValideDate(null);

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertCallOriginalForAbstractClass()
		{
			Assert.NotNull(Mock.Create<TestTreeItem>(Behavior.CallOriginal));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldCallBaseWhenCallOriginalSpecifiedForMock()
		{
			var item = Mock.Create<TestTreeItem>(Behavior.CallOriginal);
			var result = ((IComparable)item).CompareTo(10);

			Assert.Equal(1, result);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldArrangeBothInterfaceMethodAndImplementation()
		{
			var mock = Mock.Create<FrameworkElement>() as ISupportInitialize;

			bool implCalled = false;
			Mock.Arrange(() => mock.Initialize()).DoInstead(() => implCalled = true).MustBeCalled();

			mock.Initialize();
			Assert.True(implCalled);

			Mock.Assert(() => mock.Initialize());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldArrangeBothBaseAndOverriddenMethod()
		{
			var mock = Mock.Create<Control>() as FrameworkElement;

			bool implCalled = false;
			Mock.Arrange(() => mock.Initialize()).DoInstead(() => implCalled = true);

			mock.Initialize();
			Assert.True(implCalled);

			Mock.Assert(() => mock.Initialize());
			Mock.Assert(() => ((ISupportInitialize)mock).Initialize());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldArrangeBaseMethodInManyImplementations()
		{
			var fe = Mock.Create<FrameworkElement>();
			var control = Mock.Create<Control>();

			int calls = 0;
			Mock.Arrange(() => (null as ISupportInitialize).Initialize()).DoInstead(() => calls++);

			fe.Initialize();
			Assert.Equal(1, calls);

			control.Initialize();
			Assert.Equal(2, calls);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertMethodAtAllHierarchyLevels()
		{
			var control = Mock.Create<Control>();

			control.Initialize();

			Mock.Assert(() => control.Initialize(), Occurs.Once());
			Mock.Assert(() => (control as FrameworkElement).Initialize(), Occurs.Once());
			Mock.Assert(() => (control as ISupportInitialize).Initialize(), Occurs.Once());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldArrangeBaseMethodInManyImplementationsForProperty()
		{
			var fe = Mock.Create<FrameworkElement>();
			var control = Mock.Create<Control>();

			int calls = 0;
			Mock.Arrange(() => (null as ISupportInitialize).Property).DoInstead(() => calls++);

			var property = fe.Property;
			Assert.Equal(1, calls);

			property = control.Property;
			Assert.Equal(2, calls);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAssertMethodAtAllHierarchyLevelsForProperty()
		{
			var control = Mock.Create<Control>();

			var property = control.Property;

			Mock.Assert(() => control.Property, Occurs.Once());
			Mock.Assert(() => (control as FrameworkElement).Property, Occurs.Once());
			Mock.Assert(() => (control as ISupportInitialize).Property, Occurs.Once());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldArrangeInheritableMemberOfExplicitlySpecifiedType()
		{
			var ident = Mock.Create<IIdentifiable>();
			Mock.Arrange<IIdentifiable, int>(new Func<int>(() => ident.Id)).Returns(15);
			Assert.Equal(15, ident.Id);
		}

#if !SILVERLIGHT
		[TestMethod, TestCategory("Elevated"), TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldNotCreateProxyIfNotNecessary()
		{
			var mock = Mock.Create<Poco>();
			Mock.Arrange(() => mock.Data).Returns(10);
			Assert.Equal(10, mock.Data);
			if (Mock.IsProfilerEnabled)
				Assert.Same(typeof(Poco), mock.GetType());
		}
#elif !LITE_EDITION
		[TestMethod, TestCategory("Elevated"), TestCategory("Mock")]
		public void ShouldNotCreateProxyIfNotNecessary()
		{
			var mock = Mock.Create<Poco>(Constructor.Mocked);
			Mock.Arrange(() => mock.Data).Returns(10);
			Assert.Equal(10, mock.Data);
			Assert.Same(typeof(Poco), mock.GetType());
		}
#endif

#if LITE_EDITION && !COREFX
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void MockInternalMembersWithoutExplicitlyGivenVisibilitySentinel()
		{
			Assert.Throws<MockException>(() => Mock.Create<InvisibleInternal>());
		}
#endif

		public class Poco // should be inheritable but shouldn't be abstract
		{
			public virtual int Data { get { return 0; } }
		}

		public interface IIdentifiable
		{
			int Id { get; }
		}

		public interface ISupportInitialize
		{
			void Initialize();
			string Property { get; }
		}

		public abstract class FrameworkElement : ISupportInitialize
		{
			public abstract void Initialize();
			public abstract string Property { get; set; }
		}

		public abstract class Control : FrameworkElement
		{
			public override void Initialize()
			{
				throw new NotImplementedException();
			}

			public override string Property
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}
		}

		public abstract class TestTreeItem : IComparable
		{
			int IComparable.CompareTo(object obj)
			{
				return 1;
			}
		}

		public class FooNullable
		{
			public virtual void ValideDate(DateTime? date)
			{

			}
		}

		public enum SubdivisionTypeCode : byte
		{
			None = 255,
			State = 0,
			County = 1,
			City = 2,
		}

		public interface ISubdivisionTypeRepository
		{
			string Get(SubdivisionTypeCode subdivisionTypeCode);
		}

		public class WorkerHelper
		{
			public IWorker TheWorker { get; private set; }

			public WorkerHelper()
			{
				this.TheWorker = Mock.Create<IWorker>(Behavior.Strict);
			}

			public IWorker Worker
			{
				get
				{
					return this.TheWorker;
				}
			}

			public void Arrange()
			{
				Mock.Arrange(() => this.TheWorker.Echo(Arg.AnyString)).DoNothing();
			}
		}

		public interface IWorker
		{
			void Echo(string value);
		}

		public interface IMockable
		{
			T[] Get<T>(params string[] values);
		}

		public interface ILoanStringField : ILoanField
		{
			string Value { get; set; }
		}

		public interface ILoanField
		{
			void ClearValue();
			object Value { get; set; }
		}

		public interface IKioskPart
		{
		}

		public interface IKioskWellInfo
		{
		}

		public class InteractiveKioskPresenter
		{
			public virtual void ShowControl(KeyValuePair<IKioskPart, IKioskWellInfo> kPart)
			{

			}

			public virtual void DrawRect(Size size)
			{

			}

		}

		public struct Size
		{
		}

		public interface IRule
		{
			bool Equals(object obj);
		}

		public class B
		{
			public string b_string = null;

			public virtual string b_string_set_get { get { return b_string; } set { b_string = value; } }
		}

		public interface IProject : IProjectItemContainer
		{
			IEnumerable<IProjectItem> Items { get; }
			void AddChild();
		}

		public interface IProjectItem : IProjectItemContainer
		{
		}

		public interface IProjectItemContainer : IDocumentItemContainer<IProjectItem>
		{
			bool CanAddChild { get; }
		}

		public interface IDocumentItemContainer : IDocumentItem
		{
			IEnumerable<IDocumentItem> Children { get; }
		}

		public interface IDocumentItemContainer<T> : IDocumentItemContainer
		where T : IDocumentItem
		{
			IEnumerable<T> Children { get; }

			void AddChild(T child);
			void RemoveChild(T child);
		}

		public interface IDocumentItem
		{
		}

		public delegate void RefAction<T1, T2>(T1 arg1, ref T2 arg2);

		public class DoInsteadWithCustomDelegate
		{
			public virtual void Do(int k, ref int j)
			{

			}
		}

		public class ClassWithLongMethod
		{
			public virtual long AddOne(long number)
			{
				return number + 1;
			}
		}

		public class BookService
		{
			private IBookRepository repository;

			public BookService(IBookRepository repository)
			{
				this.repository = repository;
			}

			public Book GetSingleBook(int id)
			{
				return repository.GetWhere(book => book.Id == id);
			}
		}

		public interface IBookRepository
		{
			Book GetWhere(Expression<Func<Book, bool>> expression);
		}

		public class Book
		{
			public int Id { get; private set; }
			public string Title { get; set; }
		}


		#region Syntax Integrity

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldBeAbleToInvokeMustBeCalledWithIgnoreArguments()
		{
			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.Execute(0)).IgnoreArguments().MustBeCalled();

			foo.Execute(10);

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldBeAbleToUseMuseBeCalledAfterIgnoreFoFunc()
		{
			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.Echo(0)).IgnoreArguments().MustBeCalled();

			foo.Echo(10);

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldBeAbleToDoDoNothingForNonVoidCalls()
		{
			var foo = Mock.Create<Foo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt)).DoNothing();
			foo.Echo(10);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldBeAbleToSpecifyOccursAfterReturns()
		{
			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.Echo(Arg.AnyInt))
				.Returns(10)
				.Occurs(1);

			foo.Echo(10);

			Mock.Assert(foo);
		}

		internal abstract class FooAbstract
		{
			protected internal abstract bool TryCreateToken(string literal);
		}

		internal abstract class FooAbstract2 : FooAbstract
		{

		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAsssertMockHavingInternalAbstractBaseMethod()
		{
			var foo = Mock.Create<FooAbstract2>();
			foo.TryCreateToken(string.Empty);
		}

		#endregion

		public interface ISession
		{
			ICriteria CreateCriteria<T>() where T : class;
			ICriteria CreateCriteria(string entityName);
			ICriteria CreateCriteria<T>(string alias) where T : class;
			ICriteria CreateCriteria(System.Type persistentClass);
			ICriteria CreateCriteria(string entityName, string alias);
			ICriteria CreateCriteria(System.Type persistentClass, string alias);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldNotTryToWeaveMethodInSilverlightRuntime()
		{
			var foo = Mock.Create<FooSilver>();
			Assert.NotNull(foo);
		}

		public interface ICloneable
		{
			object Clone();
		}

		public abstract class FooSilver
		{
			public void Do()
			{

			}
		}

#if LITE_EDITION
		[TestMethod]
		public void ShouldThrowMockExceptionWhenTryingToMockFinalMethod()
		{
			var mock = Mock.Create<Bar>();
			Assert.Throws<MockException>(() => Mock.Arrange(() => mock.Submit()).DoNothing());
		}

		public class Bar
		{
			public void Submit()
			{
			}
		}
#endif

		#region Test Insfrastructure

		public class Foo
		{
			public virtual void Execute(int arg1)
			{
			}

			public virtual int Echo(int arg1)
			{
				return arg1;
			}

			public virtual Baz Baz { get; set; }
		}

		public class Baz
		{
			public virtual string this[string key]
			{
				get
				{
					return null;
				}
			}
		}

		public class FooParam
		{
			public virtual int GetDevicesInLocations(int arg1, bool bExclude, params MesssageBox[] box)
			{
				return default(int);
			}

			public virtual string FormatWith(string format, params object[] args)
			{
				return string.Empty;
			}
		}

		public class MesssageBox
		{
		}

		public enum ExpressionNodeType
		{
			Constant,
			Binary
		}

		public abstract class ExpressionNode
		{
			public abstract ExpressionNodeType NodeType { get; }
		}

		public class RealItem
		{
			public RealItem()
			{

			}

			public RealItem(int num)
			{

			}

			public RealItem(string text, params int[] args)
			{
				if (args.Length == 0 || string.IsNullOrEmpty(text))
				{
					throw new ArgumentException();
				}

				this.text = text;
				this.args = args;
			}

			public string Text
			{
				get
				{
					return text;
				}
			}

			public int[] Args
			{
				get
				{
					return args;
				}
			}

			public string text;
			private int[] args;
		}

		public class RealItem2
		{
			public RealItem2(int num, string str)
			{
			}
		}

		public class Item
		{
			public virtual string Name { get; set; }

			public Item(string name)
			{
				Name = name;
			}
		}

		public class FooGeneric<T>
		{
			public virtual T Get<T1, T2>(T1 p1, T2 p2)
			{
				return default(T);
			}
			public virtual void Execute<T1>(T1 arg)
			{
				throw new Exception();
			}
		}

		public class FooGeneric
		{
			public virtual TRet Get<T, TRet>(T arg1)
			{
				return default(TRet);
			}

			public virtual TRet Execute<T1, TRet>(out T1 arg1)
			{
				arg1 = default(T1);

				object[] args = new object[1];
				args[0] = arg1;

				return default(TRet);
			}

			public virtual int Get<T1>()
			{
				throw new NotImplementedException();
			}
		}

		public class FooWithInternalConstruct
		{
			internal FooWithInternalConstruct()
			{

			}

			public virtual void Execute()
			{
				throw new ArgumentException();
			}
		}

		public class Nested
		{
			public int expected;
			public int expeted1;
		}
		class FooService : IFooService { }
		interface IFooService { }

		public interface ICriteria
		{

		}

		public interface IFoo
		{
			string Execute(string arg);
			void JustCall();
			void Execute(Guid guid);
			void Execute(bool flag, Guid guid);
			void Execute(out int expected);
			void Execute(out DateTime date);
			IFoo GetFoo();

			int Echo(int arg1);
			int Echo(int arg1, int arg2);
			int Echo(int arg1, int arg2, int arg3);
			int Echo(int arg1, int arg2, int arg3, int arg4);

			void Submit(int arg1);
			void Submit(int arg1, int arg2);
			void Submit(int arg1, int arg2, int arg3);
			void Submit(int arg1, int arg2, int arg3, int arg4);
			void Submit(byte[] arg);

			void SubmitWithParams(params int[] args);

			void CallMeOnce(bool flag, Guid guid);

			bool FindOne(params ICriteria[] criteria);
		}


		public interface IBar
		{
			int Echo(int value);
			int Value { get; set; }
		}

		public interface IFooImplemted : IFoo
		{

		}

		public class CustomExepction : Exception
		{
			public CustomExepction(string message, bool throwed)
				: base(message)
			{

			}
		}

		public class Log
		{
			public virtual void Info(string message)
			{
				throw new Exception(message);
			}
		}

		public abstract class FooBase
		{
			public virtual string GetString(string inString)
			{
				return inString;
			}
			public virtual Guid GetGuid()
			{
				return default(Guid);
			}

			public virtual int Echo(int arg1)
			{
				return arg1;
			}

			public virtual void ThrowException()
			{
				throw new InvalidOperationException("This should throw expection.");
			}
		}

		public class FooChild : FooBase
		{

		}

		public class FooOverridesEquals
		{
			public FooOverridesEquals(string name)
			{
				this.name = name;
			}

			public override bool Equals(object obj)
			{
				return (obj is FooOverridesEquals) &&
					((FooOverridesEquals)obj).name == this.name;
			}

			public override int GetHashCode()
			{
				if (!string.IsNullOrEmpty(name))
				{
					return name.GetHashCode();
				}

				return base.GetHashCode();
			}

			private string name;
		}

		public interface IParams
		{
			string ExecuteByName(int index, params string[] args);
			string ExecuteParams(params string[] args);
			string ExecuteArray(string[] args);
			string ExecuteArrayWithString(string arg1, Dictionary<string, object> dic);
		}

		public class ClassNonDefaultGuidConstructor
		{
			public ClassNonDefaultGuidConstructor(Guid guidValue)
			{
				this.guidValue = guidValue;
			}

			public Guid guidValue;
		}

		public class ClassWithNonDefaultConstructor
		{
			internal ClassWithNonDefaultConstructor()
			{

			}

			public ClassWithNonDefaultConstructor(string strValue, int intValue)
			{
				this.strValue = strValue;
				this.intValue = intValue;
			}
			public ClassWithNonDefaultConstructor(string strValue, int intValue, bool boolValue)
			{
				this.strValue = strValue;
				this.intValue = intValue;
				this.boolValue = boolValue;
			}

			public override string ToString()
			{
				return string.Format("{0}+{1}+{2}", strValue, intValue, boolValue);
			}

			private string strValue;
			private int intValue;
			private bool boolValue;
		}

		#endregion

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldInterpretNullAsDefaultInitializedValueTypeInCtorCall()
		{
			var mock = Mock.Create<ValueTypeInCtor>(null, null);
			Assert.Equal(0, mock.a);
		}

		public class ValueTypeInCtor
		{
			public readonly int a;

			public ValueTypeInCtor(ValueTypeInCtor q, int a)
			{
				this.a = a;
			}
		}

#if !__IOS__
		[ComImport]
#endif
		[Guid("4256871F-E8D7-40C2-9E1E-61CFA78C3EC1")]
		public interface IVersioned
		{
			[DispId(1)]
			string Identity { [DispId(1)] get; [DispId(1)] set; }
		}

#if !__IOS__
		[ComImport]
#endif
		[Guid("8DAF6396-300A-46E2-AA4C-CCB6103FB955")]
		public interface IVersioned2 : IVersioned
		{
			[DispId(1)]
			string Identity { [DispId(1)] get; [DispId(1)] set; }
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldCoalesceDifferentMembersWithSameDispIdInVersionedInterfaces()
		{
			var mock = Mock.Create<IVersioned2>();

			mock.Identity = "id";
			var baseIdentity = ((IVersioned)mock).Identity;

			Assert.Equal("id", baseIdentity);
			Assert.Equal("id", mock.Identity);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldMockInternalAbstract()
		{
			var mock = Mock.Create<InternalAbstract>();
			Assert.NotNull(mock);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldAcceptTypedEnumAsMethodArgument()
		{
			var mock = Mock.Create<IUnmanagedService>();
			Mock.Arrange(() => mock.IsAllowed(ShortFlags.One)).Returns(true);

			Assert.True(mock.IsAllowed(ShortFlags.One));
			Assert.False(mock.IsAllowed(ShortFlags.None));
			Assert.False(mock.IsAllowed(ShortFlags.Two));
		}

		public enum ShortFlags : short
		{
			None = 0, One = 1, Two = 100
		}

		public interface IUnmanagedService
		{
			bool IsAllowed(ShortFlags flags);
		}

#if !DOTNET35 && !SILVERLIGHT && !WINDOWS_PHONE
		[TestMethod, TestCategory("Lite"), TestCategory("Regression")]
		public void ShouldInterceptDynamicProxyMethodsFromMultipleThreads()
		{
			var generator = Mock.Create<IGuidGenerator>();
			var lotsOfGuids = Enumerable.Range(0, 3000)
								.AsParallel()
								.Select(x => generator.Generate())
								.ToArray();

			// didn't throw
		}

		public interface IGuidGenerator
		{
			Guid Generate();
		}
#endif

		public class ClassWithCtor
		{
			public ClassWithCtor(string s)
			{
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldThrowMeaningfulExceptionWhenConstructorArgumentsAreIncorrect()
		{
			var ex = Assert.Throws<Exception>(() => Mock.Create<ClassWithCtor>(5));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldArrangeMemberFromAdditionalInterfaceOnClassMock()
		{
			var mock = Mock.Create<Exception>(cfg => cfg.Implements<IIdentity>());
			var identity = mock as IIdentity;
			Mock.Arrange(() => identity.Name).Returns("mock");
			Assert.Equal("mock", identity.Name);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldArrangeMemberFromAdditionalInterfaceOnInterfaceMock()
		{
			var mock = Mock.Create<IPrincipal>(cfg => cfg.Implements<IIdentity>());
			var identity = mock as IIdentity;
			Mock.Arrange(() => identity.Name).Returns("mock");
			Assert.Equal("mock", identity.Name);
		}

#if !PORTABLE
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldMockTypesFromReflectionNamespace()
		{
			var reflectionTypes = new[]
			{
				typeof(MemberInfo),
				typeof(MethodBase),
				typeof(MethodInfo),
				typeof(ConstructorInfo),
				typeof(FieldInfo),
				typeof(PropertyInfo),
				typeof(EventInfo),
			};

			foreach (var type in reflectionTypes)
			{
				var mock = Mock.Create(type) as MemberInfo;
				Mock.Arrange(() => mock.Name).Returns("name");
				Assert.Equal("name", mock.Name);
			}
		}
#endif

#if !SILVERLIGHT && !WINDOWS_PHONE
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldMockWeakReference()
		{
			var weak = Mock.Create<WeakReference>();
			Mock.Arrange(() => weak.IsAlive).Returns(true);
			Assert.True(weak.IsAlive);
		}
#endif

		public class CtorWithDefaults
		{
			public readonly int A;

			public CtorWithDefaults(int a = 5)
			{
				this.A = a;
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldCallConstructorWithDefaultArguments()
		{
			var mock = Mock.Create<CtorWithDefaults>(Behavior.CallOriginal);
			Assert.Equal(5, mock.A);
		}

#if !PORTABLE
		public interface ITwoFace
		{
			int GetFace1();
			int GetFace2();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldFilterInterceptors()
		{
			var mock = Mock.Create<ITwoFace>(conf =>
			{
				conf.SetInterceptorFilter(mi => mi.Name == "GetFace1");
			});

			Mock.Arrange(() => mock.GetFace1()).Returns(10);
			Mock.Arrange(() => mock.GetFace2()).Returns(20); // TODO: this should actually throw an exception, instead

			Assert.Equal(10, mock.GetFace1());
			Assert.Equal(0, mock.GetFace2());
		}
#endif

		public class StaticCtor
		{
			public static bool called;

			static StaticCtor()
			{
				called = true;
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldCallStaticConstructorWhenCreatingMock()
		{
			var mock = Mock.Create<StaticCtor>();
			Assert.True(StaticCtor.called);
		}

		public interface IGeneric
		{
			string Get<TItem1, TItem2>(TItem1 a, TItem2 b);
		}

		public class Generic : IGeneric
		{
			public virtual string Get<T, U>(T t, U u)
			{
				return "";
			}
		}

		[TestMethod]
		public void ShouldCreateMockWithRenamedGenericParameters()
		{
			var mock = Mock.Create<Generic>();
			Mock.Arrange(() => mock.Get<string, int>("5", 5)).Returns("string");
			Assert.Equal("string", mock.Get<string, int>("5", 5));
		}

		public class SealedGeneric : IGeneric
		{
			public string Get<TItem1, TItem2>(TItem1 a, TItem2 b)
			{
				throw new NotImplementedException();
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldArrangeFinalGenericMethodThroughInterface()
		{
			IGeneric mock = Mock.Create<SealedGeneric>();
			Mock.Arrange(() => mock.Get(5, "4")).Returns("123");
			Assert.Equal("123", mock.Get(5, "4"));
		}

#if LITE_EDITION && !COREFX
		[TestMethod, TestCategory("Lite"), TestCategory("Mock")]
		public void ShouldMockNoninheritableInterfaceMembers()
		{
			var mock = Mock.Create<PrivateInterface>(Behavior.CallOriginal);

			Assert.Throws<InvalidOperationException>(() => ((IJustDoThat)mock).DoThat());
			Mock.Arrange(() => mock.DoThat()).DoNothing();
			((IJustDoThat)mock).DoThat();

			Assert.Throws<InvalidOperationException>(() => ((IJustDoIt)mock).JustDoIt());
			Mock.Arrange(() => ((IJustDoIt)mock).JustDoIt()).DoNothing();
			((IJustDoIt)mock).JustDoIt();

			Assert.Throws<InvalidOperationException>(() => ((Scope.IImplementable)mock).Do());
			Mock.Arrange(() => ((Scope.IImplementable)mock).Do()).DoNothing();
			((Scope.IImplementable)mock).Do();

			Assert.Throws<InvalidOperationException>(() => ((Scope.IImplementable2)mock).Do());
			Mock.Arrange(() => ((Scope.IImplementable2)mock).Do()).DoNothing();
			((Scope.IImplementable2)mock).Do();

			Assert.Throws<ElevatedMockingException>(() => Mock.Arrange(() => ((INonImplementable)mock).Do()).DoNothing());
			Assert.Throws<InvalidOperationException>(() => ((INonImplementable)mock).Do());
		}

		public interface IJustDoIt
		{
			void JustDoIt();
		}

		public interface IJustDoThat
		{
			void DoThat();
		}

		private interface INonImplementable
		{
			void Do();
		}

		internal class Scope
		{
			public interface IImplementable
			{
				void Do();
			}

			protected internal interface IImplementable2
			{
				void Do();
			}
		}

		public class PrivateInterface : IJustDoIt, IJustDoThat, INonImplementable, Scope.IImplementable, Scope.IImplementable2
		{
			void IJustDoIt.JustDoIt()
			{
				throw new InvalidOperationException();
			}

			public void DoThat()
			{
				throw new InvalidOperationException();
			}

			void INonImplementable.Do()
			{
				throw new InvalidOperationException();
			}

			void Scope.IImplementable.Do()
			{
				throw new InvalidOperationException();
			}

			void Scope.IImplementable2.Do()
			{
				throw new InvalidOperationException();
			}
		}
#endif
	}

	internal abstract class InternalAbstract
	{
		internal abstract string Bar { get; set; }
	}
}
