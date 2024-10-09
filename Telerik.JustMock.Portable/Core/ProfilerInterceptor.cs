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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Telerik.JustMock.Core
{
    internal static class ProfilerInterceptor
    {
        public static bool IsProfilerAttached
        {
            get { return false; }
        }

        public static bool IsInterceptionEnabled { get; set; }

        [ThreadStatic]
        public static int ReentrancyCounter;

        public static void Initialize()
        {
        }

        public static object CreateInstanceWithArgsImpl(Type type, object[] args)
        {
            throw new NotSupportedException();
        }

        public static object GetUninitializedObjectImpl(Type type)
        {
            throw new NotSupportedException();
        }

        public static void RegisterGlobalInterceptor(MethodBase method, MocksRepository repo)
        {
            throw new NotSupportedException();
        }

        public static void UnregisterGlobalInterceptor(MethodBase method, MocksRepository repo)
        {
            throw new NotSupportedException();
        }

        public static void EnableInterception(Type type, bool enabled, MocksRepository behalf)
        {
            throw new NotSupportedException();
        }

        public static void ThrowElevatedMockingException(MemberInfo member = null)
        {
            var ex = member != null ? new ElevatedMockingException(member) : new ElevatedMockingException();
            throw ex;
        }

        public static void CreateDelegateFromBridge<T>(string bridgeMethodName, out T delg)
        {
            throw new NotSupportedException();
        }

        public static void CheckIfSafeToInterceptWholesale(Type type)
        {
        }

        public static bool TypeSupportsInstrumentation(Type type)
        {
            return false;
        }

        public static void RunClassConstructor(RuntimeTypeHandle type)
        {
            RuntimeHelpers.RunClassConstructor(type);
        }

        [DebuggerHidden]
        public static void GuardInternal(Action guardedAction)
        {
            try
            {
                ReentrancyCounter++;
                guardedAction();
            }
            finally
            {
                ReentrancyCounter--;
            }
        }

        [DebuggerHidden]
        public static T GuardInternal<T>(Func<T> guardedAction)
        {
            try
            {
                ReentrancyCounter++;
                return guardedAction();
            }
            finally
            {
                ReentrancyCounter--;
            }
        }

        [DebuggerHidden]
        public static void GuardExternal(Action guardedAction)
        {
            var oldCounter = ReentrancyCounter;
            try
            {
                ReentrancyCounter = 0;
                guardedAction();
            }
            finally
            {
                ReentrancyCounter = oldCounter;
            }
        }

        [DebuggerHidden]
        public static T GuardExternal<T>(Func<T> guardedAction)
        {
            var oldCounter = ReentrancyCounter;
            try
            {
                ReentrancyCounter = 0;
                return guardedAction();
            }
            finally
            {
                ReentrancyCounter = oldCounter;
            }
        }
    }
}
