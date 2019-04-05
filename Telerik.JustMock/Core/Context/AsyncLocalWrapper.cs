#if NETCORE

using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Telerik.JustMock.Core.Context
{
    internal class AsyncLocalWrapper : IAsyncContextResolver
    {
        static AsyncLocal<MethodBase> asyncCallPattern = new AsyncLocal<MethodBase>();
        public void CaptureContext()
        {
            MethodBase testMethod = MockingContext.GetTestMethod();
			asyncCallPattern.Value = testMethod;
        }

        public MethodBase GetContext()
        {
			return asyncCallPattern.Value;
        }
    }
}
#endif

