using Microsoft.EntityFrameworkCore;
using Products.Models;
using DataEntities;
using Products.Memory;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.AI;
using SearchEntities;

namespace Products.Tests
{
    [TestClass]
    public sealed class MemoryContextTests
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
        public void MemoryContext_Constructor_InitializesCorrectly()
        {
            // Act
            var memoryContext = new MemoryContext(_mockLogger.Object, _mockChatClient.Object, _mockEmbeddingGenerator.Object);

            // Assert
            Assert.IsNotNull(memoryContext);
            Assert.AreEqual(_mockChatClient.Object, memoryContext._chatClient);
            Assert.AreEqual(_mockEmbeddingGenerator.Object, memoryContext._embeddingGenerator);
        }

        [TestMethod]
        public async Task Search_ReturnsErrorResponse_OnNullEmbeddingGenerator()
        {
            // Arrange
            using var context = new Context(_dbOptions);
            
            // Create MemoryContext with null embedding generator to trigger error
            var memoryContext = new MemoryContext(_mockLogger.Object, _mockChatClient.Object, null);

            // Act
            var result = await memoryContext.Search("test search", context);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Response.Contains("An error occurred") || result.Response.Contains("I don't know the answer"));
        }

        [TestMethod]
        public async Task Search_ReturnsDefaultResponse_WithValidInputs()
        {
            // Arrange
            using var context = new Context(_dbOptions);
            
            // Create a real MemoryContext but with mocked dependencies
            // This will test the basic flow without complex mocking
            var memoryContext = new MemoryContext(_mockLogger.Object, _mockChatClient.Object, _mockEmbeddingGenerator.Object);

            // Act
            var result = await memoryContext.Search("test search", context);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Response);
            Assert.IsNotNull(result.Products);
        }
    }
}