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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Telerik.JustMock.Core.Context;

namespace Telerik.JustMock.Core.Behaviors
{
	internal enum RecursiveMockingBehaviorType
	{
		OnlyDuringAnalysis,
		ReturnDefault,
		ReturnMock,
	}

	internal class RecursiveMockingBehavior : IBehavior
	{
		private readonly Dictionary<MethodBase, List<KeyValuePair<object, object>>> mocks
			= new Dictionary<MethodBase, List<KeyValuePair<object, object>>>();

		private readonly RecursiveMockingBehaviorType type;

		public RecursiveMockingBehavior(RecursiveMockingBehaviorType type)
		{
			this.type = type;
		}

		public void Process(Invocation invocation)
		{
			if (invocation.IsReturnValueSet || invocation.UserProvidedImplementation)
				return;

			var returnType = invocation.Method.GetReturnType();
			if (returnType == typeof(void) || returnType.IsValueType)
				return;

			object mock = null;
			List<KeyValuePair<object, object>> mocksList;
			if (mocks.TryGetValue(invocation.Method, out mocksList))
			{
				// can't put the key part in a Dictionary,
				// because we can't be sure that GetHashCode() works
				mock = mocksList.FirstOrDefault(kvp => Equals(kvp.Key, invocation.Instance)).Value;
			}

			if (mock == null)
			{
				var parentMock = MocksRepository.GetMockMixinFromInvocation(invocation);
				var repository = parentMock.Repository;
				var replicator = parentMock as IMockReplicator;

				bool mustReturnAMock = invocation.InArrange || this.type == RecursiveMockingBehaviorType.ReturnMock;
				if (mustReturnAMock || this.type == RecursiveMockingBehaviorType.ReturnDefault)
				{
					if (returnType.IsArray)
					{
						mock = Array.CreateInstance(returnType.GetElementType(), Enumerable.Repeat(0, returnType.GetArrayRank()).ToArray());
					}

					var idictionaryType = returnType.GetImplementationOfGenericInterface(typeof(IDictionary<,>));
					if (mock == null && idictionaryType != null)
					{
						var dictType = typeof(Dictionary<,>).MakeGenericType(idictionaryType.GetGenericArguments());
						mock = MockCollection.Create(returnType, repository, replicator, (IEnumerable)MockingUtil.CreateInstance(dictType));
					}

					var ienumerableType = returnType.GetImplementationOfGenericInterface(typeof(IEnumerable<>));
					if (mock == null && ienumerableType != null)
					{
						var listType = typeof(List<>).MakeGenericType(ienumerableType.GetGenericArguments());
						mock = MockCollection.Create(returnType, repository, replicator, (IEnumerable)MockingUtil.CreateInstance(listType));
					}

					if (mock == null && mustReturnAMock)
					{
#if !LITE_EDITION
						var stackTrace = new StackTrace();
						var methodCallingArrange = stackTrace.EnumerateFrames()
							.SkipWhile(m => !Attribute.IsDefined(m, typeof(ArrangeMethodAttribute)))
							.SkipWhile(m => m.Module.Assembly == typeof(MocksRepository).Assembly)
							.FirstOrDefault();

						if (methodCallingArrange != null && invocation.Method.DeclaringType.IsAssignableFrom(methodCallingArrange.DeclaringType))
							return;
#endif

						if (typeof(String) == returnType)
						{
							mock = String.Empty;
						}
						else
						{
							try
							{
								mock = replicator.CreateSimilarMock(repository, returnType, null, true, null);
							}
							catch (MockException)
							{ }
						}
					}
				}

				if (mock == null)
					return;

				if (mocksList == null)
				{
					mocksList = new List<KeyValuePair<object, object>>();
					mocks.Add(invocation.Method, mocksList);
				}
				mocksList.Add(new KeyValuePair<object, object>(invocation.Instance, mock));

				var mockMixin = MocksRepository.GetMockMixin(mock, null);
				if (parentMock != null && mockMixin != null)
					parentMock.DependentMocks.Add(mock);
			}

			invocation.ReturnValue = mock;
			invocation.CallOriginal = false;
			invocation.UserProvidedImplementation = true;
		}
	}
}
