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
using System.Reflection;

namespace Telerik.JustMock.Core.Behaviors
{
	internal class RaiseEventBehavior : IBehavior
	{
		private readonly object instance;
		private readonly EventInfo evt;
		private readonly Delegate eventDelegateParametersFactory;

		public RaiseEventBehavior(object instance, EventInfo evt, Delegate eventDelegateParametersFactory)
		{
			this.instance = instance;
			this.evt = evt;
			this.eventDelegateParametersFactory = eventDelegateParametersFactory;
		}

		public void Process(Invocation invocation)
		{
			invocation.UserProvidedImplementation = true;
			Process(invocation.Args, invocation.Method.DeclaringType);
		}

		public void Process(object[] invocationArgs, Type declaringType)
		{
			object[] args = null;

			var func = this.eventDelegateParametersFactory as Func<object[]>;
			if (func != null)
			{
				args = ProfilerInterceptor.GuardExternal(func);
			}
			else
			{
				var invoker = MockingUtil.MakeFuncCaller(this.eventDelegateParametersFactory);
				args = (object[])ProfilerInterceptor.GuardExternal(() => invoker(invocationArgs, this.eventDelegateParametersFactory));
			}

			RaiseEventImpl(this.instance, this.evt, args);
		}

		public static void RaiseEventImpl(object instance, EventInfo evt, object[] args)
		{
			if (evt == null)
				throw new MockException("Unable to deduce which event was specified in the parameter.");

			if (args == null)
			{
				args = new object[] { null };
			}

			if (args.Length == 1
				&& (evt.EventHandlerType.IsGenericType && evt.EventHandlerType.GetGenericTypeDefinition() == typeof(EventHandler<>)
					|| evt.EventHandlerType == typeof(EventHandler)
					|| args[0] is EventArgs)
				)
			{
				args = new[] { instance, args[0] };
			}

			if (!(instance is IMockMixin))
			{
				var mockMixin = MocksRepository.GetMockMixin(instance, evt.DeclaringType);
				if (mockMixin != null)
					instance = mockMixin;
			}

			var mixin = instance as IEventsMixin;
			if (mixin != null)
			{
				mixin.RaiseEvent(evt, args);
			}
			else
			{
				MockingUtil.RaiseEventThruReflection(instance, evt, args);
			}
		}
	}
}
