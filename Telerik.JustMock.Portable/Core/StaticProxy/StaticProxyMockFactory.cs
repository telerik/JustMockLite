/*
 JustMock Lite
 Copyright © 2010-2015 Telerik AD

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
using System.Reflection;
using Telerik.JustMock.Core.Castle.DynamicProxy;

namespace Telerik.JustMock.Core.StaticProxy
{
	internal class StaticProxyMockFactory : IMockFactory
	{
		public bool IsAccessible(Type type)
		{
			return type.IsPublic || (type.IsNestedPublic && type.DeclaringType != null && IsAccessible(type.DeclaringType));
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
				&& UninitializedObjectFactory.IsSupported;

			ConstructorInfo proxyCtor = null;
			if (!mockConstructorCall && settings.Args == null)
			{
				proxyCtor = proxyType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
					.First(ctor => ctor.IsPublic || ctor.IsFamily || ctor.IsFamilyOrAssembly);
				settings.Args = proxyCtor.GetParameters()
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
				if (proxyCtor != null)
				{
					return ProfilerInterceptor.GuardExternal(() => proxyCtor.Invoke(ctorArgs));
				}
				else
				{
					return ProfilerInterceptor.GuardExternal(() => Activator.CreateInstance(proxyType, ctorArgs));
				}
			}
			else
			{
				var result = UninitializedObjectFactory.Create(proxyType);
				proxyType.GetMethod(".init").Invoke(result, ctorArgs);
				return result;
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
			return new MockMixin();
		}

		public ProxyTypeInfo CreateClassProxyType(Type classToProxy, MocksRepository repository, MockCreationSettings settings, MockMixin mockMixinImpl)
		{
			throw new NotImplementedException("//TODO");
		}

		private static void ThrowNoProxyException(Type type, params Type[] additionalInterfaces)
		{
			var typeName = type.ToString().Replace('+', '.');

			//TODO: add additional interfaces to exception message
			var message = String.Format("No proxy type found for type '{0}'. Add [assembly: MockedType(typeof({0}))] to your test assembly to explicitly emit a proxy.", typeName);

			if (ProxySourceRegistry.IsTrialWeaver)
				message += "\n\nThe trial version of JustMock for Devices lets you create mocks only for at most 5 types per test assembly.";

			if (ProxySourceRegistry.ProxyTypes.Count == 0 && ProxySourceRegistry.DelegateBackendTypes.Count == 0)
			{
				message = "No proxies available. Telerik.JustMock.Portable cannot be used on its own, but only as part of the \"JustMock for Devices\" package which contains the proxy generator.\n\n"
					+ message;
			}

			throw new MockException(message);
		}

		private static RuntimeTypeHandle[] GetAdditionalInterfaceHandles(Type type, Type[] additionalInterfaces)
		{
			var keyInterfaces = new HashSet<Type>(additionalInterfaces ?? Enumerable.Empty<Type>());
			//keyInterfaces.ExceptWith(type.GetInterfaces());

			return keyInterfaces.Count > 0
				? keyInterfaces
				.OrderBy(t => t.FullName)
				.Select(t => t.TypeHandle)
				.ToArray()
				: null;
		}
	}
}
