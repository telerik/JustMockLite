using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Telerik.JustMock.Core.Behaviors
{
	internal class ActivatorCreateInstanceTBehavior : IBehavior
	{
		private readonly IMethodMock delegatedMock;

		public ActivatorCreateInstanceTBehavior(IMethodMock delegatedMock)
		{
			this.delegatedMock = delegatedMock;
		}

		public void Process(Invocation invocation)
		{
			var method = delegatedMock.CallPattern.Member as MethodBase;
			var delegatedInvocation = new Invocation(
				MockingUtil.TryGetUninitializedObject(invocation.ReturnType),
				method,
				new object[0]);

			this.delegatedMock.Repository.DispatchInvocation(delegatedInvocation);

			if (delegatedInvocation.IsReturnValueSet)
				invocation.ReturnValue = delegatedInvocation.ReturnValue;

			invocation.CallOriginal = delegatedInvocation.CallOriginal;
			invocation.UserProvidedImplementation = delegatedInvocation.UserProvidedImplementation;
			invocation.ExceptionThrower = delegatedInvocation.ExceptionThrower;

			if (invocation.CallOriginal)
				ProfilerInterceptor.SkipMethodInterceptionOnce(method);
		}

		public static LambdaExpression TryCreateArrangementExpression(MemberInfo member)
		{
			var ctor = member as ConstructorInfo;
			if (ctor == null)
				return null;

			var createInstance = typeof(Activator).GetMethod("CreateInstance", MockingUtil.EmptyTypes);
			createInstance = createInstance.MakeGenericMethod(member.DeclaringType);
			return Expression.Lambda(Expression.Call(createInstance));
		}

		public static void Attach(IMethodMock newExprMethodMock, IMethodMock createInstanceMethodMock)
		{
			var activatorBehavior = new ActivatorCreateInstanceTBehavior(newExprMethodMock);
			createInstanceMethodMock.Behaviors.Add(activatorBehavior);
		}
	}
}
