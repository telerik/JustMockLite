﻿/*
 JustMock Lite
 Copyright © 2010-2023 Progress Software Corporation

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

namespace Telerik.JustMock.Core.Context
{
	internal class XamarinAndroidNUnitContextResolver : HierarchicalTestFrameworkContextResolver
	{
		private const string NunitAssertionExceptionName = "NUnit.Framework.AssertionException, Xamarin.Android.NUnitLite";

		public XamarinAndroidNUnitContextResolver()
			: base(NunitAssertionExceptionName)
		{
			this.SetupStandardHierarchicalTestStructure(
				new[] { "NUnit.Framework.TestAttribute, Xamarin.Android.NUnitLite" },
				new[] { "NUnit.Framework.SetUpAttribute, Xamarin.Android.NUnitLite", "NUnit.Framework.TearDownAttribute, Xamarin.Android.NUnitLite" },
				new[] { "NUnit.Framework.TestFixtureSetUpAttribute, Xamarin.Android.NUnitLite", "NUnit.Framework.TestFixtureTearDownAttribute, Xamarin.Android.NUnitLite" },
				null,
				FixtureConstuctorSemantics.InstanceConstructorCalledOncePerFixture);
		}

		public static bool IsAvailable
		{
			get { return Type.GetType(NunitAssertionExceptionName) != null; }
		}
	}
}
