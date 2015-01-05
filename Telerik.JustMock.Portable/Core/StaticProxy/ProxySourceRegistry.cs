using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Telerik.JustMock.Core.StaticProxy
{
	public static class ProxySourceRegistry
	{
		internal static readonly Dictionary<Type, Type> ProxyTypes = new Dictionary<Type, Type>();

		public static void Register(RuntimeTypeHandle moduleTypeHandle)
		{
			var assembly = Type.GetTypeFromHandle(moduleTypeHandle).Assembly;
			foreach (var type in assembly.GetLoadableTypes())
			{
				var proxyAttr = type.GetCustomAttributes(typeof(ProxyAttribute), false).FirstOrDefault() as ProxyAttribute;
				if (proxyAttr == null)
					continue;

				ProxyTypes[proxyAttr.ProxiedType] = type;
			}
		}
	}
}
