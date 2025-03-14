/*
 JustMock Lite
 Copyright © 2025 Progress Software Corporation

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
using System.Linq;
using System.Reflection;

namespace Telerik.JustMock.Core.Licensing
{
#nullable enable
    public static class LicenseManager
    {
        private const string DefaultProductName = "Telerik JustMock Lite";
        private const string DefaultProductCode = "JM";
        private const string DefaultLicenseType = "free";
        private static readonly DateTime DefaultLicenseDate = DateTime.Now;

        private static string? licenseProductName;
        private static DateTime licenseDate; 

        public static bool IsLicenseValid => true;

        public static bool IsLicenseKeyFileFound => false;

        public static bool IsLicenseKeyFileValid => false;

        public static string? LicenseProductName
        {
            get
            {
                if (licenseProductName == null)
                {
                    var assemblyProductAttribute = typeof(LicenseManager).Assembly.GetCustomAttribute(typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
                    licenseProductName =
                        assemblyProductAttribute == null
                        ?
                            DefaultProductName
                            :
                            assemblyProductAttribute.Product;
                }
                return licenseProductName;
            }
        }

        public static string LicenseProductCode => DefaultProductCode;

        public static string? LicenseProductVersion => typeof(LicenseManager).Assembly.GetName().Version.ToString();

        public static DateTime? LicenseExpiration => null;

        public static DateTime LicenseDate
        {
            get
            {
                if (licenseDate == null)
                {
                    licenseDate = DefaultLicenseDate;
                    var assemblyMetadataAttributes = typeof(LicenseManager).Assembly.GetCustomAttributes().OfType<AssemblyMetadataAttribute>();
                    foreach (var assemblyMetadataAttribute in assemblyMetadataAttributes)
                    {
                        if (assemblyMetadataAttribute.Key == "BuildDate")
                        {
                            licenseDate = DateTime.Parse(assemblyMetadataAttribute.Value);
                            break;
                        }
                    }
                }   
                return licenseDate;
            }
        }

        public static string? LicenseType => DefaultLicenseType;

        public static string Message = String.Empty;
    }
#nullable disable
}
