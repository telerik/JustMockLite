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

using System.Collections.Generic;
using Telerik.JustMock.Tests.EventFixureDependencies;
using NUnit.Framework;


namespace Telerik.JustMock.Tests
{
	[TestFixture]
	public class EventsFixtureNUnit
	{
		private ProjectNavigatorViewModel viewModel;
		private ISolutionService solutionService;

		[SetUp]
		public void Initialize()
		{
			this.solutionService = Mock.Create<ISolutionService>();
			this.viewModel = new ProjectNavigatorViewModel(this.solutionService);
		}

		[Test, Category("Lite"), Category("Events")]
		[TestCaseSource("DummyTestCaseSource")]
		public void ShouldRaiseEventsOnDataDrivenTests(object _)
		{
			Mock.Raise(() => this.solutionService.ProjectAdded += null, new ProjectEventArgs(null));
		}

		private static IEnumerable<TestCaseData> DummyTestCaseSource = new[] { new TestCaseData(null), new TestCaseData(null) };
	}
}
