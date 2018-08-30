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
using System.Reflection;

namespace Telerik.JustMock.Core.Behaviors
{
	/// <summary>
	/// An implementation detail interface. Not intended for external usage.
	/// </summary>
	[Mixin]
	public interface IEventsMixin
	{
		/// <summary>
		/// An implementation detail. Not intended for external usage.
		/// </summary>
		void RaiseEvent(EventInfo evt, object[] delegateArguments);
	}

	internal class EventStubsBehavior : IBehavior
	{
		private readonly Dictionary<EventInfo, Delegate> eventHandlers = new Dictionary<EventInfo, Delegate>();

		public object CreateMixin()
		{
			return new EventsMixin(this);
		}

		public void Process(Invocation invocation)
		{
			var method = invocation.Method;

			var candidateEvent = method.GetEventFromAddOrRemove();
			if (candidateEvent == null)
				return;

			invocation.UserProvidedImplementation = true;

			var delg = (Delegate)invocation.Args[0];
			if (candidateEvent.GetAddMethod() == invocation.Method)
				this.AddEventHandler(candidateEvent, delg);
			else
				this.RemoveEventHandler(candidateEvent, delg);
		}

		private void AddEventHandler(EventInfo evt, Delegate handler)
		{
			Delegate existing;
			eventHandlers.TryGetValue(evt, out existing);
			eventHandlers[evt] = Delegate.Combine(existing, handler);
		}

		private void RemoveEventHandler(EventInfo evt, Delegate handler)
		{
			Delegate existing;
			eventHandlers.TryGetValue(evt, out existing);
			eventHandlers[evt] = Delegate.Remove(existing, handler);
		}

		public void RaiseEvent(EventInfo evt, object[] delegateArguments)
		{
			Delegate existing;
			eventHandlers.TryGetValue(evt, out existing);

			if (existing != null)
			{
				try
				{
					object state;
					MockingUtil.BindToMethod(MockingUtil.Default, new[] { existing.Method }, ref delegateArguments, null, null, null, out state);
				}
				catch (MissingMethodException ex)
				{
					throw new MockException(String.Format("Event signature {0} is incompatible with argument types ({1})",
						existing.Method, String.Join(", ", delegateArguments.Select(x => x != null ? x.GetType().ToString() : "null").ToArray())
						), ex);
				}

				var invoker = MockingUtil.MakeFuncCaller(existing);
				ProfilerInterceptor.GuardExternal(() => invoker(delegateArguments, existing));
			}
		}

		private class EventsMixin : IEventsMixin
		{
			private readonly EventStubsBehavior events;

			public EventsMixin(EventStubsBehavior events)
			{
				this.events = events;
			}

			public void RaiseEvent(EventInfo evt, object[] delegateArguments)
			{
				events.RaiseEvent(evt, delegateArguments);
			}
		}
	}
}
