using Microsoft.EntityFrameworkCore;
using Products.Models;
using DataEntities;
using Products.Endpoints;
using Products.Memory;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.AI;
using SearchEntities;

namespace Products.Tests
{
    [TestClass]
    public sealed class ProductAiActionsTests
    {
        private DbContextOptions<Context> _dbOptions = null!;
        private Mock<ILogger<MemoryContext>> _mockLogger = null!;
        private Mock<IChatClient> _mockChatClient = null!;
        private Mock<IEmbeddingGenerator<string, Embedding<float>>> _mockEmbeddingGenerator = null!;

        [TestInitialize]
        public void TestInit()
        {
            // Use a unique database name for each test run to ensure isolation
            _dbOptions = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockLogger = new Mock<ILogger<MemoryContext>>();
            _mockChatClient = new Mock<IChatClient>();
            _mockEmbeddingGenerator = new Mock<IEmbeddingGenerator<string, Embedding<float>>>();
        }

        [TestMethod]
        public async Task AISearch_ReturnsOk_WithSearchResponse()
        {
            // Arrange
            using var context = new Context(_dbOptions);
            
            var mockMemoryContext = new Mock<MemoryContext>(_mockLogger.Object, _mockChatClient.Object, _mockEmbeddingGenerator.Object);
            var expectedResponse = new SearchResponse 
            { 
                Response = "Test search response",
                Products = new List<Product>()
            };
            
            mockMemoryContext.Setup(m => m.Search(It.IsAny<string>(), It.IsAny<Context>()))
                           .ReturnsAsync(expectedResponse);

            // Act
            var result = await ProductAiActions.AISearch("test search", context, mockMemoryContext.Object);

            // Assert
            var okResult = result as Microsoft.AspNetCore.Http.HttpResults.Ok<SearchResponse>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedResponse.Response, okResult.Value.Response);
            mockMemoryContext.Verify(m => m.Search("test search", context), Times.Once);
        }
    }
}