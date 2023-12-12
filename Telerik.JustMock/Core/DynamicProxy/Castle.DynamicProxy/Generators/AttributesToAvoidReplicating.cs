// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Telerik.JustMock.Core.Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	internal static class AttributesToAvoidReplicating
	{
		private static readonly object lockObject = new object();

		private static IList<Type> attributes;

		static AttributesToAvoidReplicating()
		{
			attributes = new List<Type>()
			{
				typeof(System.Runtime.InteropServices.ComImportAttribute),
				typeof(System.Runtime.InteropServices.MarshalAsAttribute),
				typeof(System.Runtime.InteropServices.TypeIdentifierAttribute),
#pragma warning disable SYSLIB0003
				typeof(System.Security.Permissions.SecurityAttribute),
#pragma warning restore SYSLIB0003
			};
		}

		internal static void Add(Type attribute)
		{
			lock (lockObject)
			{
				attributes.Add(attribute);
			}
		}

		internal static void Add<T>()
		{
			Add(typeof(T));
		}

		internal static bool Contains(Type attribute)
		{
			lock (lockObject)
			{
				return attributes.Any(attr => attr.IsAssignableFrom(attribute));
			}
		}

		internal static bool ShouldAvoid(Type attribute)
		{
			lock (lockObject)
			{
				return attributes.Any(attr => attr.IsAssignableFrom(attribute));
			}
		}
	}
}
