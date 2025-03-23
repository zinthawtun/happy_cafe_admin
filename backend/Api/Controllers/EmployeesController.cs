using Api.Models;
using Business.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Commands.EmployeeCafes;
using Service.Commands.Employees;
using Service.Interfaces;
using Service.Queries.Cafes;
using Service.Queries.EmployeeCafes;
using Service.Queries.Employees;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService employeeService;
        private readonly ICafeService cafeService;
        private readonly IEmployeeCafeService employeeCafeService;

        public EmployeesController(IEmployeeService employeeService, ICafeService cafeService, IEmployeeCafeService employeeCafeService)
        {
            this.employeeService = employeeService;
            this.cafeService = cafeService;
            this.employeeCafeService = employeeCafeService;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<EmployeeResponseModel>>> GetEmployees([FromQuery] Guid? cafe = null)
        {
            IEnumerable<EmployeeDto> employeeDtos;

            if (cafe.HasValue)
            {
                employeeDtos = await employeeService.GetByCafeIdAsync(cafe.Value);
            }
            else
            {
                employeeDtos = await employeeService.GetAllAsync();
            }

            List<EmployeeDto> employeeDtosList = employeeDtos.ToList();

            foreach (EmployeeDto employeeDto in employeeDtosList)
            {
                if (string.IsNullOrEmpty(employeeDto.CafeName) || !employeeDto.CafeId.HasValue)
                {
                    EmployeeCafeDto? employeeCafeDto = await employeeCafeService.GetByEmployeeIdAsync(employeeDto.Id);
                    
                    if (employeeCafeDto != null)
                    {
                        employeeDto.CafeId = employeeCafeDto.CafeId;
                        employeeDto.CafeName = employeeCafeDto.CafeName;
                        employeeDto.AssignedDate = employeeCafeDto.AssignedDate;
                        employeeDto.DaysWorked = (int)Math.Ceiling((DateTime.UtcNow - employeeCafeDto.AssignedDate).TotalDays);
                        employeeDto.IsAssignedToCafe = true;
                    }
                }
            }

            List<EmployeeResponseModel> responseList = employeeDtosList.Select(dto => new EmployeeResponseModel
            {
                Id = dto.Id,
                Name = dto.Name,
                EmailAddress = dto.EmailAddress,
                Phone = dto.Phone,
                DaysWorked = dto.DaysWorked,
                Cafe = dto.CafeName
            }).ToList();

            return Ok(responseList.OrderByDescending(e => e.DaysWorked));
        }

        [HttpPost]
        [Route("~/employee")]
        public async Task<ActionResult<EmployeeResponseModel>> CreateEmployee([FromBody] CreateEmployeeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool employeeExists = await employeeService.ExistsWithEmailOrPhoneAsync(model.EmailAddress, model.Phone);
            
            if (employeeExists)
            {
                return BadRequest("An employee with the same email address or phone number already exists.");
            }

            if (model.CafeId.HasValue)
            {
                CafeDto? cafe = await cafeService.GetByIdAsync(model.CafeId.Value);
                if (cafe == null)
                {
                    return BadRequest($"Cafe with ID {model.CafeId} does not exist");
                }
            }

            CreateEmployeeCommand command = new CreateEmployeeCommand
            {
                Name = model.Name,
                EmailAddress = model.EmailAddress,
                Phone = model.Phone,
                Gender = model.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase) ? Gender.Female : Gender.Male
            };

            EmployeeDto? employeeDto = await employeeService.CreateAsync(command);

            if (employeeDto == null)
            {
                return BadRequest("Failed to create employee");
            }

            if (model.CafeId.HasValue)
            {
                AssignEmployeeToCafeCommand assignCommand = new AssignEmployeeToCafeCommand
                {
                    EmployeeId = employeeDto.Id,
                    CafeId = model.CafeId.Value,
                    AssignedDate = DateTime.UtcNow
                };

                EmployeeCafeDto? employeeCafeDto = await employeeCafeService.AssignEmployeeToCafeAsync(assignCommand);

                if (employeeCafeDto != null)
                {
                    employeeDto.CafeId = employeeCafeDto.CafeId;
                    employeeDto.CafeName = employeeCafeDto.CafeName;
                    employeeDto.AssignedDate = employeeCafeDto.AssignedDate;
                    employeeDto.DaysWorked = (int)Math.Ceiling((DateTime.UtcNow - employeeCafeDto.AssignedDate).TotalDays);
                    employeeDto.IsAssignedToCafe = true;
                }
            }

            EmployeeResponseModel response = new EmployeeResponseModel
            {
                Id = employeeDto.Id,
                Name = employeeDto.Name,
                EmailAddress = employeeDto.EmailAddress,
                Phone = employeeDto.Phone,
                DaysWorked = employeeDto.DaysWorked,
                Cafe = employeeDto.CafeName
            };

            return CreatedAtAction(nameof(GetEmployees), response);
        }

        [HttpGet]
        [Route("employee")]
        public async Task<ActionResult<EmployeeDetailResponseModel>> GetEmployeeById([FromQuery] string id)
        {
            EmployeeDto? employeeDto = await employeeService.GetByIdAsync(id);
            
            if (employeeDto == null)
            {
                return NotFound($"Employee with ID {id} not found");
            }

            EmployeeCafeDto? employeeCafeDto = await employeeCafeService.GetByEmployeeIdAsync(id);
            
            if (employeeCafeDto != null)
            {
                employeeDto.CafeId = employeeCafeDto.CafeId;
                employeeDto.CafeName = employeeCafeDto.CafeName;
                employeeDto.AssignedDate = employeeCafeDto.AssignedDate;
                employeeDto.DaysWorked = (int)Math.Ceiling((DateTime.UtcNow - employeeCafeDto.AssignedDate).TotalDays);
                employeeDto.IsAssignedToCafe = true;
            }

            EmployeeDetailResponseModel response = new EmployeeDetailResponseModel
            {
                Id = employeeDto.Id,
                Name = employeeDto.Name,
                EmailAddress = employeeDto.EmailAddress,
                Phone = employeeDto.Phone,
                Gender = employeeDto.Gender.ToString(),
                CafeId = employeeDto.CafeId,
                CafeName = employeeDto.CafeName,
                StartDate = employeeDto.AssignedDate
            };

            return Ok(response);
        }

        [HttpPut]
        [Route("~/employee")]
        public async Task<ActionResult<EmployeeResponseModel>> UpdateEmployee([FromBody] UpdateEmployeeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EmployeeDto? existingEmployee = await employeeService.GetByIdAsync(model.Id);
            if (existingEmployee == null)
            {
                return NotFound($"Employee with ID {model.Id} not found");
            }

            if (model.CafeId.HasValue)
            {
                CafeDto? cafe = await cafeService.GetByIdAsync(model.CafeId.Value);
                if (cafe == null)
                {
                    return BadRequest($"Cafe with ID {model.CafeId} does not exist");
                }
            }

            UpdateEmployeeCommand command = new UpdateEmployeeCommand
            {
                Id = model.Id,
                Name = model.Name,
                EmailAddress = model.EmailAddress,
                Phone = model.Phone,
                Gender = model.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase) ? Gender.Female : Gender.Male
            };

            EmployeeDto? updatedEmployeeDto = await employeeService.UpdateAsync(command);

            if (updatedEmployeeDto == null)
            {
                return NotFound($"Employee with ID {model.Id} not found after update");
            }

            EmployeeCafeDto? currentAssignment = await employeeCafeService.GetByEmployeeIdAsync(model.Id);

            if (model.CafeId.HasValue)
            {
                if (currentAssignment == null || !currentAssignment.IsActive || currentAssignment.CafeId != model.CafeId.Value)
                {
                    if (currentAssignment != null && currentAssignment.IsActive)
                    {
                        await employeeCafeService.UnassignEmployeeFromCafeAsync(currentAssignment.Id);
                    }

                    AssignEmployeeToCafeCommand assignCommand = new AssignEmployeeToCafeCommand
                    {
                        EmployeeId = model.Id,
                        CafeId = model.CafeId.Value,
                        AssignedDate = DateTime.UtcNow
                    };

                    currentAssignment = await employeeCafeService.AssignEmployeeToCafeAsync(assignCommand);
                }
            }
            else if (currentAssignment != null && currentAssignment.IsActive)
            {
                await employeeCafeService.UnassignEmployeeFromCafeAsync(currentAssignment.Id);
                currentAssignment = null;
            }

            if (currentAssignment != null)
            {
                updatedEmployeeDto.CafeId = currentAssignment.CafeId;
                updatedEmployeeDto.CafeName = currentAssignment.CafeName;
                updatedEmployeeDto.AssignedDate = currentAssignment.AssignedDate;
                updatedEmployeeDto.DaysWorked = (int)Math.Ceiling((DateTime.UtcNow - currentAssignment.AssignedDate).TotalDays);
                updatedEmployeeDto.IsAssignedToCafe = currentAssignment.IsActive;
            }
            else
            {
                updatedEmployeeDto.CafeId = null;
                updatedEmployeeDto.CafeName = string.Empty;
                updatedEmployeeDto.AssignedDate = null;
                updatedEmployeeDto.DaysWorked = 0;
                updatedEmployeeDto.IsAssignedToCafe = false;
            }

            EmployeeResponseModel response = new EmployeeResponseModel
            {
                Id = updatedEmployeeDto.Id,
                Name = updatedEmployeeDto.Name,
                EmailAddress = updatedEmployeeDto.EmailAddress,
                Phone = updatedEmployeeDto.Phone,
                DaysWorked = updatedEmployeeDto.DaysWorked,
                Cafe = updatedEmployeeDto.CafeName
            };

            return Ok(response);
        }

        [HttpDelete]
        [Route("~/employee")]
        public async Task<ActionResult> DeleteEmployee([FromBody] DeleteEmployeeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EmployeeCafeDto? currentAssignment = await employeeCafeService.GetByEmployeeIdAsync(model.Id);

            if (currentAssignment != null && currentAssignment.IsActive)
            {
                await employeeCafeService.UnassignEmployeeFromCafeAsync(currentAssignment.Id);
            }

            bool result = await employeeService.DeleteAsync(model.Id);

            if (!result)
            {
                return NotFound($"Employee with ID {model.Id} not found");
            }

            return NoContent();
        }
    }
} 