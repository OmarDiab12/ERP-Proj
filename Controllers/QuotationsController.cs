using ERP.DTOs.Qiotation;
using ERP.Helpers;
using ERP.Services.Interfaces.QuotationManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuotationsController : ControllerBase
    {
        private readonly IQuotationService _service;

        public QuotationsController(IQuotationService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateQuotationDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDTO { IsValid = false, Message = "Invalid payload", Data = ModelState });

            var res = await _service.CreateAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromForm] EditQuotationDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDTO { IsValid = false, Message = "Invalid payload", Data = ModelState });

            var res = await _service.EditAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }


        [HttpPost("delete-{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.DeleteAsync(id, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("get-{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var res = await _service.GetAsync(id);
            return res.IsValid ? Ok(res) : NotFound(res);
        }

        [HttpPost("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _service.GetAllAsync();
            return res.IsValid ? Ok(res) : BadRequest(res);
        }
    }
}
