using System.Security.Claims;

public interface IJwtService
{
    string GenerateAccessToken(Guid userId, string phone, UserRole role);
    string GenerateRefreshToken();
    ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token);
}