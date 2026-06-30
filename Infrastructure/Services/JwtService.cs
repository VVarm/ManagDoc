using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));
        _configuration = configuration;
    }

    public string GenerateAccessToken(Guid userId, string phone, UserRole role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.MobilePhone, phone),
            new Claim(ClaimTypes.Role, role.ToString())
        };

        string secretKey = _configuration.GetValue<string>("JwtSettings:SecretKey");
        if (string.IsNullOrEmpty(secretKey))
            throw new ArgumentException("SecretKey is null");
        if (secretKey.Length < 32)
            throw new ArgumentException("Secret key must be at least 32 characters long.");
        var issuer = _configuration["JwtSettings:Issuer"];
        if (string.IsNullOrEmpty(issuer))
            throw new ArgumentException("Issuer is null");
        var audience = _configuration["JwtSettings:Audience"];
        if (string.IsNullOrEmpty(audience))
            throw new ArgumentException("Audience is null");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        double expiryMinutes = _configuration.GetValue<double?>("JwtSettings:AccessTokenExpiryMinutes") ?? 15;
        JwtSecurityToken token = new JwtSecurityTokenHandler().CreateJwtSecurityToken(
            issuer: issuer,
            audience: audience,
            subject: new ClaimsIdentity(claims),
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
    
    public ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token)
    {
        string secretKey = _configuration.GetValue<string>("JwtSettings:SecretKey");
        if (string.IsNullOrEmpty(secretKey))
            throw new ArgumentException("SecretKey is null");
        if (secretKey.Length < 32)
            throw new ArgumentException("Secret key must be at least 32 characters long.");
        var issuer = _configuration["JwtSettings:Issuer"];
        if (string.IsNullOrEmpty(issuer))
            throw new ArgumentException("Issuer is null");
        var audience = _configuration["JwtSettings:Audience"];
        if (string.IsNullOrEmpty(audience))
            throw new ArgumentException("Audience is null");
        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
        return new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out _);
    }
}
        