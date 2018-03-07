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

using System;
using System.Collections.Generic;
using Telerik.JustMock.AutoMock;
using Telerik.JustMock.AutoMock.Ninject;
using Telerik.JustMock.AutoMock.Ninject.Parameters;
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
using Telerik.JustMock.XUnit.Test.Assert;
using Telerik.JustMock.XUnit.Test.Attributes;
using TestCategory = Telerik.JustMock.XUnit.Test.Attributes.XUnitCategoryAttribute;
using TestClass = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestClassAttribute;
using TestMethod = Xunit.FactAttribute;
using TestInitialize = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestInitializeAttribute;
using TestCleanup = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestCleanupAttribute;
using AssertionException = Xunit.Sdk.AssertException;
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
	public class NinjectAutoMockFixture
	{
		public interface IFileSystem
		{
			void Refresh();
			bool Exists(string file);
		}

		public interface ICalendar
		{
			DateTime Now { get; }
		}

		public class FileLog
		{
			private readonly IFileSystem fs;
			private readonly ICalendar calendar;

			public FileLog(IFileSystem fs, ICalendar calendar)
			{
				this.fs = fs;
				this.calendar = calendar;
			}

			public bool LogExists()
			{
				fs.Refresh();
				return fs.Exists(calendar.Now.Ticks.ToString());
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Ninject")]
		public void ShouldCreateMocksOfDependencies()
		{
			var container = new MockingContainer<FileLog>();

			container.Arrange<IFileSystem>(x => x.Exists("123")).Returns(true);
			container.Arrange<ICalendar>(x => x.Now).Returns(new DateTime(123));

			Assert.True(container.Instance.LogExists());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Ninject")]
		public void ShouldAssertArrangedExpectations()
		{
			var container = new MockingContainer<FileLog>();

			container.Arrange<IFileSystem>(x => x.Refresh()).MustBeCalled();
			container.Arrange<IFileSystem>(x => x.Exists("123")).Returns(true).MustBeCalled();
			container.Arrange<ICalendar>(x => x.Now).Returns(new DateTime(123)).MustBeCalled();

			Assert.Throws<AssertFailedException>(() => container.Assert());

			container.Instance.LogExists();

			container.Assert();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Ninject")]
		public void ShouldAssertDependenciesDirectly()
		{
			var container = new MockingContainer<FileLog>();

			container.Arrange<ICalendar>(x => x.Now).Returns(new DateTime(123));

			Assert.Throws<AssertFailedException>(() => container.Assert<IFileSystem>(x => x.Refresh(), Occurs.Once()));
			Assert.Throws<AssertFailedException>(() => container.Assert<IFileSystem>(x => x.Exists("123"), Occurs.Once()));
			Assert.Throws<AssertFailedException>(() => container.Assert<ICalendar>(x => x.Now, Occurs.Once()));

			container.Instance.LogExists();

			container.Assert<IFileSystem>(x => x.Refresh(), Occurs.Once());
			container.Assert<IFileSystem>(x => x.Exists("123"), Occurs.Once());
			container.Assert<ICalendar>(x => x.Now, Occurs.Once());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Ninject")]
		public void ShouldSpecifyDependencyBehavior()
		{
			var container = new MockingContainer<FileLog>(new AutoMockSettings { MockBehavior = Behavior.Strict });

			Assert.Throws<StrictMockException>(() => container.Instance.LogExists());
		}

		public interface IAccount
		{
			int Id { get; }
			void Withdraw(decimal amount);
			void Deposit(decimal amount);
		}

		public class TransactionService
		{
			public readonly IAccount From;
			public readonly IAccount To;

			public TransactionService(IAccount fromAccount, IAccount toAccount)
			{
				this.From = fromAccount;
				this.To = toAccount;
			}

			[Inject]
			public IAccount BillingAccount { get; set; }

			public void TransferFunds(decimal amount)
			{
				const decimal Fee = 1;
				From.Withdraw(amount + Fee);
				To.Deposit(amount);
				BillingAccount.Deposit(Fee);
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Ninject")]
		public void ShouldArrangeSingletonInstances()
		{
			var container = new MockingContainer<TransactionService>();
			container.Arrange<IAccount>(x => x.Id).Returns(10);

			var inst = container.Instance;
			Assert.NotNull(inst.From);
			Assert.Same(inst.From, inst.To);
			Assert.Same(inst.From, inst.BillingAccount);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Ninject")]
		public void ShouldArrangeMultipleInstancesSeparatelyByParameterName()
		{
			var container = new MockingContainer<TransactionService>();
			container.Bind<IAccount>().ToMock().InjectedIntoParameter("fromAccount").AndArrange(x => Mock.Arrange(() => x.Id).Returns(10));
			container.Bind<IAccount>().ToMock().InjectedIntoParameter("toAccount").AndArrange(x => Mock.Arrange(() => x.Id).Returns(20));
			container.Bind<IAccount>().ToMock().InjectedIntoProperty((TransactionService s) => s.BillingAccount).AndArrange(x => Mock.Arrange(() => x.Id).Returns(30));

			var inst = container.Instance;
			Assert.Equal(10, inst.From.Id);
			Assert.Equal(20, inst.To.Id);
			Assert.Equal(30, inst.BillingAccount.Id);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Ninject")]
		public void ShouldArrangeMultipleInstancesSeparatelyByPropertyName()
		{
			var container = new MockingContainer<TransactionService>();
			container.Bind<IAccount>().ToMock().InjectedIntoProperty("BillingAccount").AndArrange(x => Mock.Arrange(() => x.Id).Returns(30));

			var inst = container.Instance;
			Assert.Equal(30, inst.BillingAccount.Id);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Ninject")]
		public void ShouldAssertMultipleInstancesByName()
		{
			var container = new MockingContainer<TransactionService>();
			container.Bind<IAccount>().ToMock().InjectedIntoParameter("fromAccount").Named("from")
				.AndArrange(x => Mock.Arrange(() => x.Id).Returns(10));
			container.Bind<IAccount>().ToMock().InjectedIntoParameter("toAccount").Named("to")
				.AndArrange(x => Mock.Arrange(() => x.Id).Returns(20));
			container.Bind<IAccount>().ToMock().InjectedIntoProperty((TransactionService s) => s.BillingAccount).Named("bill")
				.AndArrange(x => Mock.Arrange(() => x.Id).Returns(30));

			var inst = container.Instance;

			inst.TransferFunds(10);

			container.Assert<IAccount>("from", x => x.Withdraw(11), Occurs.Once());
			container.Assert<IAccount>("to", x => x.Deposit(10), Occurs.Once());
			container.Assert<IAccount>("bill", x => x.Deposit(1), Occurs.Once());
		}

		public class VariousCtors
		{
			public VariousCtors(IFileSystem fs)
			{
			}

			public VariousCtors(IFileSystem fs, ICalendar calendar)
			{
				throw new InvalidOperationException();
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Ninject")]
		public void ShouldSelectConstructorBasedOnSettings()
		{
			// assert the default NInject behavior that injects into the constructor with most parameters
			var container = new MockingContainer<VariousCtors>();
			Assert.Throws<InvalidOperationException>(() => { var inst = container.Instance; });

			// assert the overriden constructor lookup behavior
			var container2 = new MockingContainer<VariousCtors>(new AutoMockSettings { ConstructorArgTypes = new[] { typeof(IFileSystem) } });
			Assert.NotNull(container2.Instance);

			// assert that specifying an invalid constructor throws
			Assert.Throws<MockException>(() => new MockingContainer<VariousCtors>(new AutoMockSettings { ConstructorArgTypes = new[] { typeof(ICalendar) } }));
		}

		public interface IService { }

		public class Module
		{
			public IService service;

			public Module(IService service)
			{
				this.service = service;
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("Ninject")]
		public void ShouldMakeSingletonExplicitlyRequestedServices()
		{
			var container = new MockingContainer<Module>();
			var s1 = container.Get<IService>();
			var s2 = container.Instance.service;
			Assert.Same(s1, s2);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldAssertRaisesAgainstMethod()
		{
			var container = new MockingContainer<Executor>();

			bool raised = false;

			container.Arrange<IExecutor>(x => x.Submit()).Raises(() => container.Get<IExecutor>().Done += null, EventArgs.Empty);

			container.Get<IExecutor>().Done += delegate { raised = true; };

			container.Instance.Submit();

			Assert.True(raised);
		}

		public class Executor
		{
			public Executor(IExecutor executor)
			{
				this.executor = executor;
			}

			public void Submit()
			{
				this.executor.Submit();
			}

			private IExecutor executor;
		}

		public interface IExecutor
		{
			event EventHandler<EventArgs> Done;
			event EventHandler Executed;
			void Submit();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldAssertMockingNestedDependency()
		{
			var container = new MockingContainer<Foo>();
			container.Bind<Bar>().ToSelf();

			container.Arrange<IUnitOfWork>(uow => uow.DoWork()).MustBeCalled();

			Assert.Throws<AssertFailedException>(() => container.Assert());

			container.Instance.DoWork();

			container.Assert();
		}

		public class Foo
		{
			public Foo(Bar bar)
			{
				this.bar = bar;
			}

			public void DoWork()
			{
				this.bar.DoWork();
			}

			private readonly Bar bar;
		}

		public class Bar
		{
			public Bar(IUnitOfWork unitOfWork)
			{
				this.unitOfWork = unitOfWork;
			}

			public void DoWork()
			{
				this.unitOfWork.DoWork();
			}

			private readonly IUnitOfWork unitOfWork;
		}

		public interface IUnitOfWork
		{
			void DoWork();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldResolveTargetTypeWithInterfaceAndConcreteDependencies()
		{
			var container = new MockingContainer<Unit>();

			container.Arrange<IUnitOfWork>(uow => uow.DoWork()).MustBeCalled();

			// this is where it resolves.
			container.Instance.DoWork();

			container.Assert();
		}

		public class Unit
		{
			public Unit(IUnitOfWork unitOfWork, WorkItem workItem)
			{
				this.unitOfWork = unitOfWork;
				this.workItem = workItem;
			}

			public void DoWork()
			{
				workItem.DoWork();
				unitOfWork.DoWork();
			}

			private readonly IUnitOfWork unitOfWork;
			private readonly WorkItem workItem;
		}

		public class WorkItem
		{
			public void DoWork()
			{

			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldAssertOccurrenceFromContainerWithoutPriorArrangement()
		{
			var c = new MockingContainer<Unit>();
			c.Instance.DoWork();
			c.Assert<IUnitOfWork>(x => x.DoWork());
		}

		public class DisposableContainer : IDisposable
		{
			public IList<IDisposable> Disposables;

			public DisposableContainer(IList<IDisposable> disposables)
			{
				this.Disposables = disposables;
			}

			public void Dispose()
			{
				this.Disposables.Clear();
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldInjectContainers()
		{
			var c = new MockingContainer<DisposableContainer>();
			var disposables = new List<IDisposable> { Mock.Create<IDisposable>(), Mock.Create<IDisposable>() };
			var i = c.Get<DisposableContainer>(new ConstructorArgument("disposables", disposables));
			i.Dispose();

			Assert.Equal(0, disposables.Count);
		}

		public abstract class DependencyBase
		{
			public IDisposable Dep { get; set; }

			protected DependencyBase(IDisposable dep)
			{
				this.Dep = dep;
			}

			public abstract int Value { get; }
			public abstract string Name { get; set; }

			public int baseValue;
			public virtual int BaseValue
			{
				get { return baseValue; }
				set { baseValue = value; }
			}
		}

		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldInjectAbstractType()
		{
			var c = new MockingContainer<DependencyBase>();
			var obj = c.Instance;
			Assert.NotNull(obj.Dep);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldArrangeMethodsOnInjectedAbstractType()
		{
			var c = new MockingContainer<DependencyBase>();
			var obj = c.Instance;
			Mock.Arrange(() => obj.Value).Returns(5);
			Assert.Equal(5, obj.Value);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldCheckPropertyMixinOnNonabstractPropertyOnInjectedAbstractType()
		{
			var c = new MockingContainer<DependencyBase>();
			var obj = c.Instance;
			obj.BaseValue = 10;
			Assert.Equal(10, obj.baseValue);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldInjectAbstractTypeWithSpecifiedCtor()
		{
			var c = new MockingContainer<DependencyBase>(
				new AutoMockSettings { ConstructorArgTypes = new[] { typeof(IDisposable) } });
			var obj = c.Instance;
			Assert.NotNull(obj.Dep);
		}

		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldIncludeAssertionMessageWhenAssertingContainer()
		{
			var c = new MockingContainer<FileLog>();
			c.Arrange<ICalendar>(x => x.Now).MustBeCalled("Calendar must be used!");
			c.Arrange<IFileSystem>(x => x.Refresh()).MustBeCalled("Should use latest data!");

			var ex = Assert.Throws<AssertFailedException>(() => c.Assert("Container must be alright!"));

			Assert.True(ex.Message.Contains("Calendar must be used!"));
			Assert.True(ex.Message.Contains("Should use latest data!"));
			Assert.True(ex.Message.Contains("Container must be alright!"));
		}
	}
}
