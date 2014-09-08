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
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Behaviors;

namespace Telerik.JustMock.Setup
{
	internal class MockingBehaviorConfiguration : IMockReplicator
	{
		public Behavior Behavior { get; set; }
		
		public object CreateSimilarMock(MocksRepository repository, Type mockType, object[] constructorArgs, bool mockConstructorCall, Type[] additionalMockedInterfaces)
		{
			return repository.Create(mockType, constructorArgs, this.Behavior, additionalMockedInterfaces, mockConstructorCall);
		}
	}
}
