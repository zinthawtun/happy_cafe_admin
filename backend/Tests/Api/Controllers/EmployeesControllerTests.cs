using Api.Controllers;
using Api.Models;
using Business.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Xunit;

namespace Tests.Api.Controllers
{
    public class EmployeesControllerTests
    {
        private readonly Mock<IEmployeeService> employeeServiceMock;
        private readonly Mock<ICafeService> cafeServiceMock;
        private readonly Mock<IEmployeeCafeService> employeeCafeServiceMock;
        private readonly EmployeesController employeesController;

        public EmployeesControllerTests()
        {
            employeeServiceMock = new Mock<IEmployeeService>();
            cafeServiceMock = new Mock<ICafeService>();
            employeeCafeServiceMock = new Mock<IEmployeeCafeService>();
            employeesController = new EmployeesController(
                employeeServiceMock.Object,
                cafeServiceMock.Object,
                employeeCafeServiceMock.Object
            );
        }

        [Fact]
        public async Task GetEmployees_ShouldReturnAllEmployees_WhenNoCafeIdProvided_Test()
        {
            List<Employee> employees = new List<Employee>
            {
                new Employee(UniqueIdGenerator.GenerateUniqueId(), "John Doe", "john@example.com", "1234567890", Gender.Male),
                new Employee(UniqueIdGenerator.GenerateUniqueId(), "Jane Smith", "jane@example.com", "0987654321", Gender.Female)
            };

            employeeServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(employees);

            ActionResult<IEnumerable<EmployeeResponseModel>> result = await employeesController.GetEmployees(null);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            IEnumerable<EmployeeResponseModel> returnedEmployees = Assert.IsAssignableFrom<IEnumerable<EmployeeResponseModel>>(okResult.Value);
            
            Assert.Equal(2, returnedEmployees.Count());
            
            employeeServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetEmployees_ShouldReturnEmployeesByCafe_WhenCafeIdProvided_Test()
        {
            Guid cafeId = Guid.NewGuid();
            string emp1 = UniqueIdGenerator.GenerateUniqueId();
            string emp2 = UniqueIdGenerator.GenerateUniqueId();

            List<Employee> employees = new List<Employee>
            {
                new Employee(emp1, "John Doe", "john@example.com", "1234567890", Gender.Male),
                new Employee(emp2, "Jane Smith", "jane@example.com", "0987654321", Gender.Female)
            };

            Cafe cafe = new Cafe(
                cafeId,
                "Test Cafe",
                "Description",
                "Location",
                "logo.png"
            );

            List<EmployeeCafe> employeeCafes = new List<EmployeeCafe>
            {
                new EmployeeCafe(Guid.NewGuid(), cafeId, emp1, DateTime.Now.AddMonths(-3)),
                new EmployeeCafe(Guid.NewGuid(), cafeId, emp2, DateTime.Now.AddMonths(-2))
            };

            foreach (EmployeeCafe ec in employeeCafes)
            {
                typeof(EmployeeCafe).GetProperty("Cafe")?.SetValue(ec, cafe);
            }

            employeeServiceMock
                .Setup(s => s.GetByCafeIdAsync(cafeId))
                .ReturnsAsync(employees);

            employeeCafeServiceMock
                .Setup(s => s.GetByCafeIdAsync(cafeId))
                .ReturnsAsync(employeeCafes);

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(cafeId))
                .ReturnsAsync(cafe);

            ActionResult<IEnumerable<EmployeeResponseModel>> result = await employeesController.GetEmployees(cafeId);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            IEnumerable<EmployeeResponseModel> returnedEmployees = Assert.IsAssignableFrom<IEnumerable<EmployeeResponseModel>>(okResult.Value);
            
            Assert.Equal(2, returnedEmployees.Count());
            Assert.All(returnedEmployees, e => Assert.Equal("Test Cafe", e.Cafe));
            
            employeeServiceMock.Verify(s => s.GetByCafeIdAsync(cafeId), Times.Once);
            employeeCafeServiceMock.Verify(s => s.GetByCafeIdAsync(cafeId), Times.Once);
        }

        [Fact]
        public async Task GetEmployees_ShouldCalculateDaysWorked_FromAssignmentDate_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();

            Employee employee = new Employee(
                employeeId,
                "John Doe",
                "john@example.com",
                "1234567890",
                Gender.Male
            );

            List<Employee> employees = new List<Employee> { employee };

            Guid cafeId = Guid.NewGuid();
            DateTime assignedDate = DateTime.Now.AddDays(-30);

            Cafe cafe = new Cafe(
                cafeId,
                "Test Cafe",
                "Description",
                "Location",
                "logo.png"
            );

            EmployeeCafe employeeCafe = new EmployeeCafe(
                Guid.NewGuid(),
                cafeId,
                employeeId,
                assignedDate
            );

            typeof(EmployeeCafe).GetProperty("Cafe")?.SetValue(employeeCafe, cafe);

            List<EmployeeCafe> employeeCafes = new List<EmployeeCafe> { employeeCafe };

            employeeServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(employees);

            employeeCafeServiceMock
                .Setup(s => s.GetByCafeIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(employeeCafes);

            employeeCafeServiceMock
                .Setup(s => s.GetByEmployeeIdAsync(employeeId))
                .ReturnsAsync(employeeCafe);

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(cafeId))
                .ReturnsAsync(cafe);

            ActionResult<IEnumerable<EmployeeResponseModel>> result = await employeesController.GetEmployees(null);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            List<EmployeeResponseModel> returnedEmployees = Assert.IsAssignableFrom<IEnumerable<EmployeeResponseModel>>(okResult.Value).ToList();

            Assert.Single(returnedEmployees);
            Assert.Equal(30, returnedEmployees[0].DaysWorked);
            Assert.Equal("Test Cafe", returnedEmployees[0].Cafe);
        }

        [Fact]
        public async Task CreateEmployee_ShouldReturnCreatedEmployee_WhenValidData_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            
            CreateEmployeeModel createModel = new CreateEmployeeModel
            {
                Name = "John Doe",
                Address = "123 Main St",
                EmailAddress = "john@example.com",
                StartDate = DateTime.Now
            };

            Employee createdEmployee = new Employee(
                employeeId, 
                "John Doe", 
                "john@example.com", 
                "123 Main St", 
                Gender.Male
            );

            employeeServiceMock
                .Setup(s => s.CreateAsync(It.IsAny<Employee>()))
                .ReturnsAsync(createdEmployee);

            ActionResult<EmployeeResponseModel> result = await employeesController.CreateEmployee(createModel);

            CreatedAtActionResult createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            EmployeeResponseModel returnedEmployee = Assert.IsType<EmployeeResponseModel>(createdAtResult.Value);
            
            Assert.Equal(employeeId, returnedEmployee.Id);
            
            employeeServiceMock.Verify(s => s.CreateAsync(It.IsAny<Employee>()), Times.Once);
        }

        [Fact]
        public async Task CreateEmployee_ShouldAssignToCafe_WhenCafeIdProvided_Test()
        {
            string employeeId = "emp123";
            
            Guid cafeId = Guid.NewGuid();
            
            CreateEmployeeModel createModel = new CreateEmployeeModel
            {
                Name = "John Doe",
                Address = "123 Main St",
                EmailAddress = "john@example.com",
                StartDate = DateTime.Now,
                CafeId = cafeId
            };

            Employee createdEmployee = new Employee(
                employeeId, 
                "John Doe", 
                "john@example.com", 
                "123 Main St", 
                Gender.Male
            );

            Cafe cafe = new Cafe(
                cafeId,
                "Test Cafe",
                "Description",
                "Location",
                "logo.png"
            );

            employeeServiceMock
                .Setup(s => s.CreateAsync(It.IsAny<Employee>()))
                .ReturnsAsync(createdEmployee);

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(cafeId))
                .ReturnsAsync(cafe);

            EmployeeCafe employeeCafe = new EmployeeCafe(Guid.NewGuid(), cafeId, employeeId, DateTime.Now);

            typeof(EmployeeCafe).GetProperty("Cafe")?.SetValue(employeeCafe, cafe);

            employeeCafeServiceMock
                .Setup(s => s.AssignEmployeeToCafeAsync(cafeId, employeeId, It.IsAny<DateTime>()))
                .ReturnsAsync(employeeCafe);

            employeeCafeServiceMock
                .Setup(s => s.GetByEmployeeIdAsync(employeeId))
                .ReturnsAsync(employeeCafe);

            ActionResult<EmployeeResponseModel> result = await employeesController.CreateEmployee(createModel);

            CreatedAtActionResult createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            EmployeeResponseModel returnedEmployee = Assert.IsType<EmployeeResponseModel>(createdAtResult.Value);
            
            Assert.Equal("Test Cafe", returnedEmployee.Cafe);
            
            employeeServiceMock.Verify(s => s.CreateAsync(It.IsAny<Employee>()), Times.Once);
            cafeServiceMock.Verify(s => s.GetByIdAsync(cafeId), Times.Once);
            employeeCafeServiceMock.Verify(s => s.AssignEmployeeToCafeAsync(cafeId, employeeId, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task CreateEmployee_ShouldReturnBadRequest_WhenInvalidCafeId_Test()
        {
            Guid cafeId = Guid.NewGuid();
            
            CreateEmployeeModel createModel = new CreateEmployeeModel
            {
                Name = "John Doe",
                Address = "123 Main St",
                EmailAddress = "john@example.com",
                StartDate = DateTime.Now,
                CafeId = cafeId
            };

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(cafeId))
                .ReturnsAsync((Cafe?)null);

            ActionResult<EmployeeResponseModel> result = await employeesController.CreateEmployee(createModel);

            BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            
            employeeServiceMock.Verify(s => s.CreateAsync(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public async Task UpdateEmployee_ShouldReturnUpdatedEmployee_WhenValidData_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            
            UpdateEmployeeModel updateModel = new UpdateEmployeeModel
            {
                Id = employeeId,
                Name = "Updated Employee",
                Address = "456 Updated St",
                EmailAddress = "updated@example.com"
            };

            Employee existingEmployee = new Employee(
                employeeId,
                "John",
                "john@example.com",
                "123 Main St",
                Gender.Male
            );

            Employee updatedEmployee = new Employee(
                employeeId,
                updateModel.Name,
                updateModel.EmailAddress,
                updateModel.Address,
                Gender.Male
            );

            employeeServiceMock
                .Setup(s => s.GetByIdAsync(employeeId))
                .ReturnsAsync(existingEmployee);

            employeeServiceMock
                .Setup(s => s.UpdateAsync(It.IsAny<Employee>()))
                .ReturnsAsync(updatedEmployee);

            ActionResult<EmployeeResponseModel> result = await employeesController.UpdateEmployee(updateModel);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            EmployeeResponseModel returnedEmployee = Assert.IsType<EmployeeResponseModel>(okResult.Value);
            
            Assert.Equal(employeeId, returnedEmployee.Id);
            Assert.Equal(updateModel.Name, returnedEmployee.Name);
            
            employeeServiceMock.Verify(s => s.GetByIdAsync(employeeId), Times.Once);
            employeeServiceMock.Verify(s => s.UpdateAsync(It.IsAny<Employee>()), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployee_ShouldChangeCafeAssignment_WhenCafeIdChanged_Test()
        {
            string employeeId = "emp123";
            
            Guid oldCafeId = Guid.NewGuid();
            
            Guid newCafeId = Guid.NewGuid();
            
            UpdateEmployeeModel updateModel = new UpdateEmployeeModel
            {
                Id = employeeId,
                Name = "Updated Employee",
                Address = "456 Updated St",
                EmailAddress = "updated@example.com",
                CafeId = newCafeId
            };

            Employee existingEmployee = new Employee(
                employeeId,
                "John Doe",
                "john@example.com",
                "123 Main St",
                Gender.Male
            );

            Employee updatedEmployee = new Employee(
                employeeId,
                "Updated Employee",
                "updated@example.com",
                "456 Updated St",
                Gender.Male
            );

            Cafe oldCafe = new Cafe(
                oldCafeId,
                "Old Cafe",
                "Description",
                "Location",
                "logo.png"
            );

            Cafe newCafe = new Cafe(
                newCafeId,
                "New Cafe",
                "Description",
                "Location",
                "logo.png"
            );

            EmployeeCafe oldEmployeeCafe = new EmployeeCafe(
                Guid.NewGuid(),
                oldCafeId,
                employeeId,
                DateTime.Now.AddMonths(-2)
            );
            typeof(EmployeeCafe).GetProperty("Cafe")?.SetValue(oldEmployeeCafe, oldCafe);

            EmployeeCafe newEmployeeCafe = new EmployeeCafe(
                Guid.NewGuid(),
                newCafeId,
                employeeId,
                DateTime.Now
            );
            typeof(EmployeeCafe).GetProperty("Cafe")?.SetValue(newEmployeeCafe, newCafe);

            employeeServiceMock
                .Setup(s => s.GetByIdAsync(employeeId))
                .ReturnsAsync(existingEmployee);

            employeeServiceMock
                .Setup(s => s.UpdateAsync(It.IsAny<Employee>()))
                .ReturnsAsync(updatedEmployee);

            employeeCafeServiceMock
                .SetupSequence(s => s.GetByEmployeeIdAsync(employeeId))
                .ReturnsAsync(oldEmployeeCafe)
                .ReturnsAsync(newEmployeeCafe);   

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(newCafeId))
                .ReturnsAsync(newCafe);

            employeeCafeServiceMock
                .Setup(s => s.UnassignEmployeeFromCafeAsync(oldEmployeeCafe.Id))
                .ReturnsAsync(true);

            employeeCafeServiceMock
                .Setup(s => s.AssignEmployeeToCafeAsync(newCafeId, employeeId, It.IsAny<DateTime>()))
                .ReturnsAsync(newEmployeeCafe);

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(oldCafeId))
                .ReturnsAsync(oldCafe);

            ActionResult<EmployeeResponseModel> result = await employeesController.UpdateEmployee(updateModel);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            EmployeeResponseModel returnedEmployee = Assert.IsType<EmployeeResponseModel>(okResult.Value);
            
            Assert.Equal("New Cafe", returnedEmployee.Cafe);
            
            employeeServiceMock.Verify(s => s.GetByIdAsync(employeeId), Times.Once);
            employeeServiceMock.Verify(s => s.UpdateAsync(It.IsAny<Employee>()), Times.Once);
            employeeCafeServiceMock.Verify(s => s.UnassignEmployeeFromCafeAsync(oldEmployeeCafe.Id), Times.Once);
            employeeCafeServiceMock.Verify(s => s.AssignEmployeeToCafeAsync(newCafeId, employeeId, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployee_ShouldReturnNotFound_WhenEmployeeDoesNotExist_Test()
        {
            string employeeId = "nonexistent";
            
            UpdateEmployeeModel updateModel = new UpdateEmployeeModel
            {
                Id = employeeId,
                Name = "Updated Employee",
                Address = "456 Updated St",
                EmailAddress = "updated@example.com"
            };

            employeeServiceMock
                .Setup(s => s.GetByIdAsync(employeeId))
                .ReturnsAsync((Employee?)null);

            ActionResult<EmployeeResponseModel> result = await employeesController.UpdateEmployee(updateModel);

            Assert.IsType<NotFoundObjectResult>(result.Result);
            
            employeeServiceMock.Verify(s => s.GetByIdAsync(employeeId), Times.Once);
            employeeServiceMock.Verify(s => s.UpdateAsync(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public async Task DeleteEmployee_ShouldReturnNoContent_WhenSuccessful_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            
            DeleteEmployeeModel deleteModel = new DeleteEmployeeModel
            {
                Id = employeeId
            };

            employeeServiceMock
                .Setup(s => s.DeleteAsync(employeeId))
                .ReturnsAsync(true);

            ActionResult result = await employeesController.DeleteEmployee(deleteModel);

            Assert.IsType<NoContentResult>(result);
            
            employeeServiceMock.Verify(s => s.DeleteAsync(employeeId), Times.Once);
        }

        [Fact]
        public async Task DeleteEmployee_ShouldUnassignFromCafe_BeforeDeleting_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();

            Guid cafeId = Guid.NewGuid();
            Guid employeeCafeId = Guid.NewGuid();
            
            DeleteEmployeeModel deleteModel = new DeleteEmployeeModel
            {
                Id = employeeId
            };

            Cafe cafe = new Cafe(
                cafeId,
                "Test Cafe",
                "Description",
                "Location", 
                "logo.png"
            );

            EmployeeCafe employeeCafe = new EmployeeCafe(
                employeeCafeId,
                cafeId,
                employeeId,
                DateTime.Now.AddMonths(-1)
            );

            typeof(EmployeeCafe).GetProperty("Cafe")?.SetValue(employeeCafe, cafe);

            employeeCafeServiceMock
                .Setup(s => s.GetByEmployeeIdAsync(employeeId))
                .ReturnsAsync(employeeCafe);

            employeeCafeServiceMock
                .Setup(s => s.UnassignEmployeeFromCafeAsync(employeeCafeId))
                .ReturnsAsync(true);

            employeeServiceMock
                .Setup(s => s.DeleteAsync(employeeId))
                .ReturnsAsync(true);

            ActionResult result = await employeesController.DeleteEmployee(deleteModel);

            Assert.IsType<NoContentResult>(result);
            
            employeeCafeServiceMock.Verify(s => s.GetByEmployeeIdAsync(employeeId), Times.Once);
            employeeCafeServiceMock.Verify(s => s.UnassignEmployeeFromCafeAsync(employeeCafeId), Times.Once);
            employeeServiceMock.Verify(s => s.DeleteAsync(employeeId), Times.Once);
        }

        [Fact]
        public async Task DeleteEmployee_ShouldReturnNotFound_WhenEmployeeDoesNotExist_Test()
        {
            string employeeId = "nonexistent";
            
            DeleteEmployeeModel deleteModel = new DeleteEmployeeModel
            {
                Id = employeeId
            };

            employeeServiceMock
                .Setup(s => s.DeleteAsync(employeeId))
                .ReturnsAsync(false);

            ActionResult result = await employeesController.DeleteEmployee(deleteModel);

            Assert.IsType<NotFoundObjectResult>(result);
            
            employeeServiceMock.Verify(s => s.DeleteAsync(employeeId), Times.Once);
        }
    }
} 