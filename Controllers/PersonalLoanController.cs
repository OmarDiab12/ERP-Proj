using ERP.DTOs.PersonalLoans;
using ERP.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PersonalLoanController : ControllerBase
    {
        private readonly IPersonalLoanService _service;

        public PersonalLoanController(IPersonalLoanService service)
        {
            _service = service;
        }

        // POST: api/personalloans/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreatePersonalLoanDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.CreateAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        // POST: api/personalloans/edit
        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromBody] EditPersonalLoanDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.EditAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        // POST: api/personalloans/delete-5
        [HttpPost("delete-{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.DeleteAsync(id, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        // POST: api/personalloans/get-5
        [HttpPost("get-{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var res = await _service.GetAsync(id);
            return res.IsValid ? Ok(res) : NotFound(res);
        }

        // POST: api/personalloans/get-all-1-50
        [HttpPost("get-all-{page:int}-{pageSize:int}")]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            var res = await _service.GetAllAsync(page, pageSize);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        // POST: api/personalloans/get-overdue-2025-09-30
        [HttpPost("get-overdue-{asOfDate}")]
        public async Task<IActionResult> GetOverdue(string asOfDate)
        {
            var res = await _service.GetOverdueAsync(asOfDate);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }
    }
}
