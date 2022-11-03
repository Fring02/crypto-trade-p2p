using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Configuration;
using AuthService.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services;

public class TokenService : ITokenService
{
    private readonly AuthenticationOptions _options;
    private readonly ILogger<TokenService> _logger;
    public TokenService(IOptions<AuthenticationOptions> options, ILogger<TokenService> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    public string GenerateAccess(ICollection<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(_options.AccessLifetimeInMinutes),
            signingCredentials: credentials
        );
        var userId = claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        _logger.LogInformation("Generated access token for user {UserId}", userId);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefresh()
    {
        var rnd = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(rnd);
        _logger.LogInformation("Generated refresh token");
        return Convert.ToBase64String(rnd);
    }


    public ClaimsPrincipal GetPrincipalFromToken(string token, bool isExpired = false)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, ValidateIssuer = false, ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)), ValidateLifetime = !isExpired
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        _logger.LogInformation(isExpired ? "Decoded expired access token" : "Decoded valid access token");
        return principal;
    }
}