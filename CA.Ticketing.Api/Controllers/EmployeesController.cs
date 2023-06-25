using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.Employees;
using CA.Ticketing.Business.Services.Employees.Dto;
using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    public class EmployeesController : BaseController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeesService)
        {
            _employeeService = employeesService;
        }

        /// <summary>
        /// Get a list of employees
        /// </summary>
        /// <param name="status">Status: Active or Inactive</param>
        /// <returns>List of employees</returns>
        [Route(ApiRoutes.Employees.List)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(EmployeeStatus status = EmployeeStatus.Active)
        {
            var employees = await _employeeService.GetAll(status);
            return Ok(employees);
        }

        /// <summary>
        /// Get employee by id
        /// </summary>
        /// <param name="employeeId">Employee Id</param>
        /// <returns>EmployeeDetailsDto</returns>
        [Route(ApiRoutes.Employees.Get)]
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeDetailsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int employeeId)
        {
            var employee = await _employeeService.GetById(employeeId);
            return Ok(employee);
        }

        /// <summary>
        /// Create an employee
        /// </summary>
        /// <param name="employee">EmployeeDetailsDto</param>
        /// <returns>Employee Id</returns>
        [Route(ApiRoutes.Employees.Create)]
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(EmployeeDetailsDto employee)
        {
            var employeeId = await _employeeService.Create(employee);
            return Ok(employeeId);
        }

        /// <summary>
        /// Update an employee
        /// </summary>
        /// <param name="employee">EmployeeDetailsDto</param>
        [Route(ApiRoutes.Employees.Update)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(EmployeeDetailsDto employee)
        {
            await _employeeService.Update(employee);
            return Ok();
        }

        /// <summary>
        /// Delete an employee
        /// </summary>
        /// <param name="employeeId">Employee Id</param>
        [Route(ApiRoutes.Employees.Delete)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int employeeId)
        {
            await _employeeService.Delete(employeeId);
            return Ok();
        }

        /// <summary>
        /// Add Employee Login
        /// </summary>
        /// <param name="addEmployeeLoginModel">AddEmployeeLoginDto</param>
        [Route(ApiRoutes.Employees.AddLogin)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddLogin(AddEmployeeLoginDto addEmployeeLoginModel)
        {
            await _employeeService.AddLogin(addEmployeeLoginModel);
            return Ok();
        }

        /// <summary>
        /// Reset Employee Password
        /// </summary>
        /// <param name="resetEmployeePasswordModel">ResetEmployeePasswordDto</param>
        [Route(ApiRoutes.Employees.ResetPassword)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ResetPassword(ResetEmployeePasswordDto resetEmployeePasswordModel)
        {
            await _employeeService.ResetPassword(resetEmployeePasswordModel);
            return Ok();
        }

        /// <summary>
        /// Delete Employee Login
        /// </summary>
        /// <param name="employeeId">Employee Id</param>
        [Route(ApiRoutes.Employees.DeleteLogin)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteLogin(int employeeId)
        {
            await _employeeService.DeleteLogin(employeeId);
            return Ok();
        }
    }
}
