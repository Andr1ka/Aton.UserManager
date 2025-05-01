using Domain;
using Domain.Enums;
using LanguageExt.Common;

namespace Services.Users
{
    public class UserService : IUserService
    {
        public Task<Result<User?>> CreateUserAsync(string login, string password, string name, GenderType gender, DateTime birthday, bool isAdmin, string createdBy)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUserAsync(string login, bool softDelete, string revokedBy)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetActiveUsersSortedByCreationAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<User?>> GetUserByCredentialsAsync(string login, string password)
        {
            throw new NotImplementedException();
        }

        public Task<Result<User?>> GetUserByLoginAsync(string login)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<User>>> GetUsersOlderThanAsync(int age)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RestoreUserAsync(string login, string modifiedBy)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserBirthdayAsync(string login, DateTime newBirthday, string modifiedBy)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserGenderAsync(string login, GenderType newGender, string modifiedBy)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserLoginAsync(string login, string newLogin, string modifiedBy)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserNameAsync(string login, string newName, string modifiedBy)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserPasswordAsync(string login, string newPassword, string modifiedBy)
        {
            throw new NotImplementedException();
        }
    }
}
