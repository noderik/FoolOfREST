using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

ApiKey.Generate();

builder.Services.AddOpenApi();
builder.Services.AddControllers();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("Docker"));
        options.UseLowerCaseNamingConvention();
    });
}

var app = builder.Build();

var pythonProcess = new PythonLogger();
app.UseHttpsRedirection();

app.MapControllers();

app.Run("http://localhost:5001");
