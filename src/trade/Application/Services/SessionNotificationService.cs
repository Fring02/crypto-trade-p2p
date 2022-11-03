using Domain.Models;
using Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Application.Services;

public class SessionNotificationService
{
    private readonly IDatabase _database;
    private readonly SessionSettings _settings;
    private readonly ILogger<SessionNotificationService> _logger;
    public SessionNotificationService(IConnectionMultiplexer multiplexer, IOptions<SessionSettings> options, ILogger<SessionNotificationService> logger)
    {
        _logger = logger;
        _database = multiplexer.GetDatabase();
        _settings = options.Value;
    }

    public async Task PublishAsync(SessionMessage message)
    {
        _logger.LogInformation("Publishing notification: {Message}", message);
        await _database.StreamAddAsync(_settings.SessionStreamName, message.RecipientEmail, message.Text,
            messageId: $"{message.SessionId}-{message.LotId}");
    }
}