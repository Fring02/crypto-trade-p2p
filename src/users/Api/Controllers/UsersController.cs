using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Shared.Interfaces.Services;

namespace Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
    private readonly IAuthorizationService _authorizationService;
    public UsersController(IUserService service, IAuthorizationService authorizationService)
    {
        _service = service;
        _authorizationService = authorizationService;
    }

    [HttpGet, Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllUsersAsync(CancellationToken token, int pageCount = 10, int page = 0)
    {
        var (count, data) = await _service.GetAllAsync<UserItemDto>(page: page, pageCount: pageCount, token: token);
        if (count == 0) return NoContent();
        return Ok(new { count, data });
    }

    [HttpGet("{id:guid}"), Authorize(Roles = "user,admin")]
    public async Task<IActionResult> GetUserByIdAsync(Guid id, CancellationToken token)
    {
        var result = await VerifySameUserAsync(id);
        if (result != null) return result;
        var user = await _service.GetAsync<UserViewDto>(u => u.Id == id, token);
        if (user is null) return NoContent();
        return Ok(user);
    }

    [HttpPatch("{id:guid}"), Authorize(Roles = "user,admin")]
    public async Task<IActionResult> UpdateUserAsync(Guid id, [FromBody] UserUpdateDto dto, CancellationToken token)
    {
        var result = await VerifySameUserAsync(id);
        if (result != null) return result;
        dto.Id = id;
        await _service.UpdateAsync(dto, token);
        return Ok("Updated");
    }

    [HttpDelete("{id:guid}"), Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteUserAsync(Guid id, CancellationToken token)
    {
        await _service.DeleteByIdAsync(id, token);
        return Ok("Deleted");
    }

    private async Task<IActionResult?> VerifySameUserAsync(Guid userId)
    {
        var result = await _authorizationService.AuthorizeAsync(User, userId, "SameUserPolicy");
        if (result.Succeeded) return null;
        if (User.Identity!.IsAuthenticated)
            return Forbid();
        return Unauthorized();
    }
}