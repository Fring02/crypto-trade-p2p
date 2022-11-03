using AuthService.Dtos;

namespace AuthService.Interfaces.Services;

public interface IAuthService
{
    Task<TokensDto> LoginAsync(LoginDto dto, CancellationToken token = default);
    Task<TokensDto> RegisterAsync(RegisterDto dto, CancellationToken token = default);
    Task<TokensDto> RefreshAsync(TokensDto dto, CancellationToken token = default);
    Task RevokeAsync(string accessToken, CancellationToken token = default);
}