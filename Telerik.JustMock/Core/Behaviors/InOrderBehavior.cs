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
using System.Linq;
using Telerik.JustMock.Core.Context;

namespace Telerik.JustMock.Core.Behaviors
{
	internal class InOrderBehavior : IAssertableBehavior
	{
		private readonly MocksRepository originalRepository;
		private readonly IMockMixin mock;
		private readonly int arrangementId;
		private readonly string message;
		private bool calledInWrongOrder = false;
		private bool wasCalled = false;

		public string DebugView
		{
			get { return String.Format("{0}: in-order execution expectation. {1}", IsExpectationMet ? "Met" : "Unmet", this.message ?? ""); }
		}

		public InOrderBehavior(MocksRepository originalRepository, IMockMixin mock, string message)
		{
			this.originalRepository = originalRepository;
			this.mock = mock;
			this.arrangementId = InOrderArrangementCount++;
			this.message = message;
		}

		private MocksRepository Repository
		{
			get { return this.mock != null ? this.mock.Repository : this.originalRepository; }
		}

		private int InOrderArrangementCount
		{
			get { return this.Repository.GetValue<int>(typeof(InOrderBehavior), "count", 0); }
			set { this.Repository.StoreValue(typeof(InOrderBehavior), "count", value); }
		}

		private int LastIdInOrder
		{
			get { return this.Repository.GetValue<int>(typeof(InOrderBehavior), "id", -1); }
			set { this.Repository.StoreValue(typeof(InOrderBehavior), "id", value); }
		}

		private string InOrderExecutionLog
		{
			get { return this.Repository.GetValue<string>(typeof(InOrderBehavior), "log", null); }
			set { this.Repository.StoreValue<string>(typeof(InOrderBehavior), "log", value); }
		}

		private string InOrderExecutionMessage
		{
			get
			{
				var log = this.InOrderExecutionLog;
				return !String.IsNullOrEmpty(log) ? log : "--no calls--\n";
			}
		}

		public void Process(Invocation invocation)
		{
			this.wasCalled = true;

			bool processedOnce = this.LastIdInOrder > -1;
			this.calledInWrongOrder =
				processedOnce
				&& (this.LastIdInOrder - this.arrangementId) > 0 || (this.LastIdInOrder - this.arrangementId) < -1;

			this.LastIdInOrder = this.arrangementId;

			this.InOrderExecutionLog += invocation.InputToString() + " called at:\n" + MockingContext.GetStackTrace("    ");

			if (this.calledInWrongOrder)
			{
				MockingContext.Fail("{0}Last call executed out of order. Order of calls so far:\n{1}",
					this.message != null ? this.message + " " : "", InOrderExecutionMessage);
			}
		}

		public void Assert()
		{
			if (!IsExpectationMet)
				MockingContext.Fail("{0}Calls should be executed in the order they are expected. Actual order of calls:\n{1}",
					this.message != null ? this.message + " " : "", InOrderExecutionMessage);
		}

		private bool IsExpectationMet
		{
			get { return wasCalled && !calledInWrongOrder; }
		}
	}
}
