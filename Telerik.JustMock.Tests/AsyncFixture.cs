using System;
using System.Threading;
using System.Threading.Tasks;

#region JustMock Test Attributes
#if NUNIT
using NUnit.Framework;
using TestCategory = NUnit.Framework.CategoryAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using AssertionException = NUnit.Framework.AssertionException;
#elif XUNIT
using Xunit;
using Telerik.JustMock.XUnit.Test.Attributes;
using TestCategory = Telerik.JustMock.XUnit.Test.Attributes.XUnitCategoryAttribute;
using TestClass = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestClassAttribute;
using TestMethod = Xunit.FactAttribute;
using TestInitialize = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestInitializeAttribute;
using TestCleanup = Telerik.JustMock.XUnit.Test.Attributes.EmptyTestCleanupAttribute;
#if XUNIT2
using AssertionException = Xunit.Sdk.XunitException;
#else
using AssertionException = Xunit.Sdk.AssertException;
#endif
#elif VSTEST_PORTABLE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AssertionException = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AssertFailedException;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
#endif
#endregion

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
			Mock.Arrange(() => Arg.IsAny<ThreadCtor>().Assign()).CallOriginal();
			var mock = Mock.Create<ThreadCtor>(Constructor.NotMocked, Behavior.RecursiveLoose);
			Assert.NotNull(mock.TheItem);
		}

		public abstract class ThreadCtor
		{
			protected abstract IDisposable Item { get; }

			public ThreadCtor()
			{
				var thread = new Thread(this.Assign);
				thread.Start();
				if (!thread.Join(TimeSpan.FromMilliseconds(1500)))
					throw new TimeoutException();
			}

			public virtual void Assign()
			{
				TheItem = Item;
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
