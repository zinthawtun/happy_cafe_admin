using Moq;
using Resource.Interfaces;
using Service.Handlers.Cafes;
using Service.Queries.Cafes;

namespace Tests.Service.Handlers
{
    public class ExistsCafeByNameQueryHandlerTests
    {
        private readonly Mock<ICafeResource> cafeResourceMock;
        private readonly ExistsCafeByNameQueryHandler handler;

        public ExistsCafeByNameQueryHandlerTests()
        {
            cafeResourceMock = new Mock<ICafeResource>();
            handler = new ExistsCafeByNameQueryHandler(cafeResourceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnTrue_WhenCafeNameExists_Test()
        {
            string cafeName = "Existing Cafe";
            ExistsCafeByNameQuery query = new ExistsCafeByNameQuery { Name = cafeName };

            cafeResourceMock
                .Setup(r => r.ExistsByNameAsync(cafeName))
                .ReturnsAsync(true);

            bool result = await handler.Handle(query, CancellationToken.None);

            Assert.True(result);
            cafeResourceMock.Verify(r => r.ExistsByNameAsync(cafeName), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCafeNameDoesNotExist_Test()
        {
            string cafeName = "Non-existent Cafe";
            ExistsCafeByNameQuery query = new ExistsCafeByNameQuery { Name = cafeName };

            cafeResourceMock
                .Setup(r => r.ExistsByNameAsync(cafeName))
                .ReturnsAsync(false);

            bool result = await handler.Handle(query, CancellationToken.None);

            Assert.False(result);
            cafeResourceMock.Verify(r => r.ExistsByNameAsync(cafeName), Times.Once);
        }
    }
} 