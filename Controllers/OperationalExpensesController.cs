using ERP.DTOs.OperationalExpenses;
using ERP.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OperationalExpensesController : ControllerBase
    {
        private readonly IOperationalExpensesService _service;

        public OperationalExpensesController(IOperationalExpensesService service)
        {
            _service = service;
        }

        // ===== CRUD =====

        // POST: api/operationalexpenses/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateOperationalExpenseDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.CreateAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        // POST: api/operationalexpenses/edit
        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromBody] EditOperationalExpenseDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.EditAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        // POST: api/operationalexpenses/delete-5
        [HttpPost("delete-{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.DeleteAsync(id, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        // POST: api/operationalexpenses/get-5
        [HttpPost("get-{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var res = await _service.GetAsync(id);
            return res.IsValid ? Ok(res) : NotFound(res);
        }

        // POST: api/operationalexpenses/get-all-1-50
        [HttpPost("get-all-{page:int}-{pageSize:int}")]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            var res = await _service.GetAllAsync(page, pageSize);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        // ===== Date Range =====
        // POST: api/operationalexpenses/get-range-2025-09-01-2025-09-23
        [HttpPost("get-range-{fromDate}-{toDate}")]
        public async Task<IActionResult> GetByDateRange(string fromDate, string toDate, [FromQuery] string? category = null)
        {
            var req = new ExpenseRangeRequestDTO
            {
                DateFrom = fromDate,
                DateTo = toDate,
                Category = category
            };

            var res = await _service.GetByDateRangeAsync(req);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }
    }
}

