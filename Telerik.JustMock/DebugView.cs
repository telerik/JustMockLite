/*
 JustMock Lite
 Copyright © 2010-2015,2018,2020 Progress Software Corporation

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
using System.Text;
using Telerik.JustMock.Core;
using Telerik.JustMock.Core.Context;
#if !PORTABLE
using Telerik.JustMock.Plugins;
#endif

namespace Telerik.JustMock
{
	using Telerik.JustMock.Diagnostics;

	/// <summary>
	/// Provides introspection and tracing capabilities for ease of debugging failing tests.
	/// </summary>
	/// <remarks>
	/// Often it’s not very clear why a test that uses the mocking API fails.
	/// A test may have multiple arrangements, each with various overlapping
	/// argument matchers. When you have a complex set of arrangements and the
	/// system under test makes a call to a mock, it’s sometimes hard to
	/// understand which arrangement actually gets executed and which
	/// expectations get updated. The <see cref="DebugView"/> class can help in such times.
	/// It can be used in two ways – to provide an overview of the current
	/// internal state of the mocking API, and to provide a step-by-step replay
	/// of the interception events happening inside the mocking API.
	/// 
	/// The current internal state is exposed through the <see cref="CurrentState"/> property.
	/// It contains a human-readable description of the current state of the
	/// mocking API. It describes in detail the state of all occurrence
	/// expectations and the number of calls to all intercepted methods. The
	/// first part is useful when debugging failing expectations from
	/// arrangements. The second part is useful for debugging failing occurrence
	/// asserts.
	/// 
	/// The step-by-step replay is intended for use with an interactive debugger
	/// (e.g. the Visual Studio managed debugger). To begin using it, add the
	/// DebugView class to a Watch in the debugger. Break the test execution
	/// before your test begins. Set the <see cref="IsTraceEnabled"/> property to true from
	/// the Watch window. Now, as you step over each line in your test, the
	/// <see cref="FullTrace"/> and <see cref="LastTrace"/> properties will be updated to show the events
	/// happening inside the mocking API. <see cref="FullTrace"/> will show the entire event
	/// log so far. <see cref="LastTrace"/> will contain all events that have happened since
	/// the debugger last updated the Watch window, but only if any new events
	/// have actually occurred.
	/// </remarks>
	public static class DebugView
	{
		/// <summary>
		/// Shows in human-readable format the current state of the mocking API internals.
		/// </summary>
		public static string CurrentState
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() =>
					{
						var repo = MockingContext.ResolveRepository(UnresolvedContextBehavior.DoNotCreateNew);
						if (repo == null)
							return "N/A";

						var activeTrace = traceSink;
						traceSink = null;
						var debugView = repo.GetDebugView();
						traceSink = activeTrace;
						return debugView;
					});
			}
		}

		/// <summary>
		/// Enables or disables the step-by-step event trace.
		/// </summary>
		public static bool IsTraceEnabled
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(
					() =>
						traceSink != null && (traceSink.TraceOptions & TraceOptions.InternalTrace) != 0);
			}
			set
			{
				ProfilerInterceptor.GuardInternal(
					() =>
						{
							if (value)
							{
								if (traceSink == null)
								{
									traceSink = new Trace();
								}

								(traceSink as Trace).TraceOptions |= TraceOptions.InternalTrace;
							}
							else
							{
								if (traceSink != null)
								{
									(traceSink as Trace).TraceOptions ^= TraceOptions.InternalTrace;
								}
							}
						});
			}
		}

		internal static bool IsRemoteTraceEnabled
		{
			get
			{
				return traceSink != null && (traceSink.TraceOptions & TraceOptions.RemoteTrace) != 0;
			}
			set
			{
				if (value)
				{
					if (traceSink == null)
					{
						traceSink = new Trace();
					}

					(traceSink as Trace).TraceOptions |= TraceOptions.RemoteTrace;
				}
				else
				{
					if (traceSink != null)
					{
						(traceSink as Trace).TraceOptions ^= TraceOptions.RemoteTrace;
					}
				}
			}
		}

		/// <summary>
		/// Shows the entire event log when the event trace is enabled.
		/// </summary>
		public static string FullTrace
		{
			get { return ProfilerInterceptor.GuardInternal(() => traceSink != null ? traceSink.FullTrace : null); }
		}

		/// <summary>
		/// When the event trace is enabled, this property shows the portion
		/// of the event log that was added since the property was last evaluated.
		/// </summary>
		public static string LastTrace
		{
			get { return ProfilerInterceptor.GuardInternal(() => traceSink != null ? traceSink.LastTrace : null); }
		}

		internal static void TraceEvent(IndentLevel traceLevel, Func<string> message)
		{
			var activeTrace = traceSink;
			if (activeTrace == null || activeTrace.TraceOptions == TraceOptions.Disabled)
			{
				return;
			}

#if !PORTABLE
			var previousTraceOptions = activeTrace.TraceOptions;

			try
			{
				if ((activeTrace.TraceOptions & TraceOptions.RemoteTrace) != 0)
				{
					// traces triggered by profiler intercepted calls and repository retirement
					// could cause deadlocks and infinite loops in remote tracing, so disable it
					if (MockingContext.RetireRepositoryCounter > 0
						|| (ProfilerInterceptor.ProfilerInterceptionCounter > 0
							&& MockingContext.ResolveRepository(UnresolvedContextBehavior.DoNotCreateNew) == null))
					{
						(activeTrace as Trace).TraceOptions ^= TraceOptions.RemoteTrace;
					}
				}
#endif

				string messageStr = null;
				try
				{
					messageStr = message() ?? String.Empty;
				}
				catch (Exception ex)
				{
					messageStr = "[Exception thrown]\n" + ex;
				}

				var formattedMessage = String.Join(Environment.NewLine, messageStr.Split('\n')
						.Select(line => String.Format("{0}{1}", traceLevel.AsIndent(), line.TrimEnd())).ToArray())
					+ (traceLevel.IsLeaf() ? "" : ":");

				activeTrace.TraceEvent(formattedMessage);

				GC.KeepAlive(CurrentState); // for coverage testing

#if !PORTABLE
			}
			finally
			{
				// restore previous trace options if needed
				if (previousTraceOptions != activeTrace.TraceOptions)
				{
					(activeTrace as Trace).TraceOptions = previousTraceOptions;
				}
			}
#endif
		}

		private static string AsIndent(this IndentLevel traceLevel)
		{
			return "".Join(Enumerable.Repeat("    ", (int)traceLevel));
		}

		internal static void PrintStackTrace()
		{
			TraceEvent(IndentLevel.StackTrace, () => "Stack trace:\n" + MockingContext.GetStackTrace(IndentLevel.StackTraceInner.AsIndent()));
		}

		internal static Exception GetStateAsException()
		{
			return IsTraceEnabled ? new DebugViewDetailsException() : null;
		}

		private static ITraceSink traceSink;

		private static bool IsLeaf(this IndentLevel level)
		{
			switch (level)
			{
				case IndentLevel.DispatchResult:
				case IndentLevel.Matcher:
					return true;
				default:
					return false;
			}
		}

#if (DEBUG && !COREFX && !NETCORE)
		public static void SaveProxyAssembly()
		{
			ProfilerInterceptor.GuardInternal(() => DynamicProxyMockFactory.SaveAssembly());
		}
#endif

		internal static Action<string> DebugTrace = s =>
#if PORTABLE
			System.Diagnostics.Debug.WriteLine(s);
#else
			System.Diagnostics.Trace.WriteLine(s);
#endif
	}
}

namespace Telerik.JustMock.Diagnostics
{
	/// <summary>
	/// This exception provides additional information when assertion failures are produced.
	/// </summary>
	[Serializable]
	public sealed class DebugViewDetailsException : Exception
	{
		internal DebugViewDetailsException()
			: base(String.Format("State:\n{0}\n\nFull trace:\n{1}", DebugView.CurrentState, DebugView.FullTrace))
		{ }

#if !COREFX
		private DebugViewDetailsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif
	}

	internal enum IndentLevel
	{
		Dispatch = 0,
		Warning = 0,
		Configuration = 0,
		MethodMatch = 1,
		DispatchResult = 1,
		Matcher = 2,
		StackTrace = 1,
		StackTraceInner = 2,
	}

	internal interface ITrace
	{
		string FullTrace { get; }
		string LastTrace { get; }
	}


	[Flags]
	internal enum TraceOptions
	{
		Disabled = 0,
		InternalTrace = 1,
		RemoteTrace= 2
	}

	internal interface ITraceSink : ITrace
	{
		TraceOptions TraceOptions { get; }

		void TraceEvent(string message);
	}

	internal class Trace : ITraceSink
	{
		private readonly object logSync = new object();
		private readonly StringBuilder log = new StringBuilder();
		private readonly StringBuilder currentTrace = new StringBuilder();
		private bool currentTraceRead;

		private readonly object traceOptionsSync = new object();
		private TraceOptions traceOptions = TraceOptions.Disabled;

		public string FullTrace
		{
			get
			{
				lock (this.logSync)
				{
					return this.log.ToString();
				}
			}
		}

		public string LastTrace
		{
			get
			{
				lock (this.logSync)
				{
					this.currentTraceRead = true;
					return this.currentTrace.ToString();
				}
			}
		}

		public TraceOptions TraceOptions
		{
			get
			{
				lock (this.traceOptionsSync)
				{
					return this.traceOptions;
				}
			}
			set
			{
				lock (this.traceOptionsSync)
				{
					this.traceOptions = value;
				}
			}
		}

		public void TraceEvent(string message)
		{

#if !PORTABLE
			if ((this.TraceOptions & TraceOptions.RemoteTrace) != 0)
			{
				try
				{
					if (MockingContext.Plugins.Exists<IDebugWindowPlugin>())
					{
						var debugWindowPlugin = MockingContext.Plugins.Get<IDebugWindowPlugin>();
						debugWindowPlugin.TraceMessage(message);
					}
				}
				catch (Exception e)
				{
					System.Diagnostics.Trace.WriteLine("Exception thrown calling IDebugWindowPlugin plugin: " + e);
				}
			}
#endif

			if ((this.TraceOptions & TraceOptions.InternalTrace) != 0)
			{
				Debug.WriteLine(message);

				lock (this.logSync)
				{
					this.log.AppendLine(message);

					if (this.currentTraceRead)
					{
						this.currentTraceRead = false;
						this.currentTrace.Length = 0;
					}

					this.currentTrace.AppendLine(message);
				}
			}
		}
	}
}
