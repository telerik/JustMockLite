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
using Telerik.JustMock.DebugWindow.Service.Client;
using Telerik.JustMock.Diagnostics;
using Telerik.JustMock.Setup;

namespace Telerik.JustMock.Core.Context
{
	internal abstract class HierarchicalTestFrameworkContextResolver : MockingContextResolverBase
	{
		private readonly List<RepositoryOperationsBase> repoOperations = new List<RepositoryOperationsBase>();
		private readonly Dictionary<Assembly, HashSet<Type>> knownTestClasses = new Dictionary<Assembly, HashSet<Type>>();

		public HierarchicalTestFrameworkContextResolver(string assertFailedExceptionTypeName)
			: base(assertFailedExceptionTypeName)
		{
		}

		public override MocksRepository ResolveRepository(UnresolvedContextBehavior unresolvedContextBehavior)
		{
			lock (this.repositorySync)
			{
				int repoIdx;
				RepositoryOperationsBase entryOps = null;
				var testMethod = FindTestMethod(out repoIdx, out entryOps);
				if (testMethod == null || entryOps == null)
				{
					return null;
				}

				object entryKey = entryOps.GetKey(testMethod);
				MocksRepository repo = this.FindRepositoryInOps(entryOps, entryKey);
				if (repo != null)
				{
					return repo;
				}

				if (unresolvedContextBehavior == UnresolvedContextBehavior.DoNotCreateNew)
				{
					return null;
				}

				//Check if this is the same kind of method but from a derived class, thus building context.
				MocksRepository parentRepo = entryOps.FindRepositoryToInherit(testMethod);
				if (parentRepo == null)
				{
					for (var repoIdxParent = repoIdx + 1; parentRepo == null && repoIdxParent < this.repoOperations.Count; ++repoIdxParent)
					{
						var ops = this.repoOperations[repoIdxParent];
						if (ops.IsLeaf)
						{
							continue;
						}

						object parentKey = ops.GetKey(testMethod);
						if (ops.IsUsedOnAllThreads)
						{
							parentRepo = ops.FindRepositoryFromAnyThread(parentKey);
						}
						else
						{
							parentRepo = ops.FindRepository(parentKey) ?? ops.FindRepositoryToInherit(testMethod);
						}
					}
				}

				MocksRepository entryRepo;
				try
				{
					entryRepo = new MocksRepository(parentRepo, testMethod);
					entryOps.AddRepository(entryKey, entryRepo);
					OnMocksRepositoryCreated(repo);
				}
				catch (TypeInitializationException e)
				{
					throw e.InnerException;
				}

				return entryRepo;
			}
		}

		public override bool RetireRepository()
		{
			lock (this.repositorySync)
			{
				RepositoryOperationsBase entryOps = null;
				int repoIdx;
				var testMethod = FindTestMethod(out repoIdx, out entryOps);
				if (testMethod == null)
				{
					return false;
				}

				var entryKey = entryOps.GetKey(testMethod);
				MocksRepository repo = FindRepositoryInOps(entryOps, entryKey);
				if (repo != null)
				{
					entryOps.RetireRepository(entryKey, repo);
				}

				return true;
			}
		}

		public override MethodBase GetTestMethod()
		{
			var stackTrace = new StackTrace();
			var q = from method in stackTrace.EnumerateFrames()
					where repoOperations.Any(repo => repo.MatchesMethod(method))
					select method;

			var allTestMethods = q.Distinct().ToArray();
			if (allTestMethods.Length > 1)
			{
				string message = "Calling one test method from another could result in unexpected behavior and must be avoided. Extract common mocking logic to a non-test method. At:\n" + stackTrace;
				DebugView.DebugTrace(message);
			}

			MethodBase testMethod = allTestMethods.FirstOrDefault();

			return testMethod;
		}

		protected virtual void OnMocksRepositoryCreated(MocksRepository repo)
		{
		}

		private MethodBase FindTestMethod(out int repoIdx, out RepositoryOperationsBase entryOps)
		{
			MethodBase testMethod = this.GetTestMethod();
			if (testMethod != null)
			{
				var disableAttr = Attribute.GetCustomAttribute(testMethod, typeof(DisableAutomaticRepositoryResetAttribute)) as DisableAutomaticRepositoryResetAttribute;
				if (disableAttr != null
					&& ProfilerInterceptor.IsProfilerAttached
					&& !disableAttr.AllowMocking)
					throw new MockException("Using the mocking API in a test method decorated with DisableAutomaticRepositoryResetAttribute is unsafe. Read the documentation of the DisableAutomaticRepositoryResetAttribute class for further information and possible solutions.");
			}
			else
			{
				testMethod = AsyncContextResolver.GetContext();
			}

			repoIdx = 0;
			entryOps = null;
			if (testMethod != null)
			{
				for (repoIdx = 0; repoIdx < this.repoOperations.Count; ++repoIdx)
				{
					var ops = this.repoOperations[repoIdx];
					if (ops.MatchesMethod(testMethod))
					{
						entryOps = ops;
						break;
					}
				}

				JMDebug.Assert(entryOps != null);
			}

			return testMethod;
		}

		private MocksRepository FindRepositoryInOps(RepositoryOperationsBase entryOps, object entryKey)
		{
			if (entryOps.IsUsedOnAllThreads)
			{
				MocksRepository repo = entryOps.FindRepositoryFromAnyThread(entryKey);
				if (repo != null)
				{
					if (repo.IsRetired)
					{
						entryOps.RetireRepository(entryKey, repo);
						repo = null;
					}
				}
				return repo;
			}
			else
			{
				MocksRepository repo = entryOps.FindRepository(entryKey);
				if (repo != null)
				{
					if (repo.IsParentToAnotherRepository || repo.IsRetired)
					{
						entryOps.RetireRepository(entryKey, repo);
						repo = null;
					}
				}

				return repo;
			}
		}

		private Func<MethodBase, bool> CreateAttributeMatcher(string[] attributeTypeNames)
		{
			if (attributeTypeNames == null)
				return m => false;

			var attributeTypes = attributeTypeNames.Select(name => FindType(name, false)).ToArray();
			if (attributeTypes.Any(t => t == null))
				throw new InvalidOperationException(String.Format("Some attribute type among {0} not found.", String.Join(",", attributeTypeNames)));

			return method => attributeTypes.Any(attr => Attribute.IsDefined(method, attr));
		}

		protected void AddRepositoryOperations(string[] attributeTypeNames, Func<MethodBase, object> getKey, Func<MethodBase, object, bool> isInheritingContext, bool isLeaf, bool isUsedOnAllThreads)
		{
			this.AddRepositoryOperations(CreateAttributeMatcher(attributeTypeNames), getKey, isInheritingContext, isLeaf, isUsedOnAllThreads);
		}

		protected void AddRepositoryOperations(Func<MethodBase, bool> matchesMethod, Func<MethodBase, object> getKey, Func<MethodBase, object, bool> isInheritingContext, bool isLeaf, bool isUsedOnAllThreads)
		{
			if (isInheritingContext == null)
				isInheritingContext = (_, __) => false;

			RepositoryOperationsBase ops = this.CreateRepositoryOperations(getKey, matchesMethod, isLeaf, isUsedOnAllThreads, isInheritingContext);

			this.repoOperations.Add(ops);
		}

		private RepositoryOperationsBase CreateRepositoryOperations(Func<MethodBase, object> getKey, Func<MethodBase, bool> matchesMethod, bool isLeaf, bool isUsedOnAllThreads, Func<MethodBase, object, bool> isInheritingContext)
		{
			RepositoryOperationsBase repoOperations = null;

			if (Mock.IsProfilerEnabled)
				repoOperations = new RepositoryOperationsStrongRef();
			else
				repoOperations = new RepositoryOperationsWeakRef();

			repoOperations.GetKey = getKey;
			repoOperations.MatchesMethod = matchesMethod;
			repoOperations.IsLeaf = isLeaf;
			repoOperations.IsUsedOnAllThreads = isUsedOnAllThreads;
			repoOperations.IsInheritingContext = isInheritingContext;

			return repoOperations;
		}

		protected enum FixtureConstuctorSemantics
		{
			InstanceConstructorCalledOncePerFixture,
			InstanceConstructorCalledOncePerTest,
		}

		protected void SetupStandardHierarchicalTestStructure(
			string[] testMethodAttrs, string[] testSetupAttrs, string[] fixtureSetupAttrs,
			string[] assemblySetupAttrs, FixtureConstuctorSemantics fixtureConstructorSemantics)
		{
			this.AddRepositoryOperations(testMethodAttrs, method => method, null, true, true);

			switch (fixtureConstructorSemantics)
			{
				case FixtureConstuctorSemantics.InstanceConstructorCalledOncePerFixture:
					{
						this.AddRepositoryOperations(testSetupAttrs, method => method.DeclaringType,
							(method, prevType) => IsTypeAssignableIgnoreGenericArgs((Type)prevType, method.DeclaringType),
							false, false);

						var fixtureSetupMatcher = CreateAttributeMatcher(fixtureSetupAttrs);
						this.AddRepositoryOperations(
							method => fixtureSetupMatcher(method)
								|| MatchTestClassConstructor(ConstructorKind.Both, method, testMethodAttrs),
							method => method.DeclaringType, null, false, true);
					}
					break;
				case FixtureConstuctorSemantics.InstanceConstructorCalledOncePerTest:
					{
						var testSetupMatcher = CreateAttributeMatcher(testSetupAttrs);
						var fixtureSetupMatcher = CreateAttributeMatcher(fixtureSetupAttrs);

						this.AddRepositoryOperations(
							method => testSetupMatcher(method)
								|| MatchTestClassConstructor(ConstructorKind.Instance, method, testMethodAttrs)
								|| MatchTestClassDispose(method, testMethodAttrs),
							method => method.DeclaringType,
							(method, prevType) => IsTypeAssignableIgnoreGenericArgs((Type)prevType, method.DeclaringType),
							false, false);

						this.AddRepositoryOperations(
							method => fixtureSetupMatcher(method)
								|| MatchTestClassConstructor(ConstructorKind.Static, method, testMethodAttrs),
							method => method.DeclaringType, null, false, true);
					}
					break;
			}

			if (assemblySetupAttrs != null)
			{
				this.AddRepositoryOperations(assemblySetupAttrs, method => method.DeclaringType.Assembly, null, false, true);
			}
		}

		private static bool IsTypeAssignableIgnoreGenericArgs(Type typeToCheck, Type derivedType)
		{
			foreach (var baseType in derivedType.GetInheritanceChain())
			{
				if (baseType == typeToCheck)
					return true;

				if (typeToCheck.IsGenericTypeDefinition && baseType.IsGenericType)
				{
					var genBaseType = baseType.GetGenericTypeDefinition();
					if (genBaseType == typeToCheck)
						return true;
				}
			}

			return false;
		}

		private bool MatchTestClassConstructor(ConstructorKind kind, MethodBase method, string[] testMethodAttributes)
		{
			var constructorKindBit = method.IsStatic ? ConstructorKind.Static : ConstructorKind.Instance;
			if (!(method is ConstructorInfo) || (kind & constructorKindBit) == 0)
				return false;

			return IsDeclaredInTestFixture(method, testMethodAttributes);
		}

		private bool MatchTestClassDispose(MethodBase method, string[] testMethodAttributes)
		{
			var disposeMethod = typeof(IDisposable).GetMethod("Dispose");
			if (!disposeMethod.IsImplementedBy(method))
				return false;

			return IsDeclaredInTestFixture(method, testMethodAttributes);
		}

		private bool IsDeclaredInTestFixture(MethodBase method, string[] testMethodAttributes)
		{
			if (method.DeclaringType == null)
				return false;
			var assembly = method.DeclaringType.Assembly;
			HashSet<Type> knownTestClassesInAssembly;
			lock (knownTestClasses)
			{
				if (!knownTestClasses.TryGetValue(assembly, out knownTestClassesInAssembly))
				{
					var matcher = CreateAttributeMatcher(testMethodAttributes);

					var q = from type in assembly.GetLoadableTypes()
							where type.GetMethods().Any(m => matcher(m))
							select type;

					knownTestClassesInAssembly = new HashSet<Type>(q);
					knownTestClasses.Add(assembly, knownTestClassesInAssembly);
				}
			}

			return knownTestClassesInAssembly.Contains(method.DeclaringType);
		}

		private abstract class RepositoryOperationsBase
		{
			public abstract void AddRepository(object entryKey, MocksRepository entryRepo);
			public abstract MocksRepository FindRepository(object key);
			public abstract MocksRepository FindRepositoryFromAnyThread(object key);
			public abstract void RetireRepository(object key, MocksRepository repo);
			public abstract MocksRepository FindRepositoryToInherit(MethodBase testMethod);

			public Func<MethodBase, object> GetKey;
			public Func<MethodBase, bool> MatchesMethod;
			public Func<MethodBase/*new entry*/, object/*existing key*/, bool> IsInheritingContext;
			public bool IsLeaf;
			public bool IsUsedOnAllThreads;
		}

		private class RepositoryOperationsStrongRef : RepositoryOperationsBase
		{
			private readonly ThreadLocalProperty<Dictionary<object, MocksRepository>> currentRepositories = new ThreadLocalProperty<Dictionary<object, MocksRepository>>();

			public override void AddRepository(object entryKey, MocksRepository entryRepo)
			{
				this.GetCurrentDictionary().Add(entryKey, entryRepo);

				if (DebugView.IsRemoteTraceEnabled)
				{
					using (var publisher = new MockRepositoryPublishingServiceClient("net.tcp://localhost:10003/MockRepositoryPublishingService"))
					{
						publisher.RepositoryCreated();
					}
				}
			}

			public override MocksRepository FindRepository(object key)
			{
				var repos = GetCurrentDictionary();

				MocksRepository repo;
				repos.TryGetValue(key, out repo);
				return repo;
			}

			public override MocksRepository FindRepositoryFromAnyThread(object key)
			{
				MocksRepository repo = null;
				currentRepositories.GetAllThreadsValues().FirstOrDefault(repos => repos.TryGetValue(key, out repo));
				return repo;
			}

			public override void RetireRepository(object key, MocksRepository repo)
			{
				var dict = currentRepositories.GetAllThreadsValues().FirstOrDefault(repos => repos.ContainsValue(repo));

				if (dict != null)
				{
					dict.Remove(key);
					repo.Retire();

					if (DebugView.IsRemoteTraceEnabled)
					{
						using (var publisher = new MockRepositoryPublishingServiceClient("net.tcp://localhost:10003/MockRepositoryPublishingService"))
						{
							publisher.RepositoryRetired();
						}
					}
				}
			}

			public override MocksRepository FindRepositoryToInherit(MethodBase testMethod)
			{
				var dict = GetCurrentDictionary();

				var inheritableRepos = dict.Where(pair => this.IsInheritingContext(testMethod, pair.Key));
				var pairToInherit = inheritableRepos
					.FirstOrDefault(pair => inheritableRepos.All(pair2 =>
								pair.Equals(pair2) ||
								this.IsInheritingContext(pair.Value.Method, pair2.Key)));

				return pairToInherit.Value;

			}

			private Dictionary<object, MocksRepository> GetCurrentDictionary()
			{
				return currentRepositories.GetOrSetDefault(() => new Dictionary<object, MocksRepository>());
			}
		}

		private class RepositoryOperationsWeakRef : RepositoryOperationsBase
		{
			private readonly ThreadLocalProperty<Dictionary<object, WeakReference>> currentRepositories = new ThreadLocalProperty<Dictionary<object, WeakReference>>();

			public override void AddRepository(object entryKey, MocksRepository entryRepo)
			{
				this.GetCurrentDictionary().Add(entryKey, new WeakReference(entryRepo));

				if (DebugView.IsRemoteTraceEnabled)
				{
					using (var publisher = new MockRepositoryPublishingServiceClient("net.tcp://localhost:10003/MockRepositoryPublishingService"))
					{
						publisher.RepositoryCreated();
					}
				}
			}

			public override MocksRepository FindRepository(object key)
			{
				var repos = GetCurrentDictionary();

				return FindRepository(repos, key);
			}

			public override MocksRepository FindRepositoryFromAnyThread(object key)
			{
				var repos = currentRepositories.GetAllThreadsValues().FirstOrDefault(r => r.ContainsKey(key));

				return repos != null ? FindRepository(repos, key) : null;
			}

			private MocksRepository FindRepository(Dictionary<object, WeakReference> repos, object key)
			{
				WeakReference repo;
				MocksRepository ret = null;
				if (repos.TryGetValue(key, out repo))
				{
					ret = (MocksRepository)repo.Target;
					if (ret == null)
					{
						Cleanup(repos);
					}
				}

				return ret;
			}

			private void Cleanup(Dictionary<object, WeakReference> repos)
			{
				var keysToRemove = repos.Where(pair => !pair.Value.IsAlive).Select(pair => pair.Key).ToList();
				foreach (var key in keysToRemove)
					repos.Remove(key);
			}

			public override void RetireRepository(object key, MocksRepository repo)
			{
				var dict = currentRepositories.GetAllThreadsValues().FirstOrDefault(repos => repos.Any(pair => pair.Value.Target == repo));

				if (dict != null)
				{
					dict.Remove(key);
					repo.Retire();

					if (DebugView.IsRemoteTraceEnabled)
					{
						using (var publisher = new MockRepositoryPublishingServiceClient("net.tcp://localhost:10003/MockRepositoryPublishingService"))
						{
							publisher.RepositoryRetired();
						}
					}
				}
			}

			public override MocksRepository FindRepositoryToInherit(MethodBase testMethod)
			{
				var dict = GetCurrentDictionary();

				var inheritableRepos = dict.Where(pair => pair.Value.Target != null && this.IsInheritingContext(testMethod, pair.Key));
				var pairToInherit = inheritableRepos
					.FirstOrDefault(pair => inheritableRepos.All(pair2 =>
						{
							var repo = (MocksRepository)pair.Value.Target;
							return pair.Equals(pair2) ||
								(repo != null && this.IsInheritingContext(repo.Method, pair2.Key));
						}));

				return pairToInherit.Value != null ? (MocksRepository)pairToInherit.Value.Target : null;

			}

			private Dictionary<object, WeakReference> GetCurrentDictionary()
			{
				return currentRepositories.GetOrSetDefault(() => new Dictionary<object, WeakReference>());
			}
		}

		[Flags]
		private enum ConstructorKind
		{
			Static = 1, Instance = 2, Both = Static | Instance
		}
	}
}
