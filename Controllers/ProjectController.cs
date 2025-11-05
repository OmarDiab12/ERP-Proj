using ERP.DTOs.Projects;
using ERP.Helpers;
using ERP.Services.Interfaces.ProjectManagement;
using ERP.Services.ProjectManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly ICreateService _createService;
        private readonly IGetService _getService;

        public ProjectController(ICreateService createService,IGetService getService)
        {
            _createService = createService;
            _getService = getService;
        }

        [HttpPost("create-full")]
        public async Task<IActionResult> CreateFull([FromForm] ProjectCreateFullDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _createService.CreateFullProjectAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("create/from-quotation/{quotationId}")]
        public async Task<IActionResult> CreateFromQuotation(int quotationId)
        {
            var userId = User.GetUserId() ?? 0;
            var result = await _createService.CreateFromQuotationAsync(quotationId, userId);
            return result.IsValid ? Ok(result) : BadRequest(result);
        }

        [HttpPost("get-by-id-{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _getService.GetByIdAsync(id);
            return res.IsValid ? Ok(res) : NotFound(res);
        }

        [HttpPost("get-all")]
        public async Task<IActionResult> GetAll([FromQuery] ProjectFilterDto filter)
        {
            var res = await _getService.GetAllAsync(filter);
            return Ok(res);
        }
    }
}
