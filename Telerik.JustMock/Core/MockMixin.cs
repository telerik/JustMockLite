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
using System.Collections.Generic;
using System.Linq;
using Telerik.JustMock.Core.Behaviors;

namespace Telerik.JustMock.Core
{
	[Mixin]
	public interface IMockMixin
	{
		/// <summary>
		/// The repository that created this mock, or the repository that
		/// made the last arrangement for this mock, if this mock has been used
		/// in different contexts.
		/// </summary>
		MocksRepository Repository { get; set; }

		/// <summary>
		/// The repository that first created this mock mixin.
		/// </summary>
		MocksRepository Originator { get; }

		/// <summary>
		/// A collection of mocks that are dependent on this one. Dependent mocks
		/// are recursively asserted. Dependent mocks are added for example by arranging a call
		/// on this mock to return another mock.
		/// </summary>
		IList<object> DependentMocks { get; }

		/// <summary>
		/// Behaviors that are processed after the behaviors for any method mock are processed.
		/// </summary>
		IList<IBehavior> SupplementaryBehaviors { get; }

		/// <summary>
		/// Behaviors to process when there was no method mock for a dispatched invocation.
		/// </summary>
		IList<IBehavior> FallbackBehaviors { get; }

		Type DeclaringType { get; }

		bool IsStaticConstructorMocked { get; set; }
		bool IsInstanceConstructorMocked { get; set; }

		/// <summary>
		/// Set to the object for which this instance is an external mock mixin
		/// </summary>
		object ExternalizedMock { get; set; }
	}

	internal class MockMixin : IMockMixin
	{
		private List<object> dependentMocks;
		private readonly List<IBehavior> supplementaryBehaviors = new List<IBehavior>();
		private readonly List<IBehavior> fallbackBehaviors = new List<IBehavior>();
		private MocksRepository repository;
		private MocksRepository originator;

		public object ExternalizedMock { get; set; }

		public MocksRepository Repository
		{
			get { return this.repository; }
			set
			{
				this.repository = value;
				if (this.originator == null)
					this.originator = value;
			}
		}

		public MocksRepository Originator
		{
			get { return this.originator; }
		}

		public Type DeclaringType { get; set; }

		public bool IsStaticConstructorMocked { get; set; }
		public bool IsInstanceConstructorMocked { get; set; }

		public IList<object> DependentMocks
		{
			get
			{
				if (this.dependentMocks == null)
					this.dependentMocks = new List<object>();
				return this.dependentMocks;
			}
		}

		public IList<IBehavior> SupplementaryBehaviors
		{
			get { return this.supplementaryBehaviors; }
		}

		public IList<IBehavior> FallbackBehaviors
		{
			get { return this.fallbackBehaviors; }
		}
	}
}
