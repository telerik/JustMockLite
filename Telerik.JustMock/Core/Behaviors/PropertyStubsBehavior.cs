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

namespace Telerik.JustMock.Core.Behaviors
{
	internal class PropertyStubsBehavior : IBehavior
	{
		private readonly Dictionary<KeyValuePair<PropertyInfo, object>, object> store = new Dictionary<KeyValuePair<PropertyInfo, object>, object>();

		public void Process(Invocation invocation)
		{
			var method = invocation.Method;
			var property = method.GetPropertyFromGetOrSet();
			if (property == null)
				return;

			var args = invocation.Args;
			if (method == property.GetGetMethod(true))
			{
				object value;
				if (this.TryGetValue(property, args.FirstOrDefault(), out value))
					invocation.ReturnValue = value;
			}
			else
			{
				var index = args.Length == 2 ? args[0] : null;
				this.SetValue(property, index, args.Last());
			}
		}

		private void SetValue(PropertyInfo property, object index, object value)
		{
			store[new KeyValuePair<PropertyInfo, object>(property, index)] = value;
		}

		private bool TryGetValue(PropertyInfo property, object index, out object value)
		{
			return store.TryGetValue(new KeyValuePair<PropertyInfo, object>(property, index), out value);
		}
	}
}
