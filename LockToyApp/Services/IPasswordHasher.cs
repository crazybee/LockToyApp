namespace LockToyApp.Services
{
    public interface IPasswordHasher
    {
        string Generate(string password);
    }
}
