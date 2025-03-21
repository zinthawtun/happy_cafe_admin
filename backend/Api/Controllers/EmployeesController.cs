using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Business.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

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
        public async Task<ActionResult<IEnumerable<EmployeeResponseModel>>> GetEmployees([FromQuery] Guid? cafe = null)
        {
            IEnumerable<Employee> employees;
            List<EmployeeResponseModel> responseList = new List<EmployeeResponseModel>();

            if (cafe.HasValue)
            {
                employees = await employeeService.GetByCafeIdAsync(cafe.Value);
                IEnumerable<EmployeeCafe> employeeCafes = await employeeCafeService.GetByCafeIdAsync(cafe.Value);

                foreach (Employee employee in employees)
                {
                    EmployeeCafe? employeeCafe = employeeCafes.FirstOrDefault(ec => ec.EmployeeId == employee.Id && ec.IsActive);
                    responseList.Add(CreateEmployeeResponse(employee, employeeCafe));
                }
            }
            else
            {
                employees = await employeeService.GetAllAsync();

                foreach (Employee employee in employees)
                {
                    EmployeeCafe? employeeCafe = await employeeCafeService.GetByEmployeeIdAsync(employee.Id);
                    responseList.Add(CreateEmployeeResponse(employee, employeeCafe));
                }
            }

            return Ok(responseList.OrderByDescending(e => e.DaysWorked));
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeResponseModel>> CreateEmployee([FromBody] CreateEmployeeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.CafeId.HasValue)
            {
                Cafe? cafe = await cafeService.GetByIdAsync(model.CafeId.Value);

                if (cafe == null)
                {
                    return BadRequest($"Cafe with ID {model.CafeId} does not exist");
                }
            }

            Employee employee = new Employee(
                id: Guid.NewGuid().ToString(),
                name: model.Name,
                emailAddress: model.EmailAddress,
                phone: model.Address,
                gender: Gender.Male
            );

            Employee? createdEmployee = await employeeService.CreateAsync(employee);

            EmployeeCafe? assignedCafe = null;

            if (model.CafeId.HasValue)
            {
                assignedCafe = await employeeCafeService.AssignEmployeeToCafeAsync(model.CafeId.Value, createdEmployee.Id, DateTime.UtcNow);
            }

            return CreatedAtAction(nameof(GetEmployees), CreateEmployeeResponse(createdEmployee, assignedCafe));
        }

        [HttpPut]
        public async Task<ActionResult<EmployeeResponseModel>> UpdateEmployee([FromBody] UpdateEmployeeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Employee? existingEmployee = await employeeService.GetByIdAsync(model.Id);

            if (existingEmployee == null)
            {
                return NotFound($"Employee with ID {model.Id} not found");
            }

            if (model.CafeId.HasValue)
            {
                Cafe? cafe = await cafeService.GetByIdAsync(model.CafeId.Value);

                if (cafe == null)
                {
                    return BadRequest($"Cafe with ID {model.CafeId} does not exist");
                }
            }

            EmployeeCafe? currentAssignment = await employeeCafeService.GetByEmployeeIdAsync(model.Id);

            existingEmployee.Update(
                name: model.Name,
                emailAddress: model.EmailAddress,
                phone: model.Address,
                gender: existingEmployee.Gender
            );

            Employee? updatedEmployee = await employeeService.UpdateAsync(existingEmployee);

            if (model.CafeId.HasValue)
            {
                if (currentAssignment == null || !currentAssignment.IsActive || currentAssignment.CafeId != model.CafeId.Value)
                {
                    if (currentAssignment != null && currentAssignment.IsActive)
                    {
                        await employeeCafeService.UnassignEmployeeFromCafeAsync(currentAssignment.Id);
                    }

                    await employeeCafeService.AssignEmployeeToCafeAsync(model.CafeId.Value, model.Id, DateTime.UtcNow);
                    currentAssignment = await employeeCafeService.GetByEmployeeIdAsync(model.Id);
                }
            }
            else if (currentAssignment != null && currentAssignment.IsActive)
            {
                await employeeCafeService.UnassignEmployeeFromCafeAsync(currentAssignment.Id);
                currentAssignment = null;
            }

            if (updatedEmployee == null)
            {
                return NotFound($"Employee with ID {model.Id} not found after update");
            }

            return Ok(CreateEmployeeResponse(updatedEmployee, currentAssignment));
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteEmployee([FromBody] DeleteEmployeeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EmployeeCafe? currentAssignment = await employeeCafeService.GetByEmployeeIdAsync(model.Id);

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
        
        private EmployeeResponseModel CreateEmployeeResponse(Employee employee, EmployeeCafe? employeeCafe)
        {
            int daysWorked = 0;
            string cafeName = string.Empty;

            if (employeeCafe != null && employeeCafe.IsActive)
            {
                daysWorked = (int)Math.Ceiling((DateTime.UtcNow - employeeCafe.AssignedDate).TotalDays);
                cafeName = employeeCafe.Cafe?.Name ?? string.Empty;
            }

            return new EmployeeResponseModel
            {
                Id = employee.Id,
                Name = employee.Name,
                EmailAddress = employee.EmailAddress,
                Address = employee.Phone,
                DaysWorked = daysWorked,
                Cafe = cafeName
            };
        }
    }
} 