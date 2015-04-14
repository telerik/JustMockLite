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
using System.Threading;
using Telerik.JustMock.Core.Recording;

namespace Telerik.JustMock.Core
{
	internal class RepositorySharedContext
	{
		private volatile int nextArrangeId = 0;

		private readonly ThreadLocalProperty<IRecorder> recorder = new ThreadLocalProperty<IRecorder>();
		private readonly ThreadLocalProperty<object> inArrange = new ThreadLocalProperty<object>();
		private readonly ThreadLocalProperty<object> inCreate = new ThreadLocalProperty<object>();
		private readonly ThreadLocalProperty<object> dispatchToMethodMocks = new ThreadLocalProperty<object>();

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

		public bool InCreate
		{
			get { return this.inCreate.Get() != null; }
			private set { this.inCreate.Set(value ? (object)value : null); }
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
			return new RecordingSession(this);
		}

		public IDisposable StartArrange()
		{
			Monitor.Enter(this);
			return new InArrangeContext(this);
		}

		public IDisposable StartCreate()
		{
			Monitor.Enter(this);
			return new InCreateContext(this);
		}

		public int GetNextArrangeId()
		{
			lock (this)
				return nextArrangeId++;
		}

		private class RecordingSession : IDisposable
		{
			private readonly RepositorySharedContext context;
			private readonly int oldCounter;

			public RecordingSession(RepositorySharedContext context)
			{
				this.context = context;
				this.oldCounter = ProfilerInterceptor.ReentrancyCounter;

				ProfilerInterceptor.ReentrancyCounter = 0;
			}

			public void Dispose()
			{
				ProfilerInterceptor.ReentrancyCounter = this.oldCounter;

				context.Recorder = null;
				Monitor.Exit(context);
			}
		}

		private class InArrangeContext : IDisposable
		{
			private readonly RepositorySharedContext context;

			public InArrangeContext(RepositorySharedContext context)
			{
				this.context = context;
				Debug.Assert(!context.InArrange);
				context.InArrange = true;
			}

			public void Dispose()
			{
				context.InArrange = false;
				Monitor.Exit(context);
			}
		}

		private class InCreateContext : IDisposable
		{
			private readonly RepositorySharedContext context;

			public InCreateContext(RepositorySharedContext context)
			{
				this.context = context;
				Debug.Assert(!context.InCreate);
				context.InCreate = true;
			}

			public void Dispose()
			{
				context.InCreate = false;
				Monitor.Exit(context);
			}
		}
	}
}
