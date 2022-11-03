using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Shared.Interfaces.Services;
namespace Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "user,admin")]
public class RequisitesController : ControllerBase
{
    private readonly IRequisiteService _service;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<RequisitesController> _logger;
    public RequisitesController(IRequisiteService service, IAuthorizationService authorizationService, ILogger<RequisitesController> logger)
    {
        _service = service;
        _authorizationService = authorizationService;
        _logger = logger;
    }
    
    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> GetAllRequisitesAsync(CancellationToken token, int page = 0, int pageCount = 10, Guid? userId = null)
    {
        int count;
        IReadOnlyCollection<RequisiteItemDto>? data;
        if (userId != null)
        {
            var result = await VerifySameUserAsync(userId.Value);
            if (result != null) return result;
            (count, data) =
                await _service.GetByUserIdAsync(userId: userId.Value, page: page, pageCount: pageCount, token: token);
        }
        else
             (count, data) = await _service.GetAllAsync<RequisiteItemDto>(page: page, pageCount: pageCount, token: token);
        return Ok(new { count, data });
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetRequisiteByIdAsync(CancellationToken token, long id)
    {
        var result = await VerifyRequisiteByUserAsync(id);
        if (result != null) return result;
        var requisite = await _service.GetAsync<RequisiteViewDto>(r => r.Id == id, token);
        if (requisite is null) return NoContent();
        return Ok(requisite);
    }

    [HttpPost]
    public async Task<IActionResult> AddRequisiteAsync(CancellationToken token, [FromBody] RequisiteCreateDto dto)
    {
        var result = await VerifySameUserAsync(dto.UserId);
        if (result != null) return result;
        await _service.CreateAsync(dto, token);
        return Ok();
    }

    [HttpPatch("{id:long}")]
    public async Task<IActionResult> UpdateRequisiteAsync(CancellationToken token, [FromBody] RequisiteUpdateDto dto, long id)
    {
        var result = await VerifyRequisiteByUserAsync(id);
        if (result != null) return result;
        dto.Id = id;
        await _service.UpdateAsync(dto, token);
        return Ok();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteRequisiteAsync(CancellationToken token, long id)
    {
        var result = await VerifyRequisiteByUserAsync(id);
        if (result != null) return result;
        await _service.DeleteByIdAsync(id, token);
        return Ok();
    }
    
    private async Task<IActionResult?> VerifyRequisiteByUserAsync(long requisiteId)
    {
        var result = await _authorizationService.AuthorizeAsync(User, requisiteId, "RequisiteUserPolicy");
        if (result.Succeeded) return null;
        if (User.Identity!.IsAuthenticated)
            return Forbid();
        return Unauthorized();
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