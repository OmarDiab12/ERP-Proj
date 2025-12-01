using ERP.DTOs.Invoices;
using ERP.Helpers;
using ERP.Services.Interfaces.Invoices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _service;

        public InvoicesController(IInvoiceService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateInvoiceDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _service.CreateAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromForm] UpdateInvoiceDTO dto)
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

        [HttpPost("filter")]
        public async Task<IActionResult> Filter([FromBody] InvoiceFilterDTO dto)
        {
            var res = await _service.FilterAsync(dto);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("due-reminders")]
        public async Task<IActionResult> DueReminders([FromBody] InvoiceReminderFilterDTO dto)
        {
            var res = await _service.GetDueRemindersAsync(dto);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("due-reminders/notify")]
        public async Task<IActionResult> BroadcastDueReminders([FromBody] InvoiceReminderFilterDTO dto)
        {
            var res = await _service.NotifyDueRemindersAsync(dto);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("export")]
        public async Task<IActionResult> Export([FromBody] InvoiceExportRequestDTO dto)
        {
            var res = await _service.ExportAsync(dto);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }
    }
}
