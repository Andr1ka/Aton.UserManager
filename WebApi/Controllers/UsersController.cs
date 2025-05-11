using Microsoft.AspNetCore.Mvc;
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

        // Требование 2: Изменение имени (Может менять Администратор, либо лично пользователь, если он активен)
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

        // TODO: Добавить авторизацию, чтобы только администратор или сам пользователь могли вызывать этот метод.
        // Требование 2: Изменение пола (Может менять Администратор, либо лично пользователь, если он активен)
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

        // TODO: Добавить авторизацию, чтобы только администратор или сам пользователь могли вызывать этот метод.
        // Требование 2: Изменение даты рождения (Может менять Администратор, либо лично пользователь, если он активен)
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

        // TODO: Добавить авторизацию, чтобы только администратор или сам пользователь могли вызывать этот метод.
        // Требование 3: Изменение пароля (Может менять либо Администратор, либо лично пользователь, если он активен)
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

        // TODO: Добавить авторизацию, чтобы только администратор или сам пользователь могли вызывать этот метод.
        // Требование 4: Изменение логина (Может менять либо Администратор, либо лично пользователь, если он активен, логин должен оставаться уникальным)
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

        // TODO: Добавить авторизацию, чтобы только администраторы могли вызывать этот метод.
        // Требование 5: Запрос списка всех активных (отсутствует RevokedOn) пользователей, список отсортирован по CreatedOn (Доступно Админам)
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

        // TODO: Добавить авторизацию, чтобы только администраторы могли вызывать этот метод.
        // Требование 6: Запрос пользователя по логину, в списке должны быть имя, пол и дата рождения, статус активный или нет (Доступно Админам)
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
        
        // TODO: Добавить авторизацию, чтобы только сам пользователь мог вызывать этот метод.
        // Требование 7: Запрос пользователя по логину и паролю (Доступно только самому пользователю, если он активен (отсутствует RevokedOn))
        [HttpPost("login")]
        public async Task<IActionResult> GetUserByLoginAndPassword([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for login attempt. Login: {Login}", request.Login);
                return BadRequest(ModelState);
            }
           
            var result = await _userService.GetUserByCredentialsAsync(request.Login, request.Password);
            return result.Match<IActionResult>(
                Succ: user =>
                {
                    _logger.LogInformation("Successfully logged in user: {Login}", request.Login);
                    return Ok(_mapper.Map<AuthenticatedUserResponse>(user));
                },
                Fail: error =>
                {
                    _logger.LogError(error, "Failed to login user: {Login}", request.Login);
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

        // TODO: Добавить авторизацию, чтобы только администраторы могли вызывать этот метод.
        // Требование 8: Запрос всех пользователей старше определённого возраста (Доступно Админам)
        [HttpGet("older-than/{age}")]
        public async Task<IActionResult> GetUsersOlderThan(int age, [FromQuery] string requestedBy) // Предполагаем, что requestedBy передается как query-параметр для авторизации
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

        // TODO: Добавить авторизацию, чтобы только администраторы могли вызывать этот метод.
        // Требование 9: Удаление пользователя по логину полное или мягкое (При мягком удалении должна происходить простановка RevokedOn и RevokedBy) (Доступно Админам)
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

        // TODO: Добавить авторизацию, чтобы только администраторы могли вызывать этот метод.
        // Требование 10: Восстановление пользователя - Очистка полей (RevokedOn, RevokedBy) (Доступно Админам)
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
