#if !NETCORE
using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace Telerik.JustMock.Core.Context
{
    public class CallContextWrapper : IAsyncContextResolver
    {
        private static readonly string key = Guid.NewGuid().ToString("N");

        public MethodBase GetContext()
        {
            MethodBase methodBase = CallContext.LogicalGetData(key) as MethodBase;
            return methodBase;
        }

        public void CaptureContext()
        {
            MethodBase testMethod = MockingContext.GetTestMethod();
            SetData(testMethod);
        }

        private void SetData(MethodBase methodBase)
        {
            if (methodBase != null)
            {
                CallContext.LogicalSetData(key, methodBase);
            }
        }
    }
}
#endif
