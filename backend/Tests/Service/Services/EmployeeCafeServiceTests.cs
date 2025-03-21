using AutoMapper;
using Business.Entities;
using MediatR;
using Moq;
using Service.Commands.EmployeeCafes;
using Service.Queries.EmployeeCafes;
using Service.Services;
using Utilities;

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
        public async Task GetByIdAsync_ShouldReturnEmployeeCafe_WhenEmployeeCafeExists_Test()
        {
            Guid employeeCafeId = Guid.NewGuid();
            Guid cafeId = Guid.NewGuid();
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            DateTime assignedDate = DateTime.UtcNow.Date;
            
            EmployeeCafeDto employeeCafeDto = new EmployeeCafeDto 
            { 
                Id = employeeCafeId, 
                EmployeeId = employeeId, 
                EmployeeName = "John Doe", 
                CafeId = cafeId, 
                CafeName = "Test Cafe"
            };
            
            EmployeeCafe employeeCafe = new EmployeeCafe(
                employeeCafeId,
                cafeId,
                employeeId,
                assignedDate
            );
            
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafeByIdQuery>(q => q.Id == employeeCafeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeCafeDto);
                
            mapperMock
                .Setup(m => m.Map<EmployeeCafe>(employeeCafeDto))
                .Returns(employeeCafe);

            EmployeeCafe? result = await employeeCafeService.GetByIdAsync(employeeCafeId);

            Assert.NotNull(result);
            Assert.Equal(employeeCafeId, result.Id);
            Assert.Equal(employeeId, result.EmployeeId);
            
            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeCafeByIdQuery>(q => q.Id == employeeCafeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEmployeeCafeDoesNotExist_Test()
        {
            Guid employeeCafeId = Guid.NewGuid();
            
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafeByIdQuery>(q => q.Id == employeeCafeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EmployeeCafeDto?)null);

            EmployeeCafe? result = await employeeCafeService.GetByIdAsync(employeeCafeId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEmployeeCafes_Test()
        {
            DateTime assignedDate = DateTime.UtcNow.Date;
            
            List<EmployeeCafeDto> employeeCafeDtos = new List<EmployeeCafeDto>
            {
                new EmployeeCafeDto { Id = Guid.NewGuid(), EmployeeId = UniqueIdGenerator.GenerateUniqueId(), CafeId = Guid.NewGuid() },
                new EmployeeCafeDto { Id = Guid.NewGuid(), EmployeeId = UniqueIdGenerator.GenerateUniqueId(), CafeId = Guid.NewGuid() },
                new EmployeeCafeDto { Id = Guid.NewGuid(), EmployeeId = UniqueIdGenerator.GenerateUniqueId(), CafeId = Guid.NewGuid() }
            };
            
            IEnumerable<EmployeeCafe> employeeCafes = new List<EmployeeCafe>
            {
                new EmployeeCafe(employeeCafeDtos[0].Id, employeeCafeDtos[0].CafeId, employeeCafeDtos[0].EmployeeId, assignedDate),
                new EmployeeCafe(employeeCafeDtos[1].Id, employeeCafeDtos[1].CafeId, employeeCafeDtos[1].EmployeeId, assignedDate),
                new EmployeeCafe(employeeCafeDtos[2].Id, employeeCafeDtos[2].CafeId, employeeCafeDtos[2].EmployeeId, assignedDate)
            };
            
            mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllEmployeeCafesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeCafeDtos);
                
            mapperMock
                .Setup(m => m.Map<IEnumerable<EmployeeCafe>>(employeeCafeDtos))
                .Returns(employeeCafes);

            IEnumerable<EmployeeCafe> result = await employeeCafeService.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            
            mediatorMock.Verify(m => m.Send(It.IsAny<GetAllEmployeeCafesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByCafeIdAsync_ShouldReturnEmployeeCafes_WhenAssignmentsExist_Test()
        {
            Guid cafeId = Guid.NewGuid();
            DateTime assignedDate = DateTime.UtcNow.Date;
            
            List<EmployeeCafeDto> employeeCafeDtos = new List<EmployeeCafeDto>
            {
                new EmployeeCafeDto { Id = Guid.NewGuid(), EmployeeId = UniqueIdGenerator.GenerateUniqueId(), CafeId = cafeId },
                new EmployeeCafeDto { Id = Guid.NewGuid(), EmployeeId = UniqueIdGenerator.GenerateUniqueId(), CafeId = cafeId }
            };
            
            IEnumerable<EmployeeCafe> employeeCafes = new List<EmployeeCafe>
            {
                new EmployeeCafe(employeeCafeDtos[0].Id, cafeId, employeeCafeDtos[0].EmployeeId, assignedDate),
                new EmployeeCafe(employeeCafeDtos[1].Id, cafeId, employeeCafeDtos[1].EmployeeId, assignedDate)
            };
            
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafesByCafeIdQuery>(q => q.CafeId == cafeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeCafeDtos);
                
            mapperMock
                .Setup(m => m.Map<IEnumerable<EmployeeCafe>>(employeeCafeDtos))
                .Returns(employeeCafes);

            IEnumerable<EmployeeCafe> result = await employeeCafeService.GetByCafeIdAsync(cafeId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, ec => Assert.Equal(cafeId, ec.CafeId));
            
            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeCafesByCafeIdQuery>(q => q.CafeId == cafeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByEmployeeIdAsync_ShouldReturnEmployeeCafes_WhenAssignmentsExist_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            DateTime assignedDate = DateTime.UtcNow.Date;

            List<EmployeeCafeDto> employeeCafeDtos = new List<EmployeeCafeDto>
            {
                new EmployeeCafeDto { Id = Guid.NewGuid(), EmployeeId = employeeId, CafeId = Guid.NewGuid() }
            };

            IEnumerable<EmployeeCafe> employeeCafes = new List<EmployeeCafe>
            {
                new EmployeeCafe(employeeCafeDtos[0].Id, employeeCafeDtos[0].CafeId, employeeId, assignedDate)
            };

            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeCafeDtos);

            mapperMock
                .Setup(m => m.Map<IEnumerable<EmployeeCafe>>(employeeCafeDtos))
                .Returns(employeeCafes);

            IEnumerable<EmployeeCafe> result = await employeeCafeService.GetByEmployeeIdAsync(employeeId);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.All(result, ec => Assert.Equal(employeeId, ec.EmployeeId));

            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AssignEmployeeToCafeAsync_ShouldSendAssignCommand_AndReturnCreatedAssignment_Test()
        {
            Guid cafeId = Guid.NewGuid();
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            DateTime assignedDate = DateTime.UtcNow.Date;
            
            EmployeeCafe createdAssignment = new EmployeeCafe(
                Guid.NewGuid(),
                cafeId,
                employeeId,
                assignedDate
            );
            
            mediatorMock
                .Setup(m => m.Send(It.IsAny<AssignEmployeeToCafeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdAssignment);

            EmployeeCafe result = await employeeCafeService.AssignEmployeeToCafeAsync(cafeId, employeeId, assignedDate);

            Assert.NotNull(result);
            Assert.Equal(cafeId, result.CafeId);
            Assert.Equal(employeeId, result.EmployeeId);
            Assert.Equal(assignedDate, result.AssignedDate);
            
            mediatorMock.Verify(m => m.Send(
                It.Is<AssignEmployeeToCafeCommand>(cmd => 
                    cmd.CafeId == cafeId && 
                    cmd.EmployeeId == employeeId && 
                    cmd.AssignedDate.Date == assignedDate.Date), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Fact]
        public async Task UpdateAssignmentAsync_ShouldSendUpdateCommand_AndReturnUpdatedAssignment_Test()
        {
            Guid assignmentId = Guid.NewGuid();
            Guid cafeId = Guid.NewGuid();
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            DateTime assignedDate = DateTime.UtcNow.Date;
            bool isActive = true;
            
            EmployeeCafe updatedAssignment = new EmployeeCafe(
                assignmentId,
                cafeId,
                employeeId,
                assignedDate
            );
            
            mediatorMock
                .Setup(m => m.Send(It.Is<UpdateEmployeeCafeAssignmentCommand>(
                    cmd => cmd.IsActive == isActive), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedAssignment);

            mapperMock.Setup(m => m.Map<EmployeeCafe>(It.IsAny<EmployeeCafeDto>()))
                .Returns(updatedAssignment);

            EmployeeCafe? result = await employeeCafeService.UpdateAssignmentAsync(assignmentId, cafeId, employeeId, isActive, assignedDate);

            Assert.NotNull(result);
            Assert.Equal(assignmentId, result.Id);
            Assert.Equal(cafeId, result.CafeId);
            Assert.Equal(employeeId, result.EmployeeId);
            
            mediatorMock.Verify(m => m.Send(
                It.Is<UpdateEmployeeCafeAssignmentCommand>(cmd => 
                    cmd.Id == assignmentId &&
                    cmd.CafeId == cafeId && 
                    cmd.EmployeeId == employeeId && 
                    cmd.IsActive == isActive &&
                    cmd.AssignedDate.Date == assignedDate.Date), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Fact]
        public async Task UpdateAssignmentAsync_ShouldReturnNull_WhenAssignmentDoesNotExist_Test()
        {
            Guid assignmentId = Guid.NewGuid();
            Guid cafeId = Guid.NewGuid();
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            DateTime assignedDate = DateTime.UtcNow.Date;
            bool isActive = true;
            
            mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateEmployeeCafeAssignmentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EmployeeCafe?)null);

            EmployeeCafe? result = await employeeCafeService.UpdateAssignmentAsync(assignmentId, cafeId, employeeId, isActive, assignedDate);

            Assert.Null(result);
            
            mediatorMock.Verify(m => m.Send(It.IsAny<UpdateEmployeeCafeAssignmentCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UnassignEmployeeFromCafeAsync_ShouldSendUnassignCommand_AndReturnTrue_WhenAssignmentExists_Test()
        {
            Guid assignmentId = Guid.NewGuid();
            
            mediatorMock
                .Setup(m => m.Send(It.Is<UnassignEmployeeFromCafeCommand>(cmd => cmd.Id == assignmentId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            bool result = await employeeCafeService.UnassignEmployeeFromCafeAsync(assignmentId);

            Assert.True(result);
            
            mediatorMock.Verify(m => m.Send(It.Is<UnassignEmployeeFromCafeCommand>(cmd => cmd.Id == assignmentId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UnassignEmployeeFromCafeAsync_ShouldReturnFalse_WhenAssignmentDoesNotExist_Test()
        {
            Guid assignmentId = Guid.NewGuid();
            
            mediatorMock
                .Setup(m => m.Send(It.Is<UnassignEmployeeFromCafeCommand>(cmd => cmd.Id == assignmentId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            bool result = await employeeCafeService.UnassignEmployeeFromCafeAsync(assignmentId);

            Assert.False(result);
            
            mediatorMock.Verify(m => m.Send(It.Is<UnassignEmployeeFromCafeCommand>(cmd => cmd.Id == assignmentId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task IsEmployeeAssignedToCafeAsync_ShouldReturnTrue_WhenEmployeeIsAssigned_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            Guid cafeId = Guid.NewGuid();
            DateTime assignedDate = DateTime.UtcNow.Date;
            
            EmployeeCafeDto employeeCafeDto = new EmployeeCafeDto 
            { 
                Id = Guid.NewGuid(), 
                EmployeeId = employeeId, 
                CafeId = cafeId, 
                IsActive = true 
            };
            
            EmployeeCafe employeeCafe = new EmployeeCafe(
                employeeCafeDto.Id,
                cafeId,
                employeeId,
                assignedDate
            );
            
            List<EmployeeCafeDto> employeeCafeDtos = new List<EmployeeCafeDto> { employeeCafeDto };
            List<EmployeeCafe> employeeCafes = new List<EmployeeCafe> { employeeCafe };
            
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeCafeDtos);
                
            mapperMock
                .Setup(m => m.Map<IEnumerable<EmployeeCafe>>(It.Is<IEnumerable<EmployeeCafeDto>>(dtos => dtos.Contains(employeeCafeDto))))
                .Returns(employeeCafes);

            bool result = await employeeCafeService.IsEmployeeAssignedToCafeAsync(employeeId, cafeId);

            Assert.True(result);
            
            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<IEnumerable<EmployeeCafe>>(It.IsAny<IEnumerable<EmployeeCafeDto>>()), Times.Once);
        }

        [Fact]
        public async Task IsEmployeeAssignedToCafeAsync_ShouldReturnFalse_WhenEmployeeIsNotAssigned_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            Guid cafeId = Guid.NewGuid();
            Guid differentCafeId = Guid.NewGuid();
            DateTime assignedDate = DateTime.UtcNow.Date;
            
            EmployeeCafeDto employeeCafeDto = new EmployeeCafeDto 
            { 
                Id = Guid.NewGuid(), 
                EmployeeId = employeeId, 
                CafeId = differentCafeId, 
                IsActive = true 
            };
            
            EmployeeCafe employeeCafe = new EmployeeCafe(
                employeeCafeDto.Id,
                differentCafeId,
                employeeId,
                assignedDate
            );
            
            List<EmployeeCafeDto> employeeCafeDtos = new List<EmployeeCafeDto> { employeeCafeDto };
            List<EmployeeCafe> employeeCafes = new List<EmployeeCafe> { employeeCafe };
            
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeCafeDtos);
                
            mapperMock
                .Setup(m => m.Map<IEnumerable<EmployeeCafe>>(It.Is<IEnumerable<EmployeeCafeDto>>(dtos => dtos.Contains(employeeCafeDto))))
                .Returns(employeeCafes);

            bool result = await employeeCafeService.IsEmployeeAssignedToCafeAsync(employeeId, cafeId);

            Assert.False(result);
            
            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<IEnumerable<EmployeeCafe>>(It.IsAny<IEnumerable<EmployeeCafeDto>>()), Times.Once);
        }

        [Fact]
        public async Task IsEmployeeAssignedToCafeAsync_ShouldReturnTrue_WhenAssignmentIsInactive_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            Guid cafeId = Guid.NewGuid();
            DateTime assignedDate = DateTime.UtcNow.Date;
            
            EmployeeCafeDto employeeCafeDto = new EmployeeCafeDto 
            { 
                Id = Guid.NewGuid(), 
                EmployeeId = employeeId, 
                CafeId = cafeId, 
                IsActive = false 
            };
            
            EmployeeCafe employeeCafe = new EmployeeCafe(
                employeeCafeDto.Id,
                cafeId,
                employeeId,
                assignedDate
            );
            
            List<EmployeeCafeDto> employeeCafeDtos = new List<EmployeeCafeDto> { employeeCafeDto };
            List<EmployeeCafe> employeeCafes = new List<EmployeeCafe> { employeeCafe };
            
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeCafeDtos);
                
            mapperMock
                .Setup(m => m.Map<IEnumerable<EmployeeCafe>>(It.Is<IEnumerable<EmployeeCafeDto>>(dtos => dtos.Contains(employeeCafeDto))))
                .Returns(employeeCafes);

            bool result = await employeeCafeService.IsEmployeeAssignedToCafeAsync(employeeId, cafeId);

            Assert.True(result);
            
            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeCafesByEmployeeIdQuery>(q => q.EmployeeId == employeeId), It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(m => m.Map<IEnumerable<EmployeeCafe>>(It.IsAny<IEnumerable<EmployeeCafeDto>>()), Times.Once);
        }
    }
} 