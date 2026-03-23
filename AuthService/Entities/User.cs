namespace AuthService.Entities;

public enum UserRole
{
    ADMIN = 0,
    STUDENT = 1,
    GVHD = 2,
    TEACHER = 3
}

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
