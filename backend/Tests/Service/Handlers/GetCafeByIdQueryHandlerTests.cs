using AutoMapper;
using Business.Entities;
using Moq;
using Resource.Interfaces;
using Service.Handlers.Cafes;
using Service.Queries.Cafes;

namespace Tests.Service.Handlers
{
    public class GetCafeByIdQueryHandlerTests
    {
        private readonly Mock<ICafeResource> cafeResourceMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly GetCafeByIdQueryHandler handler;

        public GetCafeByIdQueryHandlerTests()
        {
            cafeResourceMock = new Mock<ICafeResource>();
            mapperMock = new Mock<IMapper>();
            handler = new GetCafeByIdQueryHandler(cafeResourceMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCafeDto_WhenCafeExists_Test()
        {
            Guid cafeId = Guid.NewGuid();
            GetCafeByIdQuery query = new GetCafeByIdQuery { Id = cafeId };

            Cafe cafe = new Cafe(cafeId, "Test Cafe", "Test Description", string.Empty, "Test Location");

            CafeDto cafeDto = new CafeDto
            {
                Id = cafeId,
                Name = "Test Cafe",
                Description = "Test Description",
                Location = "Test Location"
            };

            cafeResourceMock
                .Setup(r => r.GetByIdAsync(cafeId))
                .ReturnsAsync(cafe);

            mapperMock
                .Setup(m => m.Map<CafeDto>(cafe))
                .Returns(cafeDto);

            CafeDto? result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(cafeId, result.Id);
            Assert.Equal(cafe.Name, result.Name);
            Assert.Equal(cafe.Description, result.Description);
            Assert.Equal(cafe.Location, result.Location);

            cafeResourceMock.Verify(r => r.GetByIdAsync(cafeId), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenCafeDoesNotExist_Test()
        {
            Guid cafeId = Guid.NewGuid();
            GetCafeByIdQuery query = new GetCafeByIdQuery { Id = cafeId };

            cafeResourceMock
                .Setup(r => r.GetByIdAsync(cafeId))
                .ReturnsAsync((Cafe?)null);

            CafeDto? result = await handler.Handle(query, CancellationToken.None);

            Assert.Null(result);

            cafeResourceMock.Verify(r => r.GetByIdAsync(cafeId), Times.Once);
        }
    }
} 