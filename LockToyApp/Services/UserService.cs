using LockToyApp.DAL;
using LockToyApp.DBEntities;
using Microsoft.EntityFrameworkCore;

namespace LockToyApp.Services
{
    public class UserService : IUserService
    {
        private readonly LockDBContext lockContext;
        private readonly IPasswordHasher passwordHasher;
        public UserService(LockDBContext lockContext, IPasswordHasher passwordHasher)
        {
            this.lockContext = lockContext;
            this.passwordHasher = passwordHasher;
        }
        public async Task<User> GetUserByName(string userName)
        {
         
            
                var foundUser = await this.lockContext.Users.Include(a => a.UserRegistrations).FirstOrDefaultAsync(u => u.UserName == userName);

                return foundUser;
  

        }

        public async Task<bool> IsUserValid(string userName, string password)
        {
        
            
                var foundUser = await this.lockContext.Users.FirstOrDefaultAsync<User>(u => u.UserName == userName);

                if (foundUser != null)
                {
                    return foundUser.Token == this.passwordHasher.Generate(password);
                }
               
                return false;
            
        }
    }
}
