using Microsoft.AspNetCore.Mvc;
using Services.Users;
using Models.Requests;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        // private readonly ILogger<UsersController> _logger; 

        public UsersController(IUserService userService /*, ILogger<UsersController> logger */)
        {
            _userService = userService;
            // _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.CreateUserAsync(
                request.Login,
                request.Password,
                request.Name,
                request.Gender,
                request.Birthday,
                request.IsAdmin,
                request.createdBy
                );

            return result.Match<IActionResult>(
                Succ: user => Ok(user),
                Fail: error => BadRequest(error.Message)
                );
        }

    }
} 
