using Application.Commands;
using Application.Responses;
using Application.Services;
using Data.Contexts;
using Domain.Enums;
using Domain.Models;
using Domain.Settings;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Handlers;

public class BeginSessionHandler : IRequestHandler<BeginSessionCommand, GetSessionByIdResponse>
{
    private readonly AppDbContext _context;
    private readonly SessionManager _manager;
    private readonly SessionSettings _sessionSettings;
    private readonly ILogger<BeginSessionHandler> _logger;
    public BeginSessionHandler(AppDbContext context, SessionManager manager, SessionSettings sessionSettings, ILogger<BeginSessionHandler> logger)
    {
        _context = context;
        _manager = manager;
        _sessionSettings = sessionSettings;
        _logger = logger;
    }

    public async Task<GetSessionByIdResponse> Handle(BeginSessionCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Sessions.AnyAsync(s =>
                s.Details.BuyerEmail == request.BuyerEmail && s.Details.SellerEmail == request.SellerEmail, cancellationToken: cancellationToken))
            throw new ArgumentException("Session already exists");
        var session = new Session
        {
            SessionStatus = SessionStatus.Started, Details = new SessionDetails
            {
                BuyerEmail = request.BuyerEmail, SellerEmail = request.SellerEmail,
                Price = request.Price, CurrencyType = Enum.Parse<CryptoCurrency>(request.CurrencyType)
            }, ExpiresAt = DateTime.Now.AddMinutes(_sessionSettings.ExpirationInMinutes), LotId = request.LotId
        };
        _context.Sessions.Add(session);
        _logger.LogInformation("Starting new session: Buyer = {Buyer}, Seller = {Seller}, Price = {Price}, Currency = {Currency}",
            session.Details.BuyerEmail, session.Details.SellerEmail, session.Details.Price, Enum.GetName(session.Details.CurrencyType));
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Running aborting in background => {Exp} minutes left", _sessionSettings.ExpirationInMinutes);
        BackgroundJob.Schedule(() => _manager.AbortAsync(session.Id, cancellationToken), TimeSpan.FromMinutes(_sessionSettings.ExpirationInMinutes));
        return new GetSessionByIdResponse
        {
            StartedAt = session.StartsAt.ToString("F"), ExpiresAt = session.StartsAt.ToString("F"),
            BuyerEmail = session.Details.BuyerEmail, SellerEmail = session.Details.SellerEmail,
            CurrencyType = Enum.GetName(session.Details.CurrencyType)!,
            Id = session.Id, Price = session.Details.Price
        };
    }
}