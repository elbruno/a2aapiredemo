using Microsoft.EntityFrameworkCore;
using Products.Models;
using DataEntities;

namespace Products.Tests
{
    [TestClass]
    public sealed class DbInitializerTests
    {
        private DbContextOptions<Context> _dbOptions;

        [TestInitialize]
        public void TestInit()
        {
            // Use a unique database name for each test run to ensure isolation
            _dbOptions = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [TestMethod]
        public void DbInitializer_Initialize_CreatesSeedProducts()
        {
            // Arrange
            using var context = new Context(_dbOptions);

            // Act
            DbInitializer.Initialize(context);

            // Assert
            var products = context.Product.ToList();
            Assert.IsTrue(products.Count > 0, "Should have seeded products");
            
            // Verify some expected products exist
            Assert.IsTrue(products.Any(p => p.Name.Contains("Paint")), "Should contain paint products");
            Assert.IsTrue(products.Any(p => p.Name.Contains("Drill")), "Should contain drill products");
            Assert.IsTrue(products.Any(p => p.Price > 0), "All products should have positive prices");
            
            // Verify no duplicates are created on second call
            var initialCount = products.Count;
            DbInitializer.Initialize(context);
            var productsAfterSecondCall = context.Product.ToList();
            Assert.AreEqual(initialCount, productsAfterSecondCall.Count, "Should not create duplicates on second initialization");
        }
    }
}