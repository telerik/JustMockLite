/*
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
using System.Linq;
using System.Linq.Expressions;

namespace Telerik.JustMock.Core
{
	internal static partial class MockingUtil
	{
		public static readonly Type[] EmptyTypes = new Type[0];

		public static Func<object[], Delegate, object> MakeFuncCaller(Delegate delg)
		{
			var argsParam = Expression.Parameter(typeof(object[]));
			var delegateParam = Expression.Parameter(typeof(Delegate));

			var invoke = delg.GetType().GetMethod("Invoke");
			var hasReturn = invoke.ReturnType != typeof(void);
			var locals = invoke.GetParameters().Select(
				p => Expression.Variable(p.ParameterType.IsByRef ? p.ParameterType.GetElementType() : p.ParameterType))
				.ToArray();
			var assignments = Expression.Block(
				invoke.GetParameters().Select((p, i) =>
					Expression.Assign(
						locals[i],
						Expression.Convert(
							Expression.ArrayIndex(argsParam, Expression.Constant(i)), locals[i].Type)))
					.Concat(new Expression[] { Expression.Empty() }));

			Expression call = Expression.Call(
				Expression.Convert(delegateParam, delg.GetType()),
				invoke, locals);
			var resultVar = hasReturn
				? (Expression)Expression.Variable(invoke.ReturnType)
				: Expression.Constant(null, typeof(object));
			if (hasReturn)
				call = Expression.Assign(resultVar, call);

			var byrefAssignments = Expression.Block(
				invoke.GetParameters()
				.Select((p, i) => new { Type = p.ParameterType, Index = i })
				.Where(data => data.Type.IsByRef)
				.Select(data => Expression.Assign(
					Expression.ArrayAccess(argsParam, Expression.Constant(data.Index)),
					Expression.Convert(locals[data.Index], typeof(object))))
				.Concat(new Expression[] { Expression.Empty() })
				.ToArray());

			var callerBlock = Expression.Block(locals,
				assignments,
				call,
				byrefAssignments,
				Expression.Convert(resultVar, typeof(object)));

			if (hasReturn)
				callerBlock = Expression.Block(new[] { (ParameterExpression)resultVar }, callerBlock);

			var callerLambda = Expression.Lambda<Func<object[], Delegate, object>>(callerBlock, argsParam, delegateParam);
			return callerLambda.Compile();
		}
	}
}
