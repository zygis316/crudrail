using BookCRUDApp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Get connection string from Railway or config file
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrEmpty(connectionString))
{
    // Local development - use appsettings.json
    connectionString = builder.Configuration.GetConnectionString("BookCRUDAppContext");
}

// Configure database based on connection string type
if (!string.IsNullOrEmpty(connectionString))
{
    if (connectionString.Contains("postgres") || connectionString.StartsWith("postgresql://"))
    {
        // Railway PostgreSQL - convert connection string format
        var databaseUri = new Uri(connectionString);
        var userInfo = databaseUri.UserInfo.Split(':');

        var npgsqlConnection = $"Host={databaseUri.Host};" +
                             $"Port={databaseUri.Port};" +
                             $"Username={userInfo[0]};" +
                             $"Password={userInfo[1]};" +
                             $"Database={databaseUri.LocalPath.TrimStart('/')};" +
                             $"SSL Mode=Require;" +
                             $"Trust Server Certificate=true";

        builder.Services.AddDbContext<BookContext>(options =>
            options.UseNpgsql(npgsqlConnection));
    }
    else
    {
        // Local SQL Server
        builder.Services.AddDbContext<BookContext>(options =>
            options.UseSqlServer(connectionString));
    }
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run migrations automatically on startup
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<BookContext>();
        db.Database.Migrate();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Migration error: {ex.Message}");
}

app.Run();