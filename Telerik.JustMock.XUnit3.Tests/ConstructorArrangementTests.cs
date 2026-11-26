using System.Threading;
using Telerik.JustMock;
using Xunit;

namespace Telerik.JustMock.XUnit3.Tests
{
    public class ConstructorArrangementTests
    {
        private static readonly IValueService SharedService = Mock.Create<IValueService>();
        private static int nextValue;
        private readonly int expectedValue;

        public ConstructorArrangementTests()
        {
            this.expectedValue = Interlocked.Increment(ref nextValue);
            Mock.Arrange(() => SharedService.GetValue(Arg.AnyInt)).Returns(this.expectedValue);
        }

        [Fact]
        public void ConstructorArrangementIsAvailableInFact()
        {
            Assert.Equal(this.expectedValue, SharedService.GetValue(0));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ConstructorArrangementIsIsolatedPerTheoryRow(int argument)
        {
            Assert.Equal(this.expectedValue, SharedService.GetValue(argument));
        }
    }

    public interface IValueService
    {
        int GetValue(int key);
    }
}
