using AutoMapper;
using Business.Entities;
using MediatR;
using Moq;
using Service.Commands.Employees;
using Service.Queries.Employees;
using Service.Services;

namespace Tests.Service.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IMediator> mediatorMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly EmployeeService employeeService;

        public EmployeeServiceTests()
        {
            mediatorMock = new Mock<IMediator>();
            mapperMock = new Mock<IMapper>();
            employeeService = new EmployeeService(mediatorMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEmployeeDto_WhenEmployeeExists_Test()
        {
            string employeeId = "employee1";
            EmployeeDto employeeDto = new EmployeeDto { Id = employeeId, Name = "Test Employee" };
            
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeByIdQuery>(q => q.Id == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeDto);

            EmployeeDto? result = await employeeService.GetByIdAsync(employeeId);

            Assert.NotNull(result);
            Assert.Equal(employeeId, result.Id);
            Assert.Equal("Test Employee", result.Name);
            
            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeByIdQuery>(q => q.Id == employeeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEmployeeDoesNotExist_Test()
        {
            string employeeId = "nonexistent";
            
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeByIdQuery>(q => q.Id == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EmployeeDto?)null);

            EmployeeDto? result = await employeeService.GetByIdAsync(employeeId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEmployeeDtos_Test()
        {
            List<EmployeeDto> employeeDtos = new List<EmployeeDto>
            {
                new EmployeeDto { Id = "employee1", Name = "Employee 1" },
                new EmployeeDto { Id = "employee2", Name = "Employee 2" },
                new EmployeeDto { Id = "employee3", Name = "Employee 3" }
            };
            
            mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllEmployeesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeDtos);

            IEnumerable<EmployeeDto> result = await employeeService.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Contains(result, e => e.Name == "Employee 1");
            Assert.Contains(result, e => e.Name == "Employee 2");
            Assert.Contains(result, e => e.Name == "Employee 3");
            
            mediatorMock.Verify(m => m.Send(It.IsAny<GetAllEmployeesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByCafeIdAsync_ShouldReturnEmployeeDtos_WhenEmployeesExist_Test()
        {
            Guid cafeId = Guid.NewGuid();
            List<EmployeeDto> employeeDtos = new List<EmployeeDto>
            {
                new EmployeeDto { Id = "employee1", Name = "Employee 1", CafeId = cafeId },
                new EmployeeDto { Id = "employee2", Name = "Employee 2", CafeId = cafeId }
            };
            
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeesByCafeIdQuery>(q => q.CafeId == cafeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeDtos);

            IEnumerable<EmployeeDto> result = await employeeService.GetByCafeIdAsync(cafeId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, e => Assert.Equal(cafeId, e.CafeId));
            
            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeesByCafeIdQuery>(q => q.CafeId == cafeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByCafeIdAsync_ShouldReturnEmptyList_WhenNoEmployeesExist_Test()
        {
            Guid cafeId = Guid.NewGuid();
            List<EmployeeDto> emptyList = new List<EmployeeDto>();
            
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeesByCafeIdQuery>(q => q.CafeId == cafeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyList);
                
            mapperMock
                .Setup(m => m.Map<IEnumerable<Employee>>(emptyList))
                .Returns(new List<Employee>());

            IEnumerable<EmployeeDto> result = await employeeService.GetByCafeIdAsync(cafeId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldSendCreateCommand_AndReturnCreatedEmployeeDto_Test()
        {
            string employeeId = "newemployee";
            string employeeName = "New Employee";
            string emailAddress = "new.employee@example.com";
            string phone = "1234567890";
            Gender gender = Gender.Male;

            CreateEmployeeCommand command = new CreateEmployeeCommand
            {
                Name = employeeName,
                EmailAddress = emailAddress,
                Phone = phone,
                Gender = gender
            };

            Employee createdEmployee = new Employee(employeeId, employeeName, emailAddress, phone, gender);
            EmployeeDto employeeDto = new EmployeeDto 
            { 
                Id = employeeId, 
                Name = employeeName, 
                EmailAddress = emailAddress, 
                Phone = phone, 
                Gender = gender 
            };

            mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdEmployee);

            mapperMock
                .Setup(m => m.Map<EmployeeDto>(createdEmployee))
                .Returns(employeeDto);

            EmployeeDto? result = await employeeService.CreateAsync(command);

            Assert.NotNull(result);
            Assert.Equal(employeeId, result.Id);
            Assert.Equal(employeeName, result.Name);
            Assert.Equal(emailAddress, result.EmailAddress);
            
            mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldSendUpdateCommand_AndReturnUpdatedEmployeeDto_Test()
        {
            string employeeId = "employee1";
            string updatedName = "Updated Employee";
            string updatedEmail = "updated.employee@example.com";
            string updatedPhone = "9876543210";
            Gender gender = Gender.Female;

            UpdateEmployeeCommand command = new UpdateEmployeeCommand
            {
                Id = employeeId,
                Name = updatedName,
                EmailAddress = updatedEmail,
                Phone = updatedPhone,
                Gender = gender
            };

            Employee updatedEmployee = new Employee(employeeId, updatedName, updatedEmail, updatedPhone, gender);
            EmployeeDto employeeDto = new EmployeeDto 
            { 
                Id = employeeId, 
                Name = updatedName, 
                EmailAddress = updatedEmail, 
                Phone = updatedPhone, 
                Gender = gender 
            };

            mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedEmployee);

            mapperMock
                .Setup(m => m.Map<EmployeeDto>(updatedEmployee))
                .Returns(employeeDto);

            EmployeeDto? result = await employeeService.UpdateAsync(command);

            Assert.NotNull(result);
            Assert.Equal(employeeId, result.Id);
            Assert.Equal(updatedName, result.Name);
            Assert.Equal(updatedEmail, result.EmailAddress);
            Assert.Equal(updatedPhone, result.Phone);
            
            mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenEmployeeDoesNotExist_Test()
        {
            string nonExistentEmployeeId = "nonexistent";
            UpdateEmployeeCommand command = new UpdateEmployeeCommand
            {
                Id = nonExistentEmployeeId,
                Name = "Updated Name",
                EmailAddress = "updated.email@example.com",
                Phone = "1234567890",
                Gender = Gender.Male
            };

            mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Employee?)null);

            EmployeeDto? result = await employeeService.UpdateAsync(command);

            Assert.Null(result);
            
            mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSendDeleteCommand_AndReturnTrue_WhenEmployeeExists_Test()
        {
            string employeeId = "employee1";
            DeleteEmployeeCommand command = new DeleteEmployeeCommand { Id = employeeId };

            mediatorMock
                .Setup(m => m.Send(It.Is<DeleteEmployeeCommand>(cmd => cmd.Id == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            bool result = await employeeService.DeleteAsync(employeeId);

            Assert.True(result);
            
            mediatorMock.Verify(m => m.Send(It.Is<DeleteEmployeeCommand>(cmd => cmd.Id == employeeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenEmployeeDoesNotExist_Test()
        {
            string nonExistentEmployeeId = "nonexistent";
            DeleteEmployeeCommand command = new DeleteEmployeeCommand { Id = nonExistentEmployeeId };
            
            mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            bool result = await employeeService.DeleteAsync(nonExistentEmployeeId);

            Assert.False(result);

            mediatorMock.Verify(m => m.Send(It.Is<DeleteEmployeeCommand>(cmd => cmd.Id == nonExistentEmployeeId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
} 