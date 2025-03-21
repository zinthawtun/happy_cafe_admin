using AutoMapper;
using Business.Entities;
using Moq;
using Resource.Interfaces;
using Service.Commands.Cafes;
using Service.Handlers.Cafes;

namespace Tests.Service.Handlers
{
    public class UpdateCafeCommandHandlerTests
    {
        private readonly Mock<ICafeResource> cafeResourceMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly UpdateCafeCommandHandler handler;

        public UpdateCafeCommandHandlerTests()
        {
            cafeResourceMock = new Mock<ICafeResource>();
            mapperMock = new Mock<IMapper>();
            handler = new UpdateCafeCommandHandler(cafeResourceMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateCafe_AndReturnUpdatedCafe_Test()
        {
            Guid cafeId = Guid.NewGuid();
            UpdateCafeCommand command = new UpdateCafeCommand
            {
                Id = cafeId,
                Name = "Updated Cafe",
                Description = "Updated Description",
                Location = "Updated Location",
                Logo = "Updated Logo"
            };

            Cafe updatedCafe = new Cafe(cafeId, command.Name, command.Description, command.Logo, command.Location);

            cafeResourceMock
                .Setup(r => r.UpdateAsync(cafeId, command.Name, command.Description, command.Logo, command.Location))
                .ReturnsAsync(updatedCafe);

            Cafe? result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(cafeId, result.Id);
            Assert.Equal(command.Name, result.Name);
            Assert.Equal(command.Description, result.Description);
            Assert.Equal(command.Location, result.Location);
            Assert.Equal(command.Logo, result.Logo);

            cafeResourceMock.Verify(r => r.UpdateAsync(cafeId, command.Name, command.Description, command.Logo, command.Location), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenCafeDoesNotExist_Test()
        {
            Guid cafeId = Guid.NewGuid();
            UpdateCafeCommand command = new UpdateCafeCommand
            {
                Id = cafeId,
                Name = "Updated Cafe",
                Description = "Updated Description",
                Location = "Updated Location",
                Logo = "Updated Logo"
            };

            cafeResourceMock
                .Setup(r => r.UpdateAsync(cafeId, command.Name, command.Description, command.Logo, command.Location))
                .ReturnsAsync((Cafe?)null);

            Cafe? result = await handler.Handle(command, CancellationToken.None);

            Assert.Null(result);

            cafeResourceMock.Verify(r => r.UpdateAsync(cafeId, command.Name, command.Description, command.Logo, command.Location), Times.Once);
        }
    }
} 