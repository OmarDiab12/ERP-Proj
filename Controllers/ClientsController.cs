using ERP.DTOs.Clients;
using ERP.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clients;

        public ClientsController(IClientService clients)
        {
            _clients = clients;
        }

        [HttpPost("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _clients.GetAllClientsAsync();
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("get-y-id-{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _clients.GetClientAsync(id);
            return res.IsValid ? Ok(res) : NotFound(res);
        }


        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateClientDTO dto)
        {
            int? userId = PublicFunctions.GetUserId(User);
            var res = await _clients.CreateClient(dto, userId ?? 0);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("edit")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Edit([FromForm] EditClientDTO dto)
        {
            int? userId = PublicFunctions.GetUserId(User);
            var res = await _clients.EditClient(dto, userId ?? 0);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }
    }
}
