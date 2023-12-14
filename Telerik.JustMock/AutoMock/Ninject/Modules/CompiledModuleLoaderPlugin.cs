// -------------------------------------------------------------------------------------------------
// <copyright file="CompiledModuleLoaderPlugin.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
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
// -------------------------------------------------------------------------------------------------

namespace Telerik.JustMock.AutoMock.Ninject.Modules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Telerik.JustMock.AutoMock.Ninject.Components;
    using Telerik.JustMock.AutoMock.Ninject.Infrastructure;
    using Telerik.JustMock.AutoMock.Ninject.Infrastructure.Language;

    /// <summary>
    /// Loads modules from compiled assemblies.
    /// </summary>
    public class CompiledModuleLoaderPlugin : NinjectComponent, IModuleLoaderPlugin
    {
        /// <summary>
        /// The file extensions that are supported.
        /// </summary>
        private static readonly string[] Extensions = { ".dll" };

        /// <summary>
        /// The assembly name retriever.
        /// </summary>
        private readonly IAssemblyNameRetriever assemblyNameRetriever;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledModuleLoaderPlugin"/> class.
        /// </summary>
        /// <param name="kernel">The kernel into which modules will be loaded.</param>
        /// <param name="assemblyNameRetriever">The assembly name retriever.</param>
        public CompiledModuleLoaderPlugin(IKernel kernel, IAssemblyNameRetriever assemblyNameRetriever)
        {
            Ensure.ArgumentNotNull(kernel, "kernel");
            Ensure.ArgumentNotNull(assemblyNameRetriever, "assemblyNameRetriever");

            this.Kernel = kernel;
            this.assemblyNameRetriever = assemblyNameRetriever;
        }

        /// <summary>
        /// Gets the kernel into which modules will be loaded.
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        /// Gets the file extensions that the plugin understands how to load.
        /// </summary>
        public IEnumerable<string> SupportedExtensions
        {
            get { return Extensions; }
        }

        /// <summary>
        /// Loads modules from the specified files.
        /// </summary>
        /// <param name="filenames">The names of the files to load modules from.</param>
        public void LoadModules(IEnumerable<string> filenames)
        {
            var assembliesWithModules = this.assemblyNameRetriever.GetAssemblyNames(filenames, asm => asm.HasNinjectModules());
            this.Kernel.Load(assembliesWithModules.Select(asm => Assembly.Load(asm)));
        }
    }
}