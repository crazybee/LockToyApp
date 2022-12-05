using LockToyApp.Repositories;
using LockToyApp.Services;

namespace LockToyApp.Injection
{
    public static class DoorServiceProvider
    {
        public static void AddServices(IServiceCollection services)
        {     
            services.AddSingleton<IDoorOperationSender, DoorOperationSender>();
            services.AddScoped<IPasswordHasher, PasswordHasher>(); // because IOption is scoped
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}
