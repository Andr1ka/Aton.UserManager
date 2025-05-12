using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services.Users;
using Models.Requests;
using Models.Responses;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, ILogger<UsersController> logger, IMapper mapper)
        {
            _userService = userService;
            _logger = logger;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("create")]    
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user creation. Login: {Login}", request.Login);
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
                Succ: user =>
                {
                    _logger.LogInformation("Successfully created user with login: {Login}", user.Login);
                    return Ok(_mapper.Map<UserSummaryResponse>(user));
                },
                Fail: error =>
                {
                    _logger.LogError(error, "Failed to create user with login: {Login}", request.Login);
                    return error switch
                    {
                        LoginIsAlreadyExistException => Conflict(error.Message),
                        _ => BadRequest(error.Message)
                    };
                }
            );
        }

        [Authorize]
        [HttpPut("update/name/{login}")]
        public async Task<IActionResult> UpdateUserName(string login, [FromBody] UpdateUserNameRequest request)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for name update. Login: {Login}", login);
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateUserNameAsync(login, request.Name, request.updatedBy);

            return result.Match<IActionResult>(
                Succ: user =>
                {
                    _logger.LogInformation("Successfully updated name for user: {Login}", login);
                    return Ok(_mapper.Map<UserDetailResponse>(user));
                },
                Fail: error =>
                {
                    _logger.LogError(error, "Failed to update name for user: {Login}", login);
                    return error switch
                    {
                        UserDoesNotExistException => NotFound(error.Message),
                        UserIsRevokedException => Forbid(error.Message),
                        AccessIsDeniedException => Forbid(error.Message),
                        _ => BadRequest(error.Message)
                    };
                }
            );
        }

        [Authorize]
        [HttpPut("update/gender/{login}")]
        public async Task<IActionResult> UpdateUserGender(string login, [FromBody] UpdateUserGenderRequest request)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for gender update. Login: {Login}", login);
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateUserGenderAsync(login, request.Gender, request.updatedBy);
            return result.Match<IActionResult>(
                Succ: user =>
                {
                    _logger.LogInformation("Successfully updated gender for user: {Login}", login);
                    return Ok(_mapper.Map<UserDetailResponse>(user));
                },
                Fail: error =>
                {
                    _logger.LogError(error, "Failed to update gender for user: {Login}", login);
                    return error switch
                    {
                        UserDoesNotExistException => NotFound(error.Message),
                        UserIsRevokedException => Forbid(error.Message),
                        AccessIsDeniedException => Forbid(error.Message),
                        _ => BadRequest(error.Message)
                    };
                }
            );
        }

        [Authorize]
        [HttpPut("update/birthday/{login}")]
        public async Task<IActionResult> UpdateUserBirthday(string login, [FromBody] UpdateUserBirthdayRequest request)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for birthday update. Login: {Login}", login);
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateUserBirthdayAsync(login, request.Birthday, request.updatedBy);
            return result.Match<IActionResult>(
                Succ: user =>
                {
                    _logger.LogInformation("Successfully updated birthday for user: {Login}", login);
                    return Ok(_mapper.Map<UserDetailResponse>(user));
                },
                Fail: error =>
                {
                    _logger.LogError(error, "Failed to update birthday for user: {Login}", login);
                    return error switch
                    {
                        UserDoesNotExistException => NotFound(error.Message),
                        UserIsRevokedException => Forbid(error.Message),
                        AccessIsDeniedException => Forbid(error.Message),
                        _ => BadRequest(error.Message)
                    };
                }
            );
        }

        [Authorize]
        [HttpPut("update/password/{login}")]
        public async Task<IActionResult> UpdateUserPassword(string login, [FromBody] UpdateUserPasswordRequest request)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for password update. Login: {Login}", login);
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateUserPasswordAsync(login, request.NewPassword, request.updatedBy);
            return result.Match<IActionResult>(
                Succ: user =>
                {
                    _logger.LogInformation("Successfully updated password for user: {Login}", login);
                    return Ok(_mapper.Map<UserDetailResponse>(user));
                },
                Fail: error =>
                {
                    _logger.LogError(error, "Failed to update password for user: {Login}", login);
                    return error switch
                    {
                        UserDoesNotExistException => NotFound(error.Message),
                        UserIsRevokedException => Forbid(error.Message),
                        AccessIsDeniedException => Forbid(error.Message),
                        _ => BadRequest(error.Message)
                    };
                }
            );
        }

        [Authorize]
        [HttpPut("update/login/{login}")]
        public async Task<IActionResult> UpdateUserLogin(string login, [FromBody] UpdateUserLoginRequest request)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for login update. Old login: {OldLogin}, New login: {NewLogin}", login, request.NewLogin);
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateUserLoginAsync(login, request.NewLogin, request.updatedBy);
            return result.Match<IActionResult>(
                Succ: user =>
                {
                    _logger.LogInformation("Successfully updated login from {OldLogin} to {NewLogin}", login, request.NewLogin);
                    return Ok(_mapper.Map<UserDetailResponse>(user));
                },
                Fail: error =>
                {
                    _logger.LogError(error, "Failed to update login from {OldLogin} to {NewLogin}", login, request.NewLogin);
                    return error switch
                    {
                        UserDoesNotExistException => NotFound(error.Message),
                        UserIsRevokedException => Forbid(error.Message),
                        AccessIsDeniedException => Forbid(error.Message),
                        LoginIsAlreadyExistException => Conflict(error.Message),
                        _ => BadRequest(error.Message)
                    };
                }
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("active-users")]
        public async Task<IActionResult> GetActiveUsers([FromQuery] string requestedBy) // Предполагаем, что requestedBy передается как query-параметр для авторизации
        {
            var result = await _userService.GetActiveUsersSortedByCreationAsync(requestedBy);
            return result.Match<IActionResult>(
                Succ: users =>
                {
                    _logger.LogInformation("Successfully retrieved {Count} active users", users.Count());
                    return Ok(_mapper.Map<IEnumerable<UserSummaryResponse>>(users));
                },
                Fail: error =>
                {
                    _logger.LogError(error, "Failed to get active users list. Requested by: {RequestedBy}", requestedBy);
                    return error switch
                    {
                        AccessIsDeniedException => Forbid(error.Message),
                        _ => BadRequest(error.Message)
                    };
                }
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{login}")]
        public async Task<IActionResult> GetUserByLogin(string login, [FromQuery] string requestedBy)
        {
            var result = await _userService.GetUserByLoginAsync(login, requestedBy);
            return result.Match<IActionResult>(
                Succ: user =>
                {
                    _logger.LogInformation("Successfully retrieved user: {Login}", login);
                    return Ok(_mapper.Map<UserDetailResponse>(user));
                },
                Fail: error =>
                {
                    _logger.LogError(error, "Failed to get user by login: {Login}", login);
                    return error switch
                    {
                        UserDoesNotExistException => NotFound(error.Message),
                        AccessIsDeniedException => Forbid(error.Message),
                        _ => BadRequest(error.Message)
                    };
                }
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("older-than/{age}")]
        public async Task<IActionResult> GetUsersOlderThan(int age, [FromQuery] string requestedBy)
        {
            var result = await _userService.GetUsersOlderThanAsync(age, requestedBy);
            return result.Match<IActionResult>(
                Succ: users =>
                {
                    _logger.LogInformation("Successfully retrieved {Count} users older than {Age}", users.Count(), age);
                    return Ok(_mapper.Map<IEnumerable<UserSummaryResponse>>(users));
                },
                Fail: error =>
                {
                    _logger.LogError(error, "Failed to get users older than {Age}", age);
                    return error switch
                    {
                        AccessIsDeniedException => Forbid(error.Message),
                        InvalidAgeException => BadRequest(error.Message),
                        _ => BadRequest(error.Message)
                    };
                }
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{login}")]
        public async Task<IActionResult> DeleteUser(string login, [FromBody] DeleteUserRequest request)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user deletion. Login: {Login}", login);
                return BadRequest(ModelState);
            }

            var result = await _userService.DeleteUserAsync(login, request.softDelete, request.requestedBy);
            return result.Match<IActionResult>(
                Succ: user =>
                {
                    _logger.LogInformation("Successfully deleted user: {Login}", login);
                    return NoContent();
                },
                Fail: error =>
                {
                    _logger.LogError(error, "Failed to delete user: {Login}", login);
                    return error switch
                    {
                        UserDoesNotExistException => NotFound(error.Message),
                        UserIsRevokedException => Conflict(error.Message),
                        AccessIsDeniedException => Forbid(error.Message),
                        _ => BadRequest(error.Message)
                    };
                }
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("restore/{login}")]
        public async Task<IActionResult> RestoreUser(string login, [FromBody] RestoreUserRequest request)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user restoration. Login: {Login}", login);
                return BadRequest(ModelState);
            }

            var result = await _userService.RestoreUserAsync(login, request.requestedBy);
            return result.Match<IActionResult>(
                Succ: user =>
                {
                    _logger.LogInformation("Successfully restored user: {Login}", login);
                    return Ok(_mapper.Map<UserDetailResponse>(user));
                },
                Fail: error =>
                {
                    _logger.LogError(error, "Failed to restore user: {Login}", login);
                    return error switch
                    {
                        UserDoesNotExistException => NotFound(error.Message),
                        AccessIsDeniedException => Forbid(error.Message),
                        _ => BadRequest(error.Message)
                    };
                }
            );
        }
    }
}
