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

		private static DynamicMetaObject CreateRecorder(Expression expression, DynamicMetaObjectBinder binder)
		{
			return new ExpressionRecorder(Expression.Constant(new ExpressionContainer(expression), binder.ReturnType),
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
			var wrapper = this.Value as ExpressionContainer;
			var valueExpr = wrapper.Expression;
			var property = PrivateAccessor.ResolveProperty(valueExpr.Type, binder.Name, new object[0], !wrapper.IsStatic);
			if (property == null)
				ThrowMissingMemberException(valueExpr.Type, binder.Name);

			var memberExpr = Expression.Property(!wrapper.IsStatic ? valueExpr : null, property);

			return CreateRecorder(memberExpr, binder);
		}

		public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
		{
			var wrapper = this.Value as ExpressionContainer;
			var valueExpr = wrapper.Expression;
			var property = PrivateAccessor.ResolveProperty(valueExpr.Type, binder.Name, new object[0], !wrapper.IsStatic, value.Value, getter: false);
			if (property == null)
				ThrowMissingMemberException(valueExpr.Type, binder.Name);

			var memberExpr = Expression.Assign(Expression.Property(!wrapper.IsStatic ? valueExpr : null, property), FromArg(value));

			return CreateRecorder(memberExpr, binder);
		}

		public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
		{
			var wrapper = this.Value as ExpressionContainer;
			var valueExpr = wrapper.Expression;
			var property = PrivateAccessor.ResolveProperty(valueExpr.Type, "Item",
				indexes.Select(i => i.Value).ToArray(), !wrapper.IsStatic);
			if (property == null)
				ThrowMissingMemberException(valueExpr.Type, "Item");

			var memberExpr = Expression.MakeIndex(!wrapper.IsStatic ? valueExpr : null, property, indexes.Select(FromArg));
			return CreateRecorder(memberExpr, binder);
		}

		public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
		{
			var wrapper = this.Value as ExpressionContainer;
			var valueExpr = wrapper.Expression;
			var property = PrivateAccessor.ResolveProperty(valueExpr.Type, "Item",
				indexes.Select(i => i.Value).ToArray(), !wrapper.IsStatic, value.Value, getter: false);
			if (property == null)
				ThrowMissingMemberException(valueExpr.Type, "Item");

			var memberExpr = Expression.Assign(
				Expression.MakeIndex(!wrapper.IsStatic ? valueExpr : null, property, indexes.Select(FromArg)),
				FromArg(value));
			return CreateRecorder(memberExpr, binder);
		}

		public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
		{
			var wrapper = this.Value as ExpressionContainer;
			var valueExpr = wrapper.Expression;

			var candidateMethods = valueExpr.Type.GetAllMethods()
				.Where(m => m.Name == binder.Name && m.IsStatic == wrapper.IsStatic)
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

			var methodArgs = args.Select(a =>
			{
				var matcher = UnwrapMatcher(a);
				return matcher != null ? matcher.ReturnType.GetDefaultValue() : a.Value;
			}).ToArray();

			object state;
			var method = (MethodInfo)Type.DefaultBinder.BindToMethod(BindingFlags.Default, candidateMethods, ref methodArgs, null, null, null, out state);

			var memberExpr = Expression.Call(!wrapper.IsStatic ? valueExpr : null, method, args.Select(FromArg).ToArray());

			return CreateRecorder(memberExpr, binder);
		}
	}
}
