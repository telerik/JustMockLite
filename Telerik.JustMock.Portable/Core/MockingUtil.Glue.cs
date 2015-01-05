using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Telerik.JustMock.Core
{
	internal static partial class MockingUtil
	{
		public static MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref Object[] args,
			ParameterModifier[] modifiers, CultureInfo culture, string[] names, out Object state)
		{
			throw new NotImplementedException();
		}

		public class InterfaceMapping
		{
			public MethodInfo[] InterfaceMethods;
			public MethodInfo[] TargetMethods;
		}

		public static InterfaceMapping GetInterfaceMap(this Type type, Type interfaceType)
		{
			throw new NotSupportedException();
		}
	}
}
