using Microsoft.EntityFrameworkCore;
using Products.Models;
using DataEntities;
using Products.Endpoints;
using Products.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.AI;
using Moq;
using SearchEntities;

namespace Products.Tests
{
    [TestClass]
    public sealed class ProductAiActionsTests
    {
        private DbContextOptions<Context> _dbOptions;
        private Mock<ILogger> _mockLogger;
        private Mock<IChatClient> _mockChatClient;
        private Mock<IEmbeddingGenerator<string, Embedding<float>>> _mockEmbeddingGenerator;

        [TestInitialize]
        public void TestInit()
        {
            // Use a unique database name for each test run to ensure isolation
            _dbOptions = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockLogger = new Mock<ILogger>();
            _mockChatClient = new Mock<IChatClient>();
            _mockEmbeddingGenerator = new Mock<IEmbeddingGenerator<string, Embedding<float>>>();
        }

        [TestMethod]
        public async Task AISearch_ReturnsOk_WithSearchResponse()
        {
            // Arrange
            using var context = new Context(_dbOptions);
            context.Product.AddRange(new List<Product>
            {
                new Product { Id = 1, Name = "Camping Tent", Description = "Waterproof tent", Price = 150, ImageUrl = "tent.jpg" },
                new Product { Id = 2, Name = "Sleeping Bag", Description = "Warm sleeping bag", Price = 80, ImageUrl = "bag.jpg" }
            });
            context.SaveChanges();

            var mockMemoryContext = new Mock<MemoryContext>(_mockLogger.Object, _mockChatClient.Object, _mockEmbeddingGenerator.Object);
            var expectedResponse = new SearchResponse 
            { 
                Response = "Found camping products for you!",
                Products = new List<Product> { new Product { Id = 1, Name = "Camping Tent", Description = "Waterproof tent", Price = 150, ImageUrl = "tent.jpg" } }
            };

            mockMemoryContext.Setup(m => m.Search(It.IsAny<string>(), It.IsAny<Context>()))
                           .ReturnsAsync(expectedResponse);

            // Act
            var result = await ProductAiActions.AISearch("camping", context, mockMemoryContext.Object);
            
            // Assert
            var okResult = result as Microsoft.AspNetCore.Http.HttpResults.Ok<SearchResponse>;
            Assert.IsNotNull(okResult, "Result should be Ok with SearchResponse");
            Assert.AreEqual("Found camping products for you!", okResult.Value.Response);
            Assert.AreEqual(1, okResult.Value.Products.Count);
            Assert.AreEqual("Camping Tent", okResult.Value.Products[0].Name);
        }
    }
}