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
using System.Linq;
using System.Reflection;

namespace Telerik.JustMock.Core.Context
{
	internal static class LocalMockingContextResolver
	{
		[ThreadStatic]
		private static MethodBase contextMethod;

		[ThreadStatic]
		private static MocksRepository contextRepository;

		public static MocksRepository ResolveRepository(UnresolvedContextBehavior unresolvedContextBehavior)
		{
			var stackTrace = new StackTrace();

			MethodBase callingMethodOutsideJustmock = null;
			foreach (var method in stackTrace.EnumerateFrames())
			{
				if (contextMethod == method)
					return contextRepository;

				if (callingMethodOutsideJustmock == null && method.Module.Assembly != typeof(MocksRepository).Assembly)
					callingMethodOutsideJustmock = method;
			}

			if (callingMethodOutsideJustmock != null && unresolvedContextBehavior == UnresolvedContextBehavior.CreateNewContextualOrLocal)
			{
				// don't reset the old repository - because the mocks created from it may still be used, e.g. if the method
				// associated with ResolveRepository works as a factory for mocks which are then exercised elsewhere
				contextMethod = callingMethodOutsideJustmock;
				contextRepository = new MocksRepository(null, contextMethod);

				return contextRepository;
			}

			return null;
		}

		public static void RetireRepository()
		{
			var stackTrace = new StackTrace();

			if (stackTrace.EnumerateFrames().Contains(contextMethod))
			{
				contextRepository.Retire();

				contextMethod = null;
				contextRepository = null;
			}
		}

		public static Action<string, Exception> GetFailMethod()
		{
			return (msg, innerException) => { throw new MockAssertionFailedException(msg, innerException); };
		}
	}
}
