/*
 JustMock Lite
 Copyright © 2010-2015 Telerik EAD

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
		protected readonly string assertFailedExceptionTypeName;

		public abstract MocksRepository ResolveRepository(UnresolvedContextBehavior unresolvedContextBehavior);

		public abstract bool RetireRepository();

		public MockingContextResolverBase(string assertFailedExceptionTypeName)
		{
			this.assertFailedExceptionTypeName = assertFailedExceptionTypeName;
		}

		public Action<string, Exception> GetFailMethod()
		{
			var factoryExpr = CreateExceptionFactory();
			if (factoryExpr == null)
				return null;

			var factory = factoryExpr.Compile();

			return (message, innerException) =>
			{
				var exception = factory(message, innerException);
				throw exception;
			};
		}

		protected virtual Expression<Func<string, Exception, Exception>> CreateExceptionFactory()
		{
            Type assertionException = FindType(this.assertFailedExceptionTypeName);
			return this.CreateExceptionFactory(assertionException);
		}

		protected Expression<Func<string, Exception, Exception>> CreateExceptionFactory(Type assertionException)
		{
			var exceptionCtor = assertionException.GetConstructor(new[] { typeof(string), typeof(Exception) });
			var messageParam = Expression.Parameter(typeof(string), "message");
			var innerExceptionParam = Expression.Parameter(typeof(Exception), "innerException");
			var newException = Expression.New(exceptionCtor, messageParam, innerExceptionParam);
			return (Expression<Func<string, Exception, Exception>>)Expression.Lambda(typeof(Func<string, Exception, Exception>), newException, messageParam, innerExceptionParam);
		}

		protected static Type FindType(string assemblyAndTypeName, bool throwOnNotFound = true)
        {
            string[] parts = assemblyAndTypeName.Split(',').Select(s => s.Trim()).ToArray();
            string assemblyName = parts[1];

            Assembly assembly = GetAssembly(assemblyName);
            Type foundType = assembly != null ? assembly.GetType(parts[0]) : null;
            if (foundType == null && throwOnNotFound)
                throw new InvalidOperationException(String.Format("Test framework type '{0}' not found", assemblyAndTypeName));

            return foundType;
        }

        protected static Assembly GetAssembly(string assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => String.Equals(a.GetAssemblyName(), assemblyName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
