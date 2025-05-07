using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Interfaces;

namespace Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> CreateUserAsync(string login, string password, string name, GenderType gender, DateTime? birthday, bool isAdmin, string? createdBy)
        {
            var user = new User
            {
                Login = login,
                Password = password,
                Name = name,
                Gender = gender,
                Birthday = birthday,
                Admin = isAdmin,
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task DeleteUserAsync(string login, bool softDelete, string revokedBy)
        {
            var user = await GetUserByLoginAsync(login);

            if (user is null) { return; }

            if (softDelete)
            {
                if (user.RevokedOn is not null) return;

                user.ModifiedOn = DateTime.UtcNow;
                user.ModifiedBy = revokedBy;
                user.RevokedOn = DateTime.UtcNow;
                user.RevokedBy = revokedBy;
            }
            else
            {
                _dbContext.Users.Remove(user);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetActiveUsersSortedByCreationAsync()
        {
            var usersList = await _dbContext.Users
                 .Where(u => u.RevokedOn == null)
                 .AsNoTracking()
                 .OrderBy(x => x.CreatedOn)
                 .ToListAsync();

            return usersList;
        }

        public async Task<User?> GetUserByCredentialsAsync(string login, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password && u.RevokedOn == null);

            return user;
        }

        public async Task<User?> GetUserByLoginAsync(string login)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);

            return user;
        }

        public async Task<IEnumerable<User>> GetUsersOlderThanAsync(int age)
        {
            var thresholdDate = DateTime.UtcNow.AddYears(-age);

            return await _dbContext.Users
                                   .Where(u => u.Birthday.HasValue && u.Birthday.Value.Date <= thresholdDate.Date && u.RevokedOn == null)
                                   .AsNoTracking()
                                   .ToListAsync();
        }

        public async Task<bool> IsAdminAsync(string login)
        {
            var user = await _dbContext.Users
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(u => u.Login == login);
            return user?.Admin ?? false;
        }

        public async Task<bool> IsLoginAvailableAsync(string login)
        {
            return !await _dbContext.Users.AnyAsync(u => u.Login == login);
        }

        public async Task<bool> IsUserActiveAsync(string login)
        {
            var user = await _dbContext.Users
                                      .AsNoTracking()
                                      .Select(u => new { u.Login, u.RevokedOn })
                                      .FirstOrDefaultAsync(u => u.Login == login);

            return user is not null && user.RevokedOn is null;
        }

        public async Task<User?> RestoreUserAsync(string login, string modifiedBy)
        {
            var user = await GetUserByLoginAsync(login);

            if (user is null || user.RevokedOn is null)
            {
                return null;
            }

            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = modifiedBy;
            user.RevokedBy = null;
            user.RevokedOn = null;

            await _dbContext.SaveChangesAsync();
            return user;
        }


        public async Task<User?> UpdateUserBirthdayAsync(string login, DateTime newBirthday, string modifiedBy)
        {
            return await UpdateUserAsync(login, modifiedBy, user => user.Birthday = newBirthday);
        }


        public async Task<User?> UpdateUserGenderAsync(string login, GenderType newGender, string modifiedBy)
        {
            return await UpdateUserAsync(login, modifiedBy, user => user.Gender = newGender);
        }


        public async Task<User?> UpdateUserLoginAsync(string login, string newLogin, string modifiedBy)
        {
            if (!await IsLoginAvailableAsync(newLogin))
            {
                return null;
            }
            return await UpdateUserAsync(login, modifiedBy, user => user.Login = newLogin);
        }


        public async Task<User?> UpdateUserNameAsync(string login, string newName, string modifiedBy)
        {
            return await UpdateUserAsync(login, modifiedBy, user => user.Name = newName);
        }


        public async Task<User?> UpdateUserPasswordAsync(string login, string newPassword, string modifiedBy)
        {
            return await UpdateUserAsync(login, modifiedBy, user => user.Password = newPassword);
        }


        private async Task<User?> UpdateUserAsync(string login, string modifiedBy, Action<User> updateAction)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (user is null || user.RevokedOn is not null)
            {
                return null;
            }

            updateAction(user);

            user.ModifiedOn = DateTime.UtcNow;
            user.ModifiedBy = modifiedBy;

            await _dbContext.SaveChangesAsync();
            return user;
        }
    }
}
