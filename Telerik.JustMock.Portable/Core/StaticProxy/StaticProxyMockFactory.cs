using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telerik.JustMock.Core.Castle.DynamicProxy;

namespace Telerik.JustMock.Core.StaticProxy
{
	internal class StaticProxyMockFactory : IMockFactory
	{
		private static readonly MethodInfo getUninitializedObject =
			Type.GetType("System.Runtime.Serialization.FormatterServices").GetMethod("GetUninitializedObject");

		public bool IsAccessible(Type type)
		{
			return type.IsPublic;
		}

		public object Create(Type type, MocksRepository repository, IMockMixin mockMixinImpl, MockCreationSettings settings, bool createTransparentProxy)
		{
			var baseType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
			RuntimeTypeHandle proxyTypeHandle;
			var key = new ProxySourceRegistry.ProxyKey(
				baseType.TypeHandle, GetAdditionalInterfaceHandles(type, settings.AdditionalMockedInterfaces));
			if (!ProxySourceRegistry.ProxyTypes.TryGetValue(key, out proxyTypeHandle))
			{
				ThrowNoProxyException(baseType, settings.AdditionalMockedInterfaces);
			}

			var interceptor = new DynamicProxyInterceptor(repository);

			var proxyType = Type.GetTypeFromHandle(proxyTypeHandle);
			if (proxyType.IsGenericTypeDefinition)
				proxyType = proxyType.MakeGenericType(type.GetGenericArguments());

			var mockConstructorCall = settings.MockConstructorCall
				&& proxyType.BaseType != typeof(object)
				&& getUninitializedObject != null;

			if (!mockConstructorCall && settings.Args == null)
			{
				settings.Args =
					proxyType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
					.First(ctor => ctor.IsPublic || ctor.IsFamily || ctor.IsFamilyOrAssembly)
					.GetParameters()
					.TakeWhile(p => p.ParameterType != typeof(IInterceptor))
					.Select(p => (p.Attributes & ParameterAttributes.HasDefault) != 0 ? p.DefaultValue : p.ParameterType.GetDefaultValue())
					.ToArray();
			}

			var ctorArgs =
				(settings.Args ?? Enumerable.Empty<object>())
				.Concat(new object[] { interceptor, mockMixinImpl })
				.Concat(settings.Mixins).ToArray();

			if (!mockConstructorCall)
			{
				return ProfilerInterceptor.GuardExternal(() => Activator.CreateInstance(proxyType, ctorArgs));
			}
			else
			{
				try
				{
					var result = getUninitializedObject.Invoke(null, new object[] { proxyType });
					proxyType.GetMethod(".init").Invoke(result, ctorArgs);
					return result;
				}
				catch (InvalidOperationException ioe)
				{
					throw new NotSupportedException("Cannot mock constructor calls on this platform.", ioe);
				}
			}
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
			throw new NotSupportedException();
		}

		public ProxyTypeInfo CreateClassProxyType(Type classToProxy, MocksRepository repository, MockCreationSettings settings, MockMixin mockMixinImpl)
		{
			throw new NotImplementedException("//TODO");
		}

		private static void ThrowNoProxyException(Type type, params Type[] additionalInterfaces)
		{
			//TODO: add additional interfaces to exception message
			throw new MockException(String.Format("No proxy type found for type '{0}'. Add [assembly: MockedType(typeof({0}))] to your test assembly.",
				type.ToString().Replace('+', '.')));
		}

		private static RuntimeTypeHandle[] GetAdditionalInterfaceHandles(Type type, Type[] additionalInterfaces)
		{
			var keyInterfaces = new HashSet<Type>(additionalInterfaces ?? Enumerable.Empty<Type>());
			keyInterfaces.ExceptWith(type.GetInterfaces());

			return keyInterfaces.Count > 0
				? keyInterfaces
				.OrderBy(t => t.FullName)
				.Select(t => t.TypeHandle)
				.ToArray()
				: null;
		}
	}
}
