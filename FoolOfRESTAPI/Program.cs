using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

ApiKey.Generate();

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("Docker");

if (connectionString != null)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(connectionString);
        options.UseLowerCaseNamingConvention();
    });
}

var app = builder.Build();

if (connectionString != null)
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

if (Environment.GetEnvironmentVariable("DISABLE_PYTHON_LOGGER") != "true")
{
    var pythonProcess = new PythonLogger();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run("http://+:5001");
