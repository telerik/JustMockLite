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
using System.IO;
using System.Linq;
using System.Reflection;

namespace Telerik.JustMock.Plugins
{
    internal class PluginLoadHelper
    {
        private string assemblyRootPath;

        internal PluginLoadHelper(string assemblyRootPath)
        {
            this.assemblyRootPath = assemblyRootPath;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static bool MatchAssemblyNames(AssemblyName first, AssemblyName second)
        {
            // Compare shor names
            if (string.Compare(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) != 0)
                return false;

            // compare Version if exists
            if (first.Version != null && second.Version != null && !first.Version.Equals(second.Version))
                return false;

            // compare culture if exists
            if (first.CultureInfo != null && second.CultureInfo != null && first.CultureInfo.EnglishName != second.CultureInfo.EnglishName)
                return false;

            // compare PKT if exists
            var pktFirst = first.GetPublicKeyToken();
            var pktSecond = second.GetPublicKeyToken();
            if (pktFirst != null && pktSecond != null)
            {
                for (int i = 0; i < pktFirst.Length; ++i)
                {
                    if (pktFirst[i] != pktSecond[i])
                        return false;
                }
            }

            return true;
        }

        private static Assembly LoadAssemblyFromDirectoryPath(string assemblyName, string directoryPath, bool traverseTree = true)
        {
            var requiredAssemblyName = new AssemblyName(assemblyName);
            var requiredAssemblyFileBaseName = assemblyName.Split(',')[0];
            var extensions = new string[] { ".dll", ".exe" };
            foreach (var extension in extensions)
            {
                var filePath = Path.Combine(directoryPath, requiredAssemblyFileBaseName + extension);
                if (File.Exists(filePath))
                {
                    var candidateAssemblyName = AssemblyName.GetAssemblyName(filePath);
                    if (MatchAssemblyNames(candidateAssemblyName, requiredAssemblyName))
                    {
                        var asm = Assembly.LoadFrom(filePath);
                        return asm;
                    }
                }
            }

            if (traverseTree)
            {
                var subdirectories = Directory.EnumerateDirectories(directoryPath).ToList();
                foreach (var subdirectory in subdirectories)
                {
                    var asm = LoadAssemblyFromDirectoryPath(assemblyName, subdirectory);
                    if (asm != null)
                    {
                        return asm;
                    }
                }
            }

            return null;
        }

        internal Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                if (!args.Name.Contains(".resources") && !args.Name.Contains("XmlSerializers"))
                {
                    var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var domainAssembly in domainAssemblies)
                    {
                        if (domainAssembly.FullName == args.Name)
                        {
                            return domainAssembly;
                        }
                    }

                    var asm = LoadAssemblyFromDirectoryPath(args.Name, this.assemblyRootPath, false);
                    if (asm != null)
                    {
                        return asm;
                    }

                }
            }
            catch (Exception /* ex */) { }

            return null;
        }
    }
}
