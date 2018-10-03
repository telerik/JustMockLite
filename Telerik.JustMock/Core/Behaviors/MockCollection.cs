/*
 JustMock Lite
 Copyright © 2010-2015,2018 Telerik EAD

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Telerik.JustMock.Core.Behaviors
{
    internal static class MockCollection
    {
        public static object Create(Type resultCollectionType, MocksRepository repo, IMockReplicator replicator, IEnumerable collection)
        {
            if (resultCollectionType == typeof(string))
                return null;

            Type sourceType = collection.GetType();
            if (resultCollectionType.IsAssignableFrom(sourceType))
                return collection;

            var enumerableType = resultCollectionType.GetImplementationOfGenericInterface(typeof(IEnumerable<>)) ?? typeof(IEnumerable);
            if (!enumerableType.IsAssignableFrom(resultCollectionType))
                throw new MockException("Return value is not an enumerable type.");

            var elementType = enumerableType.IsGenericType ? enumerableType.GetGenericArguments()[0] : typeof(object);

            var ilistType = typeof(IList<>).MakeGenericType(elementType);
            var iqueryableType = typeof(IQueryable<>).MakeGenericType(elementType);

            IEnumerable list;
            if (typeof(ICollection).IsAssignableFrom(sourceType))
            {
                list = collection;
            }
            else
            {
                var listType = typeof(List<>).MakeGenericType(elementType);
                var castMethod = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(elementType);

                var castCollection = castMethod.Invoke(null, new[] { collection });
                list = (IEnumerable)MockingUtil.CreateInstance(listType, castCollection);
            }

            var listBehavior = new DelegatedImplementationBehavior(list,
                new[]
                {
                    ilistType,
                    typeof(IList),
                });

            var queryable = list.AsQueryable();
            var queryableType = queryable.GetType();
            var queryableBehavior = new DelegatedImplementationBehavior(queryable,
                new[] { queryableType.GetImplementationOfGenericInterface(typeof(IQueryable<>)) });

            if (replicator != null)
            {
                var mock = replicator.CreateSimilarMock(repo, resultCollectionType, null, true, null);
                IMockMixin mockMixin = MocksRepository.GetMockMixin(mock, null);
                mockMixin.FallbackBehaviors.Insert(0, queryableBehavior);
                mockMixin.FallbackBehaviors.Insert(0, listBehavior);
                return mock;
            }
            else
            {
                MockCreationSettings settings = MockCreationSettings.GetSettings(constructorArgs: null, behavior: Behavior.Loose, additionalMockedInterfaces: MockingUtil.EmptyTypes, mockConstructorCall: null,
                    additionalProxyTypeAttributes: null, supplementaryBehaviors: null, fallbackBehaviors: new List<IBehavior> { listBehavior, queryableBehavior });

                return repo.Create(resultCollectionType, settings);
            }
        }
    }
}
