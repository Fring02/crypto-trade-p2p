using Application.Commands;
using Application.Services;
using Data.Contexts;
using Domain.Enums;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers;

public class ConfirmSessionHandler : IRequestHandler<ConfirmSessionCommand>
{
    private readonly AppDbContext _context;
    private readonly SessionNotificationService _notificationService;
    private readonly ILogger<ConfirmSessionHandler> _logger;
    public ConfirmSessionHandler(AppDbContext context, SessionNotificationService notificationService, ILogger<ConfirmSessionHandler> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Unit> Handle(ConfirmSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions.FindAsync(new object?[] { request.SessionId }, cancellationToken: cancellationToken);
        if (session is null) throw new ArgumentException($"Session with id {request.SessionId} is not found");
        session.SessionStatus = SessionStatus.Pending;
        var message = new SessionMessage
        {
            LotId = session.LotId, RecipientEmail = session.Details.SellerEmail, SessionId = session.Id, 
            Text = $"User {session.Details.BuyerEmail} has confirmed payment for your lot with id {session.LotId}"
        };
        _logger.LogInformation("Setting session status as pending...");
        await _context.SaveChangesAsync(cancellationToken).ContinueWith(_ => _notificationService.PublishAsync(message), cancellationToken);
        return Unit.Value;
    }
}