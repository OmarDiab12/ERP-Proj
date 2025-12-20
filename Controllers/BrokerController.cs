using ERP.DTOs.Brokers;
using ERP.DTOs.Partners;
using ERP.Helpers;
using ERP.Services.Interfaces.Persons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BrokerController : ControllerBase
    {
        private readonly IBrokerService brokerService;

        public BrokerController(IBrokerService brokerService)
        {
            this.brokerService = brokerService;
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateNewBroker([FromBody] CreateBrokerDTO dTO)
        {
            int? userId = PublicFunctions.GetUserId(User);
            var res = await brokerService.CreateBroker(dTO, userId ?? 0);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("edit")]
        public async Task<ActionResult> EditPartner([FromBody] EditBrokerDTO dTO)
        {
            int? userId = PublicFunctions.GetUserId(User);
            var res = await brokerService.EditBroker(dTO, userId ?? 0);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("get-all")]
        public async Task<ActionResult> GetAll()
        {
            var res = await brokerService.GetAllBrokersAsync();
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("get-b-id/{id}")]
        public async Task<ActionResult> GetPartner(int id)
        {
            var res = await brokerService.GetBrokerAsync(id);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }
    }
}
