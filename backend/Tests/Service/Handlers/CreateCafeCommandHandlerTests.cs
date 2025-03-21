using AutoMapper;
using Business.Entities;
using Moq;
using Resource.Interfaces;
using Service.Commands.Cafes;
using Service.Handlers.Cafes;

namespace Tests.Service.Handlers
{
    public class CreateCafeCommandHandlerTests
    {
        private readonly Mock<ICafeResource> cafeResourceMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly CreateCafeCommandHandler handler;

        public CreateCafeCommandHandlerTests()
        {
            cafeResourceMock = new Mock<ICafeResource>();
            mapperMock = new Mock<IMapper>();
            handler = new CreateCafeCommandHandler(cafeResourceMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateCafe_AndReturnCreatedCafe_Test()
        {
            CreateCafeCommand command = new CreateCafeCommand
            {
                Name = "Test Cafe",
                Description = "Test Description",
                Location = "Test Location"
            };

            Cafe cafe = new Cafe(Guid.NewGuid(), command.Name, command.Description, string.Empty, command.Location);

            Cafe createdCafe = new Cafe(Guid.NewGuid(), command.Name, command.Description, string.Empty, command.Location);

            mapperMock
                .Setup(m => m.Map<Cafe>(command))
                .Returns(cafe);

            cafeResourceMock
                .Setup(r => r.CreateAsync(command.Name, command.Description, It.IsAny<string>(), command.Location))
                .ReturnsAsync(createdCafe);

            Cafe result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(createdCafe.Id, result.Id);
            Assert.Equal(command.Name, result.Name);
            Assert.Equal(command.Description, result.Description);
            Assert.Equal(command.Location, result.Location);

            cafeResourceMock.Verify(r => r.CreateAsync(command.Name, command.Description, string.Empty, command.Location), Times.Once);
        }
    }
} 