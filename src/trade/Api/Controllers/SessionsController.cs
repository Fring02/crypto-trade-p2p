using Application.Commands;
using Application.Dtos;
using Application.Queries;
using Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly IMediator _mediator;
    public SessionsController(IMediator mediator) => _mediator = mediator;

    private async Task<IActionResult> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken token) where TRequest : IRequest<TResponse>?
    {
        try
        {
            var response = await _mediator.Send(request, token);
            if (response is null) return NotFound();
            return Ok(response);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    private async Task<IActionResult> SendAsync<TRequest>(TRequest request, CancellationToken token) where TRequest : IRequest
    {
        try
        {
            await _mediator.Send(request, token);
            return Ok();
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet, Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllSessionsAsync(CancellationToken token, int page = 0, int pageCount = 10)
        => await SendAsync<GetAllSessionsQuery, (int, ICollection<SessionDto>)>(new GetAllSessionsQuery(page, pageCount), token);
    
    [HttpGet("{id:long}"), Authorize(Roles = "user,admin")]
    public async Task<IActionResult> GetSessionByIdAsync(CancellationToken token, long id)
        => await SendAsync<GetSessionByIdQuery<long>, GetSessionByIdResponse?>(new GetSessionByIdQuery<long>(id), token);

    [HttpPost("begin"), Authorize(Roles = "user,admin")]
    public async Task<IActionResult> BeginSessionAsync(CancellationToken token, [FromBody] BeginSessionCommand command)
        => await SendAsync<BeginSessionCommand, GetSessionByIdResponse>(command, token);

    [HttpPatch("{id:long}/cancel"), Authorize(Roles = "user,admin")]
    public async Task<IActionResult> CancelSessionAsync(CancellationToken token, long id)
    {
        return await SendAsync(new CancelSessionCommand(id), token);
    }

    [HttpPatch("{id:long}/complete"), Authorize(Roles = "user,admin")]
    public async Task<IActionResult> CompleteSessionAsync(CancellationToken token, long id, [FromBody] CompleteSessionCommand command,
        [FromHeader(Name = "Authorization")] string access, [FromHeader(Name = "Refresh")] string refresh)
    {
        command.SessionId = id;
        command.AccessToken = access[7..];
        command.RefreshToken = refresh;
        return await SendAsync(command, token);
    }
        
}