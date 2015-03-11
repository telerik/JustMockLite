using System;
using System.Collections.Generic;
using System.Linq;

namespace Telerik.JustMock.Core.StaticProxy
{
	internal class StaticProxyMockFactory : IMockFactory
	{
		public bool IsAccessible(Type type)
		{
			return type.IsPublic;
		}

		public object Create(Type type, MocksRepository repository, IMockMixin mockMixinImpl, MockCreationSettings settings, bool createTransparentProxy)
		{
			var baseType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
			RuntimeTypeHandle proxyTypeHandle;
			var key = new ProxySourceRegistry.ProxyKey(
				baseType.TypeHandle,
				settings.AdditionalMockedInterfaces != null && settings.AdditionalMockedInterfaces.Length > 0
				? settings.AdditionalMockedInterfaces.Select(t => t.TypeHandle).ToArray() : null);
			if (!ProxySourceRegistry.ProxyTypes.TryGetValue(key, out proxyTypeHandle))
			{
				ThrowNoProxyException(baseType, settings.AdditionalMockedInterfaces);
			}

			var interceptor = new DynamicProxyInterceptor(repository);

			var proxyType = Type.GetTypeFromHandle(proxyTypeHandle);
			if (proxyType.IsGenericTypeDefinition)
				proxyType = proxyType.MakeGenericType(type.GetGenericArguments());

			return Activator.CreateInstance(proxyType,
				new object[] { interceptor, mockMixinImpl }.Concat(settings.Mixins).ToArray());
		}

		public Type CreateDelegateBackend(Type delegateType)
		{
			var baseType = delegateType.IsGenericType ? delegateType.GetGenericTypeDefinition() : delegateType;
			RuntimeTypeHandle backendTypeHandle;
			if (!ProxySourceRegistry.DelegateBackendTypes.TryGetValue(baseType.TypeHandle, out backendTypeHandle))
			{
				ThrowNoProxyException(baseType);
			}

			var backendType = Type.GetTypeFromHandle(backendTypeHandle);
			if (backendType.IsGenericTypeDefinition)
				backendType = backendType.MakeGenericType(delegateType.GetGenericArguments());

			return backendType;
		}

		public IMockMixin CreateExternalMockMixin(IMockMixin mockMixin, IEnumerable<object> mixins)
		{
			throw new NotImplementedException();
		}

		public ProxyTypeInfo CreateClassProxyType(Type classToProxy, MocksRepository repository, MockCreationSettings settings)
		{
			throw new NotImplementedException();
		}

		private static void ThrowNoProxyException(Type type, params Type[] additionalInterfaces)
		{
			//TODO: add additional interfaces to exception message
			throw new MockException(String.Format("No proxy type found for type '{0}'. Add [assembly: MockedType(typeof({0}))] to your test assembly.",
				type.ToString().Replace('+', '.')));
		}
	}
}
