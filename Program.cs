using BookCRUDApp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


var connectionString = builder.Configuration.GetConnectionString("BookContext")
                       ?? Environment.GetEnvironmentVariable("DATABASE_URL");

//if (connectionString != null && connectionString.Contains("postgres"))
//{
//    builder.Services.AddDbContext<BookContext>(options =>
//        options.UseNpgsql(connectionString));
//}
//else
//{
//    builder.Services.AddDbContext<BookContext>(options =>
//        options.UseSqlServer(connectionString ?? ""));
//}

// Updated code to handle Heroku-style DATABASE_URL for PostgreSQL
if (connectionString != null && connectionString.Contains("postgres"))
{
    // Convert DATABASE_URL to a Npgsql-compatible connection string
    var databaseUri = new Uri(connectionString);
    var userInfo = databaseUri.UserInfo.Split(':');

    var pgConn = $"Host={databaseUri.Host};Port={databaseUri.Port};Database={databaseUri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";

    builder.Services.AddDbContext<BookContext>(options =>
        options.UseNpgsql(pgConn));
}
else
{
    builder.Services.AddDbContext<BookContext>(options =>
        options.UseSqlServer(connectionString ?? ""));
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


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookContext>();
    db.Database.Migrate();
}

app.Run();