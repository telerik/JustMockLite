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
using System.Linq.Expressions;

namespace Telerik.JustMock.Core.Context
{
    internal static class LocalMockingContextResolver
    {
        private static MocksRepository repository;

        public static MocksRepository ResolveRepository(UnresolvedContextBehavior unresolvedContextBehavior)
        {
            if (repository == null)
            {
                repository = new MocksRepository(null, null);
            }
            return repository;
        }

        public static bool RetireRepository()
        {
            return false;
        }

        public static Action<string, Exception> GetFailMethod()
        {
            return GetFailMethod(typeof(MockAssertionFailedException));
        }

        public static Action<string, Exception> GetFailMethod(Type assertFailedExceptionType)
        {
            var messageParam = Expression.Parameter(typeof(string));
            var innerExceptionParam = Expression.Parameter(typeof(Exception));
            var ctor = assertFailedExceptionType.GetConstructor(new[] { typeof(string), typeof(Exception) });
            return (Action<string, Exception>)Expression.Lambda(Expression.Throw(Expression.New(ctor, messageParam, innerExceptionParam)),
                messageParam, innerExceptionParam).Compile();
        }
    }
}
