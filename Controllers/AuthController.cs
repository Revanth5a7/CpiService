using Microsoft.AspNetCore.Mvc;
using CpiService.Auth;

namespace CpiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtAuthenticationManager _jwtAuth;

        public AuthController(JwtAuthenticationManager jwtAuth)
        {
            _jwtAuth = jwtAuth;
        }

        [HttpPost("token")]
        public IActionResult Authenticate([FromBody] LoginRequest request)
        {
            var token = _jwtAuth.Authenticate(request.Username, request.Password);
            if (token == null) return Unauthorized();
            return Ok(new { Token = token });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}