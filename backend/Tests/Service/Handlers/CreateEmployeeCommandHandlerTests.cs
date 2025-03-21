using AutoMapper;
using Business.Entities;
using Moq;
using Resource.Interfaces;
using Service.Commands.Employees;
using Service.Handlers.Employees;

namespace Tests.Service.Handlers
{
    public class CreateEmployeeCommandHandlerTests
    {
        private readonly Mock<IEmployeeResource> employeeResourceMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly CreateEmployeeCommandHandler handler;

        public CreateEmployeeCommandHandlerTests()
        {
            employeeResourceMock = new Mock<IEmployeeResource>();
            mapperMock = new Mock<IMapper>();
            handler = new CreateEmployeeCommandHandler(employeeResourceMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateEmployee_AndReturnCreatedEmployee_Test()
        {
            string employeeId = "employee1";
            CreateEmployeeCommand command = new CreateEmployeeCommand
            {
                Name = "Test Employee",
                EmailAddress = "test@example.com",
                Phone = "1234567890",
                Gender = Gender.Male
            };

            Employee employee = new Employee(employeeId, command.Name, command.EmailAddress, command.Phone, command.Gender);

            Employee createdEmployee = new Employee(employeeId, command.Name, command.EmailAddress, command.Phone, command.Gender);

            mapperMock
                .Setup(m => m.Map<Employee>(command))
                .Returns(employee);

            employeeResourceMock
                .Setup(r => r.CreateAsync(command.Name, command.EmailAddress, command.Phone, command.Gender))
                .ReturnsAsync(createdEmployee);

            Employee result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(employeeId, result.Id);
            Assert.Equal(command.Name, result.Name);
            Assert.Equal(command.EmailAddress, result.EmailAddress);
            Assert.Equal(command.Phone, result.Phone);
            Assert.Equal(command.Gender, result.Gender);

            employeeResourceMock.Verify(r => r.CreateAsync(command.Name, command.EmailAddress, command.Phone, command.Gender), Times.Once);
        }
    }
} 