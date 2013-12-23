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
using System.Diagnostics;
using System.Linq;
using System.Text;

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
		public static MocksRepository CurrentRepository
		{
			get { return ResolveRepository(UnresolvedContextBehavior.CreateNewContextualOrLocal); }
		}

		public static MocksRepository ResolveRepository(UnresolvedContextBehavior unresolvedContextBehavior)
		{
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

		public static void RetireRepository()
		{
			foreach (var resolver in registeredContextResolvers)
			{
				if (resolver.RetireRepository())
					return;
			}

			LocalMockingContextResolver.RetireRepository();
		}
		
		public static void Fail(string message, params object[] args)
		{
			var formattedMessage = String.Format(message, args);

			if (failureAggregator == null)
				failAction(formattedMessage);
			else
				failureAggregator.AddFailure(formattedMessage);
		}

		public static string GetStackTrace(string indent)
		{
#if !SILVERLIGHT
			var skipCount = new StackTrace().GetFrames().TakeWhile(frame => frame.GetMethod().DeclaringType.Assembly == typeof(DebugView).Assembly).Count();
			var trace = new StackTrace(skipCount, true);
#else
			var trace = new StackTrace();
#endif

			return "\n".Join(trace.ToString()
					.Split('\n')
					.Select(p => indent + p.Trim()));
		}

		public static IDisposable BeginFailureAggregation()
		{
			if (failureAggregator == null)
			{
				failureAggregator = new FailureAggregator();
			}
			else
			{
				failureAggregator.AddRef();
			}

			return failureAggregator;
		}

		private static readonly List<IMockingContextResolver> registeredContextResolvers = new List<IMockingContextResolver>();
		private static readonly Action<string> failAction;

		[ThreadStatic]
		private static FailureAggregator failureAggregator;

		[ThreadStatic]
		private static MocksRepository lastFrameworkAwareRepository;

		static MockingContext()
		{
			if (MSTestMockingContextResolver.IsAvailable)
				registeredContextResolvers.Add(new MSTestMockingContextResolver());
			if (NUnitMockingContextResolver.IsAvailable)
				registeredContextResolvers.Add(new NUnitMockingContextResolver());
			if (XUnitMockingContextResolver.IsAvailable)
				registeredContextResolvers.Add(new XUnitMockingContextResolver());
			if (MSpecContextResolver.IsAvailable)
				registeredContextResolvers.Add(new MSpecContextResolver());
			if (MbUnitContextResolver.IsAvailable)
				registeredContextResolvers.Add(new MbUnitContextResolver());

			foreach (var resolver in registeredContextResolvers)
			{
				failAction = resolver.GetFailMethod();
				if (failAction != null)
					break;
			}

			if (failAction == null)
				failAction = msg => { throw new MockAssertionFailedException(msg); };
		}

		private class FailureAggregator : IDisposable
		{
			private List<string> failures;
			private int references = 1;

			public void AddRef()
			{
				references++;
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
					failAction(failures[0]);

				var sb = new StringBuilder();
				sb.AppendLine("Multiple assertion failures:");
				for (int i = 0; i < failures.Count; ++i)
				{
					sb.AppendFormat("{0}. ", i + 1);
					sb.AppendLine(failures[i]);
				}
				failAction(sb.ToString());
			}
		}
	}
}
