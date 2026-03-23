using AuthService.Entities;
using AuthService.Models;
using AuthService.Repositories;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidOperationException("Email and password are required.");
        }

        var email = request.Email.Trim().ToLowerInvariant();
        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var user = new User
        {
            Email = email,
            Role = UserRole.STUDENT
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        await _userRepository.AddAsync(user, cancellationToken);

        var token = _tokenService.GenerateToken(user);
        return new AuthResponse(token, user.Id, user.Email, user.Role);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var token = _tokenService.GenerateToken(user);
        return new AuthResponse(token, user.Id, user.Email, user.Role);
    }

    public async Task<List<UserResponse>> GetStudentsAsync(CancellationToken cancellationToken = default)
    {
        var students = await _userRepository.GetByRoleAsync(UserRole.STUDENT, cancellationToken);
        return students.Select(MapToUserResponse).ToList();
    }

    public async Task<List<UserResponse>> GetTeachersAsync(CancellationToken cancellationToken = default)
    {
        var teachers = await _userRepository.GetByRoleAsync(UserRole.TEACHER, cancellationToken);
        return teachers.Select(MapToUserResponse).ToList();
    }

    public async Task<UserResponse?> ChangeUserRoleAsync(Guid userId, UserRole role, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        user.Role = role;
        await _userRepository.SaveChangesAsync(cancellationToken);

        return MapToUserResponse(user);
    }

    private static UserResponse MapToUserResponse(User user)
    {
        return new UserResponse(user.Id, user.Email, user.Role);
    }
}
