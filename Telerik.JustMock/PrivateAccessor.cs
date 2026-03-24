/*
 JustMock Lite
 Copyright © 2010-2015,2018-2019,2020 Progress Software Corporation

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
    /// Gives access to the non-public members of a type or instance. 
    /// Unlike standard reflection, this class can bypass essential security checks related to accessing non-public members through reflection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use <see cref="PrivateAccessor"/> when you need to call private members, read non-public properties, or set internal state in a test.
    /// It behaves like reflection, but it can bypass security restrictions that standard reflection cannot bypass.
    /// </para>
    /// <para>
    /// When the profiler is enabled, <see cref="PrivateAccessor"/> can access additional member types and runs calls with full trust.
    /// </para>
    /// <para>
    /// You can also assign a <see cref="PrivateAccessor"/> to a <see langword="dynamic"/> variable and use member syntax directly:
    /// dynamic acc = new PrivateAccessor(myobj);
    /// acc.PrivateProperty = acc.PrivateMethod(123); // PrivateProperty and PrivateMethod are private members on myobj's type.
    /// </para>
    /// </remarks>
    public sealed class PrivateAccessor : IDynamicMetaObjectProvider
    {
        private readonly object instance;
        private readonly Type type;

        /// <summary>
        /// Gets or sets a value that controls whether <see cref="CallMethod(string, object[])"/> rethrows the original exception.
        /// </summary>
        /// <remarks>
        /// By default, reflection wraps exceptions in <see cref="TargetInvocationException"/>. Set this property to
        /// <see langword="true"/> if you want to receive the original exception instead.
        /// </remarks>
        public bool RethrowOriginalOnCallMethod { get; set; }

        /// <summary>
        /// Initializes a new <see cref="PrivateAccessor"/> that wraps the given object instance,
        /// enabling access to both its instance and static non-public members.
        /// </summary>
        /// <param name="instance">The object whose non-public members you want to access. Must not be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is <see langword="null"/>.</exception>
        public PrivateAccessor(object instance)
            : this(instance, null)
        { }

        /// <summary>
        /// Creates a new <see cref="PrivateAccessor"/> wrapping the given type. Can be used to access the static members of a type.
        /// </summary>
        /// <param name="type">The type whose static non-public members you want to access. Must not be <see langword="null"/>.</param>
        /// <returns>A <see cref="PrivateAccessor"/> targeting the static members of <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/>.</exception>
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
        /// <param name="name">Name of the method to call.</param>
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

#if !PORTABLE
        /// <summary>
        /// Calls the specified method by name and given argument types and values.
        /// </summary>
        /// <param name="name">Name of the method to call.</param>
        /// <param name="argTypes">Method parameter types.</param>
        /// <param name="argModifiers">Parameter modifiers such as <c>ref</c> and <c>out</c>.</param>
        /// <param name="argValues">Argument values to pass to the method.</param>
        /// <returns>The value returned by the method.</returns>
        public object CallMethod(string name, ICollection<Type> argTypes, ParameterModifier argModifiers, object[] argValues)
        {
            return ProfilerInterceptor.GuardInternal(() => CallMethodInternal(name, argTypes, new[] { argModifiers }, argValues));
        }

        /// <summary>
        /// Calls a non-public method by name and explicit parameter types.
        /// </summary>
        /// <param name="name">Name of the method to call.</param>
        /// <param name="argTypes">Method parameter types.</param>
        /// <param name="argValues">Argument values to pass to the method.</param>
        /// <returns>The value returned by the method.</returns>
        public object CallMethod(string name, ICollection<Type> argTypes, object[] argValues)
        {
            return ProfilerInterceptor.GuardInternal(() => CallMethodInternal(name, argTypes, null, argValues));
        }

        private object CallMethodInternal(string name, ICollection<Type> argTypes, ParameterModifier[] argModifiers, object[] argValues)
        {
            if (argTypes == null)
            {
                throw new ArgumentNullException("argTypes");
            }

            if (argValues == null || argValues.Length == 0)
            {
                argValues = argTypes.Select(t => MockingUtil.GetDefaultValue(t)).ToArray();
            }

            if (argTypes.Count != argValues.Length)
            {
                throw new ArgumentException("The number of argument types does not match the number of argument values");
            }

            var argValueIndex = 0;
            foreach (var argType in argTypes)
            {
                if (argValues[argValueIndex] != null && argType != argValues[argValueIndex].GetType()
                    || argValues[argValueIndex] == null && argValues[argValueIndex] != MockingUtil.GetDefaultValue(argType))
                {
                    throw new ArgumentException("One or more arguments types does not match the argument values");
                }
                argValueIndex++;
            }

            var candidates = type.GetAllMethods()
                .Where(m => m.Name == name && MockingUtil.CanCall(m, this.instance != null))
                .Select(m => MockingUtil.TrySpecializeGenericMethod(m, argTypes.ToArray()) ?? m)
                .ToArray();

            var method = MockingUtil.SelectMethod(MockingUtil.AllMembers, candidates, argTypes.ToArray(), argModifiers);

            return CallInvoke(method, argValues);
        }
#endif

        /// <summary>
        /// Calls the specified generic method by name.
        /// </summary>
        /// <param name="name">Name of the method to call.</param>
        /// <param name="typeArguments">Type arguments to specialize the generic method.</param>
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
        /// <param name="method">Method to call.</param>
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
        /// When this value is <see langword="false"/>, JustMock does not rerun a static constructor that has already executed.
        /// When this value is <see langword="true"/>, JustMock calls the static constructor unconditionally. If the type is not
        /// initialized yet, the static constructor can run twice.
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
        /// Gets the value returned by the indexer for the specified index.
        /// </summary>
        /// <param name="index">Indexer argument.</param>
        /// <returns>The indexer value.</returns>
        public object GetIndex(object index)
        {
            return ProfilerInterceptor.GuardInternal(() => GetProperty("Item", index));
        }

        /// <summary>
        /// Gets the value of a property by name.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="indexArgs">Optional index arguments for indexed properties.</param>
        /// <returns>The property value.</returns>
        public object GetProperty(string name, params object[] indexArgs)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var prop = MockingUtil.ResolveProperty(this.type, name, false, indexArgs, this.instance != null);
                return ProfilerInterceptor.GuardExternal(() => SecuredReflectionMethods.GetProperty(prop, this.instance, indexArgs));
            });
        }

        /// <summary>
        /// Sets the value of the indexer for the specified index.
        /// </summary>
        /// <param name="index">Indexer argument.</param>
        /// <param name="value">The value to give to the indexer.</param>
        public void SetIndex(object index, object value)
        {
            ProfilerInterceptor.GuardInternal(() => SetProperty("Item", value, index));
        }

        /// <summary>
        /// Sets the value of a property by name.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">The value to set to the property.</param>
        /// <param name="indexArgs">Optional index arguments for indexed properties.</param>
        public void SetProperty(string name, object value, params object[] indexArgs)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var prop = MockingUtil.ResolveProperty(this.type, name, false, indexArgs, this.instance != null, value, getter: false);
                ProfilerInterceptor.GuardExternal(() => SecuredReflectionMethods.SetProperty(prop, this.instance, value, indexArgs));
            });
        }

        /// <summary>
        /// Gets the value of a non-public field by name on the wrapped instance or type.
        /// </summary>
        /// <param name="name">The exact name of the field to read.</param>
        /// <returns>
        /// The current value of the field, or <see langword="null"/> if the field holds a null reference.
        /// </returns>
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
        /// Sets the value of a non-public field by name on the wrapped instance or type.
        /// </summary>
        /// <param name="name">The exact name of the field to write.</param>
        /// <param name="value">
        /// The value to assign to the field. Must be assignable to the field's declared type,
        /// or <see langword="null"/> for reference and nullable types.
        /// </param>
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
        /// Gets the value of a non-public field or property by name.
        /// Fields are resolved first; if no matching field is found, the name is treated as a property.
        /// </summary>
        /// <param name="name">The name of the field or property to read.</param>
        /// <returns>
        /// The current value of the field or property, or <see langword="null"/> if the member holds a null reference.
        /// </returns>
        public object GetMember(string name)
        {
            return ProfilerInterceptor.GuardInternal(() =>
            {
                var field = ResolveField(name);
                return field != null ? GetField(name) : GetProperty(name);
            });
        }

        /// <summary>
        /// Sets the value of a non-public field or property by name.
        /// Fields are resolved first; if no matching field is found, the name is treated as a property.
        /// </summary>
        /// <param name="name">The name of the field or property to write.</param>
        /// <param name="value">The value to assign to the field or property.</param>
        /// <exception cref="MissingMemberException">
        /// Thrown when neither a field nor a property with the given <paramref name="name"/> exists on the target type.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="value"/> is not assignable to the member's declared type.
        /// </exception>
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
        /// Raises the specified event on the wrapped instance or type, invoking all currently subscribed handlers.
        /// </summary>
        /// <param name="name">The name of the event to raise.</param>
        /// <param name="eventArgs">
        /// Arguments to pass to each event handler. The values must match the event delegate's
        /// parameter list exactly (e.g., <c>sender</c> followed by the <see cref="EventArgs"/>-derived argument).
        /// </param>
        public void RaiseEvent(string name, params object[] eventArgs)
        {
            ProfilerInterceptor.GuardInternal(() =>
            {
                var evt = this.type.GetEvent(name, MockingUtil.AllMembers);
                MockingUtil.RaiseEventThruReflection(this.instance, evt, eventArgs);
            });
        }

        /// <summary>
        /// Gets the object instance wrapped by this <see cref="PrivateAccessor"/>.
        /// Returns <see langword="null"/> when this accessor was created via <see cref="ForType"/> for static member access.
        /// </summary>
        /// <value>
        /// The wrapped instance, or <see langword="null"/> for a type-only (static) accessor.
        /// </value>
        public object Instance
        {
            get
            {
                return ProfilerInterceptor.GuardInternal(() => this.instance);
            }
        }

#if !PORTABLE
        /// <summary>
        /// Gets an <see cref="IPrivateRefReturnAccessor"/> that provides access to non-public members
        /// with <see langword="ref"/> return values on the wrapped instance or type.
        /// </summary>
        /// <remarks>
        /// This property is only available when the JustMock profiler is attached, as intercepting
        /// <see langword="ref"/>-returning members requires elevated (non-portable) mode.
        /// </remarks>
        /// <value>
        /// An <see cref="IPrivateRefReturnAccessor"/> scoped to the same instance and type as this accessor.
        /// </value>
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
            try
            {
                return ProfilerInterceptor.GuardExternal(() => SecuredReflectionMethods.Invoke(method, this.instance, args));
            }
            catch (TargetInvocationException targetInvocationException)
            {
                if (this.RethrowOriginalOnCallMethod && targetInvocationException.InnerException != null)
                {
                    throw targetInvocationException.InnerException;
                }
                else
                {
                    throw;
                }
            }
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
