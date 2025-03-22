using AutoMapper;
using Business.Entities;
using Moq;
using Resource.Interfaces;
using Service.Handlers.Employees;
using Service.Queries.Employees;
using Utilities;

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
        public async Task Handle_ShouldReturnEmployee_WhenEmployeeExists_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            Employee employee = new Employee(employeeId, "Test Employee", "test0@example.com", "89123456", Gender.Male);
            
            employeeResourceMock
                .Setup(r => r.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);
                
            EmployeeDto employeeDto = new EmployeeDto
            {
                Id = employeeId,
                Name = "Test Employee",
                EmailAddress = "test0@example.com",
                Phone = "89123456",
                Gender = Gender.Male
            };
            
            mapperMock
                .Setup(m => m.Map<EmployeeDto>(employee))
                .Returns(employeeDto);
            
            GetEmployeeByIdQuery query = new GetEmployeeByIdQuery { Id = employeeId };
            
            EmployeeDto? result = await handler.Handle(query, CancellationToken.None);
            
            Assert.NotNull(result);
            Assert.Equal(employeeId, result.Id);
            Assert.Equal("Test Employee", result.Name);
            Assert.Equal("test0@example.com", result.EmailAddress);
            Assert.Equal("89123456", result.Phone);
            Assert.Equal(Gender.Male, result.Gender);
            
            employeeResourceMock.Verify(r => r.GetByIdAsync(employeeId), Times.Once);
            mapperMock.Verify(m => m.Map<EmployeeDto>(employee), Times.Once);
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