using Api.Models;
using Business.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CafesController : ControllerBase
    {
        private readonly ICafeService cafeService;
        private readonly IEmployeeCafeService employeeCafeService;

        public CafesController(ICafeService cafeService, IEmployeeCafeService employeeCafeService)
        {
            this.cafeService = cafeService;
            this.employeeCafeService = employeeCafeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CafeResponseModel>>> GetCafes([FromQuery] string? location = null)
        {
            IEnumerable<Cafe> cafes;

            if (string.IsNullOrWhiteSpace(location))
            {
                cafes = await cafeService.GetAllAsync();
            }
            else
            {
                cafes = await cafeService.GetByLocationAsync(location);
            }

            List<CafeResponseModel> response = new List<CafeResponseModel>();

            foreach (Cafe cafe in cafes)
            {
                IEnumerable<EmployeeCafe> employeeCafes = await employeeCafeService.GetByCafeIdAsync(cafe.Id);

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
        public async Task<ActionResult<CafeResponseModel>> CreateCafe([FromBody] CreateCafeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Cafe cafe = new Cafe(Guid.NewGuid(), model.Name, model.Description, model.Location, model.Logo);

            Cafe createdCafe = await cafeService.CreateAsync(cafe);

            return CreatedAtAction(nameof(GetCafes), new CafeResponseModel
            {
                Id = createdCafe.Id,
                Name = createdCafe.Name,
                Description = createdCafe.Description,
                Location = createdCafe.Location,
                Logo = createdCafe.Logo,
                Employees = 0
            });
        }

        [HttpPut]
        public async Task<ActionResult<CafeResponseModel>> UpdateCafe([FromBody] UpdateCafeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Cafe? existingCafe = await cafeService.GetByIdAsync(model.Id);
            if (existingCafe == null)
            {
                return NotFound($"Cafe with ID {model.Id} not found");
            }

            existingCafe.Update(model.Name, model.Description, model.Logo, model.Location);

            Cafe? updatedCafe = await cafeService.UpdateAsync(existingCafe);
            if (updatedCafe == null)
            {
                return NotFound($"Cafe with ID {model.Id} not found");
            }

            IEnumerable<EmployeeCafe> employeeCafes = await employeeCafeService.GetByCafeIdAsync(updatedCafe.Id);

            int employeeCount = employeeCafes.Count();

            return Ok(new CafeResponseModel
            {
                Id = updatedCafe.Id,
                Name = updatedCafe.Name,
                Description = updatedCafe.Description,
                Location = updatedCafe.Location,
                Logo = updatedCafe.Logo,
                Employees = employeeCount
            });
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteCafe([FromBody] DeleteCafeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool result = await cafeService.DeleteAsync(model.Id);

            if (!result)
            {
                return NotFound($"Cafe with ID {model.Id} not found");
            }

            return NoContent();
        }
    }
} 