namespace Telerik.JustMock.Core.Context
{
	public static class AsyncContextResolver
    {
#if NETCORE
        static IAsyncContextResolver resolver = new AsyncLocalWrapper();
#else
        static IAsyncContextResolver resolver = new CallContextWrapper();
#endif
        public static IAsyncContextResolver GetResolver()
        {
            return resolver;
        }

        public static void CaptureContext()
        {
            resolver.CaptureContext();
        }
    }
}
