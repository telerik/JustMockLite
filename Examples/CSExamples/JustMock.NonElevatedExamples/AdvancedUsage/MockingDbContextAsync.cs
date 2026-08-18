/*
 JustMock Lite
 Copyright © 2010-2024 Progress Software Corporation

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.NonElevatedExamples.AdvancedUsage.MockingDbContextAsync
{
    /// <summary>
    /// Virtual EF Core members can be arranged with JustMock Lite while asynchronous
    /// queries and persistence use the EF Core InMemory provider.
    /// </summary>
    [TestClass]
    public class MockingDbContextAsync_Tests
    {
        [TestMethod]
        public async Task ShouldQueryArrangedVirtualDbSetAsynchronously()
        {
            using (var backingContext = AsyncCatalogContext.CreateInMemory("MockingDbContextAsync.Query"))
            {
                backingContext.Products.AddRange(
                    new AsyncProduct { Id = 1, Name = "Keyboard", Category = "Hardware", IsFeatured = true },
                    new AsyncProduct { Id = 2, Name = "Monitor", Category = "Hardware", IsFeatured = false },
                    new AsyncProduct { Id = 3, Name = "Mouse", Category = "Hardware", IsFeatured = true },
                    new AsyncProduct { Id = 4, Name = "Notebook", Category = "Office", IsFeatured = true });
                await backingContext.SaveChangesAsync();

                var context = Mock.Create<AsyncCatalogContext>();
                Mock.Arrange(() => context.Products).Returns(backingContext.Products);

                var actual = await new AsyncProductCatalog(context).FindFeaturedNamesAsync("Hardware");

                CollectionAssert.AreEqual(new[] { "Keyboard", "Mouse" }, actual);
            }
        }

        [TestMethod]
        public async Task ShouldSaveThroughArrangedVirtualContextAsynchronously()
        {
            using (var backingContext = AsyncCatalogContext.CreateInMemory("MockingDbContextAsync.Save"))
            {
                var context = Mock.Create<AsyncCatalogContext>();
                Mock.Arrange(() => context.Products).Returns(backingContext.Products);
                Mock.Arrange(() => context.SaveChangesAsync(Arg.IsAny<CancellationToken>()))
                    .ReturnsAsync(1)
                    .MustBeCalled();

                var result = await new AsyncProductWriter(context).AddAsync(new AsyncProduct
                {
                    Id = 5,
                    Name = "Dock",
                    Category = "Hardware",
                    IsFeatured = false
                });

                Assert.AreEqual(1, result);
                Assert.AreEqual(1, backingContext.Products.Local.Count);
                Mock.Assert(context);
            }
        }

        [TestMethod]
        public async Task ShouldVerifyAsyncSaveOccurrence()
        {
            using (var backingContext = AsyncCatalogContext.CreateInMemory("MockingDbContextAsync.Occurrence"))
            {
                var context = Mock.Create<AsyncCatalogContext>();
                Mock.Arrange(() => context.Products).Returns(backingContext.Products);
                Mock.Arrange(() => context.SaveChangesAsync(Arg.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                var writer = new AsyncProductWriter(context);
                await writer.AddAsync(new AsyncProduct { Id = 6, Name = "Tablet", Category = "Hardware" });
                await writer.AddAsync(new AsyncProduct { Id = 7, Name = "Planner", Category = "Office" });

                Mock.Assert(
                    () => context.SaveChangesAsync(Arg.IsAny<CancellationToken>()),
                    Occurs.Exactly(2));
            }
        }
    }

    public class AsyncCatalogContext : DbContext
    {
        public AsyncCatalogContext()
        {
        }

        public AsyncCatalogContext(DbContextOptions<AsyncCatalogContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AsyncProduct> Products { get; set; }

        public static AsyncCatalogContext CreateInMemory(string databaseName)
        {
            var options = new DbContextOptionsBuilder<AsyncCatalogContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
            var context = new AsyncCatalogContext(options);

            context.Database.EnsureDeleted();
            return context;
        }
    }

    public class AsyncProductCatalog
    {
        private readonly AsyncCatalogContext context;

        public AsyncProductCatalog(AsyncCatalogContext context)
        {
            this.context = context;
        }

        public async Task<string[]> FindFeaturedNamesAsync(
            string category,
            CancellationToken cancellationToken = default)
        {
            return await context.Products
                .AsNoTracking()
                .Where(product => product.Category == category && product.IsFeatured)
                .OrderBy(product => product.Name)
                .Select(product => product.Name)
                .ToArrayAsync(cancellationToken);
        }
    }

    public class AsyncProductWriter
    {
        private readonly AsyncCatalogContext context;

        public AsyncProductWriter(AsyncCatalogContext context)
        {
            this.context = context;
        }

        public async Task<int> AddAsync(
            AsyncProduct product,
            CancellationToken cancellationToken = default)
        {
            await context.Products.AddAsync(product, cancellationToken);
            return await context.SaveChangesAsync(cancellationToken);
        }
    }

    public class AsyncProduct
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public bool IsFeatured { get; set; }
    }
}
