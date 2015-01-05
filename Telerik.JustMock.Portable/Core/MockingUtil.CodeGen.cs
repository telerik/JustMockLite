using System;

namespace Telerik.JustMock.Core
{
	internal static partial class MockingUtil
	{
		public static readonly Type[] EmptyTypes = new Type[0];

		public static Action<object[], Delegate> MakeProcCaller(Delegate delg)
		{
			Action<object[], Delegate> caller =
				(args, target) => target.Method.Invoke(target.Target, args);
			return caller;
		}

		public static Func<object[], Delegate, object> MakeFuncCaller(Delegate delg)
		{
			Func<object[], Delegate, object> caller =
				(args, target) => target.Method.Invoke(target.Target, args);
			return caller;
		}
	}
}
