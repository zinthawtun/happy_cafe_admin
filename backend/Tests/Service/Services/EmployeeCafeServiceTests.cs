using AutoMapper;
using Business.Entities;
using MediatR;
using Moq;
using Service.Commands.EmployeeCafes;
using Service.Queries.EmployeeCafes;
using Service.Services;

namespace Tests.Service.Services
{
    public class EmployeeCafeServiceTests
    {
        private readonly Mock<IMediator> mediatorMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly EmployeeCafeService employeeCafeService;

        public EmployeeCafeServiceTests()
        {
            mediatorMock = new Mock<IMediator>();
            mapperMock = new Mock<IMapper>();
            employeeCafeService = new EmployeeCafeService(mediatorMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEmployeeCafeDto_WhenEmployeeCafeExists_Test()
        {
            Guid employeeCafeId = Guid.NewGuid();
            string employeeId = "employee1";
            Guid cafeId = Guid.NewGuid();
            DateTime assignedDate = DateTime.UtcNow.AddDays(-30);

            EmployeeCafeDto employeeCafeDto = new EmployeeCafeDto
            {
                Id = employeeCafeId,
                EmployeeId = employeeId,
                CafeId = cafeId,
                AssignedDate = assignedDate,
                IsActive = true
            };

            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafeByIdQuery>(q => q.Id == employeeCafeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeCafeDto);

            EmployeeCafeDto? result = await employeeCafeService.GetByIdAsync(employeeCafeId);

            Assert.NotNull(result);
            Assert.Equal(employeeCafeId, result.Id);
            Assert.Equal(employeeId, result.EmployeeId);
            Assert.Equal(cafeId, result.CafeId);
            Assert.Equal(assignedDate, result.AssignedDate);
            Assert.True(result.IsActive);

            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeCafeByIdQuery>(q => q.Id == employeeCafeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEmployeeCafeDoesNotExist_Test()
        {
            Guid employeeCafeId = Guid.NewGuid();

            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafeByIdQuery>(q => q.Id == employeeCafeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EmployeeCafeDto?)null);

            EmployeeCafeDto? result = await employeeCafeService.GetByIdAsync(employeeCafeId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEmployeeCafeDtos_Test()
        {
            List<EmployeeCafeDto> employeeCafeDtos = new List<EmployeeCafeDto>
                {
                    new EmployeeCafeDto { Id = Guid.NewGuid(), EmployeeId = "employee1", CafeId = Guid.NewGuid(), AssignedDate = DateTime.UtcNow.AddDays(-30), IsActive = true },
                    new EmployeeCafeDto { Id = Guid.NewGuid(), EmployeeId = "employee2", CafeId = Guid.NewGuid(), AssignedDate = DateTime.UtcNow.AddDays(-20), IsActive = true },
                    new EmployeeCafeDto { Id = Guid.NewGuid(), EmployeeId = "employee3", CafeId = Guid.NewGuid(), AssignedDate = DateTime.UtcNow.AddDays(-10), IsActive = true }
                };

            mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllEmployeeCafesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeCafeDtos);

            IEnumerable<EmployeeCafeDto> result = await employeeCafeService.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Contains(result, ec => ec.EmployeeId == "employee1");
            Assert.Contains(result, ec => ec.EmployeeId == "employee2");
            Assert.Contains(result, ec => ec.EmployeeId == "employee3");

            mediatorMock.Verify(m => m.Send(It.IsAny<GetAllEmployeeCafesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByCafeIdAsync_ShouldReturnEmployeeCafeDtos_WhenEmployeeCafesExist_Test()
        {
            Guid cafeId = Guid.NewGuid();
            List<EmployeeCafeDto> employeeCafeDtos = new List<EmployeeCafeDto>
                {
                    new EmployeeCafeDto { Id = Guid.NewGuid(), EmployeeId = "employee1", CafeId = cafeId, AssignedDate = DateTime.UtcNow.AddDays(-30), IsActive = true },
                    new EmployeeCafeDto { Id = Guid.NewGuid(), EmployeeId = "employee2", CafeId = cafeId, AssignedDate = DateTime.UtcNow.AddDays(-20), IsActive = true }
                };

            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafesByCafeIdQuery>(q => q.CafeId == cafeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeCafeDtos);

            IEnumerable<EmployeeCafeDto> result = await employeeCafeService.GetByCafeIdAsync(cafeId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, ec => Assert.Equal(cafeId, ec.CafeId));

            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeCafesByCafeIdQuery>(q => q.CafeId == cafeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByEmployeeIdAsync_ShouldReturnEmployeeCafeDto_WhenEmployeeCafeExists_Test()
        {
            string employeeId = "employee1";
            Guid cafeId = Guid.NewGuid();
            Guid employeeCafeId = Guid.NewGuid();
            DateTime assignedDate = DateTime.UtcNow.AddDays(-30);

            List<EmployeeCafeDto> employeeCafeDtos = new List<EmployeeCafeDto>
                {
                    new EmployeeCafeDto {
                        Id = employeeCafeId,
                        EmployeeId = employeeId,
                        CafeId = cafeId,
                        AssignedDate = assignedDate,
                        IsActive = true
                    }
                };

            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeCafeDtos);

            EmployeeCafeDto? result = await employeeCafeService.GetByEmployeeIdAsync(employeeId);

            Assert.NotNull(result);
            Assert.Equal(employeeCafeId, result.Id);
            Assert.Equal(employeeId, result.EmployeeId);
            Assert.Equal(cafeId, result.CafeId);
            Assert.Equal(assignedDate, result.AssignedDate);
            Assert.True(result.IsActive);

            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByEmployeeIdAsync_ShouldReturnNull_WhenEmployeeIsNotAssignedToAnyCafe_Test()
        {
            string employeeId = "employee1";
            List<EmployeeCafeDto> emptyList = new List<EmployeeCafeDto>();

            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyList);

            EmployeeCafeDto? result = await employeeCafeService.GetByEmployeeIdAsync(employeeId);

            Assert.Null(result);

            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AssignEmployeeToCafeAsync_ShouldSendCommand_AndReturnEmployeeCafeDto_Test()
        {
            Guid employeeCafeId = Guid.NewGuid();
            string employeeId = "employee1";
            Guid cafeId = Guid.NewGuid();
            DateTime assignedDate = DateTime.UtcNow;

            AssignEmployeeToCafeCommand command = new AssignEmployeeToCafeCommand
            {
                EmployeeId = employeeId,
                CafeId = cafeId,
                AssignedDate = assignedDate
            };

            EmployeeCafe employeeCafe = new EmployeeCafe(
                employeeCafeId,
                cafeId,
                employeeId,
                assignedDate
            );

            EmployeeCafeDto employeeCafeDto = new EmployeeCafeDto
            {
                Id = employeeCafeId,
                EmployeeId = employeeId,
                CafeId = cafeId,
                AssignedDate = assignedDate,
                IsActive = true
            };

            mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeCafe);

            mapperMock
                .Setup(m => m.Map<EmployeeCafeDto>(employeeCafe))
                .Returns(employeeCafeDto);

            EmployeeCafeDto? result = await employeeCafeService.AssignEmployeeToCafeAsync(command);

            Assert.NotNull(result);
            Assert.Equal(employeeCafeId, result.Id);
            Assert.Equal(employeeId, result.EmployeeId);
            Assert.Equal(cafeId, result.CafeId);
            Assert.Equal(assignedDate, result.AssignedDate);
            Assert.True(result.IsActive);

            mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAssignmentAsync_ShouldSendCommand_AndReturnUpdatedEmployeeCafeDto_Test()
        {
            Guid employeeCafeId = Guid.NewGuid();
            string employeeId = "employee1";
            Guid cafeId = Guid.NewGuid();
            DateTime assignedDate = DateTime.UtcNow;

            UpdateEmployeeCafeAssignmentCommand command = new UpdateEmployeeCafeAssignmentCommand
            {
                Id = employeeCafeId,
                EmployeeId = employeeId,
                CafeId = cafeId,
                AssignedDate = assignedDate
            };

            EmployeeCafe employeeCafe = new EmployeeCafe(
                employeeCafeId,
                cafeId,
                employeeId,
                assignedDate
            );

            EmployeeCafeDto employeeCafeDto = new EmployeeCafeDto
            {
                Id = employeeCafeId,
                EmployeeId = employeeId,
                CafeId = cafeId,
                AssignedDate = assignedDate,
                IsActive = true
            };

            mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeCafe);

            mapperMock
                .Setup(m => m.Map<EmployeeCafeDto>(employeeCafe))
                .Returns(employeeCafeDto);

            EmployeeCafeDto? result = await employeeCafeService.UpdateAssignmentAsync(command);

            Assert.NotNull(result);
            Assert.Equal(employeeCafeId, result.Id);
            Assert.Equal(employeeId, result.EmployeeId);
            Assert.Equal(cafeId, result.CafeId);
            Assert.Equal(assignedDate, result.AssignedDate);
            Assert.True(result.IsActive);

            mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAssignmentAsync_ShouldReturnNull_WhenEmployeeCafeDoesNotExist_Test()
        {
            Guid nonExistentEmployeeCafeId = Guid.NewGuid();
            string employeeId = "employee1";
            Guid cafeId = Guid.NewGuid();
            DateTime assignedDate = DateTime.UtcNow;

            UpdateEmployeeCafeAssignmentCommand command = new UpdateEmployeeCafeAssignmentCommand
            {
                Id = nonExistentEmployeeCafeId,
                EmployeeId = employeeId,
                CafeId = cafeId,
                AssignedDate = assignedDate
            };

            mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync((EmployeeCafe?)null);

            EmployeeCafeDto? result = await employeeCafeService.UpdateAssignmentAsync(command);

            Assert.Null(result);

            mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UnassignEmployeeFromCafeAsync_ShouldSendCommand_AndReturnTrue_WhenSuccessful_Test()
        {
            Guid employeeCafeId = Guid.NewGuid();
            UnassignEmployeeFromCafeCommand command = new UnassignEmployeeFromCafeCommand { Id = employeeCafeId };

            mediatorMock
                .Setup(m => m.Send(It.Is<UnassignEmployeeFromCafeCommand>(cmd => cmd.Id == employeeCafeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            bool result = await employeeCafeService.UnassignEmployeeFromCafeAsync(employeeCafeId);

            Assert.True(result);

            mediatorMock.Verify(m => m.Send(It.Is<UnassignEmployeeFromCafeCommand>(cmd => cmd.Id == employeeCafeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UnassignEmployeeFromCafeAsync_ShouldReturnFalse_WhenEmployeeCafeDoesNotExist_Test()
        {
            Guid nonExistentEmployeeCafeId = Guid.NewGuid();
            UnassignEmployeeFromCafeCommand command = new UnassignEmployeeFromCafeCommand { Id = nonExistentEmployeeCafeId };

            mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            bool result = await employeeCafeService.UnassignEmployeeFromCafeAsync(nonExistentEmployeeCafeId);

            Assert.False(result);

            mediatorMock.Verify(m => m.Send(It.Is<UnassignEmployeeFromCafeCommand>(cmd => cmd.Id == nonExistentEmployeeCafeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task IsEmployeeAssignedToCafeAsync_ShouldReturnTrue_WhenEmployeeIsAssignedToCafe_Test()
        {
            string employeeId = "employee1";
            Guid cafeId = Guid.NewGuid();
            Guid employeeCafeId = Guid.NewGuid();

            EmployeeCafeDto employeeCafeDto = new EmployeeCafeDto
            {
                Id = employeeCafeId,
                EmployeeId = employeeId,
                CafeId = cafeId,
                AssignedDate = DateTime.UtcNow.AddDays(-30),
                IsActive = true
            };

            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EmployeeCafeDto> { employeeCafeDto });

            bool result = await employeeCafeService.IsEmployeeAssignedToCafeAsync(employeeId, cafeId);

            Assert.True(result);

            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task IsEmployeeAssignedToCafeAsync_ShouldReturnFalse_WhenEmployeeIsNotAssigned_Test()
        {
            string employeeId = "employee1";
            Guid cafeId = Guid.NewGuid();

            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EmployeeCafeDto>());

            bool result = await employeeCafeService.IsEmployeeAssignedToCafeAsync(employeeId, cafeId);

            Assert.False(result);

            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task IsEmployeeAssignedToCafeAsync_ShouldReturnFalse_WhenEmployeeIsAssignedToDifferentCafe_Test()
        {
            string employeeId = "employee1";
            Guid requestedCafeId = Guid.NewGuid();
            Guid assignedCafeId = Guid.NewGuid();
            Guid employeeCafeId = Guid.NewGuid();

            EmployeeCafeDto employeeCafeDto = new EmployeeCafeDto
            {
                Id = employeeCafeId,
                EmployeeId = employeeId,
                CafeId = assignedCafeId,
                AssignedDate = DateTime.UtcNow.AddDays(-30),
                IsActive = true
            };

            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EmployeeCafeDto> { employeeCafeDto });

            bool result = await employeeCafeService.IsEmployeeAssignedToCafeAsync(employeeId, requestedCafeId);

            Assert.False(result);

            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task IsEmployeeAssignedToCafeAsync_ShouldReturnFalse_WhenAssignmentIsNotActive_Test()
        {
            string employeeId = "employee1";
            Guid cafeId = Guid.NewGuid();
            Guid employeeCafeId = Guid.NewGuid();

            EmployeeCafeDto employeeCafeDto = new EmployeeCafeDto
            {
                Id = employeeCafeId,
                EmployeeId = employeeId,
                CafeId = cafeId,
                AssignedDate = DateTime.UtcNow.AddDays(-30),
                IsActive = false
            };

            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EmployeeCafeDto> { employeeCafeDto });

            bool result = await employeeCafeService.IsEmployeeAssignedToCafeAsync(employeeId, cafeId);

            Assert.False(result);

            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
} 