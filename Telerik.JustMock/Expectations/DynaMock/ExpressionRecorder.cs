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
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Telerik.JustMock.Core;

namespace Telerik.JustMock.Expectations.DynaMock
{
	internal sealed class ExpressionRecorder : DynamicMetaObject
	{
		public ExpressionRecorder(Expression expression, BindingRestrictions restrictions)
			: base(expression, restrictions)
		{ }

		public ExpressionRecorder(Expression expression, BindingRestrictions restrictions, object value)
			: base(expression, restrictions, value)
		{ }

		private static DynamicMetaObject CreateRecorder(Expression expression, Type returnType)
		{
			return new ExpressionRecorder(Expression.Constant(new ExpressionContainer(expression), returnType),
				BindingRestrictions.GetExpressionRestriction(Expression.Constant(true)));
		}

		private static Expression FromArg(DynamicMetaObject arg)
		{
			if (UnwrapMatcher(arg) != null)
			{
				return arg.Value as Expression;
			}
			else
			{
				return Expression.Constant(arg.Value, arg.LimitType);
			}
		}

		private static MethodInfo UnwrapMatcher(DynamicMetaObject arg)
		{
			var expr = arg.Value as MethodCallExpression;
			if (expr != null && expr.Method.GetCustomAttributes(typeof(ArgMatcherAttribute), false).Length != 0)
			{
				return expr.Method;
			}
			else
			{
				return null;
			}
		}

		private static void ThrowMissingMemberException(Type type, string name)
		{
			throw new MissingMemberException(String.Format("Member named '{0}' not found on type '{1}'.", name, type));
		}

		public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
		{
			return DoBindGetMember(binder.ReturnType, binder.Name, binder.IgnoreCase);
		}

		private DynamicMetaObject DoBindGetMember(Type returnType, string memberName, bool ignoreCase)
		{
			var wrapper = this.Value as ExpressionContainer;
			var valueExpr = wrapper.Expression;
			var property = MockingUtil.ResolveProperty(valueExpr.Type, memberName, ignoreCase, new object[0], !wrapper.IsStatic);
			if (property == null)
				ThrowMissingMemberException(valueExpr.Type, memberName);

			var memberExpr = Expression.Property(!wrapper.IsStatic ? valueExpr : null, property);

			return CreateRecorder(memberExpr, returnType);
		}

		public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
		{
			return DoBindSetMember(this.Value as ExpressionContainer, binder.ReturnType, binder.Name, binder.IgnoreCase, value);
		}

		private DynamicMetaObject DoBindSetMember(ExpressionContainer wrapper, Type returnType, string memberName, bool ignoreCase, DynamicMetaObject value)
		{
			var valueExpr = wrapper.Expression;
			var property = MockingUtil.ResolveProperty(valueExpr.Type, memberName, ignoreCase, new object[0], !wrapper.IsStatic, value.Value, getter: false);
			if (property == null)
				ThrowMissingMemberException(valueExpr.Type, memberName);

			var memberExpr = Expression.Assign(Expression.Property(!wrapper.IsStatic ? valueExpr : null, property), FromArg(value));

			return CreateRecorder(memberExpr, returnType);
		}

		public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
		{
			var wrapper = this.Value as ExpressionContainer;
			var valueExpr = wrapper.Expression;
			var property = MockingUtil.ResolveProperty(valueExpr.Type, "Item", false,
				indexes.Select(i => i.Value).ToArray(), !wrapper.IsStatic);
			if (property == null)
				ThrowMissingMemberException(valueExpr.Type, "Item");

			var memberExpr = Expression.MakeIndex(!wrapper.IsStatic ? valueExpr : null, property, indexes.Select(FromArg));
			return CreateRecorder(memberExpr, binder.ReturnType);
		}

		public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
		{
			var wrapper = this.Value as ExpressionContainer;
			var valueExpr = wrapper.Expression;
			var property = MockingUtil.ResolveProperty(valueExpr.Type, "Item", false,
				indexes.Select(i => i.Value).ToArray(), !wrapper.IsStatic, value.Value, getter: false);
			if (property == null)
				ThrowMissingMemberException(valueExpr.Type, "Item");

			var memberExpr = Expression.Assign(
				Expression.MakeIndex(!wrapper.IsStatic ? valueExpr : null, property, indexes.Select(FromArg)),
				FromArg(value));
			return CreateRecorder(memberExpr, binder.ReturnType);
		}

		public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
		{
			var wrapper = this.Value as ExpressionContainer;
			var valueExpr = wrapper.Expression;

			var typeArgs = MockingUtil.TryGetTypeArgumentsFromBinder(binder);

			var candidateMethods = valueExpr.Type.GetAllMethods()
				.Where(m => MockingUtil.StringEqual(m.Name, binder.Name, binder.IgnoreCase) && m.IsStatic == wrapper.IsStatic)
				.Where(m => m.GetParameters().Length >= args.Length)
				.Select(m =>
				{
					if (typeArgs == null)
					{
						return MockingUtil.TrySpecializeGenericMethod(m, args.Select(a => a.RuntimeType).ToArray()) ?? m;
					}
					else
					{
						return MockingUtil.TryApplyTypeArguments(m, typeArgs);
					}
				})
				.Where(m => m != null)
				.Where(m =>
				{
					var methodParams = m.GetParameters();
					for (int i = 0; i < args.Length; ++i)
					{
						var matcher = UnwrapMatcher(args[i]);
						if (matcher != null)
						{
							var argType = matcher.ReturnType;
							if (!methodParams[i].ParameterType.IsAssignableFrom(argType))
								return false;
						}
					}
					return true;
				})
				.ToArray();

			if (candidateMethods.Length == 0 && args.Length == 0)
			{
				return DoBindGetMember(binder.ReturnType, binder.Name, binder.IgnoreCase);
			}

			var methodArgs = args.Select(a =>
			{
				var matcher = UnwrapMatcher(a);
				return matcher != null ? matcher.ReturnType.GetDefaultValue() : a.Value;
			}).ToArray();

			object state;
			var method = (MethodInfo)MockingUtil.BindToMethod(MockingUtil.Default, candidateMethods, ref methodArgs, null, null, null, out state);

			var memberExpr = Expression.Call(!wrapper.IsStatic ? valueExpr : null, method, args.Select(FromArg).ToArray());

			return CreateRecorder(memberExpr, binder.ReturnType);
		}

		public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg)
		{
			if (binder.Operation == ExpressionType.Equal)
			{
				var wrapper = this.Value as ExpressionContainer;
				var valueExpr = wrapper.Expression as MemberExpression;
				if (valueExpr != null)
				{
					var prevContainer = new ExpressionContainer(valueExpr.Expression) { IsStatic = ((PropertyInfo)valueExpr.Member).GetGetMethod(true).IsStatic };
					return DoBindSetMember(prevContainer, binder.ReturnType, valueExpr.Member.Name, false, arg);
				}
			}

			return base.BindBinaryOperation(binder, arg);
		}
	}
}
