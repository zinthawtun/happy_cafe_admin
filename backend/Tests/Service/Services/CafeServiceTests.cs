using AutoMapper;
using Business.Entities;
using MediatR;
using Moq;
using Service.Commands.Cafes;
using Service.Queries.Cafes;
using Service.Services;

namespace Tests.Service.Services
{
    public class CafeServiceTests
    {
        private readonly Mock<IMediator> mediatorMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly CafeService cafeService;

        public CafeServiceTests()
        {
            mediatorMock = new Mock<IMediator>();
            mapperMock = new Mock<IMapper>();
            cafeService = new CafeService(mediatorMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCafe_WhenCafeExists_Test()
        {
            Guid cafeId = Guid.NewGuid();
            CafeDto cafeDto = new CafeDto { Id = cafeId, Name = "Test Cafe" };
            Cafe cafe = new Cafe(cafeId, "Test Cafe", "Description", "Logo", "Location");

            mediatorMock
                .Setup(m => m.Send(It.Is<GetCafeByIdQuery>(q => q.Id == cafeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cafeDto);

            mapperMock
                .Setup(m => m.Map<Cafe>(cafeDto))
                .Returns(cafe);

            Cafe? result = await cafeService.GetByIdAsync(cafeId);

            Assert.NotNull(result);
            Assert.Equal(cafeId, result.Id);
            Assert.Equal("Test Cafe", result.Name);

            mediatorMock.Verify(m => m.Send(It.Is<GetCafeByIdQuery>(q => q.Id == cafeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenCafeDoesNotExist()
        {
            Guid cafeId = Guid.NewGuid();

            mediatorMock
                .Setup(m => m.Send(It.Is<GetCafeByIdQuery>(q => q.Id == cafeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CafeDto?)null);

            Cafe? result = await cafeService.GetByIdAsync(cafeId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByLocationAsync_ShouldReturnCafes_WhenCafesExist_Test()
        {
            string location = "New York";
            List<CafeDto> cafeDtos = new List<CafeDto>
            {
                new CafeDto { Id = Guid.NewGuid(), Name = "Cafe 1", Location = location },
                new CafeDto { Id = Guid.NewGuid(), Name = "Cafe 2", Location = location }
            };

            IEnumerable<Cafe> cafes = new List<Cafe>
            {
                new Cafe(cafeDtos[0].Id, "Cafe 1", "Description 1", "Logo 1", location),
                new Cafe(cafeDtos[1].Id, "Cafe 2", "Description 2", "Logo 2", location)
            };

            mediatorMock
                .Setup(m => m.Send(It.Is<GetCafesByLocationQuery>(q => q.Location == location), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cafeDtos);

            mapperMock
                .Setup(m => m.Map<IEnumerable<Cafe>>(cafeDtos))
                .Returns(cafes);

            IEnumerable<Cafe> result = await cafeService.GetByLocationAsync(location);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "Cafe 1");
            Assert.Contains(result, c => c.Name == "Cafe 2");

            mediatorMock.Verify(m => m.Send(It.Is<GetCafesByLocationQuery>(q => q.Location == location), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByLocationAsync_ShouldReturnEmptyList_WhenNoCafesExist_Test()
        {
            string location = "Non-existent Location";
            List<CafeDto> emptyList = new List<CafeDto>();

            mediatorMock
                .Setup(m => m.Send(It.Is<GetCafesByLocationQuery>(q => q.Location == location), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyList);

            mapperMock
                .Setup(m => m.Map<IEnumerable<Cafe>>(emptyList))
                .Returns(new List<Cafe>());

            IEnumerable<Cafe> result = await cafeService.GetByLocationAsync(location);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCafes_Test()
        {
            List<CafeDto> cafeDtos = new List<CafeDto>
            {
                new CafeDto { Id = Guid.NewGuid(), Name = "Cafe 1" },
                new CafeDto { Id = Guid.NewGuid(), Name = "Cafe 2" },
                new CafeDto { Id = Guid.NewGuid(), Name = "Cafe 3" }
            };

            IEnumerable<Cafe> cafes = new List<Cafe>
            {
                new Cafe(cafeDtos[0].Id, "Cafe 1", "Description 1", "Logo 1", "Location 1"),
                new Cafe(cafeDtos[1].Id, "Cafe 2", "Description 2", "Logo 2", "Location 2"),
                new Cafe(cafeDtos[2].Id, "Cafe 3", "Description 3", "Logo 3", "Location 3")
            };

            mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllCafesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cafeDtos);

            mapperMock
                .Setup(m => m.Map<IEnumerable<Cafe>>(cafeDtos))
                .Returns(cafes);

            IEnumerable<Cafe> result = await cafeService.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            mediatorMock.Verify(m => m.Send(It.IsAny<GetAllCafesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldSendCreateCommand_AndReturnCreatedCafe_Test()
        {
            Cafe cafe = new Cafe(Guid.NewGuid(), "New Cafe", "Description", "logo.png", "Location");

            Cafe createdCafe = new Cafe(Guid.NewGuid(), "New Cafe", "Description", "logo.png", "Location");

            mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateCafeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdCafe);

            Cafe result = await cafeService.CreateAsync(cafe);

            Assert.NotNull(result);
            Assert.Equal(createdCafe.Id, result.Id);
            Assert.Equal(cafe.Name, result.Name);

            mediatorMock.Verify(m => m.Send(
                It.Is<CreateCafeCommand>(cmd =>
                    cmd.Name == cafe.Name &&
                    cmd.Description == cafe.Description &&
                    cmd.Location == cafe.Location &&
                    cmd.Logo == cafe.Logo),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldSendUpdateCommand_AndReturnUpdatedCafe_Test()
        {
            Cafe cafe = new Cafe(Guid.NewGuid(), "Updated Cafe", "Updated Description", "updated-logo.png", "Updated Location");

            mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateCafeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cafe);

            Cafe? result = await cafeService.UpdateAsync(cafe);

            Assert.NotNull(result);
            Assert.Equal(cafe.Id, result.Id);
            Assert.Equal(cafe.Name, result.Name);
            Assert.Equal(cafe.Description, result.Description);

            mediatorMock.Verify(m => m.Send(
                It.Is<UpdateCafeCommand>(cmd =>
                    cmd.Id == cafe.Id &&
                    cmd.Name == cafe.Name &&
                    cmd.Description == cafe.Description &&
                    cmd.Location == cafe.Location &&
                    cmd.Logo == cafe.Logo),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenCafeDoesNotExist_Test()
        {
            Cafe cafe = new Cafe(Guid.NewGuid(), "Non-existent Cafe", "Description", "logo.png", "Location");

            mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateCafeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cafe?)null);

            Cafe? result = await cafeService.UpdateAsync(cafe);

            Assert.Null(result);

            mediatorMock.Verify(m => m.Send(It.IsAny<UpdateCafeCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSendDeleteCommand_AndReturnTrue_WhenCafeExists_Test()
        {
            Guid cafeId = Guid.NewGuid();

            mediatorMock
                .Setup(m => m.Send(It.Is<DeleteCafeCommand>(cmd => cmd.Id == cafeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            bool result = await cafeService.DeleteAsync(cafeId);

            Assert.True(result);

            mediatorMock.Verify(m => m.Send(It.Is<DeleteCafeCommand>(cmd => cmd.Id == cafeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenCafeDoesNotExist_Test()
        {
            Guid cafeId = Guid.NewGuid();

            mediatorMock
                .Setup(m => m.Send(It.Is<DeleteCafeCommand>(cmd => cmd.Id == cafeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            bool result = await cafeService.DeleteAsync(cafeId);

            Assert.False(result);

            mediatorMock.Verify(m => m.Send(It.Is<DeleteCafeCommand>(cmd => cmd.Id == cafeId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
} 