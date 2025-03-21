using AutoMapper;
using Business.Entities;
using Moq;
using Resource.Interfaces;
using Service.Handlers.Employees;
using Service.Queries.Employees;

namespace Tests.Service.Handlers
{
    public class GetEmployeeByIdQueryHandlerTests
    {
        private readonly Mock<IEmployeeResource> employeeResourceMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly GetEmployeeByIdQueryHandler handler;

        public GetEmployeeByIdQueryHandlerTests()
        {
            employeeResourceMock = new Mock<IEmployeeResource>();
            mapperMock = new Mock<IMapper>();
            handler = new GetEmployeeByIdQueryHandler(employeeResourceMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmployeeDto_WhenEmployeeExists_Test()
        {
            string employeeId = "employee1";
            GetEmployeeByIdQuery query = new GetEmployeeByIdQuery { Id = employeeId };

            Employee employee = new Employee(employeeId, "Test Employee", "test@example.com", "1234567890", Gender.Male);

            EmployeeDto employeeDto = new EmployeeDto
            {
                Id = employeeId,
                Name = "Test Employee",
                EmailAddress = "test@example.com",
                Phone = "1234567890",
                Gender = Gender.Male
            };

            employeeResourceMock
                .Setup(r => r.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            mapperMock
                .Setup(m => m.Map<EmployeeDto>(employee))
                .Returns(employeeDto);

            EmployeeDto? result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(employeeId, result.Id);
            Assert.Equal(employee.Name, result.Name);
            Assert.Equal(employee.EmailAddress, result.EmailAddress);
            Assert.Equal(employee.Phone, result.Phone);
            Assert.Equal(employee.Gender, result.Gender);

            employeeResourceMock.Verify(r => r.GetByIdAsync(employeeId), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenEmployeeDoesNotExist_Test()
        {
            string employeeId = "employee1";
            GetEmployeeByIdQuery query = new GetEmployeeByIdQuery { Id = employeeId };

            employeeResourceMock
                .Setup(r => r.GetByIdAsync(employeeId))
                .ReturnsAsync((Employee?)null);

            EmployeeDto? result = await handler.Handle(query, CancellationToken.None);

            Assert.Null(result);

            employeeResourceMock.Verify(r => r.GetByIdAsync(employeeId), Times.Once);
        }
    }
} 