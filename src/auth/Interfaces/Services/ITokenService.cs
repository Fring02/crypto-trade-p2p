using System.Security.Claims;

namespace AuthService.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccess(ICollection<Claim> claims);
    string GenerateRefresh();
    ClaimsPrincipal GetPrincipalFromToken(string token, bool isExpired = false);
}