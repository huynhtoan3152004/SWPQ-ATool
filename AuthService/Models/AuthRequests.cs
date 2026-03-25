using AuthService.Entities;

namespace AuthService.Models;

public record RegisterRequest(string Email, string Password, string? FullName = null);

public record RegisterLecturerRequest(string FullName, string Email, string Password);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string AccessToken, Guid UserId, string FullName, string Email, UserRole Role, UserRole? RequestedRole);

public record UserResponse(Guid Id, string FullName, string Email, UserRole Role, UserRole? RequestedRole);

public record PendingRoleRequestResponse(Guid UserId, string FullName, string Email, UserRole CurrentRole, UserRole RequestedRole);

public record ChangeUserRoleRequest(UserRole Role);

public record AdminCreateUserRequest(string FullName, string Email, string Password, UserRole Role);
