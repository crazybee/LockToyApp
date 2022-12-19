using LockToyApp.DBEntities;
using LockToyApp.Helpers;
using LockToyApp.Models;
using LockToyApp.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace LockToyApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;
        private readonly IMemoryCache memoryCache;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IMemoryCache memoryCache)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
            this.memoryCache = memoryCache;
        }
        public async Task<User?> GetUserByName(string userName)
        {
            var foundUser = await userRepository.GetUserByNameAsync(userName);

            return foundUser ?? null;
        }

        public async Task<User?> GetUserByNameFromCache(string userName)
        {
            User? userOutput;
            userOutput = memoryCache.Get<User>("userByName");
            if (userOutput == null)
            {
                userOutput = await this.GetUserByName(userName);
                this.memoryCache.Set("userByName", userOutput, TimeSpan.FromMinutes(60));
            }

            return userOutput;
        }
        public async Task<bool> IsUserValid(string userName, string password)
        {

            var foundUser = await this.userRepository.GetUserByNameAsync(userName);

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
                var user = await this.userRepository.GetUserByNameAsync(model.Username);

                if (user == null || user.Token != calculatedHash)
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

                // TODO:log exception if happens in application insight
                return null;
            }

        }

        public async Task<ICollection<UserRegistration>?> GetUserRegistrations(string userName)
        {
            var userRegistrations = await this.userRepository.GetUserRegistrationsByUserName(userName);
            return userRegistrations;
        }

        public async Task<ICollection<UserRegistration>?> GetUserRegistrationsFromCache(string userName)
        {
            ICollection<UserRegistration>? userRegistrationOutput;

            userRegistrationOutput = memoryCache.Get<ICollection<UserRegistration>>("userregistration");

            if (userRegistrationOutput == null)
            {
                userRegistrationOutput = await this.GetUserRegistrations(userName);
                memoryCache.Set("userregistration", userRegistrationOutput, TimeSpan.FromMinutes(5));
            }


            return userRegistrationOutput;
        }
    }
}
