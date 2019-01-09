/*
 JustMock Lite
 Copyright © 2010-2015,2018 Progress Software Corporation

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
using System.Threading;
using Telerik.JustMock.Core.Recording;
#if NETCORE
using Debug = Telerik.JustMock.Diagnostics.JMDebug;
#else
using Debug = System.Diagnostics.Debug;
#endif

namespace Telerik.JustMock.Core
{
	internal class RepositorySharedContext
	{
		private volatile int nextArrangeId = 0;

		private readonly ThreadLocalProperty<IRecorder> recorder = new ThreadLocalProperty<IRecorder>();
		private readonly ThreadLocalProperty<object> inArrange = new ThreadLocalProperty<object>();
		private readonly ThreadLocalProperty<object> dispatchToMethodMocks = new ThreadLocalProperty<object>();
		private readonly ThreadLocalProperty<object> inAssertSet = new ThreadLocalProperty<object>();

		public IRecorder Recorder
		{
			get { return this.recorder.Get(); }
			private set { this.recorder.Set(value); }
		}

		public bool InArrange
		{
			get { return this.inArrange.Get() != null; }
			private set { this.inArrange.Set(value ? (object)value : null); }
		}

		public bool InAssertSet
		{
			get { return this.inAssertSet.Get() != null; }
			private set { this.inAssertSet.Set(value ? (object)value : null); }
		}

		public bool DispatchToMethodMocks
		{
			get { return this.dispatchToMethodMocks.Get() != null; }
			private set { this.dispatchToMethodMocks.Set(value ? (object)value : null); }
		}

		public IDisposable StartRecording(IRecorder recorder, bool dispatchToMethodMocks)
		{
			Monitor.Enter(this);
			this.Recorder = recorder;
			this.DispatchToMethodMocks = dispatchToMethodMocks;
			return new InRecordingContext(this);
		}

		public IDisposable StartArrange()
		{
			Monitor.Enter(this);
			return new InArrangeContext(this);
		}

		public IDisposable StartAssertSet()
		{
			Monitor.Enter(this);
			return new InAssertSetContext(this);
		}

		public int GetNextArrangeId()
		{
			lock (this)
				return nextArrangeId++;
		}

		abstract private class ContextSession : IDisposable
		{
			private readonly RepositorySharedContext context;

			public RepositorySharedContext Context { get { return context; } }

			public ContextSession(RepositorySharedContext context)
			{
				this.context = context;
			}

			public abstract void Dispose();
		}

		private class InRecordingContext : ContextSession
		{
			private readonly int oldCounter;

			public InRecordingContext(RepositorySharedContext context)
				: base(context)
			{
				this.oldCounter = ProfilerInterceptor.ReentrancyCounter;

				ProfilerInterceptor.ReentrancyCounter = 0;
			}

			public override void Dispose()
			{
				ProfilerInterceptor.ReentrancyCounter = this.oldCounter;

				this.Context.Recorder = null;
				Monitor.Exit(this.Context);
			}
		}

		private class InArrangeContext : ContextSession
		{
			public InArrangeContext(RepositorySharedContext context)
				: base(context)
			{
                Debug.Assert(!this.Context.InArrange);
				context.InArrange = true;
			}

			public override void Dispose()
			{
				this.Context.InArrange = false;
				Monitor.Exit(this.Context);
			}
		}

		private class InAssertSetContext : ContextSession
		{
			public InAssertSetContext(RepositorySharedContext context)
				: base(context)
			{
                Debug.Assert(!this.Context.InAssertSet);
				context.InAssertSet = true;
			}

			public override void Dispose()
			{
				this.Context.InAssertSet = false;
				Monitor.Exit(this.Context);
			}
		}
	}
}
