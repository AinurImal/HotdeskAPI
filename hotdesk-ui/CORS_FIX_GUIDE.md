# 🔧 CORS Setup for HotdeskAPI

## ❌ **Current Error Analysis**

**Error Code 0** typically indicates a **CORS (Cross-Origin Resource Sharing)** issue. Your Angular app (running on `http://localhost:4200`) cannot access your HotdeskAPI due to browser security policies.

## 🛠️ **Solution: Configure CORS in your HotdeskAPI**

### **Step 1: Update your HotdeskAPI Program.cs**

Add this CORS configuration to your ASP.NET Core API:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// ✅ ADD CORS CONFIGURATION
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add other services (Swagger, etc.)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ USE CORS (Important: Must be before UseAuthorization)
app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
```

### **Step 2: Alternative - Allow All Origins (Development Only)**

For quick testing, you can use this more permissive setup:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Then use:
app.UseCors("AllowAll");
```

⚠️ **WARNING: Never use AllowAnyOrigin() in production!**

### **Step 3: Restart your HotdeskAPI**

After adding CORS configuration:
1. Stop your HotdeskAPI
2. Build and restart it
3. Test the Angular app again

## 🔍 **Troubleshooting Steps**

### **1. Find Your Running API**
Use the "Find API" button in the Angular app to scan common ports.

### **2. Check API Port**
Common ASP.NET Core ports:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Custom: `http://localhost:5251` (your current setting)
- Kestrel: `https://localhost:7000`, `https://localhost:7001`

### **3. Verify API is Running**
Open browser and go to: `http://localhost:5251/api/Users`
You should see JSON data, not a CORS error.

### **4. Check Browser Console**
Open Developer Tools (F12) and check for CORS-related errors.

## 🎯 **Quick Fix Commands**

Run these in your HotdeskAPI project directory:

```bash
# Stop the API
# Add CORS configuration to Program.cs
# Then restart:
dotnet run
```

## ✅ **Expected Result**

After configuring CORS, your Angular app should successfully:
- Load users from the API
- Create new users
- Edit existing users  
- Delete users

The error "Http failure response for http://localhost:5251/api/Users: 0 undefined" should disappear.

## 📞 **Still Having Issues?**

1. Use the "Find API" button to discover the correct port
2. Check if your API is actually running
3. Verify the API responds to direct browser requests
4. Ensure CORS middleware is added **before** UseAuthorization
5. Check browser console for detailed CORS error messages
