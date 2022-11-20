
using Azure.Messaging.ServiceBus;
using LockToyApp.DAL;
using LockToyApp.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using ToyContracts;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserService, UserService>();

// Retrieve the app configuration store connection string
var appConfigConnectionString = builder.Configuration.GetConnectionString("AppConfig");
var connectionStrings = builder.Configuration.GetSection("LockToyApp:ConnectionStrings");
var appSettings = builder.Configuration.GetSection("LockToyApp:Settings");

// Get configurations from configuration store
builder.Configuration.AddAzureAppConfiguration(appConfigConnectionString);
// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ConnectionStrings>(connectionStrings);
builder.Services.Configure<Settings>(appSettings);

// get setting and connection string instances
var connectionStringItems = connectionStrings.Get<ConnectionStrings>();
var appSettingItems = appSettings.Get<Settings>();

// DI for sql db context
var sqlConnection = connectionStringItems.SqlConnectioniString;
builder.Services.AddDbContext<LockDBContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString(sqlConnection)));

// DI for door history service
var cosmosConnection = connectionStringItems.CosmosConnectionString;
builder.Services.AddSingleton(InitializeDoorHistoryDb(cosmosConnection));
static IDoorHistoryService InitializeDoorHistoryDb(string cosmosConnection)
{
    var cosmosClient = new CosmosClient(cosmosConnection);
    var doorHistoryService = new DoorHistoryService(cosmosClient, "DoorDB", "DoorHistory");
    var cosmosdb = cosmosClient.CreateDatabaseIfNotExistsAsync("DoorDB").GetAwaiter().GetResult();
    cosmosdb.Database.CreateContainerIfNotExistsAsync("DoorHistory","/id");
    return doorHistoryService;
}

// DI for service bus queue msg sender
var sbsenderConnection = connectionStringItems.SBSender;
var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };
var sbClient = new ServiceBusClient(sbsenderConnection, clientOptions);
var msgQueueName = appSettingItems.OperationQueueName;
var msgSender = sbClient.CreateSender(msgQueueName);
builder.Services.AddSingleton(msgSender);
builder.Services.AddSingleton<IDoorOperationSender, DoorOperationSender>();

var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<LockDBContext>();
        LockDBInitializer.IntializeDB(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
app.Run();