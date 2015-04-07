using System;
using System.Reflection;

namespace Telerik.JustMock.Core.StaticProxy
{
	internal static class UninitializedObjectFactory
	{
		public static readonly bool IsSupported;

		public static object Create(Type type)
		{
			object result;
			var ex = TryCreate(type, out result);
			if (ex != null)
				throw new NotSupportedException("Cannot mock constructor calls on this platform.", ex);
			return result;
		}

		private static Exception TryCreate(Type type, out object result)
		{
			result = null;
			if (getUninitializedObject == null)
				return new TypeAccessException("GetUninitializedObject");

			try
			{
				result = getUninitializedObject.Invoke(null, new object[] { type });
				return null;
			}
			catch (Exception ex)
			{
				return ex;
			}
		}

		static UninitializedObjectFactory()
		{
			var formatterServices = Type.GetType("System.Runtime.Serialization.FormatterServices");
			if (formatterServices != null)
				getUninitializedObject = formatterServices.GetMethod("GetUninitializedObject");

			object t;
			TryCreate(typeof(object), out t);
			IsSupported = t != null;
		}

		private static readonly MethodInfo getUninitializedObject;
	}
}
