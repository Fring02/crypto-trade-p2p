using Application.Commands.Ethereum.Wallets;
using AuthService.Interfaces.Services;
using MassTransit;

namespace AuthService.Services;

public class WalletsProducerService : IWalletProducerService
{
    private readonly IPublishEndpoint _publisher;
    private readonly ILogger<WalletsProducerService> _logger;
    public WalletsProducerService(IPublishEndpoint publisher, ILogger<WalletsProducerService> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }
    public async Task PublishAsync<TWalletCommand>(TWalletCommand command) where TWalletCommand : CreateEthereumWalletCommand
    {
        await _publisher.Publish(command);
        _logger.LogInformation(command is LoadEthereumWalletCommand ? "Published loading wallet message..." : "Published creating wallet message...");
    }
}