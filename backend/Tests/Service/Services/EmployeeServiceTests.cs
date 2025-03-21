using AutoMapper;
using Business.Entities;
using MediatR;
using Moq;
using Service.Commands.Employees;
using Service.Queries.Employees;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Xunit;

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
        public async Task GetByIdAsync_ShouldReturnEmployee_WhenEmployeeExists_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            EmployeeDto employeeDto = new EmployeeDto { Id = employeeId, Name = "John Doe" };
            Employee employee = new Employee(employeeId, "John Doe", "john.doe@example.com", "1234567890", Gender.Male);
            
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeByIdQuery>(q => q.Id == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeDto);
                
            mapperMock
                .Setup(m => m.Map<Employee>(employeeDto))
                .Returns(employee);

            Employee? result = await employeeService.GetByIdAsync(employeeId);

            Assert.NotNull(result);
            Assert.Equal(employeeId, result.Id);
            Assert.Equal("John Doe", result.Name);
            
            mediatorMock.Verify(m => m.Send(It.Is<GetEmployeeByIdQuery>(q => q.Id == employeeId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEmployeeDoesNotExist_Test()
        {
            string employeeId = "NonExistentEMP";
            
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeeByIdQuery>(q => q.Id == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EmployeeDto?)null);

            Employee? result = await employeeService.GetByIdAsync(employeeId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEmployees_Test()
        {
            string empID1 = UniqueIdGenerator.GenerateUniqueId();
            string empID2 = UniqueIdGenerator.GenerateUniqueId();
            string empID3 = UniqueIdGenerator.GenerateUniqueId();

            List<EmployeeDto> employeeDtos = new List<EmployeeDto>
            {
                new EmployeeDto { Id = empID1, Name = "John Doe" },
                new EmployeeDto { Id = empID2, Name = "Jane Smith" },
                new EmployeeDto { Id = empID3, Name = "Bob Johnson" }
            };
            
            IEnumerable<Employee> employees = new List<Employee>
            {
                new Employee(empID1, "John Doe", "john.doe@example.com", "1234567890", Gender.Male),
                new Employee(empID2, "Jane Smith", "jane.smith@example.com", "0987654321", Gender.Female),
                new Employee(empID3, "Bob Johnson", "bob.johnson@example.com", "5555555555", Gender.Male)
            };
            
            mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllEmployeesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeDtos);
                
            mapperMock
                .Setup(m => m.Map<IEnumerable<Employee>>(employeeDtos))
                .Returns(employees);

            IEnumerable<Employee> result = await employeeService.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            
            mediatorMock.Verify(m => m.Send(It.IsAny<GetAllEmployeesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByCafeIdAsync_ShouldReturnEmployees_WhenEmployeesExist_Test()
        {
            Guid cafeId = Guid.NewGuid();
            string empID1 = UniqueIdGenerator.GenerateUniqueId();
            string empID2 = UniqueIdGenerator.GenerateUniqueId();

            List<EmployeeDto> employeeDtos = new List<EmployeeDto>
            {
                new EmployeeDto { Id = empID1, Name = "John Doe" },
                new EmployeeDto { Id = empID2, Name = "Jane Smith" }
            };
            
            IEnumerable<Employee> employees = new List<Employee>
            {
                new Employee(empID1, "John Doe", "john.doe@example.com", "1234567890", Gender.Male),
                new Employee(empID2, "Jane Smith", "jane.smith@example.com", "0987654321", Gender.Female)
            };
            
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployeesByCafeIdQuery>(q => q.CafeId == cafeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employeeDtos);
                
            mapperMock
                .Setup(m => m.Map<IEnumerable<Employee>>(employeeDtos))
                .Returns(employees);

            IEnumerable<Employee> result = await employeeService.GetByCafeIdAsync(cafeId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, e => e.Name == "John Doe");
            Assert.Contains(result, e => e.Name == "Jane Smith");
            
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

            IEnumerable<Employee> result = await employeeService.GetByCafeIdAsync(cafeId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldSendCreateCommand_AndReturnCreatedEmployee_Test()
        {
            string name = "New Employee";
            string email = "new.employee@example.com";
            string phone = "123-456-7890";
            Gender gender = Gender.Male;
            
            Employee employee = new Employee("", name, email, phone, gender);
            
            Employee createdEmployee = new Employee(UniqueIdGenerator.GenerateUniqueId(), name, email, phone, gender);
            
            mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateEmployeeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdEmployee);

            Employee result = await employeeService.CreateAsync(employee);

            Assert.NotNull(result);
            Assert.Equal(createdEmployee.Id, result.Id);
            Assert.Equal(employee.Name, result.Name);
            
            mediatorMock.Verify(m => m.Send(
                It.Is<CreateEmployeeCommand>(cmd => 
                    cmd.Name == employee.Name && 
                    cmd.EmailAddress == email && 
                    cmd.Phone == phone && 
                    cmd.Gender == gender), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldSendUpdateCommand_AndReturnUpdatedEmployee_Test()
        {
            string id = UniqueIdGenerator.GenerateUniqueId();
            string name = "Updated Employee";
            string email = "updated.employee@example.com";
            string phone = "987-654-3210";
            Gender gender = Gender.Female;
            
            Employee employee = new Employee(id, name, email, phone, gender);
            
            mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateEmployeeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(employee);

            Employee? result = await employeeService.UpdateAsync(employee);

            Assert.NotNull(result);
            Assert.Equal(employee.Id, result.Id);
            Assert.Equal(employee.Name, result.Name);
            
            mediatorMock.Verify(m => m.Send(
                It.Is<UpdateEmployeeCommand>(cmd => 
                    cmd.Id == employee.Id &&
                    cmd.Name == employee.Name && 
                    cmd.EmailAddress == email && 
                    cmd.Phone == phone && 
                    cmd.Gender == gender), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenEmployeeDoesNotExist_Test()
        {
            string id = "NonExistentEMP";
            string name = "Non-existent Employee";
            string email = "nonexistent@example.com";
            string phone = "000-000-0000";
            Gender gender = Gender.Male;
            
            Employee employee = new Employee(id, name, email, phone, gender);
            
            mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateEmployeeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Employee?)null);

            Employee? result = await employeeService.UpdateAsync(employee);

            Assert.Null(result);
            
            mediatorMock.Verify(m => m.Send(It.IsAny<UpdateEmployeeCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSendDeleteCommand_AndReturnTrue_WhenEmployeeExists_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            
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
            string employeeId = "NonExistentEMP";
            
            mediatorMock
                .Setup(m => m.Send(It.Is<DeleteEmployeeCommand>(cmd => cmd.Id == employeeId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            bool result = await employeeService.DeleteAsync(employeeId);

            Assert.False(result);

            mediatorMock.Verify(m => m.Send(It.Is<DeleteEmployeeCommand>(cmd => cmd.Id == employeeId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
} 