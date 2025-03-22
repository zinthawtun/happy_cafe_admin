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
        public async Task GetByIdAsync_ShouldReturnCafeDto_WhenCafeExists_Test()
        {
            Guid cafeId = Guid.NewGuid();
            CafeDto cafeDto = new CafeDto { Id = cafeId, Name = "Test Cafe" };

            mediatorMock
                .Setup(m => m.Send(It.Is<GetCafeByIdQuery>(q => q.Id == cafeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cafeDto);

            CafeDto? result = await cafeService.GetByIdAsync(cafeId);

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

            CafeDto? result = await cafeService.GetByIdAsync(cafeId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByLocationAsync_ShouldReturnCafeDtos_WhenCafesExist_Test()
        {
            string location = "New York";
            List<CafeDto> cafeDtos = new List<CafeDto>
            {
                new CafeDto { Id = Guid.NewGuid(), Name = "Cafe 1", Location = location },
                new CafeDto { Id = Guid.NewGuid(), Name = "Cafe 2", Location = location }
            };

            mediatorMock
                .Setup(m => m.Send(It.Is<GetCafesByLocationQuery>(q => q.Location == location), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cafeDtos);

            IEnumerable<CafeDto> result = await cafeService.GetByLocationAsync(location);

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

            IEnumerable<CafeDto> result = await cafeService.GetByLocationAsync(location);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCafeDtos_Test()
        {
            List<CafeDto> cafeDtos = new List<CafeDto>
            {
                new CafeDto { Id = Guid.NewGuid(), Name = "Cafe 1", Location = "Location 1" },
                new CafeDto { Id = Guid.NewGuid(), Name = "Cafe 2", Location = "Location 2" },
                new CafeDto { Id = Guid.NewGuid(), Name = "Cafe 3", Location = "Location 3" }
            };

            mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllCafesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cafeDtos);

            IEnumerable<CafeDto> result = await cafeService.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Contains(result, c => c.Name == "Cafe 1");
            Assert.Contains(result, c => c.Name == "Cafe 2");
            Assert.Contains(result, c => c.Name == "Cafe 3");

            mediatorMock.Verify(m => m.Send(It.IsAny<GetAllCafesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldSendCreateCommand_AndReturnCreatedCafeDto_Test()
        {
            Guid cafeId = Guid.NewGuid();
            string cafeName = "New Cafe";
            string description = "New Description";
            string logo = "new-logo.png";
            string location = "New Location";

            CreateCafeCommand command = new CreateCafeCommand
            {
                Name = cafeName,
                Description = description,
                Logo = logo,
                Location = location
            };

            Cafe createdCafe = new Cafe(cafeId, cafeName, description, logo, location);
            CafeDto cafeDto = new CafeDto { Id = cafeId, Name = cafeName, Description = description, Logo = logo, Location = location };

            mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdCafe);

            mapperMock
                .Setup(m => m.Map<CafeDto>(createdCafe))
                .Returns(cafeDto);

            CafeDto? result = await cafeService.CreateAsync(command);

            Assert.NotNull(result);
            Assert.Equal(cafeId, result.Id);
            Assert.Equal(cafeName, result.Name);

            mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldSendUpdateCommand_AndReturnUpdatedCafeDto_Test()
        {
            Guid cafeId = Guid.NewGuid();
            string updatedName = "Updated Cafe";
            string updatedDescription = "Updated Description";
            string updatedLogo = "updated-logo.png";
            string updatedLocation = "Updated Location";

            UpdateCafeCommand command = new UpdateCafeCommand
            {
                Id = cafeId,
                Name = updatedName,
                Description = updatedDescription,
                Logo = updatedLogo,
                Location = updatedLocation
            };

            Cafe updatedCafe = new Cafe(cafeId, updatedName, updatedDescription, updatedLogo, updatedLocation);
            CafeDto cafeDto = new CafeDto { Id = cafeId, Name = updatedName, Description = updatedDescription, Logo = updatedLogo, Location = updatedLocation };

            mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedCafe);

            mapperMock
                .Setup(m => m.Map<CafeDto>(updatedCafe))
                .Returns(cafeDto);

            CafeDto? result = await cafeService.UpdateAsync(command);

            Assert.NotNull(result);
            Assert.Equal(cafeId, result.Id);
            Assert.Equal(updatedName, result.Name);
            Assert.Equal(updatedDescription, result.Description);

            mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenCafeDoesNotExist_Test()
        {
            Guid nonExistentCafeId = Guid.NewGuid();
            UpdateCafeCommand command = new UpdateCafeCommand
            {
                Id = nonExistentCafeId,
                Name = "Updated Name",
                Description = "Updated Description",
                Logo = "updated-logo.png",
                Location = "Updated Location"
            };

            mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cafe?)null);

            CafeDto? result = await cafeService.UpdateAsync(command);

            Assert.Null(result);
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
            Guid nonExistentCafeId = Guid.NewGuid();
            DeleteCafeCommand command = new DeleteCafeCommand { Id = nonExistentCafeId };

            mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            bool result = await cafeService.DeleteAsync(nonExistentCafeId);

            Assert.False(result);
        }

        [Fact]
        public async Task ExistsByNameAsync_ShouldReturnTrue_WhenCafeNameExists_Test()
        {
            string existingCafeName = "Existing Cafe";
            
            mediatorMock
                .Setup(m => m.Send(It.Is<ExistsCafeByNameQuery>(q => q.Name == existingCafeName), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            bool result = await cafeService.ExistsByNameAsync(existingCafeName);

            Assert.True(result);
            mediatorMock.Verify(m => m.Send(It.Is<ExistsCafeByNameQuery>(q => q.Name == existingCafeName), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExistsByNameAsync_ShouldReturnFalse_WhenCafeNameDoesNotExist_Test()
        {
            string nonExistentCafeName = "Non-existent Cafe";
            
            mediatorMock
                .Setup(m => m.Send(It.Is<ExistsCafeByNameQuery>(q => q.Name == nonExistentCafeName), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            bool result = await cafeService.ExistsByNameAsync(nonExistentCafeName);

            Assert.False(result);
            mediatorMock.Verify(m => m.Send(It.Is<ExistsCafeByNameQuery>(q => q.Name == nonExistentCafeName), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
} 