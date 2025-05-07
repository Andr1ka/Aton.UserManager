using Domain;
using Domain.Enums;
using Domain.Exceptions;
using LanguageExt.Common;
using Persistence.Interfaces;

namespace Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        
        public async Task<Result<User>> CreateUserAsync(string login, string password, string name, GenderType gender, DateTime? birthday, bool isAdmin, string? createdBy)
        {

            if (!await _userRepository.IsLoginAvailableAsync(login)) return new Result<User>(new LoginIsAlreadyExistException("login is already exist"));

            if (createdBy is not null && !await _userRepository.IsAdminAsync(createdBy)) isAdmin = false;

            var user = await _userRepository.CreateUserAsync(login, password, name, gender, birthday, isAdmin, createdBy);

            return new Result<User>(user);

        }

        public async Task<Result<User>> DeleteUserAsync(string login, bool softDelete, string revokedBy)
        {
            var authResult = await AuthorizeAndGetUserAsync(login, revokedBy, requireAdminOnly: true, allowSelf: false, checkTargetUserActive: false);

            if (authResult.IsFaulted) return authResult;

            var userToDelete = authResult.Match(
                Succ: user => user,
                Fail: error => default 
                );


            if (softDelete && userToDelete.RevokedOn is not null)
            {
                return new Result<User>(new UserIsRevokedException("user is already revoked"));
            }
            
            await _userRepository.DeleteUserAsync(login, softDelete, revokedBy);

            return new Result<User>(userToDelete);
        }

        public async Task<Result<IEnumerable<User>>> GetActiveUsersSortedByCreationAsync(string requestedBy)
        {
           if(!await _userRepository.IsAdminAsync(requestedBy)) return new Result<IEnumerable<User>>(new AccessIsDeniedException("access is denied"));

           var usersList = await _userRepository.GetActiveUsersSortedByCreationAsync();

           return new Result<IEnumerable<User>>(usersList);
        }

        public async Task<Result<User>> GetUserByCredentialsAsync(string login, string password, string requestedBy)
        {
            if(!login.Equals(requestedBy)) return new Result<User>(new AccessIsDeniedException("access is denied"));

            var authResult = await AuthorizeAndGetUserAsync(login, requestedBy, requireAdminOnly: false, allowSelf: true, checkTargetUserActive: true);

            if (authResult.IsFaulted) return authResult;

            var user = await _userRepository.GetUserByCredentialsAsync(login, password);

            if(user is null)
            {
                return new Result<User>(new UserDoesNotExistException("the user does not exist or credentials invalid"));
            }

            return new Result<User>(user);
        }

        public async Task<Result<User>> GetUserByLoginAsync(string login, string requestedBy)
        {
            return await AuthorizeAndGetUserAsync(login, requestedBy, requireAdminOnly: true, allowSelf: false, checkTargetUserActive: false);
        }

        public async Task<Result<IEnumerable<User>>> GetUsersOlderThanAsync(int age, string requestedBy)
        {
            if (!await _userRepository.IsAdminAsync(requestedBy)) return new Result<IEnumerable<User>>(new AccessIsDeniedException("access is denied"));

            if(age < 0) return new Result<IEnumerable<User>> (new InvalidAgeException("invalid age"));

            return new Result<IEnumerable<User>>(await _userRepository.GetUsersOlderThanAsync(age));

        }

        public async Task<Result<User>> RestoreUserAsync(string login, string modifiedBy)
        {
            var authResult = await AuthorizeAndGetUserAsync(login, modifiedBy, requireAdminOnly: true, allowSelf: false, checkTargetUserActive: false);

            if (authResult.IsFaulted) return authResult;

            var restoredUser = await _userRepository.RestoreUserAsync(login, modifiedBy);

            return new Result<User>(restoredUser);
        }

        public async Task<Result<User>> UpdateUserBirthdayAsync(string login, DateTime newBirthday, string modifiedBy)
        {
            var authResult = await AuthorizeAndGetUserAsync(login, modifiedBy, requireAdminOnly: false, allowSelf: true, checkTargetUserActive: true);

            if (authResult.IsFaulted) return authResult;

            var user = await _userRepository.UpdateUserBirthdayAsync(login, newBirthday, modifiedBy);

            return new Result<User>(user);
        }

        public async Task<Result<User>> UpdateUserGenderAsync(string login, GenderType newGender, string modifiedBy)
        {
            var authResult = await AuthorizeAndGetUserAsync(login, modifiedBy, requireAdminOnly: false, allowSelf: true, checkTargetUserActive: true);

            if (authResult.IsFaulted) return authResult;

            var user = await _userRepository.UpdateUserGenderAsync(login, newGender, modifiedBy);

            return new Result<User>(user);
        }

        public async Task<Result<User>> UpdateUserLoginAsync(string login, string newLogin, string modifiedBy)
        {
            var authResult = await AuthorizeAndGetUserAsync(login, modifiedBy, requireAdminOnly: false, allowSelf: true, checkTargetUserActive: true);

            if (authResult.IsFaulted) return authResult;

            if (!await _userRepository.IsLoginAvailableAsync(newLogin))
            {
                return new Result<User>(new LoginIsAlreadyExistException("new login is already taken"));
            }
            
            var user = await _userRepository.UpdateUserLoginAsync(login, newLogin, modifiedBy);

            return new Result<User>(user);
        }

        public async Task<Result<User>> UpdateUserNameAsync(string login, string newName, string modifiedBy)
        {
            var authResult = await AuthorizeAndGetUserAsync(login, modifiedBy, requireAdminOnly: false, allowSelf: true, checkTargetUserActive: true);

            if (authResult.IsFaulted) return authResult;

            var updatedUser = await _userRepository.UpdateUserNameAsync(login, newName, modifiedBy);

            return new Result<User>(updatedUser);
        }

        public async Task<Result<User>> UpdateUserPasswordAsync(string login, string newPassword, string modifiedBy)
        {
            var authResult = await AuthorizeAndGetUserAsync(login, modifiedBy, requireAdminOnly: false, allowSelf: true, checkTargetUserActive: true);

            if (authResult.IsFaulted) return authResult;

            var user = await _userRepository.UpdateUserPasswordAsync(login, newPassword, modifiedBy);

            return new Result<User>(user);
        }

        
        private async Task<Result<User>> AuthorizeAndGetUserAsync(string targetLogin, string requestingUserLogin, bool requireAdminOnly = false, bool allowSelf = false, bool checkTargetUserActive = true)
        {
            var user = await _userRepository.GetUserByLoginAsync(targetLogin);

            if (user is null)
            {
                return new Result<User>(new UserDoesNotExistException($"user does not exist"));
            }

           
            if (checkTargetUserActive)
            {
                if (!await _userRepository.IsUserActiveAsync(targetLogin)) 
                {
                    return new Result<User>(new UserIsRevokedException($"user is revoked"));
                }
            }

            bool isRequestingUserAdmin = await _userRepository.IsAdminAsync(requestingUserLogin);

            bool isSelfAction = targetLogin.Equals(requestingUserLogin);
           
            bool hasPermission;
            if (requireAdminOnly)
            {
                hasPermission = isRequestingUserAdmin;
            }
            else
            {
                hasPermission = isRequestingUserAdmin || (allowSelf && isSelfAction);
            }

            if (!hasPermission)
            {
                return new Result<User>(new AccessIsDeniedException("access is denied"));
            }
            return new Result<User>(user); 
        }
    }
}
