using Api.Controllers;
using Api.Models;
using Business.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Commands.EmployeeCafes;
using Service.Commands.Employees;
using Service.Interfaces;
using Service.Queries.Cafes;
using Service.Queries.EmployeeCafes;
using Service.Queries.Employees;
using Utilities;

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
            List<EmployeeDto> employeeDtos = new List<EmployeeDto>
            {
                new EmployeeDto { Id = UniqueIdGenerator.GenerateUniqueId(), Name = "John Doe", EmailAddress = "john@example.com", Phone = "87654321", Gender = Gender.Male },
                new EmployeeDto { Id = UniqueIdGenerator.GenerateUniqueId(), Name = "Jane Smith", EmailAddress = "jane@example.com", Phone = "98765432", Gender = Gender.Female }
            };

            employeeServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(employeeDtos);

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

            List<EmployeeDto> employeeDtos = new List<EmployeeDto>
            {
                new EmployeeDto { Id = emp1, Name = "John Doe", EmailAddress = "john@example.com", Phone = "87654321", Gender = Gender.Male, CafeName = "Test Cafe" },
                new EmployeeDto { Id = emp2, Name = "Jane Smith", EmailAddress = "jane@example.com", Phone = "98765432", Gender = Gender.Female, CafeName = "Test Cafe" }
            };

            CafeDto cafeDto = new CafeDto
            {
                Id = cafeId,
                Name = "Test Cafe",
                Description = "Description",
                Location = "Location",
                Logo = "logo.png"
            };

            List<EmployeeCafeDto> employeeCafeDtos = new List<EmployeeCafeDto>
            {
                new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafeId, EmployeeId = emp1, AssignedDate = DateTime.Now.AddMonths(-3), IsActive = true },
                new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafeId, EmployeeId = emp2, AssignedDate = DateTime.Now.AddMonths(-2), IsActive = true }
            };

            employeeServiceMock
                .Setup(s => s.GetByCafeIdAsync(cafeId))
                .ReturnsAsync(employeeDtos);

            employeeCafeServiceMock
                .Setup(s => s.GetByCafeIdAsync(cafeId))
                .ReturnsAsync(employeeCafeDtos);

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(cafeId))
                .ReturnsAsync(cafeDto);

            ActionResult<IEnumerable<EmployeeResponseModel>> result = await employeesController.GetEmployees(cafeId);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            IEnumerable<EmployeeResponseModel> returnedEmployees = Assert.IsAssignableFrom<IEnumerable<EmployeeResponseModel>>(okResult.Value);
            
            Assert.Equal(2, returnedEmployees.Count());
            Assert.All(returnedEmployees, e => Assert.Equal("Test Cafe", e.Cafe));
            
            employeeServiceMock.Verify(s => s.GetByCafeIdAsync(cafeId), Times.Once);
        }

        [Fact]
        public async Task GetEmployees_ShouldCalculateDaysWorked_FromAssignmentDate_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();

            EmployeeDto employeeDto = new EmployeeDto
            {
                Id = employeeId,
                Name = "John Doe",
                EmailAddress = "john@example.com",
                Phone = "87654321",
                Gender = Gender.Male,
                DaysWorked = 30,
                CafeName = "Test Cafe"
            };

            List<EmployeeDto> employeeDtos = new List<EmployeeDto> { employeeDto };

            Guid cafeId = Guid.NewGuid();
            DateTime assignedDate = DateTime.Now.AddDays(-30);

            CafeDto cafeDto = new CafeDto
            {
                Id = cafeId,
                Name = "Test Cafe",
                Description = "Description",
                Location = "Location",
                Logo = "logo.png"
            };

            employeeDto.CafeId = cafeId;    

            EmployeeCafeDto employeeCafeDto = new EmployeeCafeDto
            {
                Id = Guid.NewGuid(),
                CafeId = cafeId,
                EmployeeId = employeeId,
                AssignedDate = assignedDate,
                IsActive = true
            };

            List<EmployeeCafeDto> employeeCafeDtos = new List<EmployeeCafeDto> { employeeCafeDto };

            employeeServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(employeeDtos);

            employeeCafeServiceMock
                .Setup(s => s.GetByCafeIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(employeeCafeDtos);

            employeeCafeServiceMock
                .Setup(s => s.GetByEmployeeIdAsync(employeeId))
                .ReturnsAsync(employeeCafeDto);

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(cafeId))
                .ReturnsAsync(cafeDto);

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
                Phone = "89876543",
                EmailAddress = "john@example.com",
                Gender = "Male"
            };

            CreateEmployeeCommand command = new CreateEmployeeCommand
            {
                Name = "John Doe",
                EmailAddress = "john@example.com",
                Phone = "89876543",
                Gender = Gender.Male
            };

            EmployeeDto employeeDto = new EmployeeDto
            {
                Id = employeeId,
                Name = "John Doe",
                EmailAddress = "john@example.com",
                Phone = "89876543",
                Gender = Gender.Male
            };

            employeeServiceMock
                .Setup(s => s.ExistsWithEmailOrPhoneAsync(createModel.EmailAddress, createModel.Phone))
                .ReturnsAsync(false);

            employeeServiceMock
                .Setup(s => s.CreateAsync(It.IsAny<CreateEmployeeCommand>()))
                .ReturnsAsync(employeeDto);

            ActionResult<EmployeeResponseModel> result = await employeesController.CreateEmployee(createModel);

            CreatedAtActionResult createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            EmployeeResponseModel returnedEmployee = Assert.IsType<EmployeeResponseModel>(createdAtResult.Value);
            
            Assert.Equal(employeeId, returnedEmployee.Id);
            Assert.Equal("GetEmployees", createdAtResult.ActionName);
            
            employeeServiceMock.Verify(s => s.ExistsWithEmailOrPhoneAsync(createModel.EmailAddress, createModel.Phone), Times.Once);
            employeeServiceMock.Verify(s => s.CreateAsync(It.IsAny<CreateEmployeeCommand>()), Times.Once);
        }

        [Fact]
        public async Task CreateEmployee_ShouldAssignToCafe_WhenCafeIdProvided_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            Guid cafeId = Guid.NewGuid();
            
            CreateEmployeeModel createModel = new CreateEmployeeModel
            {
                Name = "John Doe",
                Phone = "89876543",
                EmailAddress = "john@example.com",
                Gender = "Male",
                CafeId = cafeId
            };

            EmployeeDto employeeDto = new EmployeeDto
            {
                Id = employeeId,
                Name = "John Doe",
                EmailAddress = "john@example.com",
                Phone = "89876543",
                Gender = Gender.Male
            };

            CafeDto cafeDto = new CafeDto
            {
                Id = cafeId,
                Name = "Test Cafe",
                Description = "Description",
                Location = "Location",
                Logo = "logo.png"
            };

            EmployeeCafeDto employeeCafeDto = new EmployeeCafeDto
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                CafeId = cafeId,
                AssignedDate = It.IsAny<DateTime>(),
                IsActive = true,
                CafeName = "Test Cafe"
            };

            employeeServiceMock
                .Setup(s => s.ExistsWithEmailOrPhoneAsync(createModel.EmailAddress, createModel.Phone))
                .ReturnsAsync(false);

            employeeServiceMock
                .Setup(s => s.CreateAsync(It.IsAny<CreateEmployeeCommand>()))
                .ReturnsAsync(employeeDto);

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(cafeId))
                .ReturnsAsync(cafeDto);

            employeeCafeServiceMock
                .Setup(s => s.AssignEmployeeToCafeAsync(It.IsAny<AssignEmployeeToCafeCommand>()))
                .ReturnsAsync(employeeCafeDto);

            employeeCafeServiceMock
                .Setup(s => s.GetByEmployeeIdAsync(employeeId))
                .ReturnsAsync(employeeCafeDto);

            ActionResult<EmployeeResponseModel> result = await employeesController.CreateEmployee(createModel);

            CreatedAtActionResult createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            EmployeeResponseModel returnedEmployee = Assert.IsType<EmployeeResponseModel>(createdAtResult.Value);
            
            Assert.Equal(employeeId, returnedEmployee.Id);
            Assert.Equal("Test Cafe", returnedEmployee.Cafe);
            Assert.Equal("GetEmployees", createdAtResult.ActionName);
            
            employeeServiceMock.Verify(s => s.ExistsWithEmailOrPhoneAsync(createModel.EmailAddress, createModel.Phone), Times.Once);
            employeeServiceMock.Verify(s => s.CreateAsync(It.IsAny<CreateEmployeeCommand>()), Times.Once);
            employeeCafeServiceMock.Verify(s => s.AssignEmployeeToCafeAsync(It.IsAny<AssignEmployeeToCafeCommand>()), Times.Once);
        }

        [Fact]
        public async Task CreateEmployee_ShouldReturnBadRequest_WhenInvalidCafeId_Test()
        {
            Guid invalidCafeId = Guid.NewGuid();
            
            CreateEmployeeModel createModel = new CreateEmployeeModel
            {
                Name = "John Doe",
                Phone = "89876543",
                EmailAddress = "john@example.com",
                Gender = "Male",
                CafeId = invalidCafeId
            };

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(invalidCafeId))
                .ReturnsAsync((CafeDto?)null);

            ActionResult<EmployeeResponseModel> result = await employeesController.CreateEmployee(createModel);

            Assert.IsType<BadRequestObjectResult>(result.Result);
            
            employeeServiceMock.Verify(s => s.CreateAsync(It.IsAny<CreateEmployeeCommand>()), Times.Never);
            employeeCafeServiceMock.Verify(s => s.AssignEmployeeToCafeAsync(It.IsAny<AssignEmployeeToCafeCommand>()), Times.Never);
        }

        [Fact]
        public async Task CreateEmployee_ShouldReturnBadRequest_WhenDuplicateEmailOrPhone_Test()
        {
            CreateEmployeeModel createModel = new CreateEmployeeModel
            {
                Name = "John Doe",
                Phone = "89876543",
                EmailAddress = "john@example.com",
                Gender = "Male"
            };

            employeeServiceMock
                .Setup(s => s.ExistsWithEmailOrPhoneAsync(createModel.EmailAddress, createModel.Phone))
                .ReturnsAsync(true);

            ActionResult<EmployeeResponseModel> result = await employeesController.CreateEmployee(createModel);

            BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("An employee with the same email address or phone number already exists.", badRequestResult.Value);
            
            employeeServiceMock.Verify(s => s.ExistsWithEmailOrPhoneAsync(createModel.EmailAddress, createModel.Phone), Times.Once);
            employeeServiceMock.Verify(s => s.CreateAsync(It.IsAny<CreateEmployeeCommand>()), Times.Never);
        }

        [Fact]
        public async Task UpdateEmployee_ShouldReturnUpdatedEmployee_WhenValidData_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            
            UpdateEmployeeModel updateModel = new UpdateEmployeeModel
            {
                Id = employeeId,
                Name = "Updated John Doe",
                Phone = "98765432",
                EmailAddress = "updated.john@example.com",
                Gender = "Male"
            };

            EmployeeDto existingEmployeeDto = new EmployeeDto
            {
                Id = employeeId,
                Name = "John Doe",
                EmailAddress = "john@example.com",
                Phone = "87654321",
                Gender = Gender.Male
            };

            EmployeeDto updatedEmployeeDto = new EmployeeDto
            {
                Id = employeeId,
                Name = "Updated John Doe",
                EmailAddress = "updated.john@example.com",
                Phone = "98765432",
                Gender = Gender.Male
            };

            employeeServiceMock
                .Setup(s => s.GetByIdAsync(employeeId))
                .ReturnsAsync(existingEmployeeDto);

            employeeServiceMock
                .Setup(s => s.UpdateAsync(It.IsAny<UpdateEmployeeCommand>()))
                .ReturnsAsync(updatedEmployeeDto);

            employeeCafeServiceMock
                .Setup(s => s.GetByEmployeeIdAsync(employeeId))
                .ReturnsAsync((EmployeeCafeDto?)null);

            ActionResult<EmployeeResponseModel> result = await employeesController.UpdateEmployee(updateModel);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            EmployeeResponseModel returnedEmployee = Assert.IsType<EmployeeResponseModel>(okResult.Value);
            
            Assert.Equal(employeeId, returnedEmployee.Id);
            Assert.Equal("Updated John Doe", returnedEmployee.Name);
            Assert.Equal("updated.john@example.com", returnedEmployee.EmailAddress);
            
            employeeServiceMock.Verify(s => s.GetByIdAsync(employeeId), Times.Once);
            employeeServiceMock.Verify(s => s.UpdateAsync(It.IsAny<UpdateEmployeeCommand>()), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployee_ShouldChangeCafeAssignment_WhenCafeIdChanged_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            Guid oldCafeId = Guid.NewGuid();
            Guid newCafeId = Guid.NewGuid();
            Guid employeeCafeId = Guid.NewGuid();
            
            UpdateEmployeeModel updateModel = new UpdateEmployeeModel
            {
                Id = employeeId,
                Name = "John Doe",
                Phone = "89876543",
                EmailAddress = "john@example.com",
                Gender = "Male",
                CafeId = newCafeId
            };

            EmployeeDto existingEmployeeDto = new EmployeeDto
            {
                Id = employeeId,
                Name = "John Doe",
                EmailAddress = "john@example.com",
                Phone = "87654321",
                Gender = Gender.Male,
                CafeName = "Old Cafe"
            };

            EmployeeDto updatedEmployeeDto = new EmployeeDto
            {
                Id = employeeId,
                Name = "John Doe",
                EmailAddress = "john@example.com",
                Phone = "89876543",
                Gender = Gender.Male,
                CafeName = "New Cafe"
            };

            CafeDto oldCafeDto = new CafeDto
            {
                Id = oldCafeId,
                Name = "Old Cafe",
                Description = "Old Description",
                Location = "Old Location",
                Logo = "old-logo.png"
            };

            CafeDto newCafeDto = new CafeDto
            {
                Id = newCafeId,
                Name = "New Cafe",
                Description = "New Description",
                Location = "New Location",
                Logo = "new-logo.png"
            };

            EmployeeCafeDto oldEmployeeCafeDto = new EmployeeCafeDto
            {
                Id = employeeCafeId,
                EmployeeId = employeeId,
                CafeId = oldCafeId,
                AssignedDate = DateTime.Now.AddMonths(-1),
                IsActive = true,
                CafeName = "Old Cafe"
            };

            EmployeeCafeDto newEmployeeCafeDto = new EmployeeCafeDto
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                CafeId = newCafeId,
                AssignedDate = It.IsAny<DateTime>(),
                IsActive = true,
                CafeName = "New Cafe"
            };

            employeeServiceMock
                .Setup(s => s.GetByIdAsync(employeeId))
                .ReturnsAsync(existingEmployeeDto);

            employeeServiceMock
                .Setup(s => s.UpdateAsync(It.IsAny<UpdateEmployeeCommand>()))
                .ReturnsAsync(updatedEmployeeDto);

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(oldCafeId))
                .ReturnsAsync(oldCafeDto);

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(newCafeId))
                .ReturnsAsync(newCafeDto);

           
            employeeCafeServiceMock
                .SetupSequence(s => s.GetByEmployeeIdAsync(employeeId))
                .ReturnsAsync(oldEmployeeCafeDto)   
                .ReturnsAsync(newEmployeeCafeDto); 

            employeeCafeServiceMock
                .Setup(s => s.UnassignEmployeeFromCafeAsync(employeeCafeId))
                .ReturnsAsync(true);

            employeeCafeServiceMock
                .Setup(s => s.AssignEmployeeToCafeAsync(It.IsAny<AssignEmployeeToCafeCommand>()))
                .ReturnsAsync(newEmployeeCafeDto);

            ActionResult<EmployeeResponseModel> result = await employeesController.UpdateEmployee(updateModel);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            EmployeeResponseModel returnedEmployee = Assert.IsType<EmployeeResponseModel>(okResult.Value);
            
            Assert.Equal(employeeId, returnedEmployee.Id);
            Assert.Equal("New Cafe", returnedEmployee.Cafe);
            
            employeeServiceMock.Verify(s => s.GetByIdAsync(employeeId), Times.Once);
            employeeServiceMock.Verify(s => s.UpdateAsync(It.IsAny<UpdateEmployeeCommand>()), Times.Once);
            employeeCafeServiceMock.Verify(s => s.UnassignEmployeeFromCafeAsync(employeeCafeId), Times.Once);
            employeeCafeServiceMock.Verify(s => s.AssignEmployeeToCafeAsync(It.IsAny<AssignEmployeeToCafeCommand>()), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployee_ShouldReturnNotFound_WhenEmployeeDoesNotExist_Test()
        {
            string nonExistentEmployeeId = "nonexistent";
            
            UpdateEmployeeModel updateModel = new UpdateEmployeeModel
            {
                Id = nonExistentEmployeeId,
                Name = "Updated Name",
                Phone = "87654321",
                EmailAddress = "updated.email@example.com",
                Gender = "Male"
            };

            employeeServiceMock
                .Setup(s => s.GetByIdAsync(nonExistentEmployeeId))
                .ReturnsAsync((EmployeeDto?)null);

            ActionResult<EmployeeResponseModel> result = await employeesController.UpdateEmployee(updateModel);

            Assert.IsType<NotFoundObjectResult>(result.Result);
            
            employeeServiceMock.Verify(s => s.GetByIdAsync(nonExistentEmployeeId), Times.Once);
            employeeServiceMock.Verify(s => s.UpdateAsync(It.IsAny<UpdateEmployeeCommand>()), Times.Never);
        }

        [Fact]
        public async Task DeleteEmployee_ShouldReturnNoContent_WhenSuccessful_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            DeleteEmployeeModel deleteModel = new DeleteEmployeeModel { Id = employeeId };

            employeeServiceMock
                .Setup(s => s.DeleteAsync(employeeId))
                .ReturnsAsync(true);

            employeeCafeServiceMock
                .Setup(s => s.GetByEmployeeIdAsync(employeeId))
                .ReturnsAsync((EmployeeCafeDto?)null);

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
            
            DeleteEmployeeModel deleteModel = new DeleteEmployeeModel { Id = employeeId };

            EmployeeCafeDto employeeCafeDto = new EmployeeCafeDto
            {
                Id = employeeCafeId,
                EmployeeId = employeeId,
                CafeId = cafeId,
                AssignedDate = DateTime.Now.AddMonths(-1),
                IsActive = true
            };

            employeeCafeServiceMock
                .Setup(s => s.GetByEmployeeIdAsync(employeeId))
                .ReturnsAsync(employeeCafeDto);

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
            string nonExistentEmployeeId = "nonexistent";
            DeleteEmployeeModel deleteModel = new DeleteEmployeeModel { Id = nonExistentEmployeeId };

            employeeCafeServiceMock
                .Setup(s => s.GetByEmployeeIdAsync(nonExistentEmployeeId))
                .ReturnsAsync((EmployeeCafeDto?)null);

            employeeServiceMock
                .Setup(s => s.DeleteAsync(nonExistentEmployeeId))
                .ReturnsAsync(false);

            ActionResult result = await employeesController.DeleteEmployee(deleteModel);

            Assert.IsType<NotFoundObjectResult>(result);
            
            employeeCafeServiceMock.Verify(s => s.GetByEmployeeIdAsync(nonExistentEmployeeId), Times.Once);
            employeeServiceMock.Verify(s => s.DeleteAsync(nonExistentEmployeeId), Times.Once);
        }

        [Fact]
        public async Task GetEmployeeById_ShouldReturnEmployeeDetails_WhenValidId_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            Guid cafeId = Guid.NewGuid();
            DateTime assignedDate = DateTime.UtcNow.AddDays(-30);

            EmployeeDto employeeDto = new EmployeeDto
            {
                Id = employeeId,
                Name = "John Doe",
                EmailAddress = "john@example.com",
                Phone = "87654321",
                Gender = Gender.Male
            };

            EmployeeCafeDto employeeCafeDto = new EmployeeCafeDto
            {
                Id = Guid.NewGuid(),
                CafeId = cafeId,
                CafeName = "Test Cafe",
                EmployeeId = employeeId,
                AssignedDate = assignedDate,
                IsActive = true
            };

            employeeServiceMock
                .Setup(s => s.GetByIdAsync(employeeId))
                .ReturnsAsync(employeeDto);

            employeeCafeServiceMock
                .Setup(s => s.GetByEmployeeIdAsync(employeeId))
                .ReturnsAsync(employeeCafeDto);

            ActionResult<EmployeeDetailResponseModel> result = await employeesController.GetEmployeeById(employeeId);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            EmployeeDetailResponseModel returnedEmployee = Assert.IsType<EmployeeDetailResponseModel>(okResult.Value);
            
            Assert.Equal(employeeId, returnedEmployee.Id);
            Assert.Equal("John Doe", returnedEmployee.Name);
            Assert.Equal("john@example.com", returnedEmployee.EmailAddress);
            Assert.Equal("87654321", returnedEmployee.Phone);
            Assert.Equal("Male", returnedEmployee.Gender);
            Assert.Equal(cafeId, returnedEmployee.CafeId);
            Assert.Equal("Test Cafe", returnedEmployee.CafeName);
            Assert.Equal(assignedDate, returnedEmployee.StartDate);
            
            employeeServiceMock.Verify(s => s.GetByIdAsync(employeeId), Times.Once);
            employeeCafeServiceMock.Verify(s => s.GetByEmployeeIdAsync(employeeId), Times.Once);
        }
        
        [Fact]
        public async Task GetEmployeeById_ShouldReturnNotFound_WhenInvalidId_Test()
        {
            string employeeId = UniqueIdGenerator.GenerateUniqueId();
            
            employeeServiceMock
                .Setup(s => s.GetByIdAsync(employeeId))
                .ReturnsAsync((EmployeeDto?)null);

            ActionResult<EmployeeDetailResponseModel> result = await employeesController.GetEmployeeById(employeeId);

            Assert.IsType<NotFoundObjectResult>(result.Result);
            
            employeeServiceMock.Verify(s => s.GetByIdAsync(employeeId), Times.Once);
        }
    }
} 