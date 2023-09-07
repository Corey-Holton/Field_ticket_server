using CA.Ticketing.Api.Extensions;
using CA.Ticketing.Business.Services.Payroll;
using CA.Ticketing.Business.Services.Payroll.Dto;
using CA.Ticketing.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CA.Ticketing.Api.Controllers
{
    public class PayrollsController : BaseController
    {
        private readonly IPayrollService _payrollsService;

        public PayrollsController(IPayrollService payrollsService)
        {
            _payrollsService = payrollsService;
        }

        /// <summary>
        /// Get payroll Data
        /// </summary>
        /// <returns>List of Employee PayrollData</returns>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        [Route(ApiRoutes.Payrolls.GetPayrolls)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeePayrollDataDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPayrolls(DateTime startDate, DateTime endDate)
        {
            var data = await _payrollsService.GetPayrollData(startDate, endDate);
            return Ok(data);
        }
    }
}
