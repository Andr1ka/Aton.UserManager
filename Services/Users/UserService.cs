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

        
        public async Task<Result<User>> CreateUserAsync(string login, string password, string name, GenderType gender, DateTime birthday, bool isAdmin, string createdBy)
        {

            if (!await _userRepository.IsLoginAvailableAsync(login)) return new Result<User>(new LoginIsAlreadyExistException("login is already exist"));

            if (!await _userRepository.IsAdminAsync(createdBy))
            {
                isAdmin = false;
            }

            var user = await _userRepository.CreateUserAsync(login, password, name, gender, birthday, isAdmin, createdBy);
            return new Result<User>(user);

        }

        public async Task DeleteUserAsync(string login, bool softDelete, string revokedBy) 
        {
            if (!await _userRepository.IsAdminAsync(revokedBy)) return;

            await _userRepository.DeleteUserAsync(login, softDelete, revokedBy);
        }

        public async Task<Result<IEnumerable<User>>> GetActiveUsersSortedByCreationAsync(string requestedBy)
        {
           if(await _userRepository.IsAdminAsync(requestedBy))
           {
                var usersList = await _userRepository.GetActiveUsersSortedByCreationAsync();

                return new Result<IEnumerable<User>>(usersList);
           }
           else
           {
                return new Result<IEnumerable<User>>(new AccessIsDeniedException("access is denied"));
           }
        }

        public async Task<Result<User>> GetUserByCredentialsAsync(string login, string password, string requestedBy)
        {
            if (!await _userRepository.IsUserActiveAsync(login)) return new Result<User>(new UserIsRevokedException("user is revoked"));

            if  (!login.Equals(requestedBy)) return new Result<User>(new AccessIsDeniedException("access is denied"));

            var user = await _userRepository.GetUserByCredentialsAsync(login, password);

            if(user is null) return new Result<User>(new UserDoesNotExistException("the user does not exist"));

            return new Result<User>(user);


        }

        public async Task<Result<User>> GetUserByLoginAsync(string login, string requestedBy)
        {
            var user = await _userRepository.GetUserByLoginAsync(login);

            if (!await _userRepository.IsAdminAsync(requestedBy)) return new Result<User>(new AccessIsDeniedException("access is denied"));

            if(user is not null)
            {
                return new Result<User>(user);
            }
            else
            {
                return new Result<User>(new UserDoesNotExistException("the user does not exist"));
            }
        }

        public async Task<Result<IEnumerable<User>>> GetUsersOlderThanAsync(int age, string requestedBy)
        {
            if (!await _userRepository.IsAdminAsync(requestedBy)) return new Result<IEnumerable<User>>(new AccessIsDeniedException("access is denied"));

            if(age < 0) return new Result<IEnumerable<User>> (new InvalidAgeException("invalid age"));

            return new Result<IEnumerable<User>>(await _userRepository.GetUsersOlderThanAsync(age));

        }

        public async Task<Result<User>> RestoreUserAsync(string login, string modifiedBy)
        {
            if (await _userRepository.IsAdminAsync(modifiedBy))
            {
               var user = await _userRepository.RestoreUserAsync(login, modifiedBy);

               if(user is null)
               {
                    return new Result<User>(new UserDoesNotExistException("the user does not exist or has not been revoked"));
               }
               return new Result<User>(user);
            }
            else
            {
                return new Result<User>(new AccessIsDeniedException("access is denied"));
            }
        }

        public async Task<Result<User>> UpdateUserBirthdayAsync(string login, DateTime newBirthday, string modifiedBy)
        {
            if (await _userRepository.GetUserByLoginAsync(login) is null) return new Result<User>(new UserDoesNotExistException("user does not exist"));

            if (!await _userRepository.IsUserActiveAsync(login)) return new Result<User>(new UserIsRevokedException("user is revoked"));


            if (!(await _userRepository.IsAdminAsync(modifiedBy) || login.Equals(modifiedBy)))
            {
                return new Result<User>(new AccessIsDeniedException("access is denied"));

            }

            var user = await _userRepository.UpdateUserBirthdayAsync(login, newBirthday, modifiedBy);

            return new Result<User>(user);
        }

        public async Task<Result<User>> UpdateUserGenderAsync(string login, GenderType newGender, string modifiedBy)
        {
            if (await _userRepository.GetUserByLoginAsync(login) is null) return new Result<User>(new UserDoesNotExistException("user does not exist"));

            if (!await _userRepository.IsUserActiveAsync(login)) return new Result<User>(new UserIsRevokedException("user is revoked"));


            if (!(await _userRepository.IsAdminAsync(modifiedBy) || login.Equals(modifiedBy)))
            {
                return new Result<User>(new AccessIsDeniedException("access is denied"));

            }

            var user = await _userRepository.UpdateUserGenderAsync(login, newGender, modifiedBy);

            return new Result<User>(user);
        }

        public async Task<Result<User>> UpdateUserLoginAsync(string login, string newLogin, string modifiedBy)
        {
            if (await _userRepository.GetUserByLoginAsync(login) is null) return new Result<User>(new UserDoesNotExistException("user does not exist"));

            if (!await _userRepository.IsUserActiveAsync(login)) return new Result<User>(new UserIsRevokedException("user is revoked"));


            if (!(await _userRepository.IsAdminAsync(modifiedBy) || login.Equals(modifiedBy)))
            {
                return new Result<User>(new AccessIsDeniedException("access is denied"));

            }

            if(await _userRepository.IsLoginAvailableAsync(newLogin)) return new Result<User>(new LoginIsAlreadyExistException("login must be unique"));

            var user = await _userRepository.UpdateUserLoginAsync(login, newLogin, modifiedBy);

            return new Result<User>(user);
        }

        public async Task<Result<User>> UpdateUserNameAsync(string login, string newName, string modifiedBy)
        {

            if (await _userRepository.GetUserByLoginAsync(login) is null) return new Result<User>(new UserDoesNotExistException("user does not exist"));

            if (!await _userRepository.IsUserActiveAsync(login)) return new Result<User>(new UserIsRevokedException("user is revoked"));


            if (!(await _userRepository.IsAdminAsync(modifiedBy) || login.Equals(modifiedBy)))
            {
                return new Result<User>(new AccessIsDeniedException("access is denied"));
                
            }

            var user = await _userRepository.UpdateUserNameAsync(login, newName, modifiedBy);

            return new Result<User>(user);
        }

        public async Task<Result<User>> UpdateUserPasswordAsync(string login, string newPassword, string modifiedBy)
        {
            if (await _userRepository.GetUserByLoginAsync(login) is null) return new Result<User>(new UserDoesNotExistException("user does not exist"));

            if (!await _userRepository.IsUserActiveAsync(login)) return new Result<User>(new UserIsRevokedException("user is revoked"));


            if (!(await _userRepository.IsAdminAsync(modifiedBy) || login.Equals(modifiedBy)))
            {
                return new Result<User>(new AccessIsDeniedException("access is denied"));

            }

            var user = await _userRepository.UpdateUserPasswordAsync(login, newPassword, modifiedBy);

            return new Result<User>(user);
        }

        
        private async Task<Result<User>> CheckUserAccessAsync(string targetLogin, string requestingUserLogin, bool requireAdminOnly = false, bool allowSelf = false)
        {
            var user = await _userRepository.GetUserByLoginAsync(targetLogin);
            if (user is null)
            {
                return new Result<User>(new UserDoesNotExistException($"User '{targetLogin}' does not exist"));
            }

            
            if (!await _userRepository.IsUserActiveAsync(targetLogin))
            {
                 return new Result<User>(new UserIsRevokedException($"User '{targetLogin}' is revoked"));
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
                return new Result<User>(new AccessIsDeniedException($"User '{requestingUserLogin}' does not have permission to access/modify user '{targetLogin}'"));
            }
            return new Result<User>(user);
        }
    }
}
