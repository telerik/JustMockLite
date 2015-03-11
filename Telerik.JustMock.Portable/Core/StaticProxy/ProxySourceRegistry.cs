using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Telerik.JustMock.Core.StaticProxy
{
	public static class ProxySourceRegistry
	{
		internal static readonly Dictionary<RuntimeTypeHandle, RuntimeTypeHandle> ProxyTypes = new Dictionary<RuntimeTypeHandle, RuntimeTypeHandle>();

		public static void Register(RuntimeTypeHandle proxyTypeHandle, RuntimeTypeHandle proxiedTypeHandle)
		{
			ProxyTypes.Add(proxiedTypeHandle, proxyTypeHandle);
		}
	}
}
