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
using System.Linq.Expressions;
using Telerik.JustMock.Core;

namespace Telerik.JustMock.Expectations
{
	internal class ReturnArranger : FunctionalSpecParser.IReturnArranger
	{
		private ReturnArranger()
		{
		}

		public static readonly ReturnArranger Instance = new ReturnArranger();

		public void ArrangeReturn<T>(Expression<Func<T>> callPattern, LambdaExpression returnValueExpr)
		{
			var noParams = returnValueExpr as Expression<Func<T>>;
			if (noParams != null)
			{
				Telerik.JustMock.Mock.Arrange(callPattern).Returns((T)noParams.Body.EvaluateExpression());
				return;
			}

			var method = typeof(FuncExpectation<T>).GetMethods()
												   .Single(m => m.Name == "Returns" && m.GetGenericArguments().Length == returnValueExpr.Parameters.Count);
			method = method.MakeGenericMethod(returnValueExpr.Parameters.Select(param => param.Type).ToArray());
			var expectation = Telerik.JustMock.Mock.Arrange(callPattern);
			method.Invoke(expectation, new object[] { returnValueExpr.Compile() });
		}
	}
}
