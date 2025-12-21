using ERP.DTOs.User;
using ERP.Services.Interfaces.Persons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _users;
        public UserController(IUserService users) => _users = users;


        [HttpPost("register")]
        [Authorize]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var res = await _users.RegisterAsync(dto, null);
            return res.IsValid ? Ok(res) : BadRequest(res);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var res = await _users.LoginAsync(dto, null);
            return res.IsValid ? Ok(res) : Unauthorized(res);
        }
    }
}
