/*
 JustMock Lite
 Copyright © 2010-2015 Telerik EAD

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
using System.Reflection;

namespace Telerik.JustMock.Core.Context
{
    internal abstract class NUnitMockingContextResolver : HierarchicalTestFrameworkContextResolver
    {
        private const string NunitAssertionExceptionName = "NUnit.Framework.AssertionException";

        protected const string AssemblyName = "nunit.framework";
        protected const string TestAttributeName = "NUnit.Framework.TestAttribute";
        protected const string TestCaseAttributeName = "NUnit.Framework.TestCaseAttribute";
        protected const string TestCaseSourceAttributeName = "NUnit.Framework.TestCaseSourceAttribute";
        protected const string SetUpAttributeAttributeName = "NUnit.Framework.SetUpAttribute";
        protected const string TearDownAttributeName = "NUnit.Framework.TearDownAttribute";
        protected const string TestFixtureSetUpAttributeName = "NUnit.Framework.TestFixtureSetUpAttribute";
        protected const string TestFixtureTearDownAttributeName = "NUnit.Framework.TestFixtureTearDownAttribute";
        protected const string OneTimeSetUpAttributeName = "NUnit.Framework.OneTimeSetUpAttribute";
        protected const string OneTimeTearDownAttributeName = "NUnit.Framework.OneTimeTearDownAttribute";

        public NUnitMockingContextResolver()
            : base(GetAttributeFullName(NunitAssertionExceptionName))
        {
        }

        protected static string GetAttributeFullName(string attributeName)
        {
            string fullName = attributeName + ", " + AssemblyName;

            return fullName;
        }
    }

    internal class NUnit2xMockingContextResolver : NUnitMockingContextResolver
    {
        private const string ExpectedExceptionAttributeName = "NUnit.Framework.ExpectedExceptionAttribute, " + AssemblyName;

        public NUnit2xMockingContextResolver()
            : base()
        {
            this.SetupStandardHierarchicalTestStructure(
                new[] { GetAttributeFullName(TestAttributeName), GetAttributeFullName(TestCaseAttributeName), GetAttributeFullName(TestCaseSourceAttributeName) },
                new[] { GetAttributeFullName(SetUpAttributeAttributeName), GetAttributeFullName(TearDownAttributeName) },
                new[] { GetAttributeFullName(TestFixtureTearDownAttributeName), GetAttributeFullName(TestFixtureTearDownAttributeName) },
                null,
                FixtureConstuctorSemantics.InstanceConstructorCalledOncePerFixture);
        }

        public static bool IsAvailable
        {
            get { return FindType(ExpectedExceptionAttributeName, false) != null; }
        }
    }

    internal class NUnit3xMockingContextResolver : NUnitMockingContextResolver
    {
        private static readonly Version MaxVersion = new Version(3, 8);

        public NUnit3xMockingContextResolver()
            : base()
        {
            this.SetupStandardHierarchicalTestStructure(
                new[] {
                    GetAttributeFullName(TestAttributeName), GetAttributeFullName(TestCaseAttributeName),
                    GetAttributeFullName(TestCaseSourceAttributeName)
                },
                new[] {
                    GetAttributeFullName(SetUpAttributeAttributeName), GetAttributeFullName(TearDownAttributeName),
                    GetAttributeFullName(OneTimeSetUpAttributeName), GetAttributeFullName(OneTimeTearDownAttributeName)
                },
                new[] { GetAttributeFullName(TestFixtureSetUpAttributeName), GetAttributeFullName(TestFixtureTearDownAttributeName) },
                null,
                FixtureConstuctorSemantics.InstanceConstructorCalledOncePerFixture);
        }

        public static bool IsAvailable
        {
            get
            {
                Assembly assembly = GetAssembly(AssemblyName);
                if (assembly == null)
                {
                    return false;
                }

                Version version = assembly.GetName().Version;
                if (version > MaxVersion)
                {
                    return false;
                }

                bool returnValue = FindType(GetAttributeFullName(OneTimeSetUpAttributeName), false) != null;

                return returnValue;
            }
        }
    }

    internal class NUnit3_8_xMockingContextResolver : NUnitMockingContextResolver
    {
        private static readonly Version MinVersion = new Version(3, 8);

        public NUnit3_8_xMockingContextResolver()
            : base()
        {
            this.SetupStandardHierarchicalTestStructure(
                new[] {
                    GetAttributeFullName(TestAttributeName), GetAttributeFullName(TestCaseAttributeName),
                    GetAttributeFullName(TestCaseSourceAttributeName)
                },
                new[] {
                    GetAttributeFullName(SetUpAttributeAttributeName), GetAttributeFullName(TearDownAttributeName),
                    GetAttributeFullName(OneTimeSetUpAttributeName), GetAttributeFullName(OneTimeTearDownAttributeName)
                },
                null,
                null,
                FixtureConstuctorSemantics.InstanceConstructorCalledOncePerFixture);
        }

        public static bool IsAvailable
        {
            get
            {
                Assembly assembly = GetAssembly(AssemblyName);
                if (assembly == null)
                {
                    return false;
                }

                Version version = assembly.GetName().Version;
                if (version <= MinVersion)
                {
                    return false;
                }

                bool returnValue = FindType(GetAttributeFullName(OneTimeSetUpAttributeName), false) != null;

                return returnValue;
            }
        }
    }
}
