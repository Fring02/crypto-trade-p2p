using System.Security.Claims;
using Application.Commands.Ethereum.Wallets;
using AuthService.Configuration;
using AuthService.Dtos;
using AuthService.Interfaces.Repositories;
using AuthService.Interfaces.Services;
using AuthService.Models;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
namespace AuthService.Services;
public class AuthService : IAuthService
{
    private readonly IUserRepository _repository;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IWalletProducerService _walletProducer;
    private readonly AuthenticationOptions _options;
    public AuthService(IUserRepository repository, ITokenService tokenService, IMapper mapper, IWalletProducerService walletProducer, IOptions<AuthenticationOptions> options)
    {
        _repository = repository; _tokenService = tokenService; _mapper = mapper;
        _walletProducer = walletProducer; _options = options.Value;
    }
    public async Task<TokensDto> LoginAsync(LoginDto dto, CancellationToken token = default)
    {
        var user = await _repository.GetByEmailAsync(dto.Email, token);
        if (user is null || !PasswordService.VerifyPassword(dto.Password, user)) throw new ArgumentException("Email or password is incorrect");
        user.RefreshToken = _tokenService.GenerateRefresh();
        user.RefreshExpires = DateTime.Now.AddDays(_options.RefreshLifetimeInDays);
        await _repository.UpdateAsync(user.Id, user, token);
        return new(_tokenService.GenerateAccess(GetClaims(user)), user.RefreshToken);
    }

    public async Task<TokensDto> RegisterAsync(RegisterDto dto, CancellationToken token = default)
    {
        if (await _repository.ExistsAsync(dto.Email, token))
            throw new ArgumentException("User with this email already exists");
        var user = _mapper.Map<User>(dto);
        PasswordService.HashPassword(dto.Password, user);
        user.RefreshToken = _tokenService.GenerateRefresh();
        user.RefreshExpires = DateTime.Now.AddDays(_options.RefreshLifetimeInDays);
        user = await _repository.CreateAsync(user, token);
        TokensDto tokens = new(_tokenService.GenerateAccess(GetClaims(user)), user.RefreshToken!);
        if(string.IsNullOrEmpty(dto.PrivateKey)) await _walletProducer.PublishAsync(new CreateEthereumWalletCommand(Email: dto.Email, Password: dto.Password));
        else await _walletProducer.PublishAsync(new LoadEthereumWalletCommand(email: dto.Email, password: dto.Password, privateKey: dto.PrivateKey));
        return tokens;
    }

    public async Task<TokensDto> RefreshAsync(TokensDto dto, CancellationToken token = default)
    {
        var principal = _tokenService.GetPrincipalFromToken(dto.Access, isExpired: true);
        if (principal is null) throw new SecurityTokenException("Can't validate access token");
        var user = await _repository.GetByEmailAsync(principal.Identity!.Name!, token);
        if (user is null || user.RefreshToken != dto.Refresh || user.RefreshExpires <= DateTime.Now)
            throw new SecurityTokenException("Can't validate access token");

        string access = _tokenService.GenerateAccess(principal.Claims.ToArray()), refresh = _tokenService.GenerateRefresh();
        user.RefreshToken = refresh;
        await _repository.UpdateAsync(user.Id, user, token);
        return new(access, refresh);
    }

    public async Task RevokeAsync(string accessToken, CancellationToken token = default)
    {
        var principal = _tokenService.GetPrincipalFromToken(accessToken);
        if (principal is null) throw new SecurityTokenException("Can't validate access token");
        var user = await _repository.GetByEmailAsync(principal.Identity!.Name!, token);
        if (user is null) throw new SecurityTokenException("Can't validate access token");
        user.RefreshToken = null;
        user.RefreshExpires = null;
        await _repository.UpdateAsync(user.Id, user, token);
    }

    private static ICollection<Claim> GetClaims(User user)
    {
        return new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
    }
}