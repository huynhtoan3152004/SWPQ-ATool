using AuthService.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public static class AuthSeed
{
    public static async Task SeedAsync(AuthDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Users.AnyAsync())
        {
            return;
        }

        var hasher = new PasswordHasher<User>();

        var users = new[]
        {
            CreateUser("admin@swp.local", "123456", UserRole.ADMIN, hasher),
            CreateUser("student1@swp.local", "123456", UserRole.STUDENT, hasher),
            CreateUser("gvhd1@swp.local", "123456", UserRole.GVHD, hasher),
            CreateUser("teacher1@swp.local", "123456", UserRole.TEACHER, hasher)
        };

        await dbContext.Users.AddRangeAsync(users);
        await dbContext.SaveChangesAsync();
    }

    private static User CreateUser(string email, string password, UserRole role, PasswordHasher<User> hasher)
    {
        var user = new User
        {
            Email = email,
            Role = role
        };

        user.PasswordHash = hasher.HashPassword(user, password);
        return user;
    }
}
