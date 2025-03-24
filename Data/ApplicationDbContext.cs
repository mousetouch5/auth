using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using auth.Models;

namespace auth.Data;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Friendship> Friendships { get; set; } // Add this line


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Seed Roles
        string adminRoleId = "admin-role-id"; // Fixed ID
        var roles = new List<IdentityRole>
    {
        new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" }
    };
        builder.Entity<IdentityRole>().HasData(roles);

        // Seed Admin User
        string adminUserId = "admin-user-id"; // Fixed ID
        var adminUser = new ApplicationUser
        {
            Id = adminUserId,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            EmailConfirmed = true
        };

        // Hash the password
        var hasher = new PasswordHasher<ApplicationUser>();
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

        builder.Entity<ApplicationUser>().HasData(adminUser);

        // Assign Role to Admin User
        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            UserId = adminUserId,
            RoleId = adminRoleId
        });

        // Log the model configuration to help diagnose issues
        Console.WriteLine("Model configuration completed.");
    }


}
