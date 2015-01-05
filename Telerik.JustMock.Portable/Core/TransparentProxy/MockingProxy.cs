using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Telerik.JustMock.Core.TransparentProxy
{
	internal static class MockingProxy
	{
		public static bool CanCreate(Type type)
		{
			return false;
		}

		public static IMockMixin GetMockMixin(object proxy)
		{
			return null;
		}

		public static object CreateProxy(object wrappedInstance, MocksRepository repository, IMockMixin mockMixin)
		{
			throw new NotSupportedException();
		}

		public static bool CanIntercept(object instance, MethodBase method)
		{
			return false;
		}
	}
}
