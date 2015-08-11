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
using System.Diagnostics;
using System.Linq;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Diagnostics;

namespace Telerik.JustMock.Core.Behaviors
{
	internal class InvocationOccurrenceBehavior : IAssertableBehavior
	{
		private readonly IMethodMock methodMock;

		public int? LowerBound { get; set; }
		public int? UpperBound { get; set; }
		private string message;
		private int calls;

		public string DebugView
		{
			get
			{
				if ((LowerBound == null || LowerBound <= 0) && (UpperBound == null))
					return null;

				return String.Format("{3}: Occurences must be in [{0}, {1}]; calls so far: {2}. {4}",
					LowerBound.HasValue ? (object)LowerBound.Value : "any",
					UpperBound.HasValue ? (object)UpperBound.Value : "any",
					calls,
					IsInRange(LowerBound, UpperBound, calls) ? "Met" : "Unmet",
					this.message ?? "");
			}
		}

		public InvocationOccurrenceBehavior(IMethodMock methodMock)
		{
			this.methodMock = methodMock;
		}

		public void SetBounds(int? lowerBound, int? upperBound, string message)
		{
			this.LowerBound = lowerBound;
			this.UpperBound = upperBound;
			this.message = message;
		}

		public void Process(Invocation invocation)
		{
			++calls;

			Telerik.JustMock.DebugView.TraceEvent(IndentLevel.DispatchResult, () => String.Format("Calls so far: {0}", calls));
			Assert(null, this.UpperBound, calls, this.message, null);
		}

		public void Assert()
		{
			Assert(this.LowerBound, this.UpperBound);
		}

		public void Assert(int? lowerBound, int? upperBound)
		{
			var expr = this.methodMock.ArrangementExpression;
			Assert(lowerBound, upperBound, this.calls, this.message, expr);
		}

		public static void Assert(int? lowerBound, int? upperBound, int calls, string userMessage, object expression)
		{
			if (IsInRange(lowerBound, upperBound, calls))
				return;

			var message = String.Format("{2}Occurrence expectation failed. {0}. Calls so far: {1}",
				MakeRangeString(lowerBound, upperBound),
				calls,
				userMessage != null ? userMessage + " " : string.Empty);

			if (expression != null)
				message += String.Format("\nArrange expression: {0}", expression).EscapeFormatString();

			MockingContext.Fail(message);
		}

		private static bool IsInRange(int? lowerBound, int? upperBound, int calls)
		{
			bool withinLowerBound = !lowerBound.HasValue || calls >= lowerBound.Value;
			bool withinUpperBound = !upperBound.HasValue || calls <= upperBound.Value;
			return withinLowerBound && withinUpperBound;
		}

		private static string MakeRangeString(int? lowerBound, int? upperBound)
		{
			Debug.Assert(lowerBound != null || upperBound != null);
			return upperBound == 0 ? "Expected no calls"
				: lowerBound != null && lowerBound == upperBound ? String.Format("Expected exactly {0} call{1}", lowerBound, lowerBound != 1 ? "s" : "")
				: lowerBound != null && upperBound != null ? String.Format("Expected between {0} and {1} calls", lowerBound, upperBound)
				: lowerBound != null ? String.Format("Expected at least {0} call{1}", lowerBound, lowerBound != 1 ? "s" : "")
				: upperBound != null ? String.Format("Expected at most {0} call{1}", upperBound, upperBound != 1 ? "s" : "")
				: null;
		}
	}
}
