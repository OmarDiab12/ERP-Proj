using ERP.DTOs.Inventory;
using ERP.Helpers;
using ERP.Services.Interfaces.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;

        public InventoryController(IInventoryService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateInventoryItemDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.CreateAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromForm] UpdateInventoryItemDTO dto)
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

        [HttpPost("get-low-stock")]
        public async Task<IActionResult> GetLowStock()
        {
            var res = await _service.GetLowStockAsync();
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("low-stock/notify")]
        public async Task<IActionResult> NotifyLowStock()
        {
            var res = await _service.NotifyLowStockAsync();
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
