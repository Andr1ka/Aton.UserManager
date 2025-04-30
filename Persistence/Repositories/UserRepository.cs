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

        public async Task<User> CreateUserAsync(string login, string password, string name, GenderType gender, DateTime birthday, bool isAdmin, string createdBy)
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

        public async Task<bool> DeleteUserAsync(string login, bool softDelete, string revokedBy)
        {
            var user = await GetUserByLoginAsync(login);

            if (user is null) {return false;}

            if (softDelete)
            {
                user.ModifiedOn = DateTime.UtcNow;
                user.RevokedOn = DateTime.UtcNow;
                user.RevokedBy = revokedBy;

                await _dbContext.SaveChangesAsync();

                return true;
            }
            else
            {

                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();

                return true;
            }
        }

        public async Task<IEnumerable<User>> GetActiveUsersSortedByCreationAsync()
        {
            var usersList = await _dbContext.Users
                 .Where(u => u.RevokedOn != null)
                 .AsNoTracking()
                 .OrderBy(x => x.CreatedOn)
                 .ToListAsync();

            return usersList;
        }

        public async Task<User?> GetUserByCredentialsAsync(string login, string password)
        {
            return await GetUserByLoginAsync(login);
        }

        public async Task<User?> GetUserByLoginAsync(string login)
        {
            var user = await _dbContext.Users.FindAsync(login);

            return user;
        }

        public Task<IEnumerable<User>> GetUsersOlderThanAsync(int age)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsAdminAsync(string login)
        {
            var user = await GetUserByLoginAsync(login);

            if (user is null) { return false; }

            return user.Admin;
        }

        public async Task<bool> IsLoginAvailableAsync(string login)
        {
            var user = await _dbContext.Users.FindAsync(login);

            if (user is null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> IsUserActiveAsync(string login)
        {
            var user = await GetUserByLoginAsync(login);

            if(user is null ) { return false; }

            if(user.RevokedOn is not null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RestoreUserAsync(string login, string modifiedBy)
        {
            var user = await GetUserByLoginAsync(login);

            if (user is not null) 
            {
                user.ModifiedOn = DateTime.Now;
                user.ModifiedBy = modifiedBy;

                user.RevokedBy = null;
                user.RevokedOn = null;


                await _dbContext.SaveChangesAsync();

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserBirthdayAsync(string login, DateTime newBirthday, string modifiedBy)
        {
            var user = await GetUserByLoginAsync(login);

            if (user is not null) 
            {
                user.ModifiedOn = DateTime.Now;
                user.ModifiedBy = modifiedBy;

                user.Birthday = newBirthday;

                await _dbContext.SaveChangesAsync();

                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> UpdateUserGenderAsync(string login, GenderType newGender, string modifiedBy)
        {
            var user = await GetUserByLoginAsync(login);

            if (user is not null)
            {
                user.ModifiedOn = DateTime.Now;
                user.ModifiedBy = modifiedBy;

                user.Gender = newGender;

                await _dbContext.SaveChangesAsync();

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserLoginAsync(string login, string newLogin, string modifiedBy)
        {
            var user = await GetUserByLoginAsync(login);

            if (user is not null)
            {
                user.ModifiedOn = DateTime.Now;
                user.ModifiedBy = modifiedBy;

                user.Login = newLogin;

                await _dbContext.SaveChangesAsync();

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserNameAsync(string login, string newName, string modifiedBy)
        {
            var user = await GetUserByLoginAsync(login);

            if (user is not null)
            {
                user.ModifiedOn = DateTime.Now;
                user.ModifiedBy = modifiedBy;

                user.Name = newName;

                await _dbContext.SaveChangesAsync();

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserPasswordAsync(string login, string newPassword, string modifiedBy)
        {
            var user = await GetUserByLoginAsync(login);

            if (user is not null && user.RevokedOn is null)
            {
                user.ModifiedOn = DateTime.Now;
                user.ModifiedBy = modifiedBy;

                user.Password = newPassword;

                await _dbContext.SaveChangesAsync();

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
