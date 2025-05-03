using Aton.UserManager.DTOs;
using Domain;
using Domain.Enums;
using Domain.Exceptions;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Services.Users;
using System.Security.Claims; // Required for GetCurrentUserLogin placeholder

namespace Aton.UserManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Add this later when authentication is set up
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        { 
            _userService = userService;
        }

        // Placeholder for getting current user's login from authentication context
        // In a real app, this would use HttpContext.User
        private string GetCurrentUserLogin() 
        {
            // return User.FindFirstValue(ClaimTypes.Name) ?? "anonymous"; // Example using Claims
            // For now, returning a hardcoded admin/user for testing purposes
             // IMPORTANT: Replace this with actual authentication logic
             // return "admin"; // Simulate admin
             return "testuser"; // Simulate regular user
        }

        // --- Helper to map Result<T> to IActionResult ---
        private IActionResult MapResultToActionResult<T>(Result<T> result, Func<T, IActionResult> successAction)
        {
            return result.Match<IActionResult>(
                succ => successAction(succ),
                fail => MapExceptionToActionResult(fail)
            );
        }

        private IActionResult MapExceptionToActionResult(Exception ex)
        { 
            return ex switch
            {
                LoginIsAlreadyExistException => Conflict(new { message = ex.Message }),
                UserDoesNotExistException => NotFound(new { message = ex.Message }),
                AccessIsDeniedException => Forbid(), // 403 Forbidden
                UserIsRevokedException => BadRequest(new { message = ex.Message }), // Or maybe 403?
                InvalidAgeException => BadRequest(new { message = ex.Message }),
                // Add other specific exceptions if needed
                _ => StatusCode(500, new { message = "An unexpected error occurred." }) // Generic 500
            };
        }

        // --- Controller Actions ---

        // 1. Create User (Admin only)
        [HttpPost]
        // [Authorize(Roles = "Admin")] // Add when auth is ready
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        { 
            string createdBy = GetCurrentUserLogin(); 
            var result = await _userService.CreateUserAsync(request.Login, request.Password, request.Name, request.Gender, request.Birthday ?? default, request.IsAdmin, createdBy);
            
            return MapResultToActionResult(result, user => 
                CreatedAtAction(nameof(GetUserByLoginAdmin), new { login = user.Login }, MapUserToSummaryResponse(user)) // Return 201 Created
            );
        }

        // 2. Update User Profile (Admin or Self)
        [HttpPut("{login}/profile")]
        public async Task<IActionResult> UpdateUserProfile([FromRoute] string login, [FromBody] UpdateUserProfileRequest request)
        {
            string modifiedBy = GetCurrentUserLogin();
            Result<User> result = new Result<User>(); // Initialize with a dummy result
            bool updated = false;

            // Apply updates sequentially
            if (request.Name is not null)
            {
                result = await _userService.UpdateUserNameAsync(login, request.Name, modifiedBy);
                if (result.IsFaulted) return MapExceptionToActionResult(result.IfFail(ex => ex));
                updated = true;
            }
            if (request.Gender.HasValue)
            { 
                result = await _userService.UpdateUserGenderAsync(login, request.Gender.Value, modifiedBy);
                if (result.IsFaulted) return MapExceptionToActionResult(result.IfFail(ex => ex));
                updated = true;
            }
            if (request.Birthday.HasValue)
            {
                result = await _userService.UpdateUserBirthdayAsync(login, request.Birthday.Value, modifiedBy);
                if (result.IsFaulted) return MapExceptionToActionResult(result.IfFail(ex => ex));
                updated = true;
            }

            if (!updated)
            {
                return BadRequest(new { message = "No update data provided."} );
            }

            // Return the final state of the user after the last successful update
            return MapResultToActionResult(result, user => Ok(MapUserToSummaryResponse(user))); 
        }

        // 3. Update Password (Admin or Self)
        [HttpPut("{login}/password")]
        public async Task<IActionResult> UpdateUserPassword([FromRoute] string login, [FromBody] UpdateUserPasswordRequest request)
        {
            string modifiedBy = GetCurrentUserLogin();
            var result = await _userService.UpdateUserPasswordAsync(login, request.NewPassword, modifiedBy);
            return MapResultToActionResult(result, _ => NoContent()); // Return 204 No Content on success
        }

        // 4. Update Login (Admin or Self)
        [HttpPut("{login}/login")]
        public async Task<IActionResult> UpdateUserLogin([FromRoute] string login, [FromBody] UpdateUserLoginRequest request)
        {
            string modifiedBy = GetCurrentUserLogin();
            var result = await _userService.UpdateUserLoginAsync(login, request.NewLogin, modifiedBy);
            // Return the updated user info (including the new login)
            return MapResultToActionResult(result, user => Ok(MapUserToSummaryResponse(user)));
        }

        // 5. Get Active Users (Admin only)
        [HttpGet("active")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActiveUsers()
        {
            string requestedBy = GetCurrentUserLogin();
            var result = await _userService.GetActiveUsersSortedByCreationAsync(requestedBy);
            
            return MapResultToActionResult(result, users => 
                Ok(users.Select(MapUserToSummaryResponse)) // Map to DTO list
            );
        }

        // 6. Get User by Login (Admin only)
        [HttpGet("{login}", Name = nameof(GetUserByLoginAdmin))]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByLoginAdmin([FromRoute] string login)
        { 
            string requestedBy = GetCurrentUserLogin(); 
            var result = await _userService.GetUserByLoginAsync(login, requestedBy);
            
            return MapResultToActionResult(result, user => 
                Ok(MapUserToDetailResponse(user))
            );
        }

        // 7. Get User by Credentials (Self only)
        [HttpPost("login")]
        // [AllowAnonymous] // Typically login is anonymous, but uses credentials for auth
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        { 
            // Service expects requestedBy to be the same as the login being checked
            var result = await _userService.GetUserByCredentialsAsync(request.Login, request.Password, request.Login); 

            return MapResultToActionResult(result, user => 
                Ok(MapUserToAuthenticatedResponse(user)) // Return user details (and potentially token)
            );
        }

        // 8. Get Users Older Than (Admin only)
        [HttpGet("older-than/{age}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersOlderThan([FromRoute] int age)
        {
            string requestedBy = GetCurrentUserLogin();
            var result = await _userService.GetUsersOlderThanAsync(age, requestedBy);
            
            return MapResultToActionResult(result, users => 
                Ok(users.Select(MapUserToSummaryResponse))
            );
        }

        // 9. Delete User (Admin only)
        [HttpDelete("{login}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromRoute] string login, [FromQuery] bool softDelete = true)
        {
            string revokedBy = GetCurrentUserLogin();
            // DeleteUserAsync returns void, so we handle potential admin check failure within the service.
            // We assume success if no exception is thrown by the service call itself (handled by potential admin check).
            // However, the service *should* ideally return a Result<bool> or similar to indicate success/failure/reason.
            // For now, we'll call it and assume 204 No Content if it completes.
            // We need to handle the case where the service doesn't throw but the user wasn't admin.
            // The service currently just returns early if not admin. This isn't ideal API design.
            // Let's add an explicit admin check here for clarity, though it duplicates service logic.
            
            var isAdmin = await _userService.IsAdminAsync(revokedBy); // Assuming IUserService exposes IsAdminAsync
            if (!isAdmin) return Forbid();

            try
            {
                await _userService.DeleteUserAsync(login, softDelete, revokedBy);
                // We need to check if the user actually existed before returning NoContent.
                // The current DeleteUserAsync doesn't indicate this.
                // A better approach would be for DeleteUserAsync to return Result<bool> or throw UserDoesNotExistException.
                
                // Let's check existence *after* the attempt (less efficient, but works with current service)
                 var checkResult = await _userService.GetUserByLoginAsync(login, revokedBy); // Use admin rights to check
                 if (checkResult.IsFaulted && checkResult.IfFail(ex => ex) is UserDoesNotExistException)
                 {
                      return NotFound(new { message = $"User '{login}' not found for deletion."});
                 }
                 // If soft delete, the user still exists, but is marked revoked.
                 // If hard delete, the user should no longer exist according to GetUserByLoginAsync
                 
                 // TODO: Refine this logic based on improved service layer return value/exceptions.

                return NoContent(); // 204 No Content
            }
            catch (Exception ex) // Catch any unexpected errors from the service
            {
                // This shouldn't happen if service handles its errors, but as a safeguard.
                return MapExceptionToActionResult(ex);
            }
        }

        // 10. Restore User (Admin only)
        [HttpPost("{login}/restore")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreUser([FromRoute] string login)
        {
            string modifiedBy = GetCurrentUserLogin();
            var result = await _userService.RestoreUserAsync(login, modifiedBy);

            return MapResultToActionResult(result, user => Ok(MapUserToSummaryResponse(user)));
        }

        // --- Mappers --- 
        // Consider using a mapping library like AutoMapper for complex scenarios

        private static UserSummaryResponse MapUserToSummaryResponse(User user)
        {
            return new UserSummaryResponse
            {
                Login = user.Login,
                Name = user.Name,
                Gender = user.Gender,
                Birthday = user.Birthday,
                IsAdmin = user.Admin,
                CreatedOn = user.CreatedOn
            };
        }

        private static UserDetailResponse MapUserToDetailResponse(User user)
        {
             return new UserDetailResponse
             {
                 Name = user.Name,
                 Gender = user.Gender,
                 Birthday = user.Birthday,
                 IsActive = user.RevokedOn == null // Determine active status
             };
        }

        private static AuthenticatedUserResponse MapUserToAuthenticatedResponse(User user)
        {
             return new AuthenticatedUserResponse
             {
                 Login = user.Login,
                 Name = user.Name,
                 Gender = user.Gender,
                 Birthday = user.Birthday,
                 IsAdmin = user.Admin
             };
        }

         // Add this method if IUserService does not expose IsAdminAsync
         // This requires access to the repository or modifying the service interface
         /*
         private async Task<bool> IsAdminAsync(string login)
         {
             // Replace with actual implementation - requires IUserRepository or modification to IUserService
             // Example: return await _userRepository.IsAdminAsync(login);
             return login == "admin"; // Temporary placeholder
         }
         */
    }
} 