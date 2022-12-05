namespace LockToyApp.Repositories
{
    public interface IUserRepository
    {
        Task<DBEntities.User?> GetUserByNameAsync(string userName);

        Task<ICollection<DBEntities.UserRegistration>?> GetUserRegistrationsByUserName(string userName);


    }
}
