using AuthService.Entities;

namespace AuthService.Models;

public record RegisterRequest(string Email, string Password);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string AccessToken, Guid UserId, string Email, UserRole Role);

public record UserResponse(Guid Id, string Email, UserRole Role);

public record ChangeUserRoleRequest(UserRole Role);
