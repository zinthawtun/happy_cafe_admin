using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Service.Commands.Cafes;
using Service.Interfaces;
using Service.Queries.Cafes;
using Service.Queries.EmployeeCafes;
using Infrastructure.FileManagement;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CafesController : ControllerBase
    {
        private readonly ICafeService cafeService;
        private readonly IEmployeeCafeService employeeCafeService;
        private readonly IFileService fileService;

        public CafesController(
            ICafeService cafeService, 
            IEmployeeCafeService employeeCafeService,
            IFileService fileService)
        {
            this.cafeService = cafeService;
            this.employeeCafeService = employeeCafeService;
            this.fileService = fileService;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<CafeResponseModel>>> GetCafes([FromQuery] string? location = null)
        {
            IEnumerable<CafeDto> cafeDtos;

            if (string.IsNullOrWhiteSpace(location))
            {
                cafeDtos = await cafeService.GetAllAsync();
            }
            else
            {
                cafeDtos = await cafeService.GetByLocationAsync(location);
            }

            List<CafeResponseModel> response = new List<CafeResponseModel>();

            foreach (CafeDto cafe in cafeDtos)
            {
                IEnumerable<EmployeeCafeDto> employeeCafes = await employeeCafeService.GetByCafeIdAsync(cafe.Id);
                int employeeCount = employeeCafes.Count();

                response.Add(new CafeResponseModel
                {
                    Id = cafe.Id,
                    Name = cafe.Name,
                    Description = cafe.Description,
                    Location = cafe.Location,
                    Logo = cafe.Logo,
                    Employees = employeeCount
                });
            }

            return Ok(response.OrderByDescending(c => c.Employees));
        }

        [HttpPost]
        [Route("~/cafe")]
        public async Task<ActionResult<CafeResponseModel>> CreateCafe([FromForm] CreateCafeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CreateCafeCommand createCafeCommand = new CreateCafeCommand
            {
                Name = model.Name,
                Description = model.Description,
                Location = model.Location,
                Logo = model.Logo
            };

            CafeDto? cafeDto = await cafeService.CreateAsync(createCafeCommand);

            if (cafeDto == null)
            {
                return BadRequest("Failed to create cafe");
            }

            return CreatedAtAction(nameof(GetCafes), new CafeResponseModel
            {
                Id = cafeDto.Id,
                Name = cafeDto.Name,
                Description = cafeDto.Description,
                Location = cafeDto.Location,
                Logo = cafeDto.Logo,
                Employees = 0
            });
        }

        [HttpGet]
        [Route("cafe")]
        public async Task<ActionResult<CafeDetailResponseModel>> GetCafeById([FromQuery] Guid id)
        {
            CafeDto? cafeDto = await cafeService.GetByIdAsync(id);
            
            if (cafeDto == null)
            {
                return NotFound($"Cafe with ID {id} not found");
            }

            IEnumerable<EmployeeCafeDto> employeeCafes = await employeeCafeService.GetByCafeIdAsync(id);
            int employeeCount = employeeCafes.Count();

            CafeDetailResponseModel response = new CafeDetailResponseModel
            {
                Id = cafeDto.Id,
                Name = cafeDto.Name,
                Description = cafeDto.Description,
                Location = cafeDto.Location,
                Logo = cafeDto.Logo,
                Employees = employeeCount
            };

            return Ok(response);
        }

        [HttpPut]
        [Route("~/cafe")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<CafeResponseModel>> UpdateCafe([FromForm] UpdateCafeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CafeDto? existingCafeDto = await cafeService.GetByIdAsync(model.Id);
            if (existingCafeDto == null)
            {
                return NotFound($"Cafe with ID {model.Id} not found");
            }

            UpdateCafeCommand updateCafeCommand = new UpdateCafeCommand
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Location = model.Location,
                Logo = model.Logo
            };

            CafeDto? updatedCafeDto = await cafeService.UpdateAsync(updateCafeCommand);
            if (updatedCafeDto == null)
            {
                return NotFound($"Cafe with ID {model.Id} not found");
            }

            IEnumerable<EmployeeCafeDto> employeeCafes = await employeeCafeService.GetByCafeIdAsync(updatedCafeDto.Id);
            int employeeCount = employeeCafes.Count();

            return Ok(new CafeResponseModel
            {
                Id = updatedCafeDto.Id,
                Name = updatedCafeDto.Name,
                Description = updatedCafeDto.Description,
                Location = updatedCafeDto.Location,
                Logo = updatedCafeDto.Logo,
                Employees = employeeCount
            });
        }

        [HttpDelete]
        [Route("~/cafe")]
        public async Task<ActionResult> DeleteCafe([FromBody] DeleteCafeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CafeDto? cafeDto = await cafeService.GetByIdAsync(model.Id);
            
            if (cafeDto == null)
            {
                return NotFound($"Cafe with ID {model.Id} not found");
            }

            string? logoFileName = cafeDto.Logo;

            bool result = await cafeService.DeleteAsync(model.Id);

            if (!result)
            {
                return StatusCode(500, "An error occurred while attempting to delete the cafe");
            }

            if (!string.IsNullOrWhiteSpace(logoFileName))
            {
                fileService.DeleteLogo(logoFileName);
            }

            return NoContent();
        }
    }
} 