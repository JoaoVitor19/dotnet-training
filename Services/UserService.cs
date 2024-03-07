using dotnet_training.Data;
using dotnet_training.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace dotnet_training.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> Create(User user)
        {
            //**Hash options in Utils for hasing password in one-way method**\\
            //
            //  user.Password = Helpers.Utils.HashPassword(user.Password);
            //
            var newUser = new User(user.Username, user.Password, user.Email, user.Role, false, DateTime.UtcNow);

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            newUser.Password = null;

            return newUser;
        }

        public async Task<User> FindByIdAsync(long Id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<User> FindByEmailAsync(string Email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(Email.ToLower()));
        }

        public async Task<User> FindByUsernameAsync(string Username)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username.Equals(Username));
        }

        public async Task<List<User>> FindAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> UpdateEmailAsync(User user, string newEmail)
        {
            user.Email = newEmail;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdatePasswordAsync(User user, string newPassword)
        {
            //**Hash options in Utils for hasing password in one-way method**\\
            //
            //  user.Password = Helpers.Utils.HashPassword(newPassword);
            //
            user.Password = newPassword;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUsernameAsync(User user, string newUsername)
        {
            user.Username = newUsername;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateRoleAsync(User user, string Role)
        {
            user.Role = Role;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateAsync(User user, User updateUser)
        {
            //**Hash options in Utils for hasing password in one-way method**\\
            //
            //  user.Password = Helpers.Utils.HashPassword(updateUser.Password);
            //

            user.Username = updateUser.Username;
            user.Password = updateUser.Password;
            user.Email = updateUser.Email;
            user.Role = updateUser.Role;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return updateUser;
        }

        public async Task<User> DeteleAsync(User user)
        {
            user.IsDeleted = true;
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<List<User>> FindAllRootUsers()
        {
            return await _context.Users.Where(x => x.Role.Equals("root")).ToListAsync();
        }
    }
}
