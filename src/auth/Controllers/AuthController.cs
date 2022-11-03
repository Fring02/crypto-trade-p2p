using AuthService.Configuration;
using AuthService.Dtos;
using AuthService.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthService.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    private readonly CookieOptions _cookieOptions;
    private readonly AuthenticationOptions _authenticationOptions;
    public AuthController(IAuthService service, CookieOptions cookieOptions, IOptions<AuthenticationOptions> options)
    {
        _service = service;
        _cookieOptions = cookieOptions;
        _authenticationOptions = options.Value;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken token)
    {
        (string access, string refresh) = await _service.LoginAsync(dto, token);
        _cookieOptions.Expires = DateTimeOffset.Now.AddMinutes(_authenticationOptions.AccessLifetimeInMinutes);
        Response.Cookies.Append("jwt-access", access, _cookieOptions);
        _cookieOptions.Expires = DateTimeOffset.Now.AddDays(_authenticationOptions.RefreshLifetimeInDays);
        Response.Cookies.Append("jwt-refresh", refresh, _cookieOptions);
        return Ok(new { accessToken = access, refreshToken = refresh });
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken token)
    {
        (string access, string refresh) = await _service.RegisterAsync(dto, token);
        _cookieOptions.Expires = DateTimeOffset.Now.AddMinutes(30);
        Response.Cookies.Append("jwt-access", access, _cookieOptions);
        _cookieOptions.Expires = DateTimeOffset.Now.AddDays(7);
        Response.Cookies.Append("jwt-refresh", refresh, _cookieOptions);
        return Ok(new { accessToken = access, refreshToken = refresh });
    }
    
    [HttpPatch("refresh"), Authorize]
    public async Task<IActionResult> Refresh([FromHeader(Name = "Authorization")] string accessToken, [FromHeader(Name = "Refresh")] string refreshToken,
        CancellationToken token)
    {
        accessToken = accessToken[7..];
        (string access, string refresh) = await _service.RefreshAsync(new(accessToken, refreshToken), token);
        _cookieOptions.Expires = DateTimeOffset.Now.AddMinutes(30);
        Response.Cookies.Append("jwt-access", access, _cookieOptions);
        _cookieOptions.Expires = DateTimeOffset.Now.AddDays(7);
        Response.Cookies.Append("jwt-refresh", refresh, _cookieOptions);
        return Ok(new { accessToken = access, refreshToken = refresh });
    }
    
    [HttpDelete("revoke"), Authorize]
    public async Task<IActionResult> Revoke([FromHeader(Name = "Authorization")] string accessToken,
        CancellationToken token)
    {
        accessToken = accessToken[7..];
        await _service.RevokeAsync(accessToken, token);
        Response.Cookies.Delete("jwt-access");
        Response.Cookies.Delete("jwt-refresh");
        return Ok("Revoke successful");
    }
}