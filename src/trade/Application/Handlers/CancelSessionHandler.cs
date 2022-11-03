using Application.Commands;
using Data.Contexts;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Handlers;

public class CancelSessionHandler : IRequestHandler<CancelSessionCommand>
{
    private readonly AppDbContext _context;
    private readonly ILogger<CancelSessionHandler> _logger;
    public CancelSessionHandler(AppDbContext context, ILogger<CancelSessionHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Unit> Handle(CancelSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions.Select(s => new {s.Id, s.SessionStatus})
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);
        if (session is null) throw new SessionNotFoundException();
        if (session.SessionStatus is SessionStatus.Completed or SessionStatus.Aborted)
            throw new SessionBlockedException("Session is either completed or aborted. Failed to cancel");
        _logger.LogInformation("Cancelling session with id {SessionId}", request.SessionId);
        _context.Sessions.Remove(new Session { Id = request.SessionId });
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}