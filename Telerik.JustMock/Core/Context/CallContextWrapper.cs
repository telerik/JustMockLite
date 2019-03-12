using System;
using System.Reflection;

#if !NETCORE
using System.Runtime.Remoting.Messaging;
#endif

namespace Telerik.JustMock.Core.Context
{
    internal class CallContextWrapper
    {
        private static readonly string key = Guid.NewGuid().ToString("N");

        internal static MethodBase GetData()
        {
#if !NETCORE
            MethodBase methodBase = CallContext.LogicalGetData(key) as MethodBase;

            if(methodBase != null)
            {

            }

            return methodBase;
#else
            return null;
#endif
        }

        internal static void SetAsyncStaticMockingData(CallPattern callPattern)
        {
            if (callPattern.Method.IsStatic)
            {
                MethodBase testMethod = MockingContext.GetTestMethod();
                SetData(testMethod);
            }
        }

        private static void SetData(MethodBase methodBase)
        {
#if !NETCORE
            if (methodBase != null)
            {
                CallContext.LogicalSetData(key, methodBase);
            }
#endif
        }
    }
}
