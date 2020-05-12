/*
 JustMock Lite
 Copyright © 2020 Progress Software Corporation

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
using System.Reflection;
using Telerik.JustMock.AutoMock.Ninject;
using Telerik.JustMock.AutoMock.Ninject.Infrastructure.Disposal;
using Telerik.JustMock.AutoMock.Ninject.Modules;
using Telerik.JustMock.AutoMock.Ninject.Parameters;

#if !PORTABLE
namespace Telerik.JustMock.Plugins
{
    internal class PluginsRegistry : DisposableObject
    {
        private Dictionary<Type, INinjectModule> plugins = new Dictionary<Type, INinjectModule>();
        private readonly StandardKernel kernel = new StandardKernel();

        public PluginT Register<PluginT>(string assemblyPath, params IParameter[] parameters)
            where PluginT : INinjectModule
        {
            lock (this)
            {
                var assembly = Assembly.LoadFile(assemblyPath);
                this.kernel.Load(assembly);

                var plugin = this.kernel.TryGet<PluginT>(parameters);
                if (plugin == null)
                {
                    throw new NotSupportedException(string.Format("Plugin type {0} not found, lookup assembly {1}", typeof(PluginT), assembly));
                }

                plugins.Add(typeof(PluginT), plugin);

                return plugin;
            }
        }

        public bool Exists<PluginT>() where PluginT : INinjectModule
        {
            lock (this)
            {
                return plugins.ContainsKey(typeof(PluginT));
            }
        }

        public PluginT Get<PluginT>() where PluginT : INinjectModule
        {
            lock (this)
            {
                return (PluginT)plugins[typeof(PluginT)];
            }
        }

        public PluginT Unregister<PluginT>() where PluginT : INinjectModule
        {
            lock (this)
            {
                INinjectModule plugin = null;
                if (plugins.TryGetValue(typeof(PluginT), out plugin))
                {
                    var pluginDisposable = plugin as IDisposable;
                    if (pluginDisposable != null)
                    {
                        pluginDisposable.Dispose();
                    }
                }
                return (PluginT)plugin;
            }
        }

        public override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing && !this.IsDisposed)
                {
                    foreach (var plugin in plugins.Values)
                    {
                        var pluginDisposable = plugin as IDisposable;
                        if (pluginDisposable != null)
                        {
                            pluginDisposable.Dispose();
                        }
                    }
                }

                base.Dispose(disposing);
            }
        }
    }
}
#endif
