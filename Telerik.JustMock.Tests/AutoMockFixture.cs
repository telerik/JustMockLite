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

#if !SILVERLIGHT // auto-mocking is not supported in Silverlight, but is supported in Lite
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustMock.Container;
using Telerik.JustMock.AutoMock.Ninject;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else   
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif


namespace Telerik.JustMock.Tests
{
	[TestClass]
	public class AutoMockFixture
	{
		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldAssertThatReportIsSent()
		{
			var container = new MockingContainer<Reporter>();

			container.Arrange<IReportBuilder>(rb => rb.GetReports()).Returns(new[] { new Report(), new Report() });
			container.Arrange<IReportSender>(rs => rs.SendReport(Arg.IsAny<Report>())).Occurs(2);

			container.Instance.SendReports();

			container.AssertAll();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldAssertSpecificallyThatSendReportMethodIsCalledTwice()
		{
			var container = new MockingContainer<Reporter>();

			container.Arrange<IReportBuilder>(rb => rb.GetReports()).Returns(new[] { new Report(), new Report() });
			container.Arrange<IReportSender>(rs => rs.SendReport(Arg.IsAny<Report>())).Occurs(2);

			container.Instance.SendReports();

			container.Assert<IReportSender>(rs => rs.SendReport(Arg.IsAny<Report>()));
		}

		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldAssertThatGetReportsIsCalled()
		{
			var container = new MockingContainer<Reporter>();

			container.Arrange<IReportBuilder>(rb => rb.GetReports()).Returns(new[] { new Report(), new Report() }).MustBeCalled();
			container.Arrange<IReportSender>(rs => rs.SendReport(Arg.IsAny<Report>())).Occurs(2);

			container.Instance.SendReports();

			container.Assert<IReportBuilder>(rb => rb.GetReports());
		}

		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldAssertThatReportsAreReceived()
		{
			var container = new MockingContainer<Reporter>();

			container.Arrange<IReportBuilder>(rb => rb.GetReports()).Returns(new[] { new Report(), new Report() }).MustBeCalled();
			container.Arrange<IReportSender>(rs => rs.SendReport(Arg.IsAny<Report>()));

			container.Instance.SendReports();

			container.Assert<IReportBuilder>();
		}

		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldAssertFundTransfersBetweenTwoAccounts()
		{
			var container = new MockingContainer<AccountService>();

			decimal expectedBalance = 100;

			container.Arrange<IAcccount>(x => x.Balance, Take.First()).Returns(expectedBalance);
			container.Arrange<IAcccount>(x => x.Withdraw(expectedBalance), Take.First()).MustBeCalled();
			container.Arrange<IAcccount>(x => x.Deposit(expectedBalance), Take.Last()).MustBeCalled();

			container.Instance.TransferFunds(expectedBalance);

			container.Assert();
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

		public class AccountService
		{
			public AccountService (IAcccount fromAccount, IAcccount toAccount)
			{
				this.fromAccount = fromAccount;
				this.toAccount = toAccount;
			}

			public void TransferFunds(decimal amount)
			{
				if (fromAccount.Balance <= amount)
				{
					fromAccount.Withdraw(amount);
					toAccount.Deposit(amount);
				}
			}

			private IAcccount fromAccount;
			private IAcccount toAccount;
		}

		public interface IAcccount
		{
			decimal Balance { get; }
			void Withdraw(decimal amount);
			void Deposit(decimal amount);
		}
		
	
		public class Reporter
		{
			private readonly IReportBuilder reportBuilder;
			private readonly IReportSender reportSender;

			public Reporter(IReportSender reportSender, IReportBuilder reportBuilder)
			{
				this.reportSender = reportSender;
				this.reportBuilder = reportBuilder;
			}

			public void SendReports()
			{
				var reports = reportBuilder.GetReports();

				foreach (var report in reports)
				{
					reportSender.SendReport(report);
				}
			}
		}

		public interface IReportBuilder
		{
			IList<Report> GetReports();
		}

		public interface IReportSender
		{
			void SendReport(Report report);
		}

		public class Report
		{
		}

		[TestMethod, TestCategory("Lite"), TestCategory("AutoMock")]
		public void ShouldAssertMockingNestedDependency()
		{
			var container = new MockingContainer<Foo>();

			//container.Arrange<Bar>(uow => uow.DoWork()).CallOriginal().MustBeCalled();
			container.Arrange<IUnitOfWork>(uow => uow.DoWork()).MustBeCalled();

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

		public interface IAccount
		{
			bool Deposit(decimal amt);
			bool Withdraw(decimal amt);

			event Action<decimal> DepositMade;
		}

		public class BankTransation
		{
			readonly IAccount _fromAccount;
			readonly IAccount _toAccount;

			public BankTransation(IAccount fromAccount, IAccount toAccount)
			{
				this._fromAccount = fromAccount;
				this._toAccount = toAccount;
			}

			public void TransferFunds(decimal amount)
			{
				_fromAccount.Withdraw(amount);
				_toAccount.Deposit(amount);
			}
		}

		[TestMethod]
		public void ShouldArrangeRaisesBehaviorOnSecondInstance()
		{
			var amount = 100M;
			var container = new MockingContainer<BankTransation>();

			container.Arrange<IAccount>(x => x.Withdraw(amount),
				Take.First())
					 .Returns(true)
					 .Occurs(1);

			container.Arrange<IAccount>(x => x.Deposit(amount),
				Take.At(1))
					 .Raises(() => container.Get<IAccount>(Take.At(1)).DepositMade += null, amount)
					 .Returns(true)
					 .Occurs(1);

			bool eventRaised = false;
			container.Get<IAccount>(Take.At(1)).DepositMade += d => eventRaised = true;

			container.Instance.TransferFunds(amount);

			Assert.True(eventRaised);
		}

	}
}
#endif
