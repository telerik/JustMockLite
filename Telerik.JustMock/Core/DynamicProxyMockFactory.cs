/*
 JustMock Lite
 Copyright © 2010-2015,2018 Progress Software Corporation

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
using System.Text;
using Telerik.JustMock.Core.Behaviors;
using Telerik.JustMock.Core.Castle.DynamicProxy;
using Telerik.JustMock.Core.Castle.DynamicProxy.Generators;
using Telerik.JustMock.Core.Expressions;

namespace Telerik.JustMock.Core
{
	internal class DynamicProxyMockFactory : IMockFactory
	{
		private static readonly ProxyGenerator generator;

		static DynamicProxyMockFactory()
		{
#if DEBUG
			generator = new ProxyGenerator(new DefaultProxyBuilder(new ModuleScope(savePhysicalAssembly: true)));
#else
			generator = new ProxyGenerator();
#endif
		}

#if (DEBUG && !SILVERLIGHT && !NETCORE)
		internal static void SaveAssembly()
		{
			generator.ProxyBuilder.ModuleScope.SaveAssembly();
		}
#endif

		public bool IsAccessible(Type type)
		{
            return ProxyUtil.IsAccessibleType(type);
        }

		public object Create(Type type, MocksRepository repository, IMockMixin mockMixinImpl, MockCreationSettings settings, bool createTransparentProxy)
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(mockMixinImpl);
			foreach (var mixin in settings.Mixins)
				options.AddMixinInstance(mixin);

			if (settings.AdditionalProxyTypeAttributes != null)
			{
				foreach (var attributeBuilder in settings.AdditionalProxyTypeAttributes)
				{
					options.AdditionalAttributes.Add(new CustomAttributeInfo(attributeBuilder));
				}
			}

			var interceptor = repository.Interceptor;
#if (SILVERLIGHT)
			options.Hook = new ProxyGenerationHook(false, settings.InterceptorFilter);
#else
			options.Hook = new ProxyGenerationHook(settings.MockConstructorCall, settings.InterceptorFilter);
#endif

			object instance = null;
			Exception proxyFailure = null;

			if (type.IsInterface)
			{
				if (settings.Args != null && settings.Args.Length > 0)
					throw new ArgumentException("Do not supply contructor arguments when mocking an interface or delegate.");
				try
				{
					instance = generator.CreateInterfaceProxyWithoutTarget(type, settings.AdditionalMockedInterfaces, options, interceptor);
				}
				catch (TypeLoadException ex)
				{
					proxyFailure = ex;
				}
				catch (GeneratorException ex)
				{
					proxyFailure = ex;
				}
			}
			else
			{
				try
				{
#if (SILVERLIGHT || NETCORE)
					if (settings.Args == null || settings.Args.Length == 0)
					{
						ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

						if (!constructors.Any(constr => constr.GetParameters().Length == 0))
						{
							var constructorToCall = constructors.FirstOrDefault();
							if (constructorToCall != null)
							{
								var @params = constructorToCall.GetParameters();
								settings.Args = new object[@params.Length];

								for (int i = 0; i < @params.Length; ++i)
								{
									var p = @params[i];
									settings.Args[i] = Convert.IsDBNull(p.DefaultValue)
										? p.ParameterType.GetDefaultValue()
										: p.DefaultValue;
								}
							}
						}
					}
#endif
					instance = generator.CreateClassProxy(type, settings.AdditionalMockedInterfaces, options, settings.Args, interceptor);
				}
				catch (TypeLoadException ex)
				{
					proxyFailure = ex;
				}
				catch (GeneratorException ex)
				{
					proxyFailure = ex;
				}
				catch (InvalidProxyConstructorArgumentsException ex)
				{
					proxyFailure = ex;
					if (!settings.MockConstructorCall)
						throw new MockException(ex.Message);
				}
			}
			if (proxyFailure != null)
			{
				throw new ProxyFailureException(proxyFailure);
			}
			return instance;
		}

		public Type CreateDelegateBackend(Type delegateType)
		{
			var moduleScope = generator.ProxyBuilder.ModuleScope;
			var moduleBuilder = moduleScope.ObtainDynamicModuleWithStrongName();

			var targetIntfName =
				"Castle.Proxies.Delegates." +
				delegateType.ToString()
				.Replace('.', '_')
				.Replace(',', '`')
				.Replace("+", "__")
				.Replace("[", "``")
				.Replace("]", "``");

			var typeName = moduleScope.NamingScope.GetUniqueName(targetIntfName);
			var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Interface);

			var delegateInvoke = delegateType.GetMethod("Invoke");
			typeBuilder.DefineMethod("Invoke", MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual,
				delegateInvoke.ReturnType, delegateInvoke.GetParameters().Select(p => p.ParameterType).ToArray());

			return typeBuilder.CreateType();
		}

		public IMockMixin CreateExternalMockMixin(IMockMixin mockMixin, IEnumerable<object> mixins)
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(mockMixin);
			foreach (var mixin in mixins)
				options.AddMixinInstance(mixin);

			var compoundMockMixin = (IMockMixin)generator.CreateClassProxy(typeof(MocksRepository.ExternalMockMixin), options);
			return compoundMockMixin;
		}

		public ProxyTypeInfo CreateClassProxyType(Type classToProxy, MocksRepository repository, MockCreationSettings settings, MockMixin mockMixinImpl)
		{
			var pgo = CreateProxyGenerationOptions(classToProxy, settings, mockMixinImpl);
			var typeInfo = new ProxyTypeInfo
			{
				ProxyType = generator.ProxyBuilder.CreateClassProxyType(classToProxy, Type.EmptyTypes, pgo)
			};
			typeInfo.Mixins.Add(typeof(IInterceptor), repository.Interceptor);
			foreach (var mixin in pgo.MixinData.MixinInterfaces)
			{
				typeInfo.Mixins.Add(mixin, pgo.MixinData.GetMixinInstance(mixin));
			}
			return typeInfo;
		}

		private ProxyGenerationOptions CreateProxyGenerationOptions(Type type, MockCreationSettings settings, MockMixin mockMixinImpl = null)
		{
			var options = new ProxyGenerationOptions();
			if (mockMixinImpl != null)
				options.AddMixinInstance(mockMixinImpl);
			foreach (var mixin in settings.Mixins)
				options.AddMixinInstance(mixin);

			if (settings.AdditionalProxyTypeAttributes != null)
			{
				foreach (var attributeBuilder in settings.AdditionalProxyTypeAttributes)
				{
					options.AdditionalAttributes.Add(new CustomAttributeInfo(attributeBuilder));
				}
			}

			return options;
		}

		private class ProxyGenerationHook : IProxyGenerationHook, IConstructorGenerationHook
		{
			private readonly bool myMockConstructors;
			private readonly Expression<Predicate<MethodInfo>> myInterceptorFilter;
			private readonly Predicate<MethodInfo> myInterceptorFilterImpl;

			public ProxyGenerationHook(bool mockConstructors, Expression<Predicate<MethodInfo>> interceptorFilter)
			{
				myMockConstructors = mockConstructors;
				if (interceptorFilter != null)
				{
					myInterceptorFilter = interceptorFilter;
					myInterceptorFilterImpl = myInterceptorFilter.Compile();
				}
			}

			public void MethodsInspected()
			{
			}

			public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
			{
			}

            public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
            {
                if (Attribute.IsDefined(methodInfo.DeclaringType, typeof(MixinAttribute)))
                {
                    return false;
                }

                bool profilerCannotIntercept = methodInfo.IsAbstract || methodInfo.IsExtern() || !ProfilerInterceptor.TypeSupportsInstrumentation(methodInfo.DeclaringType);

                if (ProfilerInterceptor.IsProfilerAttached && !profilerCannotIntercept)
                {
                    bool isDefaultMethodImplementation = !methodInfo.IsAbstract && methodInfo.DeclaringType.IsInterface;
                    if (type == methodInfo.DeclaringType && !isDefaultMethodImplementation)
                    {
                        return false;
                    }
                }

				return myInterceptorFilterImpl != null ? myInterceptorFilterImpl(methodInfo) : true;
			}

			public ProxyConstructorImplementation GetConstructorImplementation(ConstructorInfo constructorInfo, ConstructorImplementationAnalysis analysis)
			{
				return myMockConstructors ? ProxyConstructorImplementation.DoNotCallBase
					: analysis.IsBaseVisible ? ProxyConstructorImplementation.CallBase
					: ProxyConstructorImplementation.SkipConstructor;
			}

			public ProxyConstructorImplementation DefaultConstructorImplementation
			{
				get
				{
#if (SILVERLIGHT || NETCORE)
					return ProxyConstructorImplementation.SkipConstructor;
#else
					return myMockConstructors ? ProxyConstructorImplementation.DoNotCallBase : ProxyConstructorImplementation.SkipConstructor;
#endif
				}
			}

			#region Equality members

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;

				var other = (ProxyGenerationHook)obj;
				return this.myMockConstructors == other.myMockConstructors
					&& ((this.myInterceptorFilter == null && other.myInterceptorFilter == null)
						|| ExpressionComparer.AreEqual(this.myInterceptorFilter, other.myInterceptorFilter));
			}

			public override int GetHashCode()
			{
				return this.myMockConstructors.GetHashCode();
			}

			#endregion
		}
	}
}
