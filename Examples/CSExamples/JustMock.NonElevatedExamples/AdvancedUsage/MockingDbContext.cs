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
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.NonElevatedExamples.AdvancedUsage.MockingDbContext
{
    /// <summary>
    /// Entity Framework Core contexts expose virtual members that can be arranged with JustMock Lite.
    /// An in-memory context supplies a real DbSet while the context itself remains isolated from the test.
    /// </summary>
    [TestClass]
    public class MockingDbContext_Tests
    {
        [TestMethod]
        public void ShouldUseArrangedDbSetForQuery()
        {
            using (var backingContext = CatalogContext.CreateInMemory())
            {
                backingContext.Products.AddRange(
                    new Product { Id = 1, Name = "Keyboard", Category = "Hardware", IsFeatured = false },
                    new Product { Id = 2, Name = "Monitor", Category = "Hardware", IsFeatured = true },
                    new Product { Id = 3, Name = "Notebook", Category = "Office", IsFeatured = true });
                backingContext.SaveChanges();

                var context = Mock.Create<CatalogContext>();
                Mock.Arrange(() => context.Products).Returns(backingContext.Products);

                var actual = new ProductCatalog(context).FindFeaturedProduct("Hardware");

                Assert.IsNotNull(actual);
                Assert.AreEqual(2, actual.Id);
            }
        }

        [TestMethod]
        public void ShouldArrangeSaveChangesWithoutWritingToDatabase()
        {
            using (var backingContext = CatalogContext.CreateInMemory())
            {
                var context = Mock.Create<CatalogContext>();
                Mock.Arrange(() => context.Products).Returns(backingContext.Products);
                Mock.Arrange(() => context.SaveChanges()).Returns(1);

                var actual = new ProductWriter(context).Add(new Product
                {
                    Id = 4,
                    Name = "Mouse",
                    Category = "Hardware"
                });

                Assert.AreEqual(1, actual);
                Assert.AreEqual(1, backingContext.Products.Local.Count);
            }
        }
    }

    public class CatalogContext : DbContext
    {
        public CatalogContext()
        {
        }

        public CatalogContext(DbContextOptions<CatalogContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }

        public static CatalogContext CreateInMemory()
        {
            var options = new DbContextOptionsBuilder<CatalogContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new CatalogContext(options);
        }
    }

    public class ProductCatalog
    {
        private readonly CatalogContext context;

        public ProductCatalog(CatalogContext context)
        {
            this.context = context;
        }

        public Product FindFeaturedProduct(string category)
        {
            return this.context.Products
                .Where(product => product.Category == category && product.IsFeatured)
                .OrderBy(product => product.Id)
                .FirstOrDefault();
        }
    }

    public class ProductWriter
    {
        private readonly CatalogContext context;

        public ProductWriter(CatalogContext context)
        {
            this.context = context;
        }

        public int Add(Product product)
        {
            this.context.Products.Add(product);
            return this.context.SaveChanges();
        }
    }

    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public bool IsFeatured { get; set; }
    }
}
