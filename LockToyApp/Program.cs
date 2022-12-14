using LockToyApp.DAL;
using LockToyApp.Helpers;
using LockToyApp.Injection;
using Microsoft.EntityFrameworkCore;
using ToyContracts;

var builder = WebApplication.CreateBuilder(args);

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

// inject services
DoorServiceProvider.AddServices(builder.Services, connectionStringItems, appSettingItems);

var app = builder.Build();
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();
// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();

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
