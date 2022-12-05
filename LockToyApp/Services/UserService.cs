using LockToyApp.DBEntities;
using LockToyApp.Helpers;
using LockToyApp.Models;
using LockToyApp.Repositories;

namespace LockToyApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;
        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
        }
        public async Task<User?> GetUserByName(string userName)
        {
            var foundUser = await userRepository.GetUserByNameAsync(userName);

            return foundUser ?? null;
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

    }
}
