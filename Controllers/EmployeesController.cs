using ERP.DTOs.Employee;
using ERP.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeesController(IEmployeeService service)
        {
            _service = service;
        }

        // ===== Employees =====

        // POST: api/employees/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.CreateEmployeeAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        // POST: api/employees/edit
        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromBody] EditEmployeeDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.EditEmployeeAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        // POST: api/employees/delete
        [HttpPost("delete-{employeeId:int}")]
        public async Task<IActionResult> Delete(int employeeId)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.DeleteEmployeeAsync(employeeId, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        // POST: api/employees/get
        [HttpPost("get-{employeeId:int}")]
        public async Task<IActionResult> Get(int employeeId)
        {
            var res = await _service.GetEmployeeAsync(employeeId);
            return res.IsValid ? Ok(res) : NotFound(res);
        }

        // POST: api/employees/get-all
        [HttpPost("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _service.GetAllEmployeesAsync();
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        // ===== Transactions =====

        // POST: api/employees/transactions/add
        [HttpPost("transactions/add")]
        public async Task<IActionResult> AddTransaction([FromBody] CreateEmployeeTransactionDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.AddTransactionAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        // POST: api/employees/transactions/edit
        [HttpPost("transactions/edit")]
        public async Task<IActionResult> EditTransaction([FromBody] EditEmployeeTransactionDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.EditTransactionAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        // POST: api/employees/transactions/delete
        [HttpPost("transactions/delete-{transactionId:int}")]
        public async Task<IActionResult> DeleteTransaction(int transactionId)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.DeleteTransactionAsync(transactionId, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }


        [HttpPost("transactions/get-{employeeId:int}-{page:int}-{pagesize:int}")]
        public async Task<IActionResult> GetTransactions(int employeeId,int page ,int pagesize)
        {
            var res = await _service.GetEmployeeTransactionsAsync(employeeId, page, pagesize);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }
    }
}
    

