using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Telerik.JustMock.Core
{
	internal static partial class MockingUtil
	{
		public static MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref Object[] args,
			ParameterModifier[] modifiers, CultureInfo culture, string[] names, out Object state)
		{
			//TODO: implement entirely
			state = null;
			var theArgs = args;
			var validMatches = match.Where(m => CanBind(m, theArgs)).ToArray();
			switch (validMatches.Length)
			{
				case 1:
					return validMatches[0];
				case 0:
					throw new MissingMethodException();
				default:
					throw new AmbiguousMatchException();
			}
		}

		private static bool CanBind(MethodBase method, Object[] args)
		{
			var parameters = method.GetParameters();
			if (args.Length != parameters.Length)
				return false;

			return true;
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
