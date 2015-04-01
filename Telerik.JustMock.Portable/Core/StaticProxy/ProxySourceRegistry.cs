using System;
using System.Collections.Generic;
using System.Linq;

namespace Telerik.JustMock.Core.StaticProxy
{
	public static class ProxySourceRegistry
	{
		internal struct ProxyKey : IEquatable<ProxyKey>
		{
			public readonly RuntimeTypeHandle Type;
			public readonly RuntimeTypeHandle[] AdditionalImplementedTypes;

			public ProxyKey(RuntimeTypeHandle type, RuntimeTypeHandle[] additionalImplementedTypes)
			{
				this.Type = type;
				this.AdditionalImplementedTypes = additionalImplementedTypes;
			}

			public override int GetHashCode()
			{
				unchecked
				{
					int hash = Type.GetHashCode();

					if (AdditionalImplementedTypes != null)
					{
						foreach (var h in AdditionalImplementedTypes)
							hash = 37 * hash + h.GetHashCode();
					}

					return hash;
				}
			}

			public override bool Equals(object obj)
			{
				if (!(obj is ProxyKey))
					return false;
				return this.Equals((ProxyKey)obj);
			}

			public bool Equals(ProxyKey other)
			{
				return Type == other.Type
					&& (AdditionalImplementedTypes == null && other.AdditionalImplementedTypes == null
					|| AdditionalImplementedTypes.SequenceEqual(other.AdditionalImplementedTypes));
			}
		}

		public static bool IsTrialWeaver;

		internal static readonly Dictionary<ProxyKey, RuntimeTypeHandle> ProxyTypes = new Dictionary<ProxyKey, RuntimeTypeHandle>();

		internal static readonly Dictionary<RuntimeTypeHandle, RuntimeTypeHandle> DelegateBackendTypes = new Dictionary<RuntimeTypeHandle, RuntimeTypeHandle>();

		public static void Register(RuntimeTypeHandle proxyTypeHandle, RuntimeTypeHandle proxiedTypeHandle, RuntimeTypeHandle[] additionalImplementedTypes)
		{
			// duplicates may come from different test assemblies
			ProxyTypes[new ProxyKey(proxiedTypeHandle, additionalImplementedTypes)] = proxyTypeHandle;
		}

		public static void RegisterDelegateBackend(RuntimeTypeHandle delegateType, RuntimeTypeHandle backendType)
		{
			DelegateBackendTypes[delegateType] = backendType;
		}
	}
}
