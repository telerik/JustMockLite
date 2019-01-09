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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Telerik.JustMock.Core;
using Telerik.JustMock.Tests.EventFixureDependencies;

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
#if XUNIT2
using AssertionException = Xunit.Sdk.XunitException;
#else
using AssertionException = Xunit.Sdk.AssertException;
#endif
#elif VSTEST_PORTABLE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AssertionException = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssertFailedException;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#endif
#endregion

#if XUNIT2
#pragma warning disable xUnit1013 
#endif

namespace Telerik.JustMock.Tests
{
	[TestClass]
	public class EventsFixture
	{

		private ProjectNavigatorViewModel viewModel;
		private ISolutionService solutionService;

#if XUNIT
		public EventsFixture()
		{
			Initialize();
		}
#endif

		[TestInitialize]
		public void Initialize()
		{
			this.solutionService = Mock.Create<ISolutionService>();
			this.viewModel = new ProjectNavigatorViewModel(this.solutionService);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldRaiseCustomEventOnMethodCall()
		{
			var foo = Mock.Create<IFoo>();

			const string expected = "ping";
			string actual = string.Empty;

			Mock.Arrange(() => foo.RaiseMethod()).Raises(() => foo.CustomEvent += null, expected);

			foo.CustomEvent += (s) => { actual = s; };
			foo.RaiseMethod();

			Assert.Equal(expected, actual);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShoulRaiseCustomEventForFuncCalls()
		{
			bool echoed = false;

			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Echo("string")).Raises(() => foo.EchoEvent += null, true).Returns("echoed");
			foo.EchoEvent += (c) => { echoed = c; };

			Assert.Equal(foo.Echo("string"), "echoed");
			Assert.True(echoed);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldRaiseEventWhenExpectationIsMet()
		{
			var executor = Mock.Create<IExecutor<int>>();

			bool raised = false;

			Mock.Arrange(() => executor.Execute(Arg.IsAny<int>())).Raises(() => executor.Executed += null, EventArgs.Empty);

			executor.Executed += delegate { raised = true; };

			executor.Execute(1);

			Assert.True(raised);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldRaiseEventForEventArgsLambdaWithOneArgument()
		{
			var executor = Mock.Create<IExecutor<int>>();

			Mock.Arrange(() => executor.Execute(Arg.IsAny<string>()))
				.Raises(() => executor.Done += null, (string s) => new FooArgs { Value = s });

			FooArgs args = null;
			executor.Done += (sender, e) => args = e;
			executor.Execute("done");

			Assert.Equal(args.Value, "done");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldRaiseEventForEventArgsLambdaWithTwoArguments()
		{
			var executor = Mock.Create<IExecutor<int>>();

			Mock.Arrange(() => executor.Execute(Arg.IsAny<string>(), Arg.IsAny<int>()))
				.Raises(() => executor.Done += null, (string s, int i) => new FooArgs { Value = s + i });

			FooArgs args = null;
			executor.Done += (sender, e) => args = e;
			executor.Execute("done", 2);

			Assert.Equal(args.Value, "done2");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldRaiseEventForEventArgsLambdaWithThreeArguments()
		{
			var executor = Mock.Create<IExecutor<int>>();

			Mock.Arrange(() => executor.Execute(Arg.IsAny<string>(), Arg.IsAny<int>(), Arg.IsAny<bool>()))
				.Raises(() => executor.Done += null, (string s, int i, bool b) => new FooArgs { Value = s + i + b });

			FooArgs args = null;
			executor.Done += (sender, e) => args = e;
			executor.Execute("done", 3, true);

			Assert.Equal(args.Value, "done3True");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldRaiseEventForEventArgsLambdaWithFourArguments()
		{
			var executor = Mock.Create<IExecutor<int>>();

			Mock.Arrange(() => executor.Execute(Arg.IsAny<string>(), Arg.IsAny<int>(), Arg.IsAny<bool>(), Arg.IsAny<string>()))
				.Raises(() => executor.Done += null, (string s, int i, bool b, string s1) => new FooArgs { Value = s + i + b + s1 });

			FooArgs args = null;
			executor.Done += (sender, e) => args = e;
			executor.Execute("done", 4, true, "ok");

			Assert.Equal(args.Value, "done4Trueok");
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldAssertRaiseAndReturnForFuncCallWithOneArg()
		{
			var executor = Mock.Create<IExecutor<int>>();

			Mock.Arrange(() => executor.Echo(Arg.IsAny<string>()))
			.Raises(() => executor.Done += null, (string s) => new FooArgs { Value = s })
			.Returns((string s) => s);

			FooArgs args = null;
			executor.Done += (sender, e) => args = e;

			Assert.Equal(executor.Echo("echo"), args.Value);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldAssertMultipleEventSubscription()
		{
			var foo = Mock.Create<IFoo>();

			Mock.Arrange(() => foo.Execute()).Raises(() => foo.EchoEvent += null, true);

			bool echoed1 = false;
			bool echoed2 = false;

			foo.EchoEvent += c => { echoed1 = c; };
			foo.EchoEvent += c => { echoed2 = c; };

			foo.Execute();

			Assert.True(echoed1);
			Assert.True(echoed2);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldRaiseEventWithStandardEventArgs()
		{
			var executor = Mock.Create<IExecutor<int>>();

			string acutal = null;
			string expected = "ping";

			executor.Done += delegate(object sender, FooArgs args)
			{
				acutal = args.Value;
			};

			Mock.Raise(() => executor.Done += null, new FooArgs(expected));

			Assert.Equal(expected, acutal);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldRaiseEventWithCustomEventArgs()
		{
			var foo = Mock.Create<IFoo>();

			string expected = "ping";
			string acutal = string.Empty;


			foo.CustomEvent += delegate(string s)
			{
				acutal = s;
			};

			Mock.Raise(() => foo.CustomEvent += null, expected);
			Assert.Equal(expected, acutal);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events"), TestCategory("MockingContext")]
		public void ShouldAssertMockRaiseFromInsideAContainer()
		{
			var foo = Mock.Create<IFoo>();
			var projectEventArgs = new ProjectEventArgs(foo);
			Mock.Raise(() => this.solutionService.ProjectAdded += null, projectEventArgs);
			Assert.True(this.viewModel.IsProjectAddedCalled);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldNotCallDelegateAfterEventDetach()
		{
			var executor = Mock.Create<IExecutor<int>>();

			Mock.Arrange(() => executor.Execute(Arg.IsAny<string>()))
				.Raises(() => executor.Done += null, (string s) => new FooArgs { Value = s });

			FooArgs args = null;
			EventHandler<FooArgs> handler = (sender, e) => args = e;
			executor.Done += (o, e) => { };

			executor.Done += handler;
			executor.Execute("done");
			Assert.Equal(args.Value, "done");

			executor.Done -= handler;
			args = null;
			executor.Execute("done");
			Assert.Null(args);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldRetainArrangementsInRaiseDelegate()
		{
			var activeDocument = Mock.Create<IDocument>(Behavior.Loose);
			var activeView = Mock.Create<IDocumentView>();
			Mock.Arrange(() => activeView.Document).Returns(activeDocument);
			Mock.Raise(() => activeView.Document.IsDirtyChanged += null, EventArgs.Empty);
		}

		public interface IHasEvent
		{
			event Action StuffHappened;
			int Value { get; }
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldRetainArrangementsInMockEventHandler()
		{
			var mock = Mock.Create<IHasEvent>();
			Mock.Arrange(() => mock.Value).Returns(5);
			int actualValue = 0;
			mock.StuffHappened += () => actualValue = mock.Value;

			Mock.Raise(() => mock.StuffHappened += null);

			Assert.Equal(5, actualValue);
		}

#if !COREFX
		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldRetainArrangementsInEventHandlerFromPrivateAccessor()
		{
			var mock = Mock.Create<IHasEvent>();
			Mock.Arrange(() => mock.Value).Returns(5);
			int actualValue = 0;
			var coll = new ObservableCollection<object>();
			coll.CollectionChanged += (o, e) => actualValue = mock.Value;

			new PrivateAccessor(coll).RaiseEvent("CollectionChanged", coll, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

			Assert.Equal(5, actualValue);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldRetainArrangementsInEventHandlerFromPrivateAccessorForEventWithRaiseMethod()
		{
			var mock = Mock.Create<IHasEvent>();
			Mock.Arrange(() => mock.Value).Returns(5);
			int actualValue = 0;

			var type = EventClassFactory.CreateClassWithEventWithRaiseMethod();
			var obj = Activator.CreateInstance(type);
			Action probe = () => actualValue = mock.Value;
			type.GetField("Probe").SetValue(obj, probe);

			new PrivateAccessor(obj).RaiseEvent("StuffHappened");

			Assert.Equal(5, actualValue);
		}
#endif

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldAssertEventHandlerAddingOccurrence()
		{
			var doc = Mock.Create<IDocument>();
			Mock.ArrangeSet(() => doc.IsDirtyChanged += null).IgnoreArguments().OccursOnce();

			Assert.Throws<AssertionException>(() => Mock.Assert(doc));

			doc.IsDirtyChanged += (o, e) => { };
			Mock.Assert(doc);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldRaiseEventWithNullEventArgsArgument()
		{
			var doc = Mock.Create<IDocument>();
			EventArgs args = EventArgs.Empty;
			doc.IsDirtyChanged += (o, e) => args = e;
			Mock.Raise(() => doc.IsDirtyChanged += null, null);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ShouldThrowIncompatibleSignatureExceptionWhenExpectedArgumentsDontMatch()
		{
			var doc = Mock.Create<IDocument>();
			doc.IsDirtyChanged += (o, e) => { };
			Assert.Throws<Exception>(() => Mock.Raise(() => doc.IsDirtyChanged += null));
			Assert.Throws<Exception>(() => Mock.Raise(() => doc.IsDirtyChanged += null, 1, 2));
		}

		public interface IDocumentView
		{
			IDocument Document { get; }
		}

		public interface IDocument
		{
			event EventHandler IsDirtyChanged;
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events"), TestCategory("NonPublic")]
		public void ShouldRaiseCSharpEventOnNonmock()
		{
#if COREFX
			if (Mock.IsProfilerEnabled)
#endif
			{
				var hasEvent = new HasEvent();
				bool called = false;
				hasEvent.Event += () => called = true;
				Mock.NonPublic.Raise(hasEvent, "Event");
				Assert.True(called);
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events"), TestCategory("NonPublic")]
		public void ShouldRaiseEventOnMockByName()
		{
#if COREFX
			if (Mock.IsProfilerEnabled)
#endif
			{
				var hasEvent = Mock.Create<HasEvent>();
				bool called = false;
				hasEvent.Event += () => called = true;
				Mock.NonPublic.Raise(hasEvent, "Event");
				Assert.True(called);
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events"), TestCategory("NonPublic")]
		public void ShouldRaiseStaticEventOnNonmockByName()
		{
#if COREFX
			if (Mock.IsProfilerEnabled)
#endif
			{
				bool called = false;
				HasEvent.StaticEvent += () => called = true;
				Mock.NonPublic.Raise(typeof(HasEvent), "StaticEvent");
				Assert.True(called);
			}
		}

		public class HasEvent
		{
			public virtual event Action Event;
			public static event Action StaticEvent;
		}
	}

	[TestClass]
	public class RecordingWorksWhenTestClassHasMockMixin
	{
		private IDocumentView activeView;

#if XUNIT
		public RecordingWorksWhenTestClassHasMockMixin()
		{
			BeforeEach();
		}
#endif

		[TestInitialize]
		public void BeforeEach()
		{
			var activeDocument = Mock.Create<IDocument>();
			this.activeView = Mock.Create<IDocumentView>();
			Mock.Arrange(() => this.activeView.Document).Returns(activeDocument);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Events")]
		public void ActiveDocument_WhenIsDirtyChanged_ShouldRaiseCanExecuteChangedEvent()
		{
			Mock.Raise(() => this.activeView.Document.IsDirtyChanged += null, EventArgs.Empty);
		}

		public interface IDocumentView
		{
			IDocument Document { get; }
		}

		public interface IDocument
		{
			event EventHandler IsDirtyChanged;
		}
	}
}

#if XUNIT2
#pragma warning restore xUnit1013 
#endif
