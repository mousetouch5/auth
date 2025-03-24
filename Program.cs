// Program.cs (for .NET 6 and later)
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Identity;
using auth.Models;
using auth.Services;
using MongoDB.Driver;
using MySqlConnector;
using auth.Data;
using auth.Hubs;

var builder = WebApplication.CreateBuilder(args);

// In Program.cs
builder.Services.AddSignalR();

// At the end of your middleware configuration


// Configure MySQL database connection
var mysqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(mysqlConnectionString, Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(mysqlConnectionString)));

// Configure MongoDB services properly
builder.Services.AddSingleton<MongoClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("MongoDB")
                           ?? throw new ArgumentNullException("MongoDB connection string is missing!");

    return new MongoClient(connectionString);
});

builder.Services.AddSingleton<MongoDbService>();

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddHttpClient<ChatGPTService>();

// Configure Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Allow login without email confirmation
    options.Lockout.AllowedForNewUsers = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();
app.MapHub<ChatHub>("/chatHub");

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
