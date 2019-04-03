using System.Reflection;

namespace Telerik.JustMock.Core.Context
{
    interface IAsyncContextResolver
    {
        void SetContext(CallPattern callPattern);
        MethodBase GetContext();
    }
}
