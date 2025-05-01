using Domain;
using Domain.Enums;
using LanguageExt.Common;

namespace Services.Users
{
    public interface IUserService
    {
        Task<Result<User>> CreateUserAsync(
           string login,
           string password,
           string name,
           GenderType gender,
           DateTime birthday,
           bool isAdmin,
           string createdBy);

        Task<Result<User>> UpdateUserNameAsync(string login, string newName, string modifiedBy);

        Task<Result<User>> UpdateUserGenderAsync(string login, GenderType newGender, string modifiedBy);

        Task<Result<User>> UpdateUserBirthdayAsync(string login, DateTime newBirthday, string modifiedBy);

        Task<Result<User>> UpdateUserPasswordAsync(string login, string newPassword, string modifiedBy);

        Task<Result<User>> UpdateUserLoginAsync(string login, string newLogin, string modifiedBy);

        Task<Result<IEnumerable<User>>> GetActiveUsersSortedByCreationAsync(string requestedBy);

        Task<Result<User>> GetUserByLoginAsync(string login, string requestedBy);

        Task<Result<User>> GetUserByCredentialsAsync(string login, string password, string requestedBy);

        Task<Result<IEnumerable<User>>> GetUsersOlderThanAsync(int age, string requestedBy);

        Task DeleteUserAsync(string login, bool softDelete, string revokedBy);

        Task<Result<User>> RestoreUserAsync(string login, string modifiedBy);
    }
}
