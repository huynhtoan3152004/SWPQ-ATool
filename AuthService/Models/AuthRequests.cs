using AuthService.Entities;

namespace AuthService.Models;

public record RegisterRequest(string Email, string Password, UserRole Role);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string AccessToken, Guid UserId, string Email, UserRole Role);
