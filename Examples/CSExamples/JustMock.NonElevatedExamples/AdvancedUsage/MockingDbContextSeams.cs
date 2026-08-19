using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.NonElevatedExamples.AdvancedUsage.MockingDbContextSeams
{
    /// <summary>
    /// JustMock Lite cannot intercept constructors, non-public members, or sealed types.
    /// An explicit virtual or interface seam keeps the same workflow testable without
    /// elevated interception.
    /// </summary>
    [TestClass]
    public class MockingDbContextSeams_Tests
    {
        [TestMethod]
        public void ShouldUseVirtualContextSeamForQueries()
        {
            using (var backingContext = SeamOrderContext.CreateInMemory("MockingDbContextSeams.Virtual"))
            {
                backingContext.Orders.AddRange(
                    new SeamOrder { Id = 1, Number = "SO-100", Status = "Closed" },
                    new SeamOrder { Id = 2, Number = "SO-101", Status = "Open" });
                backingContext.SaveChanges();

                var context = Mock.Create<SeamOrderContext>();
                Mock.Arrange(() => context.Orders).Returns(backingContext.Orders);

                var actual = new VirtualOrderReader(context).FindOpenOrder();

                Assert.IsNotNull(actual);
                Assert.AreEqual("SO-101", actual.Number);
            }
        }

        [TestMethod]
        public void ShouldUseInterfaceContextSeamForWrites()
        {
            using (var backingContext = SeamOrderContext.CreateInMemory("MockingDbContextSeams.Interface"))
            {
                var context = Mock.Create<ISeamOrderContext>();
                Mock.Arrange(() => context.Orders).Returns(backingContext.Orders);
                Mock.Arrange(() => context.SaveChanges())
                    .Returns(1)
                    .MustBeCalled();

                var result = new InterfaceOrderWriter(context).Add(new SeamOrder
                {
                    Id = 3,
                    Number = "SO-102",
                    Status = "Open"
                });

                Assert.AreEqual(1, result);
                Assert.AreEqual(1, backingContext.Orders.Local.Count);
                Mock.Assert(context);
            }
        }
    }

    public class SeamOrderContext : DbContext
    {
        public SeamOrderContext()
        {
        }

        public SeamOrderContext(DbContextOptions<SeamOrderContext> options)
            : base(options)
        {
        }

        public virtual DbSet<SeamOrder> Orders { get; set; }

        public static SeamOrderContext CreateInMemory(string databaseName)
        {
            var options = new DbContextOptionsBuilder<SeamOrderContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
            var context = new SeamOrderContext(options);

            context.Database.EnsureDeleted();
            return context;
        }
    }

    public interface ISeamOrderContext
    {
        DbSet<SeamOrder> Orders { get; }

        int SaveChanges();
    }

    public class VirtualOrderReader
    {
        private readonly SeamOrderContext context;

        public VirtualOrderReader(SeamOrderContext context)
        {
            this.context = context;
        }

        public SeamOrder FindOpenOrder()
        {
            return context.Orders
                .Where(order => order.Status == "Open")
                .OrderBy(order => order.Id)
                .FirstOrDefault();
        }
    }

    public class InterfaceOrderWriter
    {
        private readonly ISeamOrderContext context;

        public InterfaceOrderWriter(ISeamOrderContext context)
        {
            this.context = context;
        }

        public int Add(SeamOrder order)
        {
            context.Orders.Add(order);
            return context.SaveChanges();
        }
    }

    public class SeamOrder
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public string Status { get; set; }
    }
}
