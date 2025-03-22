using AutoMapper;
using Business.Entities;
using Moq;
using Resource.Interfaces;
using Service.Handlers.Cafes;
using Service.Queries.Cafes;

namespace Tests.Service.Handlers
{
    public class GetCafesByLocationQueryHandlerTests
    {
        private readonly Mock<ICafeResource> cafeResourceMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly GetCafesByLocationQueryHandler handler;

        public GetCafesByLocationQueryHandlerTests()
        {
            cafeResourceMock = new Mock<ICafeResource>();
            mapperMock = new Mock<IMapper>();
            handler = new GetCafesByLocationQueryHandler(cafeResourceMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFilteredCafes_WhenLocationMatches_Test()
        {
            string location = "Ang Mo Kio";
            GetCafesByLocationQuery query = new GetCafesByLocationQuery { Location = location };

            List<Cafe> cafes = new List<Cafe>
            {
                new Cafe(Guid.NewGuid(), "Cafe 1", "Description 1", string.Empty, location),
                new Cafe(Guid.NewGuid(), "Cafe 2", "Description 2", string.Empty, location)
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
                .Setup(r => r.GetByLocationAsync(location))
                .ReturnsAsync(cafes);

            mapperMock
                .Setup(m => m.Map<IEnumerable<CafeDto>>(cafes))
                .Returns(cafeDtos);

            IEnumerable<CafeDto> result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(cafes.Count, result.Count());
            Assert.All(result, dto => Assert.Equal(location, dto.Location));

            cafeResourceMock.Verify(r => r.GetByLocationAsync(location), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoMatchingLocations_Test()
        {
            string location = "Non-existent location";
            GetCafesByLocationQuery query = new GetCafesByLocationQuery { Location = location };

            List<Cafe> cafes = new List<Cafe>();
            List<CafeDto> cafeDtos = new List<CafeDto>();

            cafeResourceMock
                .Setup(r => r.GetByLocationAsync(location))
                .ReturnsAsync(cafes);

            mapperMock
                .Setup(m => m.Map<IEnumerable<CafeDto>>(cafes))
                .Returns(cafeDtos);

            IEnumerable<CafeDto> result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);

            cafeResourceMock.Verify(r => r.GetByLocationAsync(location), Times.Once);
        }
    }
} 