/*
 JustMock Lite
 Copyright © 2010-2015,2019 Progress Software Corporation

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
using System.Reflection;

namespace Telerik.JustMock.Core.Context
{
	internal class MSpecContextResolver : MockingContextResolverBase
	{
		private const string MSpecAssertionFailedName = "Machine.Specifications.SpecificationException, Machine.Specifications";

		private readonly Dictionary<Type, MocksRepository> repositories = new Dictionary<Type, MocksRepository>();

		public MSpecContextResolver()
			: base(MSpecAssertionFailedName)
		{
		}

		public override MocksRepository ResolveRepository(UnresolvedContextBehavior unresolvedContextBehavior)
		{
			lock (this.repositorySync)
			{
				var testMethod = this.GetTestMethod();
				if (testMethod != null)
				{
					return repositories[testMethod.DeclaringType];
				}

				if (unresolvedContextBehavior == UnresolvedContextBehavior.DoNotCreateNew)
				{
					return null;
				}

				var stackTrace = new StackTrace();
				var frames = stackTrace.EnumerateFrames().ToList();
				var caller = frames.FirstOrDefault(method => method.Module.Assembly != typeof(MocksRepository).Assembly);
				var mspecTestClass = caller.DeclaringType;

				MocksRepository parentRepo;
				repositories.TryGetValue(mspecTestClass.BaseType, out parentRepo);

				var repo = new MocksRepository(parentRepo, caller);
				repositories.Add(mspecTestClass, repo);

				return repo;
			}
		}

		public override bool RetireRepository()
		{
			lock (this.repositorySync)
			{
				var stackTrace = new StackTrace();
				var testMethod = FindExistingTestMethod(stackTrace.EnumerateFrames());
				if (testMethod == null)
				{
					return false;
				}

				var key = testMethod.DeclaringType;
				var repo = repositories[key];
				repositories.Remove(key);
				repo.Retire();

				return true;
			}
		}

		public override MethodBase GetTestMethod()
		{
			var stackTrace = new StackTrace();
			var frames = stackTrace.EnumerateFrames().ToList();
			var testMethod = this.FindExistingTestMethod(frames);

			return testMethod;
		}

		public static bool IsAvailable
		{
			get { return FindType(MSpecAssertionFailedName, false) != null; }
		}

		private MethodBase FindExistingTestMethod(IEnumerable<MethodBase> frames)
		{
			var q = from method in frames
					where method.DeclaringType != null && repositories.ContainsKey(method.DeclaringType)
					select method;

			return q.FirstOrDefault();
		}
	}
}
