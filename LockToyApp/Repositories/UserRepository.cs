using LockToyApp.DAL;
using LockToyApp.DBEntities;
using Microsoft.EntityFrameworkCore;

namespace LockToyApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly LockDBContext lockContext;

        public UserRepository(LockDBContext lockContext)
        {
            this.lockContext = lockContext;
        }

        public async Task<User?> GetUserByNameAsync(string userName)
        {
            var foundUser = await this.lockContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            return foundUser;

        }

        public async Task<ICollection<UserRegistration>?> GetUserRegistrationsByUserName(string userName)
        {
            var foundUser = await this.lockContext.Users.Include(a => a.UserRegistrations).FirstOrDefaultAsync(u => u.UserName == userName);
            return foundUser?.UserRegistrations;
        }
    }
}
