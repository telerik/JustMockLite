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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Telerik.JustMock.Core.Castle.Core.Internal;

namespace Telerik.JustMock.Core.Context
{
	internal abstract class MockingContextResolverBase : IMockingContextResolver
	{
		private readonly string assertFailedExceptionTypeName;
		protected readonly Dictionary<string, Assembly> frameworkAssemblies;

		public abstract MocksRepository ResolveRepository(UnresolvedContextBehavior unresolvedContextBehavior);

		public abstract bool RetireRepository();

		public MockingContextResolverBase(string assertFailedExceptionTypeName, params string[] frameworkAssemblyNames)
		{
			this.assertFailedExceptionTypeName = assertFailedExceptionTypeName;

			frameworkAssemblies = AppDomain.CurrentDomain.GetAssemblies()
				.Join(frameworkAssemblyNames, asm => asm.GetAssemblyName(), name => name, (asm, name) => asm, StringComparer.OrdinalIgnoreCase)
				.ToLookup(asm => asm.GetAssemblyName(), asm => asm, StringComparer.OrdinalIgnoreCase)
				.Select(group => group.MaxElement(asm => new AssemblyName(asm.FullName).Version))
				.ToDictionary(asm => asm.GetAssemblyName(), asm => asm, StringComparer.OrdinalIgnoreCase);

			if (frameworkAssemblies.Count != frameworkAssemblyNames.Length)
				throw new MockException(String.Format("{0} did not resolve the framework assemblies.", this.GetType()));
		}

		public Action<string, Exception> GetFailMethod()
		{
			var assertionFailedExceptionType = this.FindType(assertFailedExceptionTypeName);
			var factoryExpr = CreateExceptionFactory(assertionFailedExceptionType);
			var factory = factoryExpr.Compile();

			return (message, innerException) =>
			{
				var exception = factory(message, innerException);
				throw exception;
			};
		}

		protected virtual Expression<Func<string, Exception, Exception>> CreateExceptionFactory(Type assertionException)
		{
			var exceptionCtor = assertionException.GetConstructor(new[] { typeof(string), typeof(Exception) });
			var messageParam = Expression.Parameter(typeof(string), "message");
			var innerExceptionParam = Expression.Parameter(typeof(Exception), "innerException");
			var newException = Expression.New(exceptionCtor, messageParam, innerExceptionParam);
			return (Expression<Func<string, Exception, Exception>>)Expression.Lambda(typeof(Func<string, Exception, Exception>), newException, messageParam, innerExceptionParam);
		}

		protected Type FindType(string assemblyAndTypeName)
		{
			if (assemblyAndTypeName == null)
				return null;

			var parts = assemblyAndTypeName.Split('!');
			Type foundType;
			if (parts.Length == 2)
			{
				var asm = this.frameworkAssemblies[parts[0]];
				foundType = asm.GetType(parts[1]);
			}
			else
			{
				foundType = this.frameworkAssemblies.Values
					.Select(asm => asm.GetType(assemblyAndTypeName))
					.First(type => type != null);
			}
			if (foundType == null)
				throw new InvalidOperationException(String.Format("Test framework type '{0}' not found", assemblyAndTypeName));

			return foundType;
		}

		protected static bool IsAssemblyLoaded(string assemblyName)
		{
#if SILVERLIGHT
			return AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.GetAssemblyName().Equals(assemblyName, StringComparison.OrdinalIgnoreCase));
#else
			return AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.GetName().Name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase));
#endif
		}
	}
}
