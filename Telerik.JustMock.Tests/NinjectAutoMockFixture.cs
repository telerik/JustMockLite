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
using System.Linq;
using Telerik.JustMock.AutoMock;
using Telerik.JustMock.AutoMock.Ninject;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;

#else   
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using AssertFailedException = NUnit.Framework.AssertionException;
#endif

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
		public void ShouldImplicitlyArrangeSeparateInstances()
		{
			var container = new MockingContainer<TransactionService>();

			var inst = container.Instance;
			Assert.NotNull(inst.From);
			Assert.NotNull(inst.To);
			Assert.NotNull(inst.BillingAccount);

			Assert.NotSame(inst.From, inst.To);
			Assert.NotSame(inst.From, inst.BillingAccount);
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
	}
}
