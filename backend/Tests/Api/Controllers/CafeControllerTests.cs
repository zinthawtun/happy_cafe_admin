using Api.Controllers;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Commands.Cafes;
using Service.Interfaces;
using Service.Queries.Cafes;
using Service.Queries.EmployeeCafes;
using Utilities;

namespace Tests.Api.Controllers
{
    public class CafeControllerTests
    {
        private readonly Mock<ICafeService> cafeServiceMock;
        private readonly Mock<IEmployeeCafeService> employeeCafeServiceMock;
        private readonly CafesController cafesController;

        public CafeControllerTests()
        {
            cafeServiceMock = new Mock<ICafeService>();
            employeeCafeServiceMock = new Mock<IEmployeeCafeService>();
            cafesController = new CafesController(cafeServiceMock.Object, employeeCafeServiceMock.Object);
        }

        [Fact]
        public async Task GetCafes_ShouldReturnAllCafes_WhenNoLocationProvided_Test()
        {
            IEnumerable<CafeDto> cafeDtos = new List<CafeDto>
            {
                new CafeDto { Id = Guid.NewGuid(), Name = "Cafe 1", Description = "Description 1", Location = "Location 1", Logo = "logo1.png" },
                new CafeDto { Id = Guid.NewGuid(), Name = "Cafe 2", Description = "Description 2", Location = "Location 2", Logo = "logo2.png" }
            };

            cafeServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(cafeDtos);
                
            foreach (CafeDto cafeDto in cafeDtos)
            {
                List<EmployeeCafeDto> employeeCafeDtos = new List<EmployeeCafeDto>
                {
                    new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafeDto.Id, EmployeeId = UniqueIdGenerator.GenerateUniqueId(), AssignedDate = DateTime.UtcNow, IsActive = true },
                    new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafeDto.Id, EmployeeId = UniqueIdGenerator.GenerateUniqueId(), AssignedDate = DateTime.UtcNow, IsActive = true },
                    new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafeDto.Id, EmployeeId = UniqueIdGenerator.GenerateUniqueId(), AssignedDate = DateTime.UtcNow, IsActive = true },
                    new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafeDto.Id, EmployeeId = UniqueIdGenerator.GenerateUniqueId(), AssignedDate = DateTime.UtcNow, IsActive = true },
                    new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafeDto.Id, EmployeeId = UniqueIdGenerator.GenerateUniqueId(), AssignedDate = DateTime.UtcNow, IsActive = true }
                };
                
                employeeCafeServiceMock
                    .Setup(s => s.GetByCafeIdAsync(cafeDto.Id))
                    .ReturnsAsync(employeeCafeDtos);
            }

            ActionResult<IEnumerable<CafeResponseModel>> result = await cafesController.GetCafes();

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            IEnumerable<CafeResponseModel> returnedCafes = Assert.IsAssignableFrom<IEnumerable<CafeResponseModel>>(okResult.Value);
            
            Assert.Equal(2, returnedCafes.Count());
            Assert.All(returnedCafes, c => Assert.Equal(5, c.Employees));
            
            cafeServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetCafes_ShouldReturnCafesByLocation_WhenLocationProvided_Test()
        {
            string location = "Ang Mo Kio";
            IEnumerable<CafeDto> cafeDtos = new List<CafeDto>
            {
                new CafeDto { Id = Guid.NewGuid(), Name = "Cafe 1", Description = "Description 1", Location = location, Logo = "logo1.png" },
                new CafeDto { Id = Guid.NewGuid(), Name = "Cafe 2", Description = "Description 2", Location = location, Logo = "logo2.png" }
            };

            cafeServiceMock
                .Setup(s => s.GetByLocationAsync(location))
                .ReturnsAsync(cafeDtos);
                
            foreach (CafeDto cafeDto in cafeDtos)
            {
                List<EmployeeCafeDto> employeeCafeDtos = new List<EmployeeCafeDto>
                {
                    new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafeDto.Id, EmployeeId = UniqueIdGenerator.GenerateUniqueId(), AssignedDate = DateTime.UtcNow, IsActive = true },
                    new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafeDto.Id, EmployeeId = UniqueIdGenerator.GenerateUniqueId(), AssignedDate = DateTime.UtcNow, IsActive = true },
                    new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafeDto.Id, EmployeeId = UniqueIdGenerator.GenerateUniqueId(), AssignedDate = DateTime.UtcNow, IsActive = true },
                    new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafeDto.Id, EmployeeId = UniqueIdGenerator.GenerateUniqueId(), AssignedDate = DateTime.UtcNow, IsActive = true },
                    new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafeDto.Id, EmployeeId = UniqueIdGenerator.GenerateUniqueId(), AssignedDate = DateTime.UtcNow, IsActive = true }
                };
                
                employeeCafeServiceMock
                    .Setup(s => s.GetByCafeIdAsync(cafeDto.Id))
                    .ReturnsAsync(employeeCafeDtos);
            }

            ActionResult<IEnumerable<CafeResponseModel>> result = await cafesController.GetCafes(location);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            IEnumerable<CafeResponseModel> returnedCafes = Assert.IsAssignableFrom<IEnumerable<CafeResponseModel>>(okResult.Value);
            
            Assert.Equal(2, returnedCafes.Count());
            Assert.All(returnedCafes, c => Assert.Equal(location, c.Location));

            cafeServiceMock.Verify(s => s.GetByLocationAsync(location), Times.Once);
        }

        [Fact]
        public async Task GetCafes_ShouldReturnEmptyList_WhenNoMatchingLocation_Test()
        {
            string location = "Non-existent Location";
            IEnumerable<CafeDto> cafeDtos = new List<CafeDto>();

            cafeServiceMock
                .Setup(s => s.GetByLocationAsync(location))
                .ReturnsAsync(cafeDtos);

            ActionResult<IEnumerable<CafeResponseModel>> result = await cafesController.GetCafes(location);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            IEnumerable<CafeResponseModel> returnedCafes = Assert.IsAssignableFrom<IEnumerable<CafeResponseModel>>(okResult.Value);
            
            Assert.Empty(returnedCafes);
            
            cafeServiceMock.Verify(s => s.GetByLocationAsync(location), Times.Once);
        }

        [Fact]
        public async Task GetCafes_ShouldOrderByEmployeeCount_Descending_Test()
        {
            Guid cafe1Id = Guid.NewGuid();
            Guid cafe2Id = Guid.NewGuid();
            Guid cafe3Id = Guid.NewGuid();

            IEnumerable<CafeDto> cafeDtos = new List<CafeDto>
            {
                new CafeDto { Id = cafe1Id, Name = "Cafe 1", Description = "Description 1", Location = "Location 1", Logo = "logo1.png" },
                new CafeDto { Id = cafe2Id, Name = "Cafe 2", Description = "Description 2", Location = "Location 2", Logo = "logo2.png" },
                new CafeDto { Id = cafe3Id, Name = "Cafe 3", Description = "Description 3", Location = "Location 3", Logo = "logo3.png" }
            };

            cafeServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(cafeDtos);

            List<EmployeeCafeDto> cafe1Employees = new List<EmployeeCafeDto>();
            for (int i = 0; i < 1; i++)
            {
                cafe1Employees.Add(new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafe1Id, EmployeeId = UniqueIdGenerator.GenerateUniqueId(), AssignedDate = DateTime.UtcNow, IsActive = true });
            }
            employeeCafeServiceMock
                .Setup(s => s.GetByCafeIdAsync(cafe1Id))
                .ReturnsAsync(cafe1Employees);

            List<EmployeeCafeDto> cafe2Employees = new List<EmployeeCafeDto>();
            for (int i = 0; i < 3; i++)
            {
                cafe2Employees.Add(new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafe2Id, EmployeeId = UniqueIdGenerator.GenerateUniqueId(), AssignedDate = DateTime.UtcNow, IsActive = true });
            }
            employeeCafeServiceMock
                .Setup(s => s.GetByCafeIdAsync(cafe2Id))
                .ReturnsAsync(cafe2Employees);

            List<EmployeeCafeDto> cafe3Employees = new List<EmployeeCafeDto>();
            for (int i = 0; i < 2; i++)
            {
                cafe3Employees.Add(new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafe3Id, EmployeeId = UniqueIdGenerator.GenerateUniqueId(), AssignedDate = DateTime.UtcNow, IsActive = true });
            }
            employeeCafeServiceMock
                .Setup(s => s.GetByCafeIdAsync(cafe3Id))
                .ReturnsAsync(cafe3Employees);

            ActionResult<IEnumerable<CafeResponseModel>> result = await cafesController.GetCafes();

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            List<CafeResponseModel> returnedCafes = Assert.IsAssignableFrom<IEnumerable<CafeResponseModel>>(okResult.Value).ToList();
            
            Assert.Equal(3, returnedCafes.Count);
            Assert.Equal(3, returnedCafes[0].Employees);
            Assert.Equal(2, returnedCafes[1].Employees);
            Assert.Equal(1, returnedCafes[2].Employees);
            
            cafeServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateCafe_ShouldReturnCreatedCafe_WhenValidData_Test()
        {
            Guid cafeId = Guid.NewGuid();
            string cafeName = "New Cafe";
            string description = "New Description";
            string logo = "new-logo.png";
            string location = "New Location";

            CreateCafeModel model = new CreateCafeModel
            {
                Name = cafeName,
                Description = description,
                Logo = logo,
                Location = location
            };

            CafeDto cafeDto = new CafeDto
            {
                Id = cafeId,
                Name = cafeName,
                Description = description,
                Logo = logo,
                Location = location
            };

            CreateCafeCommand command = new CreateCafeCommand
            {
                Name = cafeName,
                Description = description,
                Logo = logo,
                Location = location
            };

            cafeServiceMock
                .Setup(s => s.ExistsByNameAsync(cafeName))
                .ReturnsAsync(false);

            cafeServiceMock
                .Setup(s => s.CreateAsync(It.Is<CreateCafeCommand>(c => 
                    c.Name == cafeName && 
                    c.Description == description && 
                    c.Logo == logo && 
                    c.Location == location)))
                .ReturnsAsync(cafeDto);

            employeeCafeServiceMock
                .Setup(s => s.GetByCafeIdAsync(cafeId))
                .ReturnsAsync(new List<EmployeeCafeDto>());

            ActionResult<CafeResponseModel> result = await cafesController.CreateCafe(model);

            CreatedAtActionResult createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            CafeResponseModel returnedCafe = Assert.IsType<CafeResponseModel>(createdResult.Value);
            
            Assert.Equal(cafeId, returnedCafe.Id);
            Assert.Equal(cafeName, returnedCafe.Name);
            Assert.Equal(description, returnedCafe.Description);
            Assert.Equal(logo, returnedCafe.Logo);
            Assert.Equal(location, returnedCafe.Location);
            Assert.Equal(0, returnedCafe.Employees);
            
            cafeServiceMock.Verify(s => s.ExistsByNameAsync(cafeName), Times.Once);
            cafeServiceMock.Verify(s => s.CreateAsync(It.IsAny<CreateCafeCommand>()), Times.Once);
        }

        [Fact]
        public async Task CreateCafe_ShouldReturnBadRequest_WhenModelStateIsInvalid_Test()
        {
            CreateCafeModel model = new CreateCafeModel
            {
                Name = "Test Cafe",
                Description = "Test Description",
                Logo = "test-logo.png",
                Location = "Test Location"
            };

            cafesController.ModelState.AddModelError("Name", "Required");

            ActionResult<CafeResponseModel> result = await cafesController.CreateCafe(model);

            Assert.IsType<BadRequestObjectResult>(result.Result);
            
            cafeServiceMock.Verify(s => s.CreateAsync(It.IsAny<CreateCafeCommand>()), Times.Never);
        }

        [Fact]
        public async Task CreateCafe_ShouldReturnBadRequest_WhenCafeNameAlreadyExists_Test()
        {
            string existingCafeName = "Existing Cafe";
            
            CreateCafeModel model = new CreateCafeModel
            {
                Name = existingCafeName,
                Description = "Test Description",
                Logo = "test-logo.png",
                Location = "Test Location"
            };

            cafeServiceMock
                .Setup(s => s.ExistsByNameAsync(existingCafeName))
                .ReturnsAsync(true);

            ActionResult<CafeResponseModel> result = await cafesController.CreateCafe(model);

            BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            string errorMessage = Assert.IsType<string>(badRequestResult.Value);
            
            Assert.Contains(existingCafeName, errorMessage);
            
            cafeServiceMock.Verify(s => s.ExistsByNameAsync(existingCafeName), Times.Once);
            cafeServiceMock.Verify(s => s.CreateAsync(It.IsAny<CreateCafeCommand>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCafe_ShouldReturnUpdatedCafe_WhenValidData_Test()
        {
            Guid cafeId = Guid.NewGuid();
            string cafeName = "Updated Cafe";
            string description = "Updated Description";
            string logo = "updated-logo.png";
            string location = "Updated Location";

            UpdateCafeModel model = new UpdateCafeModel
            {
                Id = cafeId,
                Name = cafeName,
                Description = description,
                Logo = logo,
                Location = location
            };

            CafeDto existingCafeDto = new CafeDto
            {
                Id = cafeId,
                Name = "Old Cafe",
                Description = "Old Description",
                Logo = "old-logo.png",
                Location = "Old Location"
            };

            CafeDto updatedCafeDto = new CafeDto
            {
                Id = cafeId,
                Name = cafeName,
                Description = description,
                Logo = logo,
                Location = location
            };

            UpdateCafeCommand command = new UpdateCafeCommand
            {
                Id = cafeId,
                Name = cafeName,
                Description = description,
                Logo = logo,
                Location = location
            };

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(cafeId))
                .ReturnsAsync(existingCafeDto);

            cafeServiceMock
                .Setup(s => s.UpdateAsync(It.Is<UpdateCafeCommand>(c => 
                    c.Id == cafeId && 
                    c.Name == cafeName && 
                    c.Description == description && 
                    c.Logo == logo && 
                    c.Location == location)))
                .ReturnsAsync(updatedCafeDto);

            List<EmployeeCafeDto> employeeCafeDtos = new List<EmployeeCafeDto>();
            for (int i = 0; i < 3; i++)
            {
                employeeCafeDtos.Add(new EmployeeCafeDto { Id = Guid.NewGuid(), CafeId = cafeId, EmployeeId = UniqueIdGenerator.GenerateUniqueId(), AssignedDate = DateTime.UtcNow, IsActive = true });
            }

            employeeCafeServiceMock
                .Setup(s => s.GetByCafeIdAsync(cafeId))
                .ReturnsAsync(employeeCafeDtos);

            ActionResult<CafeResponseModel> result = await cafesController.UpdateCafe(model);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            CafeResponseModel returnedCafe = Assert.IsType<CafeResponseModel>(okResult.Value);
            
            Assert.Equal(cafeId, returnedCafe.Id);
            Assert.Equal(cafeName, returnedCafe.Name);
            Assert.Equal(description, returnedCafe.Description);
            Assert.Equal(logo, returnedCafe.Logo);
            Assert.Equal(location, returnedCafe.Location);
            Assert.Equal(3, returnedCafe.Employees);
            
            cafeServiceMock.Verify(s => s.GetByIdAsync(cafeId), Times.Once);
            cafeServiceMock.Verify(s => s.UpdateAsync(It.IsAny<UpdateCafeCommand>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCafe_ShouldReturnNotFound_WhenCafeDoesNotExist_Test()
        {
            Guid cafeId = Guid.NewGuid();
            UpdateCafeModel model = new UpdateCafeModel
            {
                Id = cafeId,
                Name = "Updated Cafe",
                Description = "Updated Description",
                Logo = "updated-logo.png",
                Location = "Updated Location"
            };

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(cafeId))
                .ReturnsAsync((CafeDto?)null);

            ActionResult<CafeResponseModel> result = await cafesController.UpdateCafe(model);

            Assert.IsType<NotFoundObjectResult>(result.Result);
            
            cafeServiceMock.Verify(s => s.GetByIdAsync(cafeId), Times.Once);
            cafeServiceMock.Verify(s => s.UpdateAsync(It.IsAny<UpdateCafeCommand>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCafe_ShouldReturnNoContent_WhenSuccessful_Test()
        {
            Guid cafeId = Guid.NewGuid();
            DeleteCafeModel model = new DeleteCafeModel { Id = cafeId };

            cafeServiceMock
                .Setup(s => s.DeleteAsync(cafeId))
                .ReturnsAsync(true);

            ActionResult result = await cafesController.DeleteCafe(model);

            Assert.IsType<NoContentResult>(result);
            
            cafeServiceMock.Verify(s => s.DeleteAsync(cafeId), Times.Once);
        }

        [Fact]
        public async Task DeleteCafe_ShouldReturnNotFound_WhenCafeDoesNotExist_Test()
        {
            Guid cafeId = Guid.NewGuid();
            DeleteCafeModel model = new DeleteCafeModel { Id = cafeId };

            cafeServiceMock
                .Setup(s => s.DeleteAsync(cafeId))
                .ReturnsAsync(false);

            ActionResult result = await cafesController.DeleteCafe(model);

            Assert.IsType<NotFoundObjectResult>(result);
            
            cafeServiceMock.Verify(s => s.DeleteAsync(cafeId), Times.Once);
        }

        [Fact]
        public async Task DeleteCafe_ShouldReturnBadRequest_WhenModelStateIsInvalid_Test()
        {
            DeleteCafeModel model = new DeleteCafeModel { Id = Guid.NewGuid() };
            cafesController.ModelState.AddModelError("Id", "Required");

            ActionResult result = await cafesController.DeleteCafe(model);

            Assert.IsType<BadRequestObjectResult>(result);
            
            cafeServiceMock.Verify(s => s.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
} 