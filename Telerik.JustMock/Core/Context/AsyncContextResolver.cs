namespace Telerik.JustMock.Core.Context
{
	public static class AsyncContextResolver
    {
#if NETCORE
        static AsyncLocalWrapper resolver = new AsyncLocalWrapper();
#else
        static CallContextWrapper resolver = new CallContextWrapper();
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
