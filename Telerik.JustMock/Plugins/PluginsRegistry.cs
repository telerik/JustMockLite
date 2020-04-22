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
using Telerik.JustMock.AutoMock.Ninject.Parameters;

#if !PORTABLE
namespace Telerik.JustMock.Plugins
{
    internal class PluginsRegistry
    {
        private Dictionary<Type, object> plugins = new Dictionary<Type, object>();

        public void Register<PluginT>(string assemblyPath, params IParameter[] parameters)
        {
            var kernel = new StandardKernel();
            var assembly = Assembly.LoadFile(assemblyPath);
            kernel.Load(assembly);
            var plugin = kernel.TryGet<PluginT>(parameters);
            if (plugin == null)
            {
                throw new NotSupportedException(string.Format("Plugin type {0} not found, lookup assembly {1}", typeof(PluginT), assembly));
            }
            plugins.Add(typeof(PluginT), plugin);
        }

        public bool Exists<PluginT>()
        {
            return plugins.ContainsKey(typeof(PluginT));
        }

        public PluginT Get<PluginT>()
        {
            return (PluginT)plugins[typeof(PluginT)];
        }
    }
}
#endif
