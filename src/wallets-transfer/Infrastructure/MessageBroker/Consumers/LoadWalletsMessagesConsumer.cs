using Application.Commands.Ethereum.Wallets;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.MessageBroker.Consumers;

public class LoadWalletsMessagesConsumer : IConsumer<LoadEthereumWalletCommand>
{
    private readonly IMediator _mediator;
    private readonly ILogger<LoadWalletsMessagesConsumer> _logger;

    public LoadWalletsMessagesConsumer(IMediator mediator, ILogger<LoadWalletsMessagesConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LoadEthereumWalletCommand> context)
    {
        _logger.LogInformation("Received load wallet message: {Message}",  JsonConvert.SerializeObject(context.Message));
        await _mediator.Send(context.Message, context.CancellationToken);
    }
}