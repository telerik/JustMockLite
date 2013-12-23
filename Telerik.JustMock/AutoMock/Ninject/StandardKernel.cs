//-------------------------------------------------------------------------------
// <copyright file="StandardKernel.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2009, Enkari, Ltd.
//   Copyright (c) 2009-2011 Ninject Project Contributors
//   Authors: Nate Kohari (nate@enkari.com)
//            Remo Gloor (remo.gloor@gmail.com)
//           
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Telerik.JustMock.AutoMock.Ninject
{
    using System;
    using Telerik.JustMock.AutoMock.Ninject.Activation;
    using Telerik.JustMock.AutoMock.Ninject.Activation.Caching;
    using Telerik.JustMock.AutoMock.Ninject.Activation.Strategies;
    using Telerik.JustMock.AutoMock.Ninject.Components;
    using Telerik.JustMock.AutoMock.Ninject.Injection;
    using Telerik.JustMock.AutoMock.Ninject.Modules;
    using Telerik.JustMock.AutoMock.Ninject.Planning;
    using Telerik.JustMock.AutoMock.Ninject.Planning.Bindings.Resolvers;
    using Telerik.JustMock.AutoMock.Ninject.Planning.Strategies;
    using Telerik.JustMock.AutoMock.Ninject.Selection;
    using Telerik.JustMock.AutoMock.Ninject.Selection.Heuristics;

    /// <summary>
    /// The standard implementation of a kernel.
    /// </summary>
    public class StandardKernel : KernelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StandardKernel"/> class.
        /// </summary>
        /// <param name="modules">The modules to load into the kernel.</param>
        public StandardKernel(params INinjectModule[] modules) : base(modules)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardKernel"/> class.
        /// </summary>
        /// <param name="settings">The configuration to use.</param>
        /// <param name="modules">The modules to load into the kernel.</param>
        public StandardKernel(INinjectSettings settings, params INinjectModule[] modules) : base(settings, modules)
        {
        }

        /// <summary>
        /// Gets the kernel.
        /// </summary>
        /// <value>The kernel.</value>
        protected override IKernel KernelInstance
        {
            get
            {
                return this;
            }
        }

        protected virtual bool ShouldAddComponent(Type component, Type implementation)
        {
            return true;
        }

        private void AddComponent<TComponent, TImplementation>()
            where TComponent : INinjectComponent
            where TImplementation : TComponent, INinjectComponent
        {
            if (ShouldAddComponent(typeof(TComponent), typeof(TImplementation)))
            {
                Components.Add<TComponent, TImplementation>();
            }
        }
        
        /// <summary>
        /// Adds components to the kernel during startup.
        /// </summary>
        protected override void AddComponents()
        {
            AddComponent<IPlanner, Planner>();
            AddComponent<IPlanningStrategy, ConstructorReflectionStrategy>();
            AddComponent<IPlanningStrategy, PropertyReflectionStrategy>();
            AddComponent<IPlanningStrategy, MethodReflectionStrategy>();

            AddComponent<ISelector, Selector>();
            AddComponent<IConstructorScorer, StandardConstructorScorer>();
            AddComponent<IInjectionHeuristic, StandardInjectionHeuristic>();

            AddComponent<IPipeline, Pipeline>();
            if (!Settings.ActivationCacheDisabled)
            {
                AddComponent<IActivationStrategy, ActivationCacheStrategy>();
            }

            AddComponent<IActivationStrategy, PropertyInjectionStrategy>();
            AddComponent<IActivationStrategy, MethodInjectionStrategy>();
            AddComponent<IActivationStrategy, InitializableStrategy>();
            AddComponent<IActivationStrategy, StartableStrategy>();
            AddComponent<IActivationStrategy, BindingActionStrategy>();
            AddComponent<IActivationStrategy, DisposableStrategy>();

            AddComponent<IBindingResolver, StandardBindingResolver>();
            AddComponent<IBindingResolver, OpenGenericBindingResolver>();

            AddComponent<IMissingBindingResolver, DefaultValueBindingResolver>();
            AddComponent<IMissingBindingResolver, SelfBindingResolver>();

#if !NO_LCG
            if (!Settings.UseReflectionBasedInjection)
            {
                AddComponent<IInjectorFactory, DynamicMethodInjectorFactory>();
            }
            else
#endif
            {
                AddComponent<IInjectorFactory, ReflectionInjectorFactory>();
            }

            AddComponent<ICache, Cache>();
            AddComponent<IActivationCache, ActivationCache>();
            AddComponent<ICachePruner, GarbageCollectionCachePruner>();

            #if !NO_ASSEMBLY_SCANNING
            AddComponent<IModuleLoader, ModuleLoader>();
            AddComponent<IModuleLoaderPlugin, CompiledModuleLoaderPlugin>();
            AddComponent<IAssemblyNameRetriever, AssemblyNameRetriever>();
            #endif
        }
    }
}
