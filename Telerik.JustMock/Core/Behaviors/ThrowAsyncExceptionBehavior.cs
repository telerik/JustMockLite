/*
 JustMock Lite
 Copyright © 2019 Progress Software Corporation

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
using System.Threading.Tasks;
using Telerik.JustMock.Core.Context;
using Telerik.JustMock.Setup;

namespace Telerik.JustMock.Core.Behaviors
{
    internal class ThrowAsyncExceptionBehavior : IBehavior
    {
        private readonly Exception exception;

        public ThrowAsyncExceptionBehavior(Exception exception)
        {
            this.exception = exception;
        }

        public void Process(Invocation invocation)
        {
            if (invocation.Recording || invocation.InArrange || invocation.InAssertSet)
            {
                return;
            }

            var returnType = invocation.Method.GetReturnType();
            if (!typeof(Task).IsAssignableFrom(returnType))
            {
                MockingContext.Fail("Wrong invocation to arrangement: return type of {0}.{1} is not a task",
                    invocation.Instance != null ? MockingUtil.GetUnproxiedType(invocation.Instance) : invocation.Method.DeclaringType, invocation.Method.Name);
            }

            var elementType = returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>)
                    ? returnType.GetGenericArguments()[0] : typeof(object);
            Expression<Func<Task<object>>> taskFromException =
                () => MockingUtil.TaskFromException<object>((Exception)null);
            var mock =
                ((MethodCallExpression)taskFromException.Body).Method
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(elementType)
                    .Invoke(null, new object[] { this.exception });

            var parentMock = invocation.MockMixin;
            var mockMixin = MocksRepository.GetMockMixin(mock, null);
            if (parentMock != null && mockMixin != null)
            {
                parentMock.DependentMocks.Add(mock);
            }

            invocation.ReturnValue = mock;
            invocation.CallOriginal = false;
            invocation.UserProvidedImplementation = true;
        }
    }
}
