using Telerik.JustMock.Core;
using Telerik.JustMock.Expectations.Abstraction.Local;
using Telerik.JustMock.Expectations.Abstraction.Local.Function;

namespace Telerik.JustMock.Expectations
{
	internal sealed class LocalExpectation : ILocalExpectation
	{
		public IFunctionExpectation Function {
			get
			{
				return ProfilerInterceptor.GuardInternal(() => new FunctionExpectation());
			}
		}
	}
}