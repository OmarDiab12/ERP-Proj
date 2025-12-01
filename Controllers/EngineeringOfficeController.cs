using ERP.DTOs.EngineeringOffice;
using ERP.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EngineeringOfficeController : ControllerBase
    {
        private readonly IEngineeringOfficeService _service;

        public EngineeringOfficeController(IEngineeringOfficeService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateEngineeringProjectDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.CreateAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromForm] UpdateEngineeringProjectDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.UpdateAsync(dto, userId);
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
            return Ok(res);
        }
    }
}
