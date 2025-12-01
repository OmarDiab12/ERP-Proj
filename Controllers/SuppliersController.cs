using ERP.DTOs.Suppliers;
using ERP.Helpers;
using ERP.Services.Interfaces.Suppliers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SuppliersController(ISupplierService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateSupplierDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.CreateAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromBody] UpdateSupplierDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.UpdateAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _service.GetAllAsync();
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("get-{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var res = await _service.GetAsync(id);
            return res.IsValid ? Ok(res) : NotFound(res);
        }
    }
}
