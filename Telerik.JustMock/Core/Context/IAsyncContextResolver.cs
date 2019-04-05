using System.Reflection;

namespace Telerik.JustMock.Core.Context
{
    public interface IAsyncContextResolver
    {
        void CaptureContext();
        MethodBase GetContext();
    }
}
