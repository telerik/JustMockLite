/*
 JustMock Lite
 Copyright © 2010-2015,2020 Progress Software Corporation

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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Telerik.JustMock.Diagnostics;
#if !PORTABLE
using Telerik.JustMock.Helpers;
using Telerik.JustMock.Plugins;
using Telerik.JustMock.AutoMock.Ninject.Parameters;
#if NETCORE
using System.Runtime.InteropServices;
#endif
#endif

namespace Telerik.JustMock.Core.Context
{
	internal enum UnresolvedContextBehavior
	{
		DoNotCreateNew,
		CreateNewContextual,
		CreateNewContextualOrLocal,
	}

	internal static class MockingContext
	{
		internal const string DebugViewPluginAssemblyFileName = "Telerik.JustMock.DebugWindow.Plugin.dll";

		public static MocksRepository CurrentRepository
		{
			get { return ResolveRepository(UnresolvedContextBehavior.CreateNewContextualOrLocal); }
		}

		public static MocksRepository ResolveRepository(UnresolvedContextBehavior unresolvedContextBehavior)
		{
			if (unresolvedContextBehavior != UnresolvedContextBehavior.DoNotCreateNew)
			{
				DebugView.TraceEvent(IndentLevel.StackTrace, () => String.Format("Resolving repository with unresolved context behavior {0}", unresolvedContextBehavior));
			}

			foreach (var resolver in registeredContextResolvers)
			{
				var repo = resolver.ResolveRepository(unresolvedContextBehavior);
				if (repo != null)
				{
					lastFrameworkAwareRepository = repo;
					return repo;
				}
			}

			if (lastFrameworkAwareRepository != null && !ProfilerInterceptor.IsProfilerAttached)
				return lastFrameworkAwareRepository;

			return LocalMockingContextResolver.ResolveRepository(unresolvedContextBehavior);
		}

#if !PORTABLE
		private static readonly object retireRepositoryounterSync = new object();
		private static int retireRepositoryCounter = 0;

		[DebuggerHidden]
		public static int RetireRepositoryCounter
		{
			get
			{
				lock (retireRepositoryounterSync)
				{
					return retireRepositoryCounter;
				}
			}
			set
			{
				lock (retireRepositoryounterSync)
				{
					if (value < 0)
					{
						throw new InvalidOperationException();
					}
					retireRepositoryCounter = value;
				}
			}
		}
#endif

		public static void RetireRepository()
		{
#if !PORTABLE
			try
			{
				RetireRepositoryCounter++;
#endif
				foreach (var resolver in registeredContextResolvers)
				{
					if (resolver.RetireRepository())
						return;
				}

				LocalMockingContextResolver.RetireRepository();

#if !PORTABLE
				DynamicTypeHelper.Reset();
			}
			finally
			{
				RetireRepositoryCounter--;
			}
#endif
		}

		public static MethodBase GetTestMethod()
		{
			foreach (IMockingContextResolver resolver in registeredContextResolvers)
			{
				var testMethod = resolver.GetTestMethod();
				if (testMethod != null)
				{
					return testMethod;
				}
			}

			return null;
		}

		public static void Fail(string message, params object[] args)
		{
			var formattedMessage = String.Format(message, args);

			if (failureAggregator == null)
				Fail(formattedMessage);
			else
				failureAggregator.AddFailure(formattedMessage);
		}

		public static string GetStackTrace(string indent)
		{
#if SILVERLIGHT
			var trace = new System.Diagnostics.StackTrace().ToString();
#elif PORTABLE
			var trace = new StackTrace().ToString();
#else
			var skipCount = new System.Diagnostics.StackTrace().GetFrames().TakeWhile(frame => frame.GetMethod().DeclaringType.Assembly == typeof(DebugView).Assembly).Count();
			var trace = new System.Diagnostics.StackTrace(skipCount, true).ToString();
#endif

			return "\n".Join(trace
					.Split('\n')
					.Select(p => indent + p.Trim()));
		}

		public static IDisposable BeginFailureAggregation(string message)
		{
			if (failureAggregator == null)
			{
				failureAggregator = new FailureAggregator(message);
			}
			else
			{
				failureAggregator.AddRef(message);
			}

			return failureAggregator;
		}

		private static readonly List<IMockingContextResolver> registeredContextResolvers = new List<IMockingContextResolver>();

		private static Action<string, Exception> failThrower;

		[ThreadStatic]
		private static FailureAggregator failureAggregator;

		[ThreadStatic]
		private static MocksRepository lastFrameworkAwareRepository;

#if !PORTABLE
		public static PluginsRegistry Plugins { get; private set; }
		private static PluginLoadHelper pluginLoadHelper;
#if DEBUG
		private static TraceSource wcfTraceSource;
#endif
#endif

		static MockingContext()
		{
#if !PORTABLE
			MockingContext.Plugins = new PluginsRegistry();
			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;

#if DEBUG
			System.Diagnostics.Trace.AutoFlush = true;

			wcfTraceSource = new TraceSource("System.ServiceModel", SourceLevels.All);
			wcfTraceSource.Listeners.Add(new TextWriterTraceListener(@"c:\temp\justmock_wcf.log"));
#endif

			try
			{
				var clrVersion = Environment.Version;
#if !NETCORE
				if (clrVersion.Major >= 4 && clrVersion.Minor >= 0
					&& clrVersion.Build >= 30319 && clrVersion.Revision >= 42000)
#endif
				{
					var debugWindowEnabledEnv = Environment.GetEnvironmentVariable("JUSTMOCK_DEBUG_VIEW_ENABLED");
					var debugWindowServicesStringEnv = Environment.GetEnvironmentVariable("JUSTMOCK_DEBUG_VIEW_SERVICES");
					var debugWindowAssemblyDirectoryEnv = Environment.GetEnvironmentVariable("JUSTMOCK_DEBUG_VIEW_PLUGIN_DIRECTORY");
					if (!string.IsNullOrEmpty(debugWindowEnabledEnv)
						&& !string.IsNullOrEmpty(debugWindowServicesStringEnv)
						&& !string.IsNullOrEmpty(debugWindowAssemblyDirectoryEnv)
						&& debugWindowEnabledEnv == "1" && Directory.Exists(debugWindowAssemblyDirectoryEnv))
					{
#if NETCORE
						if (RuntimeInformation.FrameworkDescription.Contains(".NET Core"))
						{
							// append 'netcoreapp2.1' suffix if necessary
							if (string.Compare(Path.GetDirectoryName(debugWindowAssemblyDirectoryEnv), "netcoreapp2.1", StringComparison.InvariantCultureIgnoreCase) != 0)
							{
								debugWindowAssemblyDirectoryEnv = Path.Combine(debugWindowAssemblyDirectoryEnv, "netcoreapp2.1");
							}
						}
#endif
						var debugWindowAssemblyPath = Path.Combine(debugWindowAssemblyDirectoryEnv, DebugViewPluginAssemblyFileName);
						MockingContext.pluginLoadHelper = new PluginLoadHelper(debugWindowAssemblyDirectoryEnv);
						MockingContext.Plugins.Register<IDebugWindowPlugin>(
							debugWindowAssemblyPath, new ConstructorArgument("debugWindowServicesString", debugWindowServicesStringEnv));
						DebugView.IsRemoteTraceEnabled = true;
					}
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine("Exception thrown during plugin registration: " + e);
			}
#endif

#if PORTABLE
			if (VisualStudioPortableContextResolver.IsAvailable)
				registeredContextResolvers.Add(new VisualStudioPortableContextResolver());
			if (XamarinAndroidNUnitContextResolver.IsAvailable)
				registeredContextResolvers.Add(new XamarinAndroidNUnitContextResolver());
			if (XamarinIosNUnitContextResolver.IsAvailable)
				registeredContextResolvers.Add(new XamarinIosNUnitContextResolver());
#else
			if (XUnit1xMockingContextResolver.IsAvailable)
				registeredContextResolvers.Add(new XUnit1xMockingContextResolver());
			if (XUnit2xMockingContextResolver.IsAvailable)
				registeredContextResolvers.Add(new XUnit2xMockingContextResolver());
			if (NUnit2xMockingContextResolver.IsAvailable)
				registeredContextResolvers.Add(new NUnit2xMockingContextResolver());
			if (NUnit3xMockingContextResolver.IsAvailable)
				registeredContextResolvers.Add(new NUnit3xMockingContextResolver());
			if (NUnit3_8_xMockingContextResolver.IsAvailable)
				registeredContextResolvers.Add(new NUnit3_8_xMockingContextResolver());
			if (MSpecContextResolver.IsAvailable)
				registeredContextResolvers.Add(new MSpecContextResolver());
			if (MbUnitContextResolver.IsAvailable)
				registeredContextResolvers.Add(new MbUnitContextResolver());
			if (MSTestMockingContextResolver.IsAvailable)
				registeredContextResolvers.Add(new MSTestMockingContextResolver());
			if (MSTestV2MockingContextResolver.IsAvailable)
				registeredContextResolvers.Add(new MSTestV2MockingContextResolver());
#endif

			foreach (var resolver in registeredContextResolvers)
			{
				failThrower = resolver.GetFailMethod();
				if (failThrower != null)
					break;
			}

			if (failThrower == null)
				failThrower = LocalMockingContextResolver.GetFailMethod();
		}

#if !PORTABLE
		private static void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			MockingContext.Plugins.Dispose();
		}
#endif

		private static void Fail(string msg)
		{
			failThrower(msg, DebugView.GetStateAsException());
		}

		private class FailureAggregator : IDisposable
		{
			private List<string> failures;
			private int references = 1;
			private string userMessage;

			public FailureAggregator(string message)
			{
				userMessage = message;
			}

			public void AddRef(string message)
			{
				references++;
				userMessage = message;
			}

			public void AddFailure(string msg)
			{
				if (failures == null)
					failures = new List<string>();

				failures.Add(msg);
			}

			public void Dispose()
			{
				if (references > 1)
				{
					references--;
					return;
				}

				failureAggregator = null;

				if (failures == null)
					return;

				if (failures.Count == 1)
					Fail((userMessage != null ? userMessage + Environment.NewLine : null) + failures[0]);

				var sb = new StringBuilder();
				if (userMessage != null)
					sb.AppendLine(userMessage);
				sb.AppendLine("Multiple assertion failures:");
				for (int i = 0; i < failures.Count; ++i)
				{
					sb.AppendFormat("{0}. ", i + 1);
					sb.AppendLine(failures[i]);
				}
				Fail(sb.ToString());
			}
		}

#if PORTABLE
		private class StackTraceGeneratorException : Exception
		{ }
#endif
	}
}
