using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Application.Commands;
using Application.Services;
using Data.Contexts;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Settings;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Application.Handlers;

public class CompleteSessionHandler : IRequestHandler<CompleteSessionCommand>
{
    private readonly AppDbContext _context;
    private readonly SessionNotificationService _notificationService;
    private readonly HttpClient _client;
    private readonly SessionSettings _settings;
    private readonly ILogger<CompleteSessionHandler> _logger;
    public CompleteSessionHandler(AppDbContext context, SessionNotificationService notificationService, HttpClient client, IOptions<SessionSettings> options, ILogger<CompleteSessionHandler> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _client = client;
        _logger = logger;
        _settings = options.Value;
    }
    public async Task<Unit> Handle(CompleteSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions.FindAsync(new object?[] { request.SessionId }, cancellationToken: cancellationToken);
        if (session is null) throw new ArgumentException($"Session with id {request.SessionId} is not found");
        session.SessionStatus = SessionStatus.Completed;
        await TransferAsync(request, session.Details.SellerEmail);
        _logger.LogInformation("Session completed");
        var message = new SessionMessage
        {
            LotId = session.LotId, RecipientEmail = session.Details.BuyerEmail, SessionId = session.Id, 
            Text = $"User {session.Details.SellerEmail} has confirmed payment for your lot with id {session.LotId}"
        };
        await _context.SaveChangesAsync(cancellationToken).ContinueWith(_ => _notificationService.PublishAsync(message), cancellationToken);
        return Unit.Value;
    }


    private async Task TransferAsync(CompleteSessionCommand command, string walletOwnerEmail)
    {
        var sellerWallet = await GetWalletByEmailAsync(command.AccessToken, command.RefreshToken, walletOwnerEmail);
        if (sellerWallet.Balance < command.Amount) throw new SessionBlockedException("Not enough balance on wallet");
        _logger.LogInformation("Retrieved wallet: {Wallet}", sellerWallet);
        using var request = new HttpRequestMessage();
        request.RequestUri = new Uri(_settings.TransferUrl);
        request.Headers.Authorization = AuthenticationHeaderValue.Parse("Bearer " + command.AccessToken);
        request.Headers.Add("Refresh", command.RefreshToken);
        request.Method = HttpMethod.Post;
        var transferMessage = new TransferMessage
        {
            WalletId = sellerWallet.Id, RecipientId = command.RecipientId, Amount = command.Amount
        };
        request.Content = new StringContent(JsonConvert.SerializeObject(transferMessage), Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        _logger.LogInformation("Successful transfer");
    }

    private async Task<WalletResponse> GetWalletByEmailAsync(string access, string refresh, string email)
    {
        using var request = new HttpRequestMessage();
        request.RequestUri = new Uri(_settings.WalletsUrl + "?email=" + email);
        request.Headers.Authorization = AuthenticationHeaderValue.Parse("Bearer " + access);
        request.Headers.Add("Refresh", refresh);
        request.Method = HttpMethod.Get;
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<WalletResponse>(await response.Content.ReadAsStringAsync());
    }
}