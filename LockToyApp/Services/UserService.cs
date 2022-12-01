using LockToyApp.DAL;
using LockToyApp.DBEntities;
using LockToyApp.Helpers;
using LockToyApp.Models;
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

            var foundUser = this.lockContext.Users.FirstOrDefault<User>(u => u.UserName == userName);

            if (foundUser != null)
            {
                return foundUser.Token == this.passwordHasher.Generate(password);
            }

            return false;

        }

        public async Task<AuthenticationResponse?> Authenticate(AuthenticationRequest model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return null;

            }
            var calculatedHash = this.passwordHasher.Generate(model.Password);

            try
            {
                var user = await this.lockContext.Users.FirstOrDefaultAsync<User>(x => x.UserName == model.Username && x.Token == calculatedHash);

                if (user == null)
                {
                    return null;
                }

                var userToReturn = new ToyContracts.User()
                {
                    UserName = user.UserName,
                    Id = user.UserID
                };

                // generate jwt token
                var token = TokenHelper.GenerateJwtToken(userToReturn);

                return new AuthenticationResponse(userToReturn, token);
            }

            catch (Exception ex)
            {

                // TODO:log exception if happens
                return null;
            }

        }
    }
}
