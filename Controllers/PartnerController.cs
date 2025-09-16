using ERP.DTOs.Partners;
using ERP.Models;
using ERP.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerService partnerService;

        public PartnerController(IPartnerService partnerService)
        {
            this.partnerService = partnerService;
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateNewPartner([FromBody] CreatePartnerDTO dTO)
        {
            var res = await partnerService.CreateNewPartner(dTO);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("edit")]
        public async Task<ActionResult> EditPartner([FromBody] EditPartnerDTO dTO)
        {
            var res = await partnerService.EditPartner(dTO);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("get-all")]
        public async Task<ActionResult> GetAll()
        {
            var res = await partnerService.GetAllPartners();
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("get-b-id/{id}")]
        public async Task<ActionResult> GetPartner(int id)
        {
            var res = await partnerService.GetPartner(id);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }
    }
}
