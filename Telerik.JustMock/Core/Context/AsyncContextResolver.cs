namespace Telerik.JustMock.Core.Context
{
	internal static class AsyncContextResolver
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
    }
}
