using LockToyApp.DBEntities;

namespace LockToyApp.Services
{
    public interface IUserService
    {
        Task<User> GetUserByName(string userName );

        Task<Boolean> IsUserValid(string userName, string userToken);

    }
}
