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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Telerik.JustMock.Core.TransparentProxy
{
	internal static class MockingProxy
	{
		public static bool CanCreate(Type type)
		{
			return false;
		}

		public static IMockMixin GetMockMixin(object proxy)
		{
			return null;
		}

		public static object CreateProxy(object wrappedInstance, MocksRepository repository, IMockMixin mockMixin)
		{
			throw new NotSupportedException();
		}

		public static bool CanIntercept(object instance, MethodBase method)
		{
			return false;
		}

		public static object Unwrap(object maybeProxy)
		{
			return maybeProxy;
		}
	}
}
