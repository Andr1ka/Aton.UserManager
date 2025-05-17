using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services.Users;
using Models.Requests;
using Models.Responses;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Security.Claims;

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

        /// <summary>
        /// Registers a new user in the system
        /// </summary>
        /// <param name="request">User registration data including login, password, and personal details</param>
        /// <response code="200">Returns the newly created user's summary information</response>
        /// <response code="400">Returned when the request is invalid or validation fails</response>
        /// <response code="409">Returned when the requested login is already taken</response>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user registration. Login: {Login}", request.Login);
                return BadRequest(ModelState);
            }

            var result = await _userService.CreateUserAsync(
                request.Login,
                request.Password,
                request.Name,
                request.Gender,
                request.Birthday,
                false, 
                null
                );

            return result.Match<IActionResult>(
                Succ: user =>
                {
                    _logger.LogInformation("Successfully registered new user with login: {Login}", user.Login);
                    return Ok(_mapper.Map<UserSummaryResponse>(user));
                },
                Fail: error =>
                {
                    _logger.LogError(error, "Failed to register user with login: {Login}", request.Login);
                    return error switch
                    {
                        LoginIsAlreadyExistException => Conflict(error.Message),
                        _ => BadRequest(error.Message)
                    };
                }
            );
        }

        /// <summary>
        /// Creates a new user (requires authentication)
        /// </summary>
        /// <param name="request">User creation data including login, password, and personal details</param>
        /// <response code="200">Returns the newly created user's summary information</response>
        /// <response code="400">Returned when the request is invalid or validation fails</response>
        /// <response code="401">Returned when user is not authenticated</response>
        /// <response code="409">Returned when the login is already taken</response>
        [Authorize]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user creation. Login: {Login}", request.Login);
                return BadRequest(ModelState);
            }

           
            var authenticatedUserLogin = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(authenticatedUserLogin))
            {
                return Unauthorized("User is not authenticated");
            }

            var result = await _userService.CreateUserAsync(
                request.Login,
                request.Password,
                request.Name,
                request.Gender,
                request.Birthday,
                request.IsAdmin,
                authenticatedUserLogin 
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

        /// <summary>
        /// Updates a user's name
        /// </summary>
        /// <param name="login">Login of the user to update</param>
        /// <param name="request">New name value</param>
        /// <response code="200">Returns the updated user details</response>
        /// <response code="400">Returned when the request is invalid</response>
        /// <response code="401">Returned when user is not authenticated</response>
        /// <response code="403">Returned when access is denied or user is revoked</response>
        /// <response code="404">Returned when user is not found</response>
        [Authorize]
        [HttpPut("update/name/{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserName(string login, [FromBody] UpdateUserNameRequest request)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for name update. Login: {Login}", login);
                return BadRequest(ModelState);
            }

            var authenticatedUserLogin = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(authenticatedUserLogin))
            {
                return Unauthorized("User is not authenticated");
            }

            var result = await _userService.UpdateUserNameAsync(login, request.newName, authenticatedUserLogin);

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

        /// <summary>
        /// Updates a user's gender
        /// </summary>
        /// <param name="login">Login of the user to update</param>
        /// <param name="request">New gender value</param>
        /// <response code="200">Returns the updated user details</response>
        /// <response code="400">Returned when the request is invalid</response>
        /// <response code="401">Returned when user is not authenticated</response>
        /// <response code="403">Returned when access is denied or user is revoked</response>
        /// <response code="404">Returned when user is not found</response>
        [Authorize]
        [HttpPut("update/gender/{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserGender(string login, [FromBody] UpdateUserGenderRequest request)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for gender update. Login: {Login}", login);
                return BadRequest(ModelState);
            }

            var authenticatedUserLogin = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(authenticatedUserLogin))
            {
                return Unauthorized("User is not authenticated");
            }

            var result = await _userService.UpdateUserGenderAsync(login, request.newGender, authenticatedUserLogin);
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

        /// <summary>
        /// Updates a user's birthday
        /// </summary>
        /// <param name="login">Login of the user to update</param>
        /// <param name="request">New birthday value</param>
        /// <response code="200">Returns the updated user details</response>
        /// <response code="400">Returned when the request is invalid</response>
        /// <response code="401">Returned when user is not authenticated</response>
        /// <response code="403">Returned when access is denied or user is revoked</response>
        /// <response code="404">Returned when user is not found</response>
        [Authorize]
        [HttpPut("update/birthday/{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserBirthday(string login, [FromBody] UpdateUserBirthdayRequest request)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for birthday update. Login: {Login}", login);
                return BadRequest(ModelState);
            }

            var authenticatedUserLogin = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(authenticatedUserLogin))
            {
                return Unauthorized("User is not authenticated");
            }

            var result = await _userService.UpdateUserBirthdayAsync(login, request.newBirthday, authenticatedUserLogin);

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

        /// <summary>
        /// Updates a user's password
        /// </summary>
        /// <param name="login">Login of the user to update</param>
        /// <param name="request">New password value</param>
        /// <response code="200">Returns the updated user details</response>
        /// <response code="400">Returned when the request is invalid</response>
        /// <response code="401">Returned when user is not authenticated</response>
        /// <response code="403">Returned when access is denied or user is revoked</response>
        /// <response code="404">Returned when user is not found</response>
        [Authorize]
        [HttpPut("update/password/{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserPassword(string login, [FromBody] UpdateUserPasswordRequest request)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for password update. Login: {Login}", login);
                return BadRequest(ModelState);
            }

            var authenticatedUserLogin = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(authenticatedUserLogin))
            {
                return Unauthorized("User is not authenticated");
            }

            var result = await _userService.UpdateUserPasswordAsync(login, request.newPassword, authenticatedUserLogin);
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

        /// <summary>
        /// Updates a user's login
        /// </summary>
        /// <param name="login">Current login of the user</param>
        /// <param name="request">New login value</param>
        /// <response code="200">Returns the updated user details</response>
        /// <response code="400">Returned when the request is invalid</response>
        /// <response code="401">Returned when user is not authenticated</response>
        /// <response code="403">Returned when access is denied or user is revoked</response>
        /// <response code="404">Returned when user is not found</response>
        /// <response code="409">Returned when the new login is already taken</response>
        [Authorize]
        [HttpPut("update/login/{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateUserLogin(string login, [FromBody] UpdateUserLoginRequest request)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for login update. Old login: {OldLogin}, New login: {NewLogin}", login, request.newLogin);
                return BadRequest(ModelState);
            }

            var authenticatedUserLogin = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(authenticatedUserLogin))
            {
                return Unauthorized("User is not authenticated");
            }

            var result = await _userService.UpdateUserLoginAsync(login, request.newLogin, authenticatedUserLogin);
            return result.Match<IActionResult>(
                Succ: user =>
                {
                    _logger.LogInformation("Successfully updated login from {OldLogin} to {NewLogin}", login, request.newLogin);
                    return Ok(_mapper.Map<UserDetailResponse>(user));
                },
                Fail: error =>
                {
                    _logger.LogError(error, "Failed to update login from {OldLogin} to {NewLogin}", login, request.newLogin);
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

        /// <summary>
        /// Gets a list of all active users
        /// </summary>
        /// <response code="200">Returns a list of active users</response>
        /// <response code="401">Returned when user is not authenticated</response>
        /// <response code="403">Returned when access is denied or user is revoked</response>
        /// <response code="400">Returned when the request is invalid</response>
        [Authorize]
        [HttpGet("active-users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetActiveUsers()
        {
            var authenticatedUserLogin = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(authenticatedUserLogin))
            {
                return Unauthorized("User is not authenticated");
            }

            var result = await _userService.GetActiveUsersSortedByCreationAsync(authenticatedUserLogin);
            return result.Match<IActionResult>(
                Succ: users =>
                {
                    _logger.LogInformation("Successfully retrieved {Count} active users", users.Count());
                    return Ok(_mapper.Map<IEnumerable<UserSummaryResponse>>(users));
                },
                Fail: error =>
                {
                    _logger.LogError(error, "Failed to get active users list. Requested by: {RequestedBy}", authenticatedUserLogin);
                    return error switch
                    {
                        AccessIsDeniedException => Forbid(error.Message),
                        _ => BadRequest(error.Message)
                    };
                }
            );
        }

        /// <summary>
        /// Gets detailed information about a specific user (admin only)
        /// </summary>
        /// <param name="login">Login of the user to retrieve</param>
        /// <response code="200">Returns the user's detailed information</response>
        /// <response code="401">Returned when user is not authenticated</response>
        /// <response code="403">Returned when user is not an admin</response>
        /// <response code="404">Returned when user is not found</response>
        /// <response code="400">Returned when the request is invalid</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserByLogin(string login)
        {
            var authenticatedUserLogin = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(authenticatedUserLogin))
            {
                return Unauthorized("User is not authenticated");
            }

            var result = await _userService.GetUserByLoginAsync(login, authenticatedUserLogin);
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

        /// <summary>
        /// Gets a list of users older than specified age (admin only)
        /// </summary>
        /// <param name="age">Minimum age of users to retrieve</param>
        /// <response code="200">Returns a list of users older than specified age</response>
        /// <response code="400">Returned when the age is invalid</response>
        /// <response code="401">Returned when user is not authenticated</response>
        /// <response code="403">Returned when user is not an admin</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("older-than/{age}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsersOlderThan(int age)
        {
            var authenticatedUserLogin = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(authenticatedUserLogin))
            {
                return Unauthorized("User is not authenticated");
            }

            var result = await _userService.GetUsersOlderThanAsync(age, authenticatedUserLogin);
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

        /// <summary>
        /// Deletes a user (admin only)
        /// </summary>
        /// <param name="login">Login of the user to delete</param>
        /// <param name="request">Deletion confirmation data</param>
        /// <response code="200">Returned when the user has been successfully deleted</response>
        /// <response code="400">Returned when the request is invalid</response>
        /// <response code="401">Returned when user is not authenticated</response>
        /// <response code="403">Returned when user is not an admin</response>
        /// <response code="404">Returned when user is not found</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(string login, [FromBody] DeleteUserRequest request)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user deletion. Login: {Login}", login);
                return BadRequest(ModelState);
            }

            var authenticatedUserLogin = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(authenticatedUserLogin))
            {
                return Unauthorized("User is not authenticated");
            }

            var result = await _userService.DeleteUserAsync(login, request.softDelete, authenticatedUserLogin);
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

        /// <summary>
        /// Restores a previously deleted user (admin only)
        /// </summary>
        /// <param name="login">Login of the user to restore</param>
        /// <response code="200">Returns the restored user's information</response>
        /// <response code="401">Returned when user is not authenticated</response>
        /// <response code="403">Returned when user is not an admin</response>
        /// <response code="404">Returned when user is not found</response>
        /// <response code="400">Returned when the request is invalid</response>
        [Authorize(Roles = "Admin")]
        [HttpPost("restore/{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RestoreUser(string login)
        {
            var authenticatedUserLogin = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(authenticatedUserLogin))
            {
                return Unauthorized("User is not authenticated");
            }

            var result = await _userService.RestoreUserAsync(login, authenticatedUserLogin);
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
