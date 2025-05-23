// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Telerik.JustMock.Core.Castle.DynamicProxy.Generators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
#if FEATURE_SERIALIZATION
    using System.Xml.Serialization;
#endif

    using Telerik.JustMock.Core.Castle.DynamicProxy.Contributors;
    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators.Emitters;
    using Telerik.JustMock.Core.Castle.DynamicProxy.Generators.Emitters.SimpleAST;
    using Telerik.JustMock.Core.Castle.DynamicProxy.Internal;

    internal abstract class BaseInterfaceProxyGenerator : BaseProxyGenerator
    {
        protected readonly Type proxyTargetType;

        protected FieldReference targetField;

        protected BaseInterfaceProxyGenerator(ModuleScope scope, Type targetType, Type[] interfaces,
                                              Type proxyTargetType, ProxyGenerationOptions options)
            : base(scope, targetType, interfaces, options)
        {
            CheckNotGenericTypeDefinition(proxyTargetType, nameof(proxyTargetType));
            EnsureValidBaseType(ProxyGenerationOptions.BaseTypeForInterfaceProxy);

            this.proxyTargetType = proxyTargetType;
        }

        protected abstract bool AllowChangeTarget { get; }

        protected abstract string GeneratorType { get; }

        protected abstract CompositeTypeContributor GetProxyTargetContributor(Type proxyTargetType, INamingScope namingScope);

        protected abstract ProxyTargetAccessorContributor GetProxyTargetAccessorContributor();

        protected abstract void AddMappingForAdditionalInterfaces(CompositeTypeContributor contributor, Type[] proxiedInterfaces,
                                                                  IDictionary<Type, ITypeContributor> typeImplementerMapping,
                                                                  ICollection<Type> targetInterfaces);

        protected virtual ITypeContributor AddMappingForTargetType(IDictionary<Type, ITypeContributor> typeImplementerMapping,
                                                                   Type proxyTargetType, ICollection<Type> targetInterfaces,
                                                                   INamingScope namingScope)
        {
            var contributor = GetProxyTargetContributor(proxyTargetType, namingScope);
            var proxiedInterfaces = targetType.GetAllInterfaces();
            foreach (var @interface in proxiedInterfaces)
            {
                contributor.AddInterfaceToProxy(@interface);
                AddMappingNoCheck(@interface, contributor, typeImplementerMapping);
            }

            AddMappingForAdditionalInterfaces(contributor, proxiedInterfaces, typeImplementerMapping, targetInterfaces);
            return contributor;
        }

#if FEATURE_SERIALIZATION
        protected override void CreateTypeAttributes(ClassEmitter emitter)
        {
            base.CreateTypeAttributes(emitter);
            emitter.DefineCustomAttribute<SerializableAttribute>();
        }
#endif

        protected override CacheKey GetCacheKey()
        {
            return new CacheKey(proxyTargetType, targetType, interfaces, ProxyGenerationOptions);
        }

        protected override Type GenerateType(string typeName, INamingScope namingScope)
        {
            IEnumerable<ITypeContributor> contributors;
            var allInterfaces = GetTypeImplementerMapping(proxyTargetType, out contributors, namingScope);

            var model = new MetaType();
            // Collect methods
            foreach (var contributor in contributors)
            {
                contributor.CollectElementsToProxy(ProxyGenerationOptions.Hook, model);
            }

            ProxyGenerationOptions.Hook.MethodsInspected();

            ClassEmitter emitter;
            FieldReference interceptorsField;
            var baseType = Init(typeName, out emitter, proxyTargetType, out interceptorsField, allInterfaces);

            // Constructor

            var cctor = GenerateStaticConstructor(emitter);
            var ctorArguments = new List<FieldReference>();

            foreach (var contributor in contributors)
            {
                contributor.Generate(emitter);

                // TODO: redo it
                if (contributor is MixinContributor)
                {
                    ctorArguments.AddRange((contributor as MixinContributor).Fields);
                }
            }

            ctorArguments.Add(interceptorsField);
            ctorArguments.Add(targetField);
            var selector = emitter.GetField("__selector");
            if (selector != null)
            {
                ctorArguments.Add(selector);
            }

            GenerateConstructors(emitter, baseType, ctorArguments.ToArray());

            // Complete type initializer code body
            CompleteInitCacheMethod(cctor.CodeBuilder);

            // non-inheritable attributes from proxied type
            var nonInheritableAttributesContributor = new NonInheritableAttributesContributor(targetType);
            nonInheritableAttributesContributor.Generate(emitter);

            // Crosses fingers and build type
            var generatedType = emitter.BuildType();

            InitializeStaticFields(generatedType);
            return generatedType;
        }

        protected virtual InterfaceProxyWithoutTargetContributor GetContributorForAdditionalInterfaces(
            INamingScope namingScope)
        {
            return new InterfaceProxyWithoutTargetContributor(namingScope, (c, m) => NullExpression.Instance) { Logger = Logger };
        }

        protected virtual IEnumerable<Type> GetTypeImplementerMapping(Type proxyTargetType,
                                                                      out IEnumerable<ITypeContributor> contributors,
                                                                      INamingScope namingScope)
        {
            var contributorsList = new List<ITypeContributor>(capacity: 5);
            var targetInterfaces = proxyTargetType.GetAllInterfaces();
            var typeImplementerMapping = new Dictionary<Type, ITypeContributor>();

            // Order of interface precedence:
            // 1. first target
            var targetContributor = AddMappingForTargetType(typeImplementerMapping, proxyTargetType, targetInterfaces, namingScope);
            contributorsList.Add(targetContributor);

            // 2. then mixins
            if (ProxyGenerationOptions.HasMixins)
            {
                var mixinContributor = new MixinContributor(namingScope, AllowChangeTarget) { Logger = Logger };
                contributorsList.Add(mixinContributor);

                foreach (var mixinInterface in ProxyGenerationOptions.MixinData.MixinInterfaces)
                {
                    if (targetInterfaces.Contains(mixinInterface))
                    {
                        // OK, so the target implements this interface. We now do one of two things:
                        if (interfaces.Contains(mixinInterface))
                        {
                            // we intercept the interface, and forward calls to the target type
                            AddMapping(mixinInterface, targetContributor, typeImplementerMapping);
                        }
                        // we do not intercept the interface
                        mixinContributor.AddEmptyInterface(mixinInterface);
                    }
                    else
                    {
                        if (!typeImplementerMapping.ContainsKey(mixinInterface))
                        {
                            mixinContributor.AddInterfaceToProxy(mixinInterface);
                            typeImplementerMapping.Add(mixinInterface, mixinContributor);
                        }
                    }
                }
            }

            // 3. then additional interfaces
            if (interfaces.Length > 0)
            {
                var additionalInterfacesContributor = GetContributorForAdditionalInterfaces(namingScope);
                contributorsList.Add(additionalInterfacesContributor);

                foreach (var @interface in interfaces)
                {
                    if (typeImplementerMapping.ContainsKey(@interface))
                    {
                        continue;
                    }
                    if (ProxyGenerationOptions.MixinData.ContainsMixin(@interface))
                    {
                        continue;
                    }

                    additionalInterfacesContributor.AddInterfaceToProxy(@interface);
                    AddMappingNoCheck(@interface, additionalInterfacesContributor, typeImplementerMapping);
                }
            }

            // 4. plus special interfaces

#if FEATURE_SERIALIZATION
            var serializableContributor = new InterfaceProxySerializableContributor(targetType, GeneratorType, interfaces);
            contributorsList.Add(serializableContributor);
            AddMappingForISerializable(typeImplementerMapping, serializableContributor);
#endif

            var proxyTargetAccessorContributor = GetProxyTargetAccessorContributor();
            contributorsList.Add(proxyTargetAccessorContributor);
            try
            {
                AddMappingNoCheck(typeof(IProxyTargetAccessor), proxyTargetAccessorContributor, typeImplementerMapping);
            }
            catch (ArgumentException)
            {
                HandleExplicitlyPassedProxyTargetAccessor(targetInterfaces);
            }

            contributors = contributorsList;
            return typeImplementerMapping.Keys;
        }

        protected virtual Type Init(string typeName, out ClassEmitter emitter, Type proxyTargetType,
                                    out FieldReference interceptorsField, IEnumerable<Type> allInterfaces)
        {
            var baseType = ProxyGenerationOptions.BaseTypeForInterfaceProxy;

            emitter = BuildClassEmitter(typeName, baseType, allInterfaces);

            CreateFields(emitter, proxyTargetType);
            CreateTypeAttributes(emitter);

            interceptorsField = emitter.GetField("__interceptors");
            return baseType;
        }

        private void CreateFields(ClassEmitter emitter, Type proxyTargetType)
        {
            base.CreateFields(emitter);
            targetField = emitter.CreateField("__target", proxyTargetType);
#if FEATURE_SERIALIZATION
            emitter.DefineCustomAttributeFor<XmlIgnoreAttribute>(targetField);
#endif
        }

        private void EnsureValidBaseType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentException(
                    "Base type for proxy is null reference. Please set it to System.Object or some other valid type.");
            }

            if (!type.IsClass)
            {
                ThrowInvalidBaseType(type, "it is not a class type");
            }

            if (type.IsSealed)
            {
                ThrowInvalidBaseType(type, "it is sealed");
            }

            var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                  null, Type.EmptyTypes, null);

            if (constructor == null || constructor.IsPrivate)
            {
                ThrowInvalidBaseType(type, "it does not have accessible parameterless constructor");
            }
        }

        private void ThrowInvalidBaseType(Type type, string doesNotHaveAccessibleParameterlessConstructor)
        {
            var format =
                "Type {0} is not valid base type for interface proxy, because {1}. Only a non-sealed class with non-private default constructor can be used as base type for interface proxy. Please use some other valid type.";
            throw new ArgumentException(string.Format(format, type, doesNotHaveAccessibleParameterlessConstructor));
        }
    }
}