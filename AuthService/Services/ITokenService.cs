using AuthService.Entities;

namespace AuthService.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
