/*
 JustMock Lite
 Copyright © 2010-2015 Telerik EAD

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
using Telerik.JustMock.AutoMock.Ninject;
using Telerik.JustMock.AutoMock.Ninject.Activation;
using Telerik.JustMock.AutoMock.Ninject.Components;
using Telerik.JustMock.AutoMock.Ninject.Infrastructure;
using Telerik.JustMock.AutoMock.Ninject.Planning.Bindings;
using Telerik.JustMock.AutoMock.Ninject.Planning.Bindings.Resolvers;
using Telerik.JustMock.Core.Context;

namespace Telerik.JustMock.AutoMock
{
	internal interface IMockResolver : INinjectComponent
	{
		void ForEachMock(Action<object> action);
		void AttachToBinding(IBindingConfiguration binding, Type type);
	}

	internal sealed class MockResolver : IMissingBindingResolver, IMockResolver
	{
		private sealed class MockProvider : IProvider
		{
			private readonly Behavior behavior;
			public Type Type { get; private set; }

			public MockProvider(Type type, Behavior behavior)
			{
				this.Type = type;
				this.behavior = behavior;
			}

			public object Create(IContext context)
			{
				return Mock.Create(Type, behavior);
			}
		}

		private readonly List<object> mocks = new List<object>();

		public void Dispose()
		{
		}

		public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, IRequest request)
		{
			var binding = new Binding(request.Service);
			AttachToBinding(binding, request.Service);
			binding.ScopeCallback = StandardScopeCallbacks.Singleton;
			binding.IsImplicit = true;
			return new[] { binding };
		}

		public void AttachToBinding(IBindingConfiguration binding, Type type)
		{
			binding.ProviderCallback = ctx => new MockProvider(type, MockBehavior);
			binding.ActivationActions.Add((context, obj) => mocks.Add(obj));
			binding.DeactivationActions.Add((context, obj) => mocks.Remove(obj));
		}

		public void ForEachMock(Action<object> action)
		{
			using (MockingContext.BeginFailureAggregation(null))
			{
				foreach (var mock in this.mocks)
					action(mock);
			}
		}

		public Behavior MockBehavior
		{
			get { return ((AutoMockSettings)Settings).MockBehavior; }
		}

		public INinjectSettings Settings { get; set; }
	}
}
