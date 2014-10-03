using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Telerik.JustMock.Tests
{
	[TestClass]
	public class AsyncFixture
	{
		[TestMethod, TestCategory("Async"), TestCategory("Lite")]
		public async Task ShouldRetainMockingContextInContinuation()
		{
			var model = Mock.Create<IAsyncModel>();
			Mock.Arrange(() => model.GetData()).Returns(5);
			var controller = new TestAsyncController();
			controller.DoStuff(model);
			//no exception
		}
	}

	public interface IAsyncModel
	{
		int GetData();
	}

	public class TestAsyncController
	{
		public async Task DoStuff(IAsyncModel model)
		{
			await Task.Factory.StartNew(() => Thread.Sleep(5));
			// continuation begins here
			var result = model.GetData();
			if (result != 5)
			{
				throw new Exception();
			}
		}
	}
}
