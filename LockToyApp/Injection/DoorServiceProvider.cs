using Azure.Messaging.ServiceBus;
using LockToyApp.Repositories;
using LockToyApp.Services;
using Microsoft.Azure.Cosmos;

namespace LockToyApp.Injection
{
    public static class DoorServiceProvider
    {
        public static void AddServices(IServiceCollection services, ToyContracts.ConnectionStrings connectionStringItems, ToyContracts.Settings appSettingItems)
        {
            // DI for door history service
            var cosmosConnection = connectionStringItems.CosmosConnectionString;
            var cosmosDbClient = new CosmosClient(cosmosConnection);

            // Create cosmos db if not exist
            var cosmosdb = cosmosDbClient.CreateDatabaseIfNotExistsAsync("DoorDB").GetAwaiter().GetResult();
            cosmosdb.Database.CreateContainerIfNotExistsAsync("DoorHistory", "/id");

            services.AddSingleton<CosmosClient>(cosmosDbClient);
            services.AddSingleton<IDoorHistoryRepository, DoorHistoryRepository>();
            services.AddSingleton<IDoorHistoryService, DoorHistoryService>();

            // DI for service bus queue msg sender
            var sbsenderConnection = connectionStringItems.SBSender;
            var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };
            var sbClient = new ServiceBusClient(sbsenderConnection, clientOptions);
            var msgQueueName = appSettingItems.OperationQueueName;
            var msgSender = sbClient.CreateSender(msgQueueName);
            services.AddSingleton(msgSender);

            services.AddSingleton<IDoorOperationSender, DoorOperationSender>();
            services.AddScoped<IPasswordHasher, PasswordHasher>(); // because IOption is scoped
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddMemoryCache();
        }
    }
}
