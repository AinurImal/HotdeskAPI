using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HotdeskAPI.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HotdeskAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HotdeskAPIContext") ?? throw new InvalidOperationException("Connection string 'HotdeskAPIContext' not found.")));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Auto-create database and apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HotdeskAPIContext>();
    try
    {
        // Ensure database is created and migrations are applied
        context.Database.EnsureCreated();
        
        // Apply any pending migrations
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        // Log the error but don't stop the application
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating/migrating the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
