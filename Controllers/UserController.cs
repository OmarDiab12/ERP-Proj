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
<<<<<<< HEAD
        public UserController(IUserService users) => _users = users;

=======

        public UserController(IUserService users) => _users = users;
>>>>>>> aa3343d400ec576b57971ead435e9ce8afb0a0d2

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
