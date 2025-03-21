using AutoMapper;
using Moq;
using Resource.Interfaces;
using Service.Commands.Cafes;
using Service.Handlers.Cafes;

namespace Tests.Service.Handlers
{
    public class DeleteCafeCommandHandlerTests
    {
        private readonly Mock<ICafeResource> cafeResourceMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly DeleteCafeCommandHandler handler;

        public DeleteCafeCommandHandlerTests()
        {
            cafeResourceMock = new Mock<ICafeResource>();
            mapperMock = new Mock<IMapper>();
            handler = new DeleteCafeCommandHandler(cafeResourceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCallDeleteAsync_AndReturnTrue_WhenCafeExists_Test()
        {
            Guid cafeId = Guid.NewGuid();
            DeleteCafeCommand command = new DeleteCafeCommand { Id = cafeId };

            cafeResourceMock
                .Setup(r => r.DeleteAsync(cafeId))
                .ReturnsAsync(true);

            bool result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result);
            cafeResourceMock.Verify(r => r.DeleteAsync(cafeId), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCallDeleteAsync_AndReturnFalse_WhenCafeDoesNotExist_Test()
        {
            Guid cafeId = Guid.NewGuid();
            DeleteCafeCommand command = new DeleteCafeCommand { Id = cafeId };

            cafeResourceMock
                .Setup(r => r.DeleteAsync(cafeId))
                .ReturnsAsync(false);

            bool result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            cafeResourceMock.Verify(r => r.DeleteAsync(cafeId), Times.Once);
        }
    }
} 