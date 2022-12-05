using LockToyApp.DBEntities;
using LockToyApp.Models;

namespace LockToyApp.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByName(string userName );

        Task<Boolean> IsUserValid(string userName, string userToken);

        Task<AuthenticationResponse?> Authenticate(AuthenticationRequest request);

        Task<ICollection<UserRegistration>?> GetUserRegistrations(string userName);
    }
}
