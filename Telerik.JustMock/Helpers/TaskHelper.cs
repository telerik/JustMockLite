using System.Threading.Tasks;
using Telerik.JustMock.Core;
using Telerik.JustMock.Expectations.Abstraction;

namespace Telerik.JustMock
{
	public static class TaskHelper
	{
		public static IAssertable TaskResult<T>(this IFunc<Task<T>> expectation, T result)
		{
			return expectation.Returns(MockingUtil.TaskFromResult(result));
		}
	}
}
