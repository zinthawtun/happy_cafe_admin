using AutoMapper;
using Business.Entities;
using Moq;
using Resource.Interfaces;
using Service.Handlers.Cafes;
using Service.Queries.Cafes;

namespace Tests.Service.Handlers
{
    public class GetAllCafesQueryHandlerTests
    {
        private readonly Mock<ICafeResource> cafeResourceMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly GetAllCafesQueryHandler handler;

        public GetAllCafesQueryHandlerTests()
        {
            cafeResourceMock = new Mock<ICafeResource>();
            mapperMock = new Mock<IMapper>();
            handler = new GetAllCafesQueryHandler(cafeResourceMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedCafeDtos_Test()
        {
            GetAllCafesQuery query = new GetAllCafesQuery();

            List<Cafe> cafes = new List<Cafe>
            {
                new Cafe(Guid.NewGuid(), "Cafe 1", "Description 1", string.Empty, "Location 1"),
                new Cafe(Guid.NewGuid(), "Cafe 2", "Description 2", string.Empty, "Location 2")
            };

            List<CafeDto> cafeDtos = new List<CafeDto>
            {
                new CafeDto
                {
                    Id = cafes[0].Id,
                    Name = cafes[0].Name,
                    Description = cafes[0].Description,
                    Location = cafes[0].Location
                },
                new CafeDto
                {
                    Id = cafes[1].Id,
                    Name = cafes[1].Name,
                    Description = cafes[1].Description,
                    Location = cafes[1].Location
                }
            };

            cafeResourceMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(cafes);

            mapperMock
                .Setup(m => m.Map<IEnumerable<CafeDto>>(cafes))
                .Returns(cafeDtos);

            IEnumerable<CafeDto> result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(cafes.Count, result.Count());
            Assert.Equal(cafes[0].Id, result.First().Id);
            Assert.Equal(cafes[0].Name, result.First().Name);
            Assert.Equal(cafes[1].Id, result.Skip(1).First().Id);
            Assert.Equal(cafes[1].Name, result.Skip(1).First().Name);

            cafeResourceMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoCafesExist_Test()
        {
            GetAllCafesQuery query = new GetAllCafesQuery();

            List<Cafe> cafes = new List<Cafe>();
            List<CafeDto> cafeDtos = new List<CafeDto>();

            cafeResourceMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(cafes);

            mapperMock
                .Setup(m => m.Map<IEnumerable<CafeDto>>(cafes))
                .Returns(cafeDtos);

            IEnumerable<CafeDto> result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);

            cafeResourceMock.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
} 