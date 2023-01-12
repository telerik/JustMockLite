﻿/*
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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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

		public struct InterfaceMapping
		{
			public readonly MethodInfo[] InterfaceMethods;
			public readonly MethodInfo[] TargetMethods;

			public InterfaceMapping(MethodInfo[] interfaceMethods, MethodInfo[] targetMethods)
			{
				this.InterfaceMethods = interfaceMethods;
				this.TargetMethods = targetMethods;
			}
		}

		public static InterfaceMapping GetInterfaceMap(this Type type, Type interfaceType)
		{
			var mapping = getRuntimeInterfaceMap(getTypeInfo(type), interfaceType);
			return new InterfaceMapping(getInterfaceMethods(mapping), getTargetMethods(mapping));
		}

		private static int? GetDispId(MemberInfo member)
		{
			var attr = Attribute.GetCustomAttribute(member, dispIdAttribute);
			return attr != null ? (int?)getDispId(attr) : null;
		}

		private static readonly Func<Type, object> getTypeInfo;
		private static readonly Func<object, Type, object> getRuntimeInterfaceMap;
		private static readonly Func<object, MethodInfo[]> getInterfaceMethods;
		private static readonly Func<object, MethodInfo[]> getTargetMethods;

		private static readonly Type dispIdAttribute;
		private static readonly Func<Attribute, int> getDispId;

		static MockingUtil()
		{
			var typeInfoType = Type.GetType("System.Reflection.TypeInfo");
			var introspectionType = Type.GetType("System.Reflection.IntrospectionExtensions");

			var typeParam = Expression.Parameter(typeof(Type));
			var getTypeInfoMethod = introspectionType.GetMethod("GetTypeInfo");
			getTypeInfo = Expression.Lambda<Func<Type, object>>(
				Expression.Call(null, getTypeInfoMethod, typeParam),
				typeParam).Compile();

			var interfaceMappingType = Type.GetType("System.Reflection.InterfaceMapping");
			var interfaceMethodsField = interfaceMappingType.GetField("InterfaceMethods");
			var targetMethodsField = interfaceMappingType.GetField("TargetMethods");
			var extensionsType = Type.GetType("System.Reflection.RuntimeReflectionExtensions");
			var getRuntimeInterfaceMapMethod = extensionsType.GetMethod("GetRuntimeInterfaceMap");

			var objectParam = Expression.Parameter(typeof(object));
			var interfaceTypeParam = Expression.Parameter(typeof(Type));

			getRuntimeInterfaceMap = Expression.Lambda<Func<object, Type, object>>(
				Expression.Convert(
					Expression.Call(null, getRuntimeInterfaceMapMethod, Expression.Convert(objectParam, typeInfoType), interfaceTypeParam),
					typeof(object)),
				objectParam, interfaceTypeParam).Compile();

			getInterfaceMethods = Expression.Lambda<Func<object, MethodInfo[]>>(
				Expression.Field(Expression.Convert(objectParam, interfaceMappingType), interfaceMethodsField),
				objectParam).Compile();
			getTargetMethods = Expression.Lambda<Func<object, MethodInfo[]>>(
				Expression.Field(Expression.Convert(objectParam, interfaceMappingType), targetMethodsField),
				objectParam).Compile();

			dispIdAttribute = Type.GetType("System.Runtime.InteropServices.DispIdAttribute");
			var attributeParam = Expression.Parameter(typeof(Attribute));
			getDispId = Expression.Lambda<Func<Attribute, int>>(
				Expression.Property(
					Expression.Convert(attributeParam, dispIdAttribute),
					dispIdAttribute.GetProperty("Value")),
				attributeParam).Compile();
		}
	}
}
