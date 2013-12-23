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
using Telerik.JustMock.Core.Context;

namespace Telerik.JustMock.Core.Behaviors
{
	internal class InOrderBehavior : IAssertableBehavior
	{
		private readonly IMockMixin mock;
		private readonly int arrangementId;
		private bool calledInWrongOrder = false;
		private bool wasCalled = false;

		public string DebugView
		{
			get { return String.Format("{0}: in-order execution expectation.", IsExpectationMet ? "Met" : "Unmet"); }
		}

		public InOrderBehavior(IMockMixin mock)
		{
			this.mock = mock;
			this.arrangementId = InOrderArrangementCount++;
		}

		private int InOrderArrangementCount
		{
			get { return this.mock.Repository.GetValue<int>(typeof(InOrderBehavior), "count", 0); }
			set { this.mock.Repository.StoreValue(typeof(InOrderBehavior), "count", value); }
		}

		private int LastIdInOrder
		{
			get { return this.mock.Repository.GetValue<int>(typeof(InOrderBehavior), "id", -1); }
			set { this.mock.Repository.StoreValue(typeof(InOrderBehavior), "id", value); }
		}

		private string InOrderExecutionLog
		{
			get { return this.mock.Repository.GetValue<string>(typeof(InOrderBehavior), "log", null); }
			set { this.mock.Repository.StoreValue<string>(typeof(InOrderBehavior), "log", value); }
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
			this.calledInWrongOrder = (this.LastIdInOrder != this.arrangementId - 1);
			this.LastIdInOrder = this.arrangementId;

			this.InOrderExecutionLog += invocation.InputToString() + " called at:\n" + MockingContext.GetStackTrace("    ");

			if (this.calledInWrongOrder)
			{
				MockingContext.Fail("Last call executed out of order. Order of calls so far:\n{1}", invocation.InputToString(), InOrderExecutionMessage);
			}
		}

		public void Assert()
		{
			if (!IsExpectationMet)
				MockingContext.Fail("Calls should be executed in the order they are expected. Actual order of calls:\n{0}", InOrderExecutionMessage);
		}
  
		private bool IsExpectationMet
		{
			get { return wasCalled && !calledInWrongOrder; }
		}
	}
}
