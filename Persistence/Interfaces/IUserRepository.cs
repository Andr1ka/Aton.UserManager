using Domain;
using Domain.Enums;

namespace Persistence.Interfaces
{
    public interface IUserRepository
    {

        Task<User> CreateUserAsync(
            string login,
            string password,
            string name,
            GenderType gender,
            DateTime birthday,
            bool isAdmin,
            string createdBy);

        Task<bool> UpdateUserNameAsync(string login, string newName, string modifiedBy);

        Task<bool> UpdateUserGenderAsync(string login, GenderType newGender, string modifiedBy);

        Task<bool> UpdateUserBirthdayAsync(string login, DateTime newBirthday, string modifiedBy);

        Task<bool> UpdateUserPasswordAsync(string login, string newPassword, string modifiedBy);

        Task<bool> UpdateUserLoginAsync(string login, string newLogin, string modifiedBy);

        Task<IEnumerable<User>> GetActiveUsersSortedByCreationAsync();

        Task<User?> GetUserByLoginAsync(string login);

        Task<User?> GetUserByCredentialsAsync(string login, string password);

        Task<IEnumerable<User>> GetUsersOlderThanAsync(int age);

        Task<bool> DeleteUserAsync(string login, bool softDelete, string revokedBy);

        Task<bool> RestoreUserAsync(string login, string modifiedBy);

        Task<bool> IsLoginAvailableAsync(string login);
        Task<bool> IsUserActiveAsync(string login);
        Task<bool> IsAdminAsync(string login);
    }
}
