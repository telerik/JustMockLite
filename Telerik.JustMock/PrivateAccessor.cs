/*
 JustMock Lite
 Copyright © 2010-2014 Telerik AD

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
using System.Linq;
using System.Reflection;
using Telerik.JustMock.Core;

#if !SILVERLIGHT
using System.Security;
using System.Security.Permissions;
#endif

namespace Telerik.JustMock
{
	/// <summary>
	/// Gives access to the non-public members of a type or instance. This class provides functionality similar to the
	/// one that exists in regular reflection classes with the added benefit that it can bypass essential security checks related
	/// to accessing non-public members through reflection.
	/// When the profiler is enabled, PrivateAccessor acquires additional power:
	/// - It can even be used to access all kinds of non-public members in Silverlight (and when running in partial trust in general).
	/// - All calls made through PrivateAccessor are always made with full trust (unrestricted) permissions.
	/// </summary>
	public sealed class PrivateAccessor
	{
		private object instance;
		private Type type;

		/// <summary>
		/// Creates a new <see cref="PrivateAccessor"/> wrapping the given instance. Can be used to access both instance and static members.
		/// </summary>
		/// <param name="instance">The instance to wrap.</param>
		public PrivateAccessor(object instance)
			: this(instance, instance.GetType())
		{}

		/// <summary>
		/// Creates a new <see cref="PrivateAccessor"/> wrapping the given type. Can be used to access the static members of a type.
		/// </summary>
		/// <param name="type">Targeted type.</param>
		/// <returns>PrivateAccessor type.</returns>
		public static PrivateAccessor ForType(Type type)
		{
			return ProfilerInterceptor.GuardInternal(() => new PrivateAccessor(null, type));
		}

		private PrivateAccessor(object instance, Type type)
		{
			this.instance = instance;
			this.type = type;
		}

		/// <summary>
		/// Calls the specified method by name.
		/// </summary>
		/// <param name="name">The name of the method to call.</param>
		/// <param name="args">Arguments to pass to the method.</param>
		/// <returns>The value returned by the specified method.</returns>
		public object CallMethod(string name, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					args = args ?? MockingUtil.NoObjects;
					var candidates = type.GetAllMethods()
						.Where(m => m.Name == name && CanCall(m))
						.ToArray();
					object state;
					var method = Type.DefaultBinder.BindToMethod(MockingUtil.AllMembers,
						candidates, ref args, null, null, null, out state);

					return CallInvoke(method, args);
				});
		}

		/// <summary>
		/// Calls the specified method.
		/// </summary>
		/// <param name="method">The method to call.</param>
		/// <param name="args">Arguments to pass to the method.</param>
		/// <returns>Return value of the method.</returns>
		public object CallMethod(MethodBase method, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() => CallInvoke(method, args));
		}

		/// <summary>
		/// Gets the value returned by the indexer (<code>this[T index]</code> in C#) for the specified index value.
		/// </summary>
		/// <param name="index">The index argument to pass to the indexer.</param>
		/// <returns>The value returned by the indexer.</returns>
		public object GetIndex(object index)
		{
			return ProfilerInterceptor.GuardInternal(() => GetProperty("Item", index));
		}

		/// <summary>
		/// Gets the value of a property by name.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="indexArgs">Optional index arguments if the property has any.</param>
		/// <returns>The value of the property.</returns>
		public object GetProperty(string name, params object[] indexArgs)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var prop = ResolveProperty(name, indexArgs);
					return ProfilerInterceptor.GuardExternal(() => SecuredReflectionMethods.GetProperty(prop, this.instance, indexArgs));
				});
		}

		/// <summary>
		/// Sets a value to the indexer (<code>this[T index]</code> in C#) for the specified index value.
		/// </summary>
		/// <param name="index">The index argument to pass to the indexer.</param>
		/// <param name="value">The value to give to the indexer.</param>
		public void SetIndex(object index, object value)
		{
			ProfilerInterceptor.GuardInternal(() => SetProperty("Item", value, index));
		}

		/// <summary>
		/// Sets the value of a property by name.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="value">The value to set to the property.</param>
		/// <param name="indexArgs">Optional index arguments if the property has any.</param>
		public void SetProperty(string name, object value, params object[] indexArgs)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var prop = ResolveProperty(name, indexArgs);
					ProfilerInterceptor.GuardExternal(() => SecuredReflectionMethods.SetProperty(prop, this.instance, value, indexArgs));
				});
		}

		/// <summary>
		/// Gets the value of a field.
		/// </summary>
		/// <param name="name">The name of the field.</param>
		/// <returns>The value of the field</returns>
		public object GetField(string name)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var field = ResolveField(name);
					CheckMemberInfo("field", name, field);
					return SecuredReflectionMethods.GetField(field, this.instance);
				});
		}

		/// <summary>
		/// Sets the value of a field.
		/// </summary>
		/// <param name="name">The name of the field.</param>
		/// <param name="value">The new value of the field.</param>
		public void SetField(string name, object value)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var field = ResolveField(name);
					CheckMemberInfo("field", name, field);
					SecuredReflectionMethods.SetField(field, this.instance, value);
				});
		}

		private void CheckMemberInfo(string kind, string name, MemberInfo mi)
		{
			if (mi == null)
				throw new MissingMemberException(String.Format("Couldn't find '{0}' '{1}' on type '{2}'.", kind, name, this.type));
		}

		private PropertyInfo ResolveProperty(string name, object[] indexArgs)
		{
			var candidates = type.GetAllProperties().Where(prop => prop.Name == name).ToArray();
			if (candidates.Length == 1)
				return candidates[0];

			var getters = candidates
				.Select(prop => prop.GetGetMethod(true))
				.Where(m => m != null && CanCall(m))
				.ToArray();

			indexArgs = indexArgs ?? MockingUtil.NoObjects;
			object state;
			var foundGetter = Type.DefaultBinder.BindToMethod(MockingUtil.AllMembers, getters, ref indexArgs, null, null, null, out state);
			return candidates.First(prop => prop.GetGetMethod(true) == foundGetter);
		}

		private FieldInfo ResolveField(string name)
		{
			return type.GetAllFields().FirstOrDefault(f => f.Name == name);
		}

		private bool CanCall(MethodBase method)
		{
			return method.IsStatic || this.instance != null;
		}

		private object CallInvoke(MethodBase method, object[] args)
		{
			return ProfilerInterceptor.GuardExternal(() => SecuredReflectionMethods.Invoke(method, this.instance, args));
		}
	}

	internal static class SecuredReflectionMethods
	{
		public delegate object InvokeDelegate(MethodBase method, object instance, object[] args);
		public delegate object GetPropertyDelegate(PropertyInfo property, object instance, object[] indexArgs);
		public delegate void SetPropertyDelegate(PropertyInfo property, object instance, object value, object[] indexArgs);
		public delegate object GetFieldDelegate(FieldInfo field, object instance);
		public delegate void SetFieldDelegate(FieldInfo field, object instance, object value);

		public static readonly InvokeDelegate Invoke;
		public static readonly GetPropertyDelegate GetProperty;
		public static readonly SetPropertyDelegate SetProperty;
		public static readonly GetFieldDelegate GetField;
		public static readonly SetFieldDelegate SetField;

		static SecuredReflectionMethods()
		{
			if (!HasReflectionPermission)
			{
				if (!ProfilerInterceptor.IsProfilerAttached)
					ProfilerInterceptor.ThrowElevatedMockingException();

				ProfilerInterceptor.CreateDelegateFromBridge("ReflectionInvoke", out Invoke);
				ProfilerInterceptor.CreateDelegateFromBridge("ReflectionGetProperty", out GetProperty);
				ProfilerInterceptor.CreateDelegateFromBridge("ReflectionSetProperty", out SetProperty);
				ProfilerInterceptor.CreateDelegateFromBridge("ReflectionGetField", out GetField);
				ProfilerInterceptor.CreateDelegateFromBridge("ReflectionSetField", out SetField);
			}
			else
			{
				Invoke = (method, instance, args) => method.Invoke(instance, args);
				GetProperty = (prop, instance, indexArgs) => prop.GetValue(instance, indexArgs);
				SetProperty = (prop, instance, value, indexArgs) => prop.SetValue(instance, value, indexArgs);
				GetField = (field, instance) => field.GetValue(instance);
				SetField = (field, instance, value) => field.SetValue(instance, value);
			}
		}

		internal static bool HasReflectionPermission
		{
			get
			{
#if SILVERLIGHT
				return false;
#else
				try
				{
					new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Demand();
					return true;
				}
				catch (SecurityException)
				{
					return false;
				}
#endif
			}
		}
	}
}
