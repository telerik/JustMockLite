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
using Telerik.JustMock.Core.Behaviors;

namespace Telerik.JustMock.Core
{
	internal interface IMethodMock
	{
		MocksRepository Repository { get; set; }
		IMockMixin Mock { get; set; }
		CallPattern CallPattern { get; set; }
		ICollection<IBehavior> Behaviors { get; }
		InvocationOccurrenceBehavior OccurencesBehavior { get; }
		string ArrangementExpression { get; set; }
		bool IsSequential { get; set; }
		bool IsInOrder { get; set; }
		bool IsUsed { get; set; }
		ImplementationOverrideBehavior AcceptCondition { get; set; }
	}
}
