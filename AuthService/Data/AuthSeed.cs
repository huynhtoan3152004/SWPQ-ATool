using AuthService.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public static class AuthSeed
{
    public static async Task SeedAsync(AuthDbContext dbContext, IConfiguration configuration)
    {
        await dbContext.Database.EnsureCreatedAsync();

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""FullName"" character varying(150);
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Users"" ADD COLUMN IF NOT EXISTS ""RequestedRole"" text;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            UPDATE ""Users""
            SET ""FullName"" = COALESCE(""FullName"", split_part(""Email"", '@', 1));
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Users"" ALTER COLUMN ""FullName"" SET NOT NULL;
        ");

        var createDefaultAdmin = configuration.GetValue<bool>("Seed:CreateDefaultAdmin");
        if (createDefaultAdmin)
        {
            var email = (configuration["Seed:AdminEmail"] ?? "admin@swp.local").Trim().ToLowerInvariant();
            var fullName = (configuration["Seed:AdminFullName"] ?? "System Admin").Trim();
            var password = configuration["Seed:AdminPassword"] ?? "123456";

            var hasher = new PasswordHasher<User>();
            var existing = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (existing is null)
            {
                var newUser = CreateUser(fullName, email, password, UserRole.ADMIN, hasher, null);
                await dbContext.Users.AddAsync(newUser);
            }
            else
            {
                existing.FullName = fullName;
                existing.Role = UserRole.ADMIN;
                existing.RequestedRole = null;
                existing.PasswordHash = hasher.HashPassword(existing, password);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static User CreateUser(string fullName, string email, string password, UserRole role, PasswordHasher<User> hasher, UserRole? requestedRole = null)
    {
        var user = new User
        {
            FullName = fullName,
            Email = email,
            Role = role,
            RequestedRole = requestedRole
        };

        user.PasswordHash = hasher.HashPassword(user, password);
        return user;
    }
}
