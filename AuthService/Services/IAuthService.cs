using AuthService.Entities;
using AuthService.Models;

namespace AuthService.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<List<UserResponse>> GetStudentsAsync(CancellationToken cancellationToken = default);
    Task<List<UserResponse>> GetTeachersAsync(CancellationToken cancellationToken = default);
    Task<UserResponse?> ChangeUserRoleAsync(Guid userId, UserRole role, CancellationToken cancellationToken = default);
}
