using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.NonElevatedExamples.AdvancedUsage.MockingDbSetQueries
{
    /// <summary>
    /// A DbSet can be arranged to an EF Core in-memory set so LINQ operators execute normally.
    /// </summary>
    [TestClass]
    public class MockingDbSetQueries_Tests
    {
        [TestMethod]
        public void ShouldRunQueryAgainstArrangedDbSet()
        {
            using (var backingContext = CustomerContext.CreateInMemory())
            {
                backingContext.Customers.AddRange(
                    new Customer { Id = 1, Name = "Ada", Region = "North", IsActive = true },
                    new Customer { Id = 2, Name = "Grace", Region = "North", IsActive = false },
                    new Customer { Id = 3, Name = "Linus", Region = "South", IsActive = true });
                backingContext.SaveChanges();

                var context = Mock.Create<CustomerContext>();
                Mock.Arrange(() => context.Customers).Returns(backingContext.Customers);

                var actual = new CustomerDirectory(context).GetActiveNames("North");

                CollectionAssert.AreEqual(new[] { "Ada" }, actual);
            }
        }
    }

    public class CustomerContext : DbContext
    {
        public CustomerContext()
        {
        }

        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }

        public static CustomerContext CreateInMemory()
        {
            var options = new DbContextOptionsBuilder<CustomerContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new CustomerContext(options);
        }
    }

    public class CustomerDirectory
    {
        private readonly CustomerContext context;

        public CustomerDirectory(CustomerContext context)
        {
            this.context = context;
        }

        public string[] GetActiveNames(string region)
        {
            return this.context.Customers
                .AsNoTracking()
                .Where(customer => customer.Region == region && customer.IsActive)
                .OrderBy(customer => customer.Name)
                .Select(customer => customer.Name)
                .ToArray();
        }
    }

    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Region { get; set; }

        public bool IsActive { get; set; }
    }
}
