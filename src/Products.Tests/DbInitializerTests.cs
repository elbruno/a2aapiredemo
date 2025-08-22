using Microsoft.EntityFrameworkCore;
using Products.Models;
using DataEntities;

namespace Products.Tests
{
    [TestClass]
    public sealed class DbInitializerTests
    {
        private DbContextOptions<Context> _dbOptions = null!;

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
            Assert.IsTrue(context.Product.Any(), "Products should be seeded");
            
            // Verify some expected products exist
            var products = context.Product.ToList();
            Assert.IsTrue(products.Count > 0, "Should have seeded products");
            
            // Verify specific products from the seed data
            Assert.IsTrue(products.Any(p => p.Name.Contains("Interior Wall Paint")), "Should contain Interior Wall Paint");
            Assert.IsTrue(products.Any(p => p.Name.Contains("Exterior Wood Stain")), "Should contain Exterior Wood Stain");
            
            // Verify all products have required properties
            foreach (var product in products)
            {
                Assert.IsFalse(string.IsNullOrEmpty(product.Name), "Product name should not be empty");
                Assert.IsFalse(string.IsNullOrEmpty(product.Description), "Product description should not be empty");
                Assert.IsTrue(product.Price > 0, "Product price should be greater than 0");
                Assert.IsFalse(string.IsNullOrEmpty(product.ImageUrl), "Product ImageUrl should not be empty");
            }
        }

        [TestMethod]
        public void DbInitializer_Initialize_DoesNotSeedIfProductsExist()
        {
            // Arrange
            using var context = new Context(_dbOptions);
            
            // Add a product manually first
            context.Product.Add(new Product 
            { 
                Name = "Existing Product", 
                Description = "Already exists", 
                Price = 10.00m, 
                ImageUrl = "existing.png" 
            });
            context.SaveChanges();
            
            var existingCount = context.Product.Count();

            // Act
            DbInitializer.Initialize(context);

            // Assert
            var finalCount = context.Product.Count();
            Assert.AreEqual(existingCount, finalCount, "Should not add more products when products already exist");
            Assert.IsTrue(context.Product.Any(p => p.Name == "Existing Product"), "Original product should still exist");
        }
    }
}