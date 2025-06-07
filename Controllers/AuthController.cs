using Microsoft.AspNetCore.Mvc;
using générationdétiquettes.Data;
using générationdétiquettes.Models;
using Microsoft.EntityFrameworkCore;
using générationdétiquettes.DTO;

namespace générationdétiquettes.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(UserDto dto)
        {
            var token = await _authService.Register(dto);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto dto)
        {
            var token = await _authService.Login(dto);
            return Ok(new { token });
        }
    }


}