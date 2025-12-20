using ERP.DTOs.PrivatePartnerships;
using ERP.Helpers;
using ERP.Services.Interfaces.PrivatePartnerships;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrivatePartnershipsController : ControllerBase
    {
        private readonly IPrivatePartnershipService _privatePartnershipService;

        public PrivatePartnershipsController(IPrivatePartnershipService privatePartnershipService)
        {
            _privatePartnershipService = privatePartnershipService;
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateProject([FromBody] CreatePrivatePartnershipProjectDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _privatePartnershipService.CreateProjectAsync(dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("{projectId:int}/transactions")]
        public async Task<ActionResult> AddTransaction(int projectId, [FromBody] CreatePrivatePartnershipTransactionDTO dto)
        {
            var userId = User.GetUserId() ?? 0;
            var res = await _privatePartnershipService.AddTransactionAsync(projectId, dto, userId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpGet]
        public async Task<ActionResult> GetProjects()
        {
            var res = await _privatePartnershipService.GetProjectsAsync();
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpGet("{projectId:int}")]
        public async Task<ActionResult> GetProject(int projectId)
        {
            var res = await _privatePartnershipService.GetProjectSummaryAsync(projectId);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }
    }
}
