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
using System.IO;
using System.Linq;
using Telerik.JustMock.Core;
using Telerik.JustMock.Diagnostics;

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

namespace Telerik.JustMock.Tests
{
	/// <summary>
	/// Validates Mock.Assert capabilities
	/// </summary>
	[TestClass]
	public class AssertionFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertNotArrangedAndInvoked()
		{
			var foo = Mock.Create<IFoo>();

			foo.VoidCall();

			Mock.Assert(() => foo.VoidCall());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldThrowForNotInvokedAndNotArranged()
		{
			var foo = Mock.Create<IFoo>();
			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.VoidCall()));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertCallWithSetup()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.VoidCall()).DoNothing();

			foo.VoidCall();

			Mock.Assert(() => foo.VoidCall());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertOnlyTheOnesMarkedAsMustBeCalled()
		{
			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.Echo(1)).Returns(10).MustBeCalled();
			Mock.Arrange(() => foo.Echo(2)).Returns(11);

			Assert.Equal(foo.Echo(1), 10);
			Assert.Equal(foo.Echo(2), 11);

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldThrowForRequiredCallThatIsNotInvoked()
		{
			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.Echo(1)).Returns(10).MustBeCalled();
			Mock.Arrange(() => foo.Echo(2)).Returns(11).MustBeCalled();


			Assert.Equal(foo.Echo(1), 10);
			Assert.Throws<AssertionException>(() => Mock.Assert(foo));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertOnlyTheSpecifiedOne()
		{
			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.Echo(1)).Returns(10).MustBeCalled();
			Mock.Arrange(() => foo.Echo(2)).Returns(11).MustBeCalled();

			// calling Echo 1
			Assert.Equal(foo.Echo(1), 10);
			// asserting Echo 2
			Mock.Assert(() => foo.Echo(1));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldPassWhenExpectedSetupIsInvokedForAssertAll()
		{
			var foo = Mock.Create<Foo>();
			Mock.Arrange(() => foo.Echo(1)).Returns(10);

			foo.Echo(1);

			Mock.AssertAll(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldNotRaiseForAssertAllWhenProperyIsAutoArranged()
		{
			var foo = Mock.Create<Foo>();

			foo.Value = true;

			Mock.AssertAll(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldNotRaiseForAssertAllWhenArrangeSetIsApplied()
		{
			var foo = Mock.Create<Foo>();

			Mock.ArrangeSet(() => foo.Value = true);

			foo.Value = true;

			Mock.AssertAll(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldThrowWheMockedSetupIsNotInvokedForAssertAll()
		{
			var foo = Mock.Create<Foo>();
			Mock.Arrange(() => foo.Echo(1)).Returns(10);
			Assert.Throws<AssertionException>(() => Mock.AssertAll(foo));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldFailAssertionForOtherThanTheOneSpecified()
		{
			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.Echo(1)).Returns(10).MustBeCalled();
			Mock.Arrange(() => foo.Echo(2)).Returns(11).MustBeCalled();

			Assert.Equal(foo.Echo(1), 10);
			Assert.Throws<AssertionException>(() => { Mock.Assert(() => foo.Echo(2)); });
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertExpecationRecursively()
		{
			var bar = Mock.Create<Bar>();

			Mock.Arrange(() => bar.Echo(1)).Returns(2).MustBeCalled();

			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.GetBar()).Returns(bar);

			Assert.Equal(foo.GetBar().Echo(1), 2);
			//asserts bar.
			Mock.Assert(foo.GetBar());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertAssertablesRecursively()
		{
			var bar = Mock.Create<Bar>();

			Mock.Arrange(() => bar.Echo(1)).Returns(2).MustBeCalled();
			Mock.Arrange(() => bar.Echo(2)).Returns(3).MustBeCalled();

			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.GetBar()).Returns(bar);

			Assert.Equal(foo.GetBar().Echo(1), 2);
			Assert.Throws<AssertionException>(() => Mock.Assert(foo));
			Assert.Equal(foo.GetBar().Echo(2), 3);

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertMethodWihtMultipleParameters()
		{
			var foo = Mock.Create<Foo>();

			// first call wont be asserted.
			Mock.Arrange(() => foo.Sum(1, 3)).Returns(1);
			// second call will be asserted.
			Mock.Arrange(() => foo.Sum(1, 2)).Returns(1);

			Assert.Equal(foo.Sum(1, 2), 1);

			Mock.Assert(() => foo.Sum(1, 2));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertMethodWithOutputParam()
		{
			var foo = Mock.Create<IFoo>();

			bool expected = true;

			Mock.Arrange(() => foo.EchoOut(out expected)).DoNothing();

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.EchoOut(out expected)));

			foo.EchoOut(out expected);

			Mock.Assert(() => foo.EchoOut(out expected));

			Assert.True(expected);
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldThrowForUnUsedSetup()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo(1)).Returns(10);

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Echo(1)));
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertPropertyGetCall()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Value).Returns(10);

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Value));
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertCallWithMatchers()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo(Arg.IsAny<int>())).Returns((int i) => i);

			Assert.Equal(foo.Echo(10), 10);

			Mock.Assert(() => foo.Echo(Arg.Matches<int>(x => x == 10)));

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Echo(Arg.Matches<int>(x => x == 11))));
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertMatherWithAnyInAssertion()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.Matches<int>(x => x == 10))).Returns((int i) => i);

			Assert.Equal(foo.Echo(10), 10);

			Mock.Assert(() => foo.Echo(Arg.IsAny<int>()));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertCallHavingMultipleArgsWithMatchers()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(Arg.IsAny<int>(), Arg.IsAny<int>())).Returns((int id, int i) => i);

			Assert.Equal(foo.Execute(100, 10), 10);

			Mock.Assert(() => foo.Execute(Arg.IsAny<int>(), Arg.Matches<int>(x => x == 10)));
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertMatcherWithRealValueArguments()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(Arg.IsAny<int>(), Arg.IsAny<int>())).Returns((int id, int i) => i);

			Assert.Equal(foo.Execute(100, 10), 10);

			Mock.Assert(() => foo.Execute(100, Arg.Matches<int>(x => x == 10)));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldThrowForInvalidAssertionWithMatcher()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(Arg.IsAny<int>(), Arg.IsAny<int>())).Returns((int id, int i) => i);

			Assert.Equal(foo.Execute(100, 10), 10);

			Assert.Throws<AssertionException>(() =>
			{
				Mock.Assert(() => foo.Execute(Arg.Matches<int>(x => x == 4), Arg.Matches<int>(x => x == 10)));
			});
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldThrowForNonSpecificLambdaCallsOnAssert()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo(1)).Returns(2);

			foo.Echo(1);

			Assert.Throws<MockException>(() => Mock.Assert(() => foo));

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertPropertySetUsingAssertable()
		{
			var foo = Mock.Create<IFoo>();

			Mock.ArrangeSet(() => { foo.Value = 1; }).DoNothing().MustBeCalled();

			Assert.Throws<AssertionException>(() => Mock.Assert(foo));

			foo.Value = 1;

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertSpecificPropertySet()
		{
			var foo = Mock.Create<IFoo>();

			Mock.ArrangeSet(() => { foo.Value = 1; });

			Assert.Throws<AssertionException>(() => Mock.AssertSet(() => foo.Value = 1));

			foo.Value = 1;

			Mock.AssertSet(() => foo.Value = 1);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldBeAbleToSpecifyOccurenceForAssertSet()
		{
			var foo = Mock.Create<IFoo>();

			foo.Value = 1;
			foo.Value = 2;

			Mock.AssertSet(() => foo.Value = Arg.AnyInt, Occurs.Exactly(2));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertSetWithMatcherWhenItInvokesAnotherMethodDuringSet()
		{
			var foo = Mock.Create<Foo>(Behavior.CallOriginal);

			// invokes OnFooValueChanged.
			foo.FooValue = Mock.Create<IFoo>(Behavior.CallOriginal);

			Mock.AssertSet(() => foo.FooValue = Arg.IsAny<IFoo>(), Occurs.Once());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldNotThrowDuringAssertForCallOriginalWhenNoArrangeSpecified()
		{
			var foo = Mock.Create<FooWithSetThatThows>(Behavior.CallOriginal);
			Mock.AssertSet(() => foo.Value = Arg.AnyInt, Occurs.Never());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertMethodCallWithOutputParam()
		{
			var foo = Mock.Create<IFoo>();

			bool expected = true;

			Mock.Arrange(() => foo.EchoOut(out expected)).DoNothing();

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.EchoOut(out expected)));

			bool actual = false;

			foo.EchoOut(out actual);

			Mock.Assert(() => foo.EchoOut(out expected));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShoudThrowForUninitializedIndexedSet()
		{
			var foo = Mock.Create<IFooIndexed>();

			Mock.ArrangeSet(() => foo[0] = "ping");

			Assert.Throws<AssertionException>(() => Mock.AssertSet(() => foo[0] = "ping"));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertIndexerSet()
		{
			var foo = Mock.Create<IFooIndexed>();

			Mock.ArrangeSet(() => foo[0] = "ping");

			foo[0] = "ping";

			Mock.AssertSet(() => foo[0] = "ping");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertSetWithIndexerWithMatcher()
		{
			var foo = Mock.Create<IFooIndexed>();

			Mock.ArrangeSet(() => foo[0] = "ping");

			foo[0] = "ping";

			Mock.AssertSet(() => foo[0] = Arg.Matches<string>(x => x.StartsWith("p")));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldThrowSetIndexerWithMatcherThatIsNotCalled()
		{
			var foo = Mock.Create<IFooIndexed>();

			Mock.ArrangeSet(() => foo[0] = "ping");

			Assert.Throws<AssertionException>(() =>
			{
				Mock.AssertSet(() => foo[0] = Arg.Matches<string>(x => x.StartsWith("p")));
			});
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertMatcherSetupWithMatcherForIndexer()
		{
			var foo = Mock.Create<IFooIndexed>();

			Mock.ArrangeSet(() => foo[0] = Arg.IsAny<string>());

			foo[0] = "ping";

			Mock.AssertSet(() => foo[0] = Arg.Matches<string>(x => string.Compare("ping", x) == 0));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldEnsureMockAssertionAfterThrows()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(Arg.IsAny<string>())).Throws(new InvalidOperationException()).MustBeCalled();

			Assert.Throws<InvalidOperationException>(() => foo.Execute(string.Empty));

			// should not throw any exception.
			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertCallsHavingListAsReturn()
		{
			var repository = Mock.Create<IFooRepository>();

			Mock.Arrange(() => repository.GetFoos()).Returns(new List<Foo>
			{
				new Foo(),
				new Foo(),
				new Foo(),
				new Foo(),
				new Foo()
			})
			.MustBeCalled();

			IList<Foo> foos = repository.GetFoos();

			var expected = 5;
			var actual = foos.Count;
			Assert.Equal(expected, actual);

			Mock.Assert(repository);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertSetupWithIgnoreArguments()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute(0, 0)).IgnoreArguments().Returns(10);

			foo.Execute(1, 1);

			Mock.Assert(() => foo.Execute(Arg.AnyInt, Arg.AnyInt));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertCallWithMockAsArgument()
		{
			FooResult result = Mock.Create<FooResult>();

			var data = Mock.Create<IDataAccess>();

			data.ProcessFilterResult(result, "a", "b");

			Mock.Assert(() => data.ProcessFilterResult(result, "a", "b"), Occurs.Once());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShoudIgnoreExecptionForReturnDuringAssert()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo(Arg.AnyInt))
				.Returns((int value) =>
				{
					if (value == default(int))
					{
						throw new InvalidOperationException();
					}

					return value;
				});

			foo.Echo(10);

			Mock.Assert(() => foo.Echo(Arg.AnyInt), Occurs.Once());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldNotResursivelyAssertForSetupThatReturnItSelf()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.GetFoo()).Returns(foo).MustBeCalled();

			foo.GetFoo();

			Mock.Assert(foo);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertCallWithArrayArguments()
		{
			var expression = Mock.Create<FooExrepssion>();

			var expected = new[] { "x", "y" };

			expression.Update(expected);

			Mock.Assert(() => expression.Update(expected));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldFailCallWithArrayArgumentsHavingDifferentValues()
		{
			var expression = Mock.Create<FooExrepssion>();

			var expected = new[] { "x", "y" };
			var assert = new[] { "x", "z" };

			expression.Update(expected);

			Assert.Throws<AssertionException>(() =>
			{
				Mock.Assert(() => expression.Update(assert));
			});
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertCallWithArrayOfValueTypeArguments()
		{
			var expression = Mock.Create<FooExrepssion>();

			expression.Update(new[] { 1, 2 });

			Mock.Assert(() => expression.Update(new[] { 1, 2 }));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertCallWithNullValuedArgument()
		{
			var expression = Mock.Create<FooExrepssion>();

			expression.UpdateIt(null);

			Mock.Assert(() => expression.UpdateIt(null));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldBeAbleToAssertOccursUsingMatcherForSimilarCallAtOnce()
		{
			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.Echo(1)).Returns((int arg) => arg);
			Mock.Arrange(() => foo.Echo(2)).Returns((int arg) => arg);
			Mock.Arrange(() => foo.Echo(3)).Returns((int arg) => arg);

			foo.Echo(1);
			foo.Echo(2);
			foo.Echo(3);

			Mock.Assert(() => foo.Echo(Arg.AnyInt), Occurs.Exactly(3));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldFailForOccursUsingMatcherForSimilarCallWhenNotExpected()
		{
			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.Echo(1)).Returns((int arg) => arg);
			Mock.Arrange(() => foo.Echo(2)).Returns((int arg) => arg);
			Mock.Arrange(() => foo.Echo(3)).Returns((int arg) => arg);

			foo.Echo(1);
			foo.Echo(2);

			Assert.Throws<AssertionException>(() =>
			{
				Mock.Assert(() => foo.Echo(Arg.AnyInt), Occurs.Exactly(3));
			});
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertWithAnyAssertForExpectedInvocationOfSetupWithOccursFollowedBySimilarSetup()
		{
			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.Echo(1)).Returns((int arg) => arg).Occurs(2);
			Mock.Arrange(() => foo.Echo(2)).Returns((int arg) => arg);

			foo.Echo(1);
			foo.Echo(2);
			foo.Echo(1);

			Mock.Assert(() => foo.Echo(Arg.AnyInt));
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldFailAnyAssertWhenNumberOfTimesExecutedIsNotSameAsExpected()
		{
			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.Echo(1)).Returns((int arg) => arg).Occurs(2);

			foo.Echo(1);

			Assert.Throws<AssertionException>(() => Mock.Assert(() => foo.Echo(Arg.AnyInt)));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldFailPreOccursForAnyAssertIfNotExpectedAsReqThatIsFollowedBySimilarSetup()
		{
			var foo = Mock.Create<Foo>();

			Mock.Arrange(() => foo.Echo(1)).Returns((int arg) => arg).Occurs(2);
			Mock.Arrange(() => foo.Echo(2)).Returns((int arg) => arg);

			foo.Echo(1);
			foo.Echo(2);

			Assert.Throws<AssertionException>(() =>
			{

				Mock.Assert(() => foo.Echo(Arg.AnyInt));
			});
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertCallWhenCombinedWithEnumFollowedByAnyTypeArgs()
		{
			var region = Mock.Create<IRegionManager>();

			region.RequestNavigate(RegionNames.OperationsEditRegion, new FooExrepssion());

			Mock.Assert(() => region.RequestNavigate(RegionNames.OperationsEditRegion, Arg.IsAny<FooExrepssion>()), Occurs.Once());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertForAnyArgumentsWhenIgnoreSwitchIsSpecified()
		{
			var region = Mock.Create<IRegionManager>();

			region.RequestNavigate(RegionNames.OperationsEditRegion, new FooExrepssion());

			Mock.Assert(() => region.RequestNavigate(RegionNames.OperationsEditRegion, null), Args.Ignore());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertForAnyArgumentsWhenIgnoreSwitchAndOccursSpecified()
		{
			var region = Mock.Create<IRegionManager>();
			Mock.Assert(() => region.RequestNavigate(RegionNames.OperationsEditRegion, null), Args.Ignore(), Occurs.Never());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertForArgMatchesWhenArgumentCalulatedBasedOnMockValues()
		{
			var viewServiceMock = Mock.Create<IViewService>();

			var view1 = Mock.Create<IView>();
			var view2 = Mock.Create<IView>();
			var view3 = Mock.Create<IView>();

			Mock.Arrange(() => viewServiceMock.Views).Returns(new[] { view1, view2, view3 });
			Mock.Arrange(() => viewServiceMock.ActiveView).Returns(view2);

			Mock.Arrange(() => viewServiceMock.TryCloseViews(Arg.IsAny<IEnumerable<IView>>()));

			viewServiceMock.TryCloseViews(viewServiceMock.Views.Except(new[] { viewServiceMock.ActiveView }));

			Mock.Assert(() => viewServiceMock.TryCloseViews(Arg.Matches<IEnumerable<IView>>((views) => views.All((view) => view == view1 || view == view3))), Occurs.Once());
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldVerifyThatMockArgumentIsNotAssertedInsteadOfExpected()
		{
			var viewServiceMock = Mock.Create<IViewService>();

			var view1 = Mock.Create<IView>();
			var view2 = Mock.Create<IView>();
			var view3 = Mock.Create<IView>();

			Mock.Arrange(() => viewServiceMock.Views).Returns(new[] { view1, view2, view3 });
			Mock.Arrange(() => viewServiceMock.ActiveView).Returns(view2);

			Mock.Arrange(() => viewServiceMock.TryCloseViews(Arg.IsAny<IEnumerable<IView>>()));

			viewServiceMock.TryCloseViews(viewServiceMock.Views.Except(new[] { viewServiceMock.ActiveView }));

			// this will increase the execution number of GetHashCode()
			Assert.True(new[] { view1, view3 }.All((view) => view == view1 || view == view3));
			Mock.Assert(() => viewServiceMock.TryCloseViews(Arg.Matches<IEnumerable<IView>>((views) => views.All((view) => view == view1 || view == view3))), Occurs.Once());
		}


		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldThrowAssertFailedWithCompositeFailureMessage()
		{
			var foo = Mock.Create<IFoo>();
			Mock.Arrange(() => foo.Echo(Arg.AnyInt)).Occurs(3);
			Mock.Arrange(() => foo.Echo(15)).InOrder().OccursOnce();
			Mock.Arrange(() => foo.Echo(20)).InOrder().OccursOnce();

			var ex = Assert.Throws<AssertionException>(() => Mock.Assert(foo));
			Assert.True(ex.Message.Contains("1. "));
			Assert.True(ex.Message.Contains("2. "));
			Assert.True(ex.Message.Contains("3. "));
			Assert.True(ex.Message.Contains("4. "));
			Assert.True(ex.Message.Contains("--no calls--"));
		}

		public abstract class AbstractCUT
		{
			public void A() { this.DoA(); }
			public abstract void DoA();

			public void B() { this.DoB(); }
			protected abstract void DoB();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertAbstractMethodExercisedOnAbstractCUT()
		{
			var target = Mock.Create<AbstractCUT>(Behavior.CallOriginal);

			Mock.Arrange(() => target.DoA()).DoNothing();
			target.A();
			Mock.Assert(() => target.DoA(), Occurs.Once());

			Mock.NonPublic.Arrange(target, "DoB").DoNothing();
			target.B();
			Mock.NonPublic.Assert(target, "DoB", Occurs.Once());
		}

		public class FooWithSetThatThows
		{
			public virtual int Value
			{
				get { return value; }
				set { throw new NotImplementedException(); }
			}
			private readonly int value;
		}

		public interface IView
		{
		}

		public interface IViewService
		{
			void TryCloseViews(IEnumerable<IView> param1);
			IView ActiveView { get; set; }
			IView[] Views { get; set; }
		}

		public enum RegionNames
		{
			OperationsEditRegion
		}

		public interface IRegionManager
		{
			void RequestNavigate(RegionNames names, FooExrepssion exp);
		}


		public class FooExrepssion
		{
			public virtual void Update(IEnumerable<string> arguments)
			{

			}
			public virtual void Update(IEnumerable<int> arguments)
			{

			}
			public virtual void UpdateIt(FooResult value)
			{

			}
		}

		public class FooResult
		{

		}

		public interface IDataAccess
		{
			void ProcessFilterResult(FooResult result, string email, string body);
		}

		public interface IFooRepository
		{
			IList<Foo> GetFoos();
		}

		public interface IFoo
		{
			void EchoOut(out bool expected);
			void VoidCall();
			int Value { get; set; }
			void Execute(string arg);
			int Echo(int intValue);
			int Execute(int arg1, int arg2);

			IFoo GetFoo();
		}

		public interface IFooIndexed
		{
			string this[int key] { get; set; }
		}

		public class Foo
		{
			public virtual int Echo(int input)
			{
				return input;
			}
			public virtual int Sum(int x, int y)
			{
				return x + y;
			}

			public virtual Bar GetBar()
			{
				return new Bar();
			}

			public virtual bool Value { get; set; }

			protected virtual void OnFooValueChanged()
			{

			}

			public virtual IFoo FooValue
			{
				get
				{
					return fooValue;

				}
				set
				{
					if (this.fooValue != value)
					{
						this.fooValue = value;
						OnFooValueChanged();
					}
				}
			}

			private IFoo fooValue;
		}

		public class Bar
		{
			public virtual int Echo(int input)
			{
				return input;
			}
		}

		public interface IMyInterface
		{
			int Foo(object stuff);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldAssertFunctionCallExpressionWithArgsAndOccurs()
		{
			var thing = Mock.Create<IMyInterface>();

			thing.Foo(123);

			Mock.Assert(() => thing.Foo(new object()), Args.Ignore(), Occurs.Once());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldGetTimesCalledOfFunction()
		{
			var mock = Mock.Create<IFoo>();
			var x = mock.Value;
			Assert.Equal(1, Mock.GetTimesCalled(() => mock.Value));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldGetTimesCalledOfVoidMethod()
		{
			var mock = Mock.Create<IFoo>();
			mock.VoidCall();
			Assert.Equal(1, Mock.GetTimesCalled(() => mock.VoidCall()));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldGetTimesCalledOfFunctionWithArgs()
		{
			var mock = Mock.Create<IFoo>();
			mock.Echo(5);
			Assert.Equal(1, Mock.GetTimesCalled(() => mock.Echo(0), Args.Ignore()));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldGetTimesCalledOfVoidMethodWithArgs()
		{
			var mock = Mock.Create<IFoo>();
			mock.Execute("aaa");
			Assert.Equal(1, Mock.GetTimesCalled(() => mock.Execute(null), Args.Ignore()));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldGetTimesCalledOfSetter()
		{
			var mock = Mock.Create<IFoo>();
			mock.Value = 10;
			Assert.Equal(1, Mock.GetTimesSetCalled(() => mock.Value = 10));
			Assert.Equal(0, Mock.GetTimesSetCalled(() => mock.Value = 20));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldGetTimesCalledOfSetterWithArgs()
		{
			var mock = Mock.Create<IFoo>();
			mock.Value = 10;
			Assert.Equal(1, Mock.GetTimesSetCalled(() => mock.Value = 0, Args.Ignore()));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Assertion")]
		public void ShouldGetDebugViewTraceInMockException()
		{
			var traceEnabled = DebugView.IsTraceEnabled;
			try
			{
				DebugView.IsTraceEnabled = true;

				var mock = Mock.Create<IFoo>();
				var ex = Assert.Throws<AssertionException>(() => Mock.Assert(() => mock.Value, Occurs.Once()));

				Assert.NotNull(ex.InnerException);
				Assert.Equal(typeof(DebugViewDetailsException), ex.InnerException.GetType());
			}
			finally
			{
				DebugView.IsTraceEnabled = traceEnabled;
			}
		}
	}

#if !PORTABLE
#if !NUNIT
	[TestClass]
	public class DebugViewTests
	{
#if !SILVERLIGHT
		private static string resultsDirectory;
#endif

		[AssemblyInitialize]
		public static void AssemblyInit(TestContext testContext)
		{
#if !SILVERLIGHT
			resultsDirectory = testContext.TestRunResultsDirectory;
#endif
			DebugView.IsTraceEnabled = true;
		}

		[AssemblyCleanup]
		public static void AssemblyUninit()
		{
			var trace = DebugView.FullTrace;
			DebugView.IsTraceEnabled = false;

#if !SILVERLIGHT
			Directory.CreateDirectory(resultsDirectory);
			File.WriteAllText(Path.Combine(resultsDirectory, "VSTest.FullTrace.log"), trace);
#endif
		}
	}
#else
	[SetUpFixture]
	public class DebugViewTests
	{
		[SetUp]
		public void AssemblyInit()
		{
			DebugView.IsTraceEnabled = true;
		}

		[TearDown]
		public void AssemblyUninit()
		{
			var trace = DebugView.FullTrace;
			DebugView.IsTraceEnabled = false;


			File.WriteAllText(Path.Combine(TestContext.CurrentContext.WorkDirectory, "NUnit.FullTrace.log"), trace);
		}
	}
#endif
#endif
}
