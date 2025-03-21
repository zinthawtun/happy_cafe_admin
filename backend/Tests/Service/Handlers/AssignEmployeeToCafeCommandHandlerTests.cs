using AutoMapper;
using Business.Entities;
using Moq;
using Resource.Interfaces;
using Service.Commands.EmployeeCafes;
using Service.Handlers.EmployeeCafes;

namespace Tests.Service.Handlers
{
    public class AssignEmployeeToCafeCommandHandlerTests
    {
        private readonly Mock<IEmployeeCafeResource> employeeCafeResourceMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly AssignEmployeeToCafeCommandHandler handler;

        public AssignEmployeeToCafeCommandHandlerTests()
        {
            employeeCafeResourceMock = new Mock<IEmployeeCafeResource>();
            mapperMock = new Mock<IMapper>();
            handler = new AssignEmployeeToCafeCommandHandler(employeeCafeResourceMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldAssignEmployeeToCafe_AndReturnAssignment_Test()
        {
            Guid cafeId = Guid.NewGuid();
            string employeeId = "employee1";
            Guid assignmentId = Guid.NewGuid();
            DateTime assignedDate = DateTime.UtcNow.AddDays(-1);

            AssignEmployeeToCafeCommand command = new AssignEmployeeToCafeCommand
            {
                CafeId = cafeId,
                EmployeeId = employeeId,
                AssignedDate = assignedDate
            };

            EmployeeCafe employeeCafe = new EmployeeCafe(assignmentId, cafeId, employeeId, assignedDate);

            employeeCafeResourceMock
                .Setup(r => r.CreateAsync(cafeId, employeeId, assignedDate))
                .ReturnsAsync(employeeCafe);

            EmployeeCafe result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(assignmentId, result.Id);
            Assert.Equal(cafeId, result.CafeId);
            Assert.Equal(employeeId, result.EmployeeId);
            Assert.Equal(assignedDate, result.AssignedDate);
            Assert.True(result.IsActive);

            employeeCafeResourceMock.Verify(r => r.CreateAsync(cafeId, employeeId, assignedDate), Times.Once);
        }
    }
} 