using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using Shared.Interfaces.Services;
namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "user,admin")]
public class LotsController : ControllerBase
{
    private readonly ILotsService _service;
    private readonly IMapper _mapper;
    private readonly IAuthorizationService _authorizationService;
    public LotsController(ILotsService service, IMapper mapper, IAuthorizationService authorizationService)
    {
        _service = service;
        _mapper = mapper;
        _authorizationService = authorizationService;
    }

    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> GetLotsAsync([FromQuery] LotFilterDto? filter, CancellationToken token,
        int page = 0, int pageCount = 10)
    {
        var (count, data) = await _service.GetAllAsync(filter, page, pageCount, token);
        if (count == 0) return NoContent();
        return Ok(new { count, data });
    }

    [HttpGet("{id:long}"), AllowAnonymous]
    public async Task<LotViewDto?> GetLotByIdAsync(CancellationToken token, long id)
    {
        var accessToken = Request.Cookies["jwt-access"];
        var lot = await _service.GetByIdAsync(id, accessToken, token);
        return lot;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLotAsync([FromBody] LotCreateDto dto, CancellationToken token)
    {
        var result = await _authorizationService.AuthorizeAsync(User, dto.OwnerEmail, "CreateLotOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        var entity = await _service.CreateAsync(dto, token);
        var data = _mapper.Map<LotCreateDto>(entity);
        return Created(HttpContext.Request.Path, new {data, id = entity.Id});
    }

    [HttpPatch("{id:long}")]
    public async Task<IActionResult> UpdateLotAsync([FromBody] LotUpdateDto dto, long id, CancellationToken token)
    {
        if (!await _service.ExistsAsync(l => l.Id == id, token)) return NoContent();
        var result = await _authorizationService.AuthorizeAsync(User, id, "CheckLotOwnerPolicy");
        if (!result.Succeeded) return Forbid();
        dto.Id = id;
        var lot = await _service.UpdateAsync(dto, token);
        var data = _mapper.Map<LotCreateDto>(lot);
        return Ok(new{ data, id });
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteLotAsync(long id, CancellationToken token)
    {
        if (!await _service.ExistsAsync(l => l.Id == id, token)) return NotFound("This lot doesn't exist");
        var result = await _authorizationService.AuthorizeAsync(User, id, "CheckLotOwnerPolicy");
        if (!result.Succeeded) return Forbid();
       await _service.DeleteAsync(id, token);
       return Ok();
    }
}