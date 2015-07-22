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
			await controller.DoStuff(model);
			//no exception
		}

		[TestMethod, TestCategory("Async"), TestCategory("Lite")]
		public void ShouldCreateRecursiveMockInConstructorOnAnotherThread()
		{
			var mock = Mock.Create<ThreadCtor>(Constructor.NotMocked, Behavior.RecursiveLoose);
			Assert.NotNull(mock.TheItem);
		}

		public abstract class ThreadCtor
		{
			protected abstract IDisposable Item { get; }

			public ThreadCtor()
			{
				var thread = new Thread(() => TheItem = Item);
				thread.Start();
				if (!thread.Join(TimeSpan.FromMilliseconds(1500)))
					throw new TimeoutException();
			}

			public IDisposable TheItem;
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
