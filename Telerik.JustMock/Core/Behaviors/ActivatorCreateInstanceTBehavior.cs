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
			var delegatedInvocation = new Invocation(
				MockingUtil.TryGetUninitializedObject(invocation.Method.GetReturnType()),
				delegatedMock.CallPattern.Method,
				new object[0]);

			this.delegatedMock.Repository.DispatchInvocation(delegatedInvocation);

			if (delegatedInvocation.IsReturnValueSet)
				invocation.ReturnValue = delegatedInvocation.ReturnValue;

			invocation.CallOriginal = delegatedInvocation.CallOriginal;
			invocation.UserProvidedImplementation = delegatedInvocation.UserProvidedImplementation;
			invocation.ExceptionThrower = delegatedInvocation.ExceptionThrower;

			if (invocation.CallOriginal)
				ProfilerInterceptor.SkipMethodInterceptionOnce(delegatedMock.CallPattern.Method);
		}

		public static LambdaExpression TryCreateArrangementExpression(MethodBase method)
		{
			var ctor = method as ConstructorInfo;
			if (ctor == null)
				return null;

			var createInstance = typeof(Activator).GetMethod("CreateInstance", MockingUtil.EmptyTypes);
			createInstance = createInstance.MakeGenericMethod(method.DeclaringType);
			return Expression.Lambda(Expression.Call(createInstance));
		}

		public static void Attach(IMethodMock newExprMethodMock, IMethodMock createInstanceMethodMock)
		{
			var activatorBehavior = new ActivatorCreateInstanceTBehavior(newExprMethodMock);
			createInstanceMethodMock.Behaviors.Add(activatorBehavior);
		}
	}
}
