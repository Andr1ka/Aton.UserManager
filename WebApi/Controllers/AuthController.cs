using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Services.Auth;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for login attempt. Login: {Login}", request.Login);
                return BadRequest(ModelState);
            }

            var (success, token) = await _authService.AuthenticateAsync(request.Login, request.Password);

            if (!success)
            {
                return Unauthorized(new { message = "Invalid login or password" });
            }

            return Ok(new { token });

        }
    }
} 