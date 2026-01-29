using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public sealed class TokenService : ITokenService
{
    private readonly JwtOptions _options;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public string GenerateToken(Guid userId, string? email, IEnumerable<string> roles)
    {
        var key = _options.Key;
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Jwt:Key configuration is missing.");

        var keyBytes = Encoding.UTF8.GetBytes(key);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, email ?? string.Empty)
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes),
            SigningCredentials = credentials,
            Issuer = _options.Issuer,
            Audience = _options.Audience
        };

        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(securityToken);
    }
}