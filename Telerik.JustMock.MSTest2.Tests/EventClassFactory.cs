using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Telerik.JustMock.MSTest2.Tests
{
	public static class EventClassFactory
	{
		private static ModuleBuilder moduleBuilder;
		private const string ModuleName = "Telerik.JustMock.DemoLib.Dynamic";

		public static Type CreateClassWithEventWithRaiseMethod()
		{
			if (moduleBuilder == null)
			{
				var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(ModuleName), AssemblyBuilderAccess.Run);
				moduleBuilder = assemblyBuilder.DefineDynamicModule(ModuleName);
			}

			var type = moduleBuilder.DefineType(ModuleName + ".Type" + Guid.NewGuid().ToString("N"), TypeAttributes.Public);

			var probe = type.DefineField("Probe", typeof(Action), FieldAttributes.Public);

			var raise = type.DefineMethod("Raise", MethodAttributes.Private);
			var il = raise.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, probe);
			il.Emit(OpCodes.Callvirt, typeof(Action).GetMethod("Invoke"));
			il.Emit(OpCodes.Ret);

			var evt = type.DefineEvent("StuffHappened", EventAttributes.None, typeof(Action));
			evt.SetRaiseMethod(raise);


			return type.CreateType();
		}
	}
}
