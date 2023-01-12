/*
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
