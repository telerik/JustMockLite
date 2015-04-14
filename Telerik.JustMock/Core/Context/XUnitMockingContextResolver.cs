/*
 JustMock Lite
 Copyright © 2010-2014 Telerik AD

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
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Telerik.JustMock.Core.Context
{
	internal class XUnitMockingContextResolver : HierarchicalTestFrameworkContextResolver
	{
		private const string XunitAssertionExceptionName = "Xunit.Sdk.AssertException, xunit";

		public XUnitMockingContextResolver()
			: base(XunitAssertionExceptionName)
		{
			SetupStandardHierarchicalTestStructure(
				new[] { "Xunit.FactAttribute, xunit" },
				null, null, null,
				FixtureConstuctorSemantics.InstanceConstructorCalledOncePerTest);
		}

		public static bool IsAvailable
		{
			get { return FindType(XunitAssertionExceptionName, false) != null; }
		}

		private Type exceptionType;

		protected override Expression<Func<string, Exception, Exception>> CreateExceptionFactory()
		{
			if (exceptionType == null)
			{
				CreateExceptionType();
			}
			return this.CreateExceptionFactory(this.exceptionType);
		}

		private void CreateExceptionType()
		{
			var baseType = FindType(XunitAssertionExceptionName);
			var typeBuilder = MockingUtil.ModuleBuilder.DefineType(
				"Telerik.JustMock.Xunit.AssertFailedException", TypeAttributes.Public, baseType);

			var signature = new[] { typeof(string), typeof(Exception) };
			var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, signature);
			var il = ctor.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Call, baseType.GetConstructor(
				BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, signature, null));
			il.Emit(OpCodes.Ret);

			this.exceptionType = typeBuilder.CreateType();
		}
	}
}
