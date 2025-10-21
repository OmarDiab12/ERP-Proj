using ERP.DTOs.Brokers;
using ERP.DTOs.Contractors;
using ERP.Helpers;
using ERP.Services;
using ERP.Services.Interfaces.Persons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContractorController : ControllerBase
    {
        private readonly IContractorService _contractorService;

        public ContractorController(IContractorService contractorService)
        {
            _contractorService = contractorService;
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateNewContractor([FromBody] CreateContratorDTO dTO)
        {
            int? userId = PublicFunctions.GetUserId(User);
            var res = await _contractorService.CreateContractor(dTO, userId ?? 0);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("edit")]
        public async Task<ActionResult> EditContractor([FromBody] EditContractorDTO dTO)
        {
            int? userId = PublicFunctions.GetUserId(User);
            var res = await _contractorService.EditContractor(dTO, userId ?? 0);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("get-all")]
        public async Task<ActionResult> GetAll()
        {
            var res = await _contractorService.GetAllContractorsAsync();
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("get-b-id/{id}")]
        public async Task<ActionResult> GetContractor(int id)
        {
            var res = await _contractorService.GetContractorAsync(id);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }
    }
}
