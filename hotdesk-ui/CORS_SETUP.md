# CORS Configuration for HotdeskAPI

If you encounter CORS errors when connecting from Angular, add this to your ASP.NET Core API:

## Program.cs (or Startup.cs for older versions):

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:4200") // Angular dev server
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
```

## For Production:
Replace `"http://localhost:4200"` with your production Angular app URL.

## Alternative - Allow All Origins (Development Only):
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Then use: app.UseCors("AllowAll");
```

⚠️ **Never use AllowAnyOrigin() in production!**
