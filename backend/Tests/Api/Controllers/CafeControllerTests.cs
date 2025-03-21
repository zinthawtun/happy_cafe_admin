using Api.Controllers;
using Api.Models;
using Business.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Interfaces;
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
            IEnumerable<Cafe> cafes = new List<Cafe>
            {
                new Cafe(Guid.NewGuid(), "Cafe 1", "Description 1", "Location 1", "logo1.png"),
                new Cafe(Guid.NewGuid(), "Cafe 2", "Description 2", "Location 2", "logo2.png")
            };

            cafeServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(cafes);

            foreach (Cafe cafe in cafes)
            {
                List<EmployeeCafe> employeeCafes = new List<EmployeeCafe>
                {
                    new EmployeeCafe(Guid.NewGuid(), cafe.Id, UniqueIdGenerator.GenerateUniqueId(), DateTime.UtcNow),
                    new EmployeeCafe(Guid.NewGuid(), cafe.Id, UniqueIdGenerator.GenerateUniqueId(), DateTime.UtcNow),
                    new EmployeeCafe(Guid.NewGuid(), cafe.Id, UniqueIdGenerator.GenerateUniqueId(), DateTime.UtcNow),
                    new EmployeeCafe(Guid.NewGuid(), cafe.Id, UniqueIdGenerator.GenerateUniqueId(), DateTime.UtcNow),
                    new EmployeeCafe(Guid.NewGuid(), cafe.Id, UniqueIdGenerator.GenerateUniqueId(), DateTime.UtcNow)
                };

                employeeCafeServiceMock
                    .Setup(s => s.GetByCafeIdAsync(cafe.Id))
                    .ReturnsAsync(employeeCafes);
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
            IEnumerable<Cafe> cafes = new List<Cafe>
            {
                new Cafe(Guid.NewGuid(), "Cafe 1", "Description 1", "logo1.png", location),
                new Cafe(Guid.NewGuid(), "Cafe 2", "Description 2", "logo2.png", location)
            };

            cafeServiceMock
                .Setup(s => s.GetByLocationAsync(location))
                .ReturnsAsync(cafes);

            foreach (Cafe cafe in cafes)
            {
                List<EmployeeCafe> employeeCafes = new List<EmployeeCafe>
                {
                    new EmployeeCafe(Guid.NewGuid(), cafe.Id, UniqueIdGenerator.GenerateUniqueId(), DateTime.UtcNow),
                    new EmployeeCafe(Guid.NewGuid(), cafe.Id, UniqueIdGenerator.GenerateUniqueId(), DateTime.UtcNow),
                    new EmployeeCafe(Guid.NewGuid(), cafe.Id, UniqueIdGenerator.GenerateUniqueId(), DateTime.UtcNow),
                    new EmployeeCafe(Guid.NewGuid(), cafe.Id, UniqueIdGenerator.GenerateUniqueId(), DateTime.UtcNow),
                    new EmployeeCafe(Guid.NewGuid(), cafe.Id, UniqueIdGenerator.GenerateUniqueId(), DateTime.UtcNow)
                };

                employeeCafeServiceMock
                    .Setup(s => s.GetByCafeIdAsync(cafe.Id))
                    .ReturnsAsync(employeeCafes);
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
            string location = "Unknown Location";
            
            cafeServiceMock
                .Setup(s => s.GetByLocationAsync(location))
                .ReturnsAsync(new List<Cafe>());

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
            
            List<Cafe> cafes = new List<Cafe>
            {
                new Cafe(cafe1Id, "Cafe 1", "Description 1", "Location 1", "logo1.png"),
                new Cafe(cafe2Id, "Cafe 2", "Description 2", "Location 2", "logo2.png"),
                new Cafe(cafe3Id, "Cafe 3", "Description 3", "Location 3", "logo3.png")
            };

            cafeServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(cafes);

            List<EmployeeCafe> cafe1Employees = new List<EmployeeCafe>();
            for (int i = 0; i < 5; i++)
            {
                cafe1Employees.Add(new EmployeeCafe(Guid.NewGuid(), cafe1Id, UniqueIdGenerator.GenerateUniqueId(), DateTime.UtcNow));
            }
            employeeCafeServiceMock
                .Setup(s => s.GetByCafeIdAsync(cafe1Id))
                .ReturnsAsync(cafe1Employees);
                
            List<EmployeeCafe> cafe2Employees = new List<EmployeeCafe>();
            for (int i = 0; i < 10; i++)
            {
                cafe2Employees.Add(new EmployeeCafe(Guid.NewGuid(), cafe2Id, UniqueIdGenerator.GenerateUniqueId(), DateTime.UtcNow));
            }
            employeeCafeServiceMock
                .Setup(s => s.GetByCafeIdAsync(cafe2Id))
                .ReturnsAsync(cafe2Employees);
                
            List<EmployeeCafe> cafe3Employees = new List<EmployeeCafe>();
            for (int i = 0; i < 2; i++)
            {
                cafe3Employees.Add(new EmployeeCafe(Guid.NewGuid(), cafe3Id, UniqueIdGenerator.GenerateUniqueId(), DateTime.UtcNow));
            }
            employeeCafeServiceMock
                .Setup(s => s.GetByCafeIdAsync(cafe3Id))
                .ReturnsAsync(cafe3Employees);

            ActionResult<IEnumerable<CafeResponseModel>> result = await cafesController.GetCafes();

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            List<CafeResponseModel> returnedCafes = Assert.IsAssignableFrom<IEnumerable<CafeResponseModel>>(okResult.Value).ToList();
            
            Assert.Equal(3, returnedCafes.Count);
            Assert.Equal(10, returnedCafes[0].Employees);
            Assert.Equal(5, returnedCafes[1].Employees);
            Assert.Equal(2, returnedCafes[2].Employees);
        }

        [Fact]
        public async Task CreateCafe_ShouldReturnCreatedCafe_WhenValidData_Test()
        {
            Guid cafeId = Guid.NewGuid();
            CreateCafeModel createModel = new CreateCafeModel
            {
                Name = "New Cafe",
                Description = "New Description",
                Location = "New Location",
                Logo = "new-logo.png"
            };

            Cafe createdCafe = new Cafe(
                cafeId,
                createModel.Name,
                createModel.Description,
                createModel.Logo,
                createModel.Location              
            );

            cafeServiceMock
                .Setup(s => s.CreateAsync(It.IsAny<Cafe>()))
                .ReturnsAsync(createdCafe);

            employeeCafeServiceMock
                .Setup(s => s.GetByCafeIdAsync(cafeId))
                .ReturnsAsync(new List<EmployeeCafe>());

            ActionResult<CafeResponseModel> result = await cafesController.CreateCafe(createModel);

            CreatedAtActionResult createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            CafeResponseModel returnedCafe = Assert.IsType<CafeResponseModel>(createdAtResult.Value);
            
            Assert.Equal(cafeId, returnedCafe.Id);
            Assert.Equal(createModel.Name, returnedCafe.Name);
            Assert.Equal(createModel.Description, returnedCafe.Description);
            Assert.Equal(createModel.Location, returnedCafe.Location);
            Assert.Equal(createModel.Logo, returnedCafe.Logo);
            Assert.Equal(0, returnedCafe.Employees);
            
            cafeServiceMock.Verify(s => s.CreateAsync(It.IsAny<Cafe>()), Times.Once);
        }

        [Fact]
        public async Task CreateCafe_ShouldReturnBadRequest_WhenModelStateIsInvalid_Test()
        {
            CreateCafeModel createModel = new CreateCafeModel
            {
                Description = "New Description",
                Location = "New Location"
            };

            cafesController.ModelState.AddModelError("Name", "Name is required");

            ActionResult<CafeResponseModel> result = await cafesController.CreateCafe(createModel);

            Assert.IsType<BadRequestObjectResult>(result.Result);
            
            cafeServiceMock.Verify(s => s.CreateAsync(It.IsAny<Cafe>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCafe_ShouldReturnUpdatedCafe_WhenValidData_Test()
        {
            Guid cafeId = Guid.NewGuid();
            UpdateCafeModel updateModel = new UpdateCafeModel
            {
                Id = cafeId,
                Name = "Updated Cafe",
                Description = "Updated Description",
                Location = "Updated Location",
                Logo = "updated-logo.png"
            };

            Cafe existingCafe = new Cafe(
                cafeId,
                "Original Cafe",
                "Original Description",
                "Original Location",
                "original-logo.png"
            );

            Cafe updatedCafe = new Cafe(
                cafeId,
                updateModel.Name,
                updateModel.Description,
                updateModel.Logo,
                updateModel.Location               
            );

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(cafeId))
                .ReturnsAsync(existingCafe);

            cafeServiceMock
                .Setup(s => s.UpdateAsync(It.IsAny<Cafe>()))
                .ReturnsAsync(updatedCafe);

            List<EmployeeCafe> cafeEmployees = new List<EmployeeCafe>();
            for (int i = 0; i < 3; i++)
            {
                cafeEmployees.Add(new EmployeeCafe(Guid.NewGuid(), cafeId, UniqueIdGenerator.GenerateUniqueId(), DateTime.UtcNow));
            }
            employeeCafeServiceMock
                .Setup(s => s.GetByCafeIdAsync(cafeId))
                .ReturnsAsync(cafeEmployees);

            ActionResult<CafeResponseModel> result = await cafesController.UpdateCafe(updateModel);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            CafeResponseModel returnedCafe = Assert.IsType<CafeResponseModel>(okResult.Value);
            
            Assert.Equal(cafeId, returnedCafe.Id);
            Assert.Equal(updateModel.Name, returnedCafe.Name);
            Assert.Equal(updateModel.Description, returnedCafe.Description);
            Assert.Equal(updateModel.Location, returnedCafe.Location);
            Assert.Equal(updateModel.Logo, returnedCafe.Logo);
            Assert.Equal(3, returnedCafe.Employees);
            
            cafeServiceMock.Verify(s => s.GetByIdAsync(cafeId), Times.Once);
            cafeServiceMock.Verify(s => s.UpdateAsync(It.IsAny<Cafe>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCafe_ShouldReturnNotFound_WhenCafeDoesNotExist_Test()
        {
            Guid cafeId = Guid.NewGuid();
            UpdateCafeModel updateModel = new UpdateCafeModel
            {
                Id = cafeId,
                Name = "Updated Cafe",
                Description = "Updated Description",
                Location = "Updated Location",
                Logo = "updated-logo.png"
            };

            cafeServiceMock
                .Setup(s => s.GetByIdAsync(cafeId))
                .ReturnsAsync((Cafe?)null);

            ActionResult<CafeResponseModel> result = await cafesController.UpdateCafe(updateModel);

            Assert.IsType<NotFoundObjectResult>(result.Result);
            
            cafeServiceMock.Verify(s => s.GetByIdAsync(cafeId), Times.Once);
            cafeServiceMock.Verify(s => s.UpdateAsync(It.IsAny<Cafe>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCafe_ShouldReturnNoContent_WhenSuccessful_Test()
        {
            Guid cafeId = Guid.NewGuid();
            DeleteCafeModel deleteModel = new DeleteCafeModel
            {
                Id = cafeId
            };

            cafeServiceMock
                .Setup(s => s.DeleteAsync(cafeId))
                .ReturnsAsync(true);

            ActionResult result = await cafesController.DeleteCafe(deleteModel);

            Assert.IsType<NoContentResult>(result);
            
            cafeServiceMock.Verify(s => s.DeleteAsync(cafeId), Times.Once);
        }

        [Fact]
        public async Task DeleteCafe_ShouldReturnNotFound_WhenCafeDoesNotExist_Test()
        {
            Guid cafeId = Guid.NewGuid();
            DeleteCafeModel deleteModel = new DeleteCafeModel
            {
                Id = cafeId
            };

            cafeServiceMock
                .Setup(s => s.DeleteAsync(cafeId))
                .ReturnsAsync(false);

            ActionResult result = await cafesController.DeleteCafe(deleteModel);

            Assert.IsType<NotFoundObjectResult>(result);
            
            cafeServiceMock.Verify(s => s.DeleteAsync(cafeId), Times.Once);
        }

        [Fact]
        public async Task DeleteCafe_ShouldReturnBadRequest_WhenModelStateIsInvalid_Test()
        {
            DeleteCafeModel deleteModel = new DeleteCafeModel
            {
                Id = Guid.Empty
            };

            cafesController.ModelState.AddModelError("Id", "Id is required");

            ActionResult result = await cafesController.DeleteCafe(deleteModel);

            Assert.IsType<BadRequestObjectResult>(result);
            
            cafeServiceMock.Verify(s => s.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
} 