/*
 JustMock Lite
 Copyright © 2010-2015,2018-2019 Progress Software Corporation

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
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Telerik.JustMock.Core;
#if !COREFX
using System.Security;
using System.Security.Permissions;
#if !NETCORE
using System.Runtime.Remoting;
#endif
using Telerik.JustMock.Core.TransparentProxy;
#endif

namespace Telerik.JustMock
{
	/// <summary>
	/// Gives access to the non-public members of a type or instance. This class provides functionality similar to the
	/// one that exists in regular reflection classes with the added benefit that it can bypass essential security checks related
	/// to accessing non-public members through reflection.
	/// </summary>
	/// <remarks>
	/// When the profiler is enabled, PrivateAccessor acquires additional power:
	/// - It can even be used to access all kinds of non-public members in Silverlight (and when running in partial trust in general).
	/// - All calls made through PrivateAccessor are always made with full trust (unrestricted) permissions.
	/// 
	/// You can assign a PrivateAccessor to a dynamic variable and access it as if you're referencing the original object:
	/// dynamic acc = new PrivateAccessor(myobj);
	/// acc.PrivateProperty = acc.PrivateMethod(123); // PrivateProperty and PrivateMethod are private members on myobj's type.
	/// </remarks>
	public sealed class PrivateAccessor : IDynamicMetaObjectProvider
	{
		private readonly object instance;
		private readonly Type type;

		/// <summary>
		/// Creates a new <see cref="PrivateAccessor"/> wrapping the given instance. Can be used to access both instance and static members.
		/// </summary>
		/// <param name="instance">The instance to wrap.</param>
		public PrivateAccessor(object instance)
			: this(instance, null)
		{ }
		/// <summary>
		/// Creates a new <see cref="PrivateAccessor"/> wrapping the given type. Can be used to access the static members of a type.
		/// </summary>
		/// <param name="type">Targeted type.</param>
		/// <returns>PrivateAccessor type.</returns>
		public static PrivateAccessor ForType(Type type)
		{
			return ProfilerInterceptor.GuardInternal(() => new PrivateAccessor(null, type));
		}

		/// <summary>
		/// Gets the value of a dynamic private accessor expression. Use this when the value to get
		/// is of type Object, otherwise cast the expression to the desired type.
		/// </summary>
		/// <param name="privateAccessor">A PrivateAccessor expression built from a dynamic variable.</param>
		/// <returns>The value of the private accessor expression</returns>
		public static object Unwrap(dynamic privateAccessor)
		{
			return ProfilerInterceptor.GuardInternal(() =>
			{
				var obj = (object)privateAccessor;
				var acc = obj as PrivateAccessor;
				return acc != null ? acc.Instance : obj;
			});
		}

		private PrivateAccessor(object instance, Type type)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					if (instance != null)
					{
#if (!COREFX && !NETCORE)
						var realProxy = MockingProxy.GetRealProxy(instance);
						if (realProxy != null)
						{
							instance = realProxy.WrappedInstance;
						}
#endif
						type = instance.GetType();
					}
					if (type.IsProxy() && type.BaseType != typeof(object))
					{
						type = type.BaseType;
					}

#if (!PORTABLE && !LITE_EDITION)
					Mock.Intercept(type);
#endif
				});

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
					.Where(m => m.Name == name && MockingUtil.CanCall(m, this.instance != null))
					.Select(m => MockingUtil.TrySpecializeGenericMethod(m, args.Select(a => a != null ? a.GetType() : null).ToArray()) ?? m)
					.ToArray();
				object state;
				var method = MockingUtil.BindToMethod(MockingUtil.AllMembers,
					candidates, ref args, null, null, null, out state);

				return CallInvoke(method, args);
			});
		}

		/// <summary>
		/// Calls the specified generic method by name.
		/// </summary>
		/// <param name="name">The name of the method to call.</param>
		/// <param name="typeArguments">The type arguments to specialize the generic method.</param>
		/// <param name="args">Arguments to pass to the method.</param>
		/// <returns>The value returned by the specified method.</returns>
		public object CallMethodWithTypeArguments(string name, ICollection<Type> typeArguments, params object[] args)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var candidates = type.GetAllMethods()
						.Where(m => m.Name == name && MockingUtil.CanCall(m, this.instance != null))
						.Select(m => MockingUtil.TryApplyTypeArguments(m, typeArguments.ToArray()))
						.Where(m => m != null)
						.ToArray();

					args = args ?? MockingUtil.NoObjects;
					object state;
					var method = MockingUtil.BindToMethod(MockingUtil.AllMembers,
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
		/// Calls the type's static constructor. The static constructor can be executed even when the runtime
		/// has already called it as part of type's initialization.
		/// </summary>
		/// <param name="forceCall">
		/// When 'false', the static constructor will not be called if it has already run as part of
		/// type initialization. When 'true', the static constructor will be called unconditionally.
		/// If the type is not yet initialized and 'true' is given, the static constructor will be run
		/// twice.
		/// </param>
		public void CallStaticConstructor(bool forceCall)
		{
			ProfilerInterceptor.GuardInternal(() =>
			{
				if (forceCall)
				{
					var staticCtor = this.type.GetMember(".cctor", BindingFlags.Static | BindingFlags.NonPublic).FirstOrDefault() as MethodBase;
					if (staticCtor != null)
						this.CallMethod(staticCtor);
				}
				else
				{
					ProfilerInterceptor.RunClassConstructor(this.type.TypeHandle);
				}
			});
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
				var prop = MockingUtil.ResolveProperty(this.type, name, false, indexArgs, this.instance != null);
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
					var prop = MockingUtil.ResolveProperty(this.type, name, false, indexArgs, this.instance != null, value, getter: false);
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

		/// <summary>
		/// Gets the value of a field or property.
		/// </summary>
		/// <param name="name">Name of a field or property to get.</param>
		/// <returns>The value of the field or property.</returns>
		public object GetMember(string name)
		{
			return ProfilerInterceptor.GuardInternal(() =>
				{
					var field = ResolveField(name);
					return field != null ? GetField(name) : GetProperty(name);
				});
		}

		/// <summary>
		/// Sets the value of a field or property.
		/// </summary>
		/// <param name="name">The name of a field or property to set</param>
		/// <param name="value">The new value of the field or property.</param>
		public void SetMember(string name, object value)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var field = ResolveField(name);
					if (field != null)
						SetField(name, value);
					else
						SetProperty(name, value);
				});
		}

		/// <summary>
		/// Raises the specified event passing the given arguments to the handlers.
		/// </summary>
		/// <param name="name">The name of the event to raise.</param>
		/// <param name="eventArgs">Arguments to pass to the event handlers. Must match the event handler signature exactly.</param>
		public void RaiseEvent(string name, params object[] eventArgs)
		{
			ProfilerInterceptor.GuardInternal(() =>
				{
					var evt = this.type.GetEvent(name, MockingUtil.AllMembers);
					MockingUtil.RaiseEventThruReflection(this.instance, evt, eventArgs);
				});
		}

		/// <summary>
		/// The wrapped instance.
		/// </summary>
		public object Instance
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() => this.instance);
			}
		}

#if !PORTABLE
		/// <summary>
		/// Non public ref return interface for mocking.
		/// </summary>
		public IPrivateRefReturnAccessor RefReturn
		{
			get
			{
				return ProfilerInterceptor.GuardInternal(() => new PrivateRefReturnAccessor(this.instance, this.type));
			}
		}
#endif

		private void CheckMemberInfo(string kind, string name, MemberInfo mi)
		{
			if (mi == null)
				throw new MissingMemberException(String.Format("Couldn't find {0} '{1}' on type '{2}'.", kind, name, this.type));
		}

		private FieldInfo ResolveField(string name)
		{
			return type.GetAllFields().FirstOrDefault(f => f.Name == name);
		}

		private object CallInvoke(MethodBase method, object[] args)
		{
			return ProfilerInterceptor.GuardExternal(() => SecuredReflectionMethods.Invoke(method, this.instance, args));
		}

		DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
		{
			return new PrivateAccessorMetaObject(parameter, BindingRestrictions.Empty, this);
		}

		private class PrivateAccessorMetaObject : DynamicMetaObject
		{
			public PrivateAccessorMetaObject(Expression expression, BindingRestrictions restrictions)
				: base(expression, restrictions)
			{ }

			public PrivateAccessorMetaObject(Expression expression, BindingRestrictions restrictions, object value)
				: base(expression, restrictions, value)
			{ }

			private DynamicMetaObject CreateMetaObject(Expression value, bool wrap = true)
			{
				if (wrap)
				{
					var valueVar = Expression.Variable(value.Type);
					var statements = new List<Expression>();

					if (!value.Type.IsValueType)
					{
						statements.Add(Expression.Assign(valueVar, value));
						statements.Add(Expression.Condition(
							Expression.Equal(valueVar, Expression.Constant(null)),
							Expression.Constant(null, typeof(PrivateAccessor)),
							Expression.New(typeof(PrivateAccessor).GetConstructor(new[] { typeof(object) }), valueVar)));
					}
					else
					{
						statements.Add(value);
					}

					value = Expression.Block(new[] { valueVar }, statements.ToArray());
				}
				return new PrivateAccessorMetaObject(value, BindingRestrictions.GetTypeRestriction(this.Expression, typeof(PrivateAccessor)));
			}

			public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
			{
				var callResult = Expression.Variable(typeof(object));
				var argsVar = Expression.Variable(typeof(object[]));

				MethodInfo invoke = typeof(PrivateAccessor).GetMethod("CallMethod", new[] { typeof(string), typeof(object[]) });

				var invokeArgs = new List<Expression>
				{
					Expression.Constant(binder.Name),
					argsVar
				};

				var typeArgs = MockingUtil.TryGetTypeArgumentsFromBinder(binder);
				if (typeArgs != null)
				{
					invoke = typeof(PrivateAccessor).GetMethod("CallMethodWithTypeArguments");
					invokeArgs.Insert(1, Expression.Constant(typeArgs));
				}

				var executionList = new List<Expression>
				{
					Expression.Assign(argsVar, Expression.NewArrayInit(typeof(object), args.Select(a => Expression.Convert(a.Expression, typeof(object))))),
					Expression.Assign(callResult, Expression.Call(
						Expression.Convert(this.Expression, typeof(PrivateAccessor)), invoke, invokeArgs.ToArray())),
				};

				executionList.AddRange(args
					.Select((a, i) => new { expr = a.Expression, i })
					.Where(p => p.expr is ParameterExpression)
					.Select(p => Expression.Assign(p.expr, Expression.Convert(Expression.ArrayIndex(argsVar, Expression.Constant(p.i)), p.expr.Type))));
				executionList.Add(callResult);

				return CreateMetaObject(Expression.Block(new[] { argsVar, callResult }, executionList));
			}

			private DynamicMetaObject BindSetMember(Type returnType, string propertyName, Expression value, IEnumerable<Expression> indexes = null)
			{
				bool hasIndexes = indexes != null;

				var setProp = typeof(PrivateAccessor).GetMethod(hasIndexes ? "SetProperty" : "SetMember");

				var tempValue = Expression.Variable(value.Type);
				var arguments = new List<Expression>
				{
					Expression.Constant(propertyName),
					Expression.Convert(tempValue, typeof(object)),
				};
				if (hasIndexes)
					arguments.Add(Expression.NewArrayInit(typeof(object), indexes.Select(a => Expression.Convert(a, typeof(object)))));

				var call = Expression.Call(Expression.Convert(this.Expression, typeof(PrivateAccessor)), setProp, arguments.ToArray());
				return CreateMetaObject(
					Expression.Block(new[] { tempValue },
					new Expression[]
					{
						Expression.Assign(tempValue, value),
						call,
						Expression.Convert(tempValue, returnType),
					}), wrap: false);
			}

			public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
			{
				var getProp = typeof(PrivateAccessor).GetMethod("GetMember");
				var call = Expression.Call(Expression.Convert(this.Expression, typeof(PrivateAccessor)), getProp, Expression.Constant(binder.Name));
				return CreateMetaObject(call);
			}

			public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
			{
				return BindSetMember(binder.ReturnType, binder.Name, value.Expression);
			}

			public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
			{
				var getProp = typeof(PrivateAccessor).GetMethod("GetProperty");

				var call = Expression.Call(Expression.Convert(this.Expression, typeof(PrivateAccessor)), getProp,
					Expression.Constant("Item"),
					Expression.NewArrayInit(typeof(object), indexes.Select(a => Expression.Convert(a.Expression, typeof(object)))));
				return CreateMetaObject(call);
			}

			public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
			{
				return BindSetMember(binder.ReturnType, "Item", value.Expression, indexes.Select(i => i.Expression));
			}

			public override DynamicMetaObject BindConvert(ConvertBinder binder)
			{
				var obj = typeof(PrivateAccessor).GetProperty("Instance");
				return new DynamicMetaObject(
					Expression.Convert(Expression.Property(Expression.Convert(this.Expression, typeof(PrivateAccessor)), obj), binder.Type),
					BindingRestrictions.GetTypeRestriction(this.Expression, typeof(PrivateAccessor)));
			}
		}
	}

	internal static class SecuredReflection
	{
		internal static bool HasReflectionPermission { get; private set; }

		internal static bool IsAvailable
		{
			get { return HasReflectionPermission || ProfilerInterceptor.IsProfilerAttached; }
		}

		static SecuredReflection()
		{
			HasReflectionPermission = CheckReflectionPermission();
		}

		private static bool CheckReflectionPermission()
		{
#if (COREFX)
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
			if (!SecuredReflection.HasReflectionPermission)
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
	}
}
