/*
 JustMock Lite
 Copyright © 2010-2015,2019 Progress Software Corporation

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
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
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

        public RecursiveMockingBehaviorType Type { get { return type; } }

        public void Process(Invocation invocation)
        {
            if (invocation.IsReturnValueSet)
                return;

            var returnType = invocation.Method.GetReturnType();
            if (returnType == typeof(void) || returnType.IsValueType)
                return;

            if (invocation.Method.Name == "ToString"
                && invocation.Method.GetParameters().Length == 0
                && invocation.UserProvidedImplementation)
                return;
            if (invocation.Method.Name == "GetType"
                && invocation.Method.GetReturnType() == typeof(Type)
                && invocation.Method.GetParameters().Length == 0)
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
                var parentMock = invocation.MockMixin;
                var repository = parentMock.Repository;

                if (MustReturnMock(invocation) || this.type == RecursiveMockingBehaviorType.ReturnDefault)
                {
                    mock = CreateMock(returnType, repository, invocation);
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

        private bool MustReturnMock(Invocation invocation, bool checkPropertyOnTestFixture = false)
        {
            if (checkPropertyOnTestFixture)
            {
#if !LITE_EDITION
                var stackTrace = new StackTrace();
                var methodCallingArrange = stackTrace.EnumerateFrames()
                    .SkipWhile(m => !Attribute.IsDefined(m, typeof(ArrangeMethodAttribute)))
                    .SkipWhile(m => m.Module.Assembly == typeof(MocksRepository).Assembly)
                    .FirstOrDefault();

                if (methodCallingArrange != null && invocation.Method.DeclaringType.IsAssignableFrom(methodCallingArrange.DeclaringType))
                    return false;
#endif
            }

            // mock invocations in static constructors according to the behavior
            if (invocation.InRunClassConstructor)
            {
                return invocation.InArrange && !invocation.CallOriginal;
            }

            return invocation.InArrange && !invocation.InArrangeArgMatching || this.type == RecursiveMockingBehaviorType.ReturnMock;
        }

        private object CreateMock(Type returnType, MocksRepository repository, Invocation invocation)
        {
            var parentMock = invocation.MockMixin;
            var replicator = parentMock as IMockReplicator;

            object mock = null;
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

            if (mock == null && typeof(Task).IsAssignableFrom(returnType))
            {
                var elementType = returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>)
                    ? returnType.GetGenericArguments()[0] : typeof(object);
                var taskResultValue = MustReturnMock(invocation)
                    ? CreateMock(elementType, repository, invocation)
                    : elementType.GetDefaultValue();

                var taskFromResultMethod = typeof(MockingUtil).GetMethod("TaskFromResult", BindingFlags.Static | BindingFlags.Public);

                mock = taskFromResultMethod
                    .MakeGenericMethod(elementType)
                    .Invoke(null, new object[] { taskResultValue });
            }

#if !PORTABLE
            if (mock == null && returnType.IsByRef)
            {
                var delegateType = typeof(object).Assembly.GetType("Telerik.JustMock.RefDelegate`1").MakeGenericType(new [] { returnType.GetElementType() });
                ConstructorInfo constructor = delegateType.GetConstructor(new[] { typeof(object), typeof(IntPtr) });

                MethodInfo genericMethodInfo = this.GetType().GetMethod("GetDefaultRef", BindingFlags.NonPublic | BindingFlags.Instance);
                MethodInfo methodInfo = genericMethodInfo.MakeGenericMethod(returnType.GetElementType());

                mock = constructor.Invoke(new object[] { this, methodInfo.MethodHandle.GetFunctionPointer() });
            }
#endif

            if (mock == null && MustReturnMock(invocation, checkPropertyOnTestFixture: true))
            {
                if (typeof(String) == returnType)
                {
                    mock = String.Empty;
                }
                else if (returnType.IsValueType)
                {
                    mock = returnType.GetDefaultValue();
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

            return mock;
        }

#if !PORTABLE
        ref T GetDefaultRef<T>()
        {
            return ref DefaultRef<T>.Ref();
        }
#endif
    }

#if !PORTABLE
    public sealed class DefaultRef<T>
    {
        static T value;

        public static ref T Ref() { return ref DefaultRef<T>.value; }
    }
#endif
}
