using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.JustMock.Core.Context
{
	internal abstract class MockingContextResolverBase : IMockingContextResolver
	{
		private readonly string assertFailedExceptionTypeName;

		public MockingContextResolverBase(string assertFailedExceptionTypeName, params string[] frameworkAssemblyNames)
		{
			this.assertFailedExceptionTypeName = assertFailedExceptionTypeName;
		}

		public abstract MocksRepository ResolveRepository(UnresolvedContextBehavior unresolvedContextBehavior);

		public abstract bool RetireRepository();

		public Action<string, Exception> GetFailMethod()
		{
			return LocalMockingContextResolver.GetFailMethod(Type.GetType(this.assertFailedExceptionTypeName));
		}

		protected Type FindType(string name)
		{
			return Type.GetType(this.assertFailedExceptionTypeName).Assembly.GetType(name);
		}
	}
}
