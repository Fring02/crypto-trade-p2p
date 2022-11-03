using Application.Commands.Ethereum.Wallets;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.MessageBroker.Consumers;

public class CreateWalletMessagesConsumer : IConsumer<CreateEthereumWalletCommand>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateWalletMessagesConsumer> _logger;
    public CreateWalletMessagesConsumer(IMediator mediator, ILogger<CreateWalletMessagesConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CreateEthereumWalletCommand> context)
    {
        _logger.LogInformation("Received create wallet message: {Message}",  JsonConvert.SerializeObject(context.Message));
        await _mediator.Send(context.Message, context.CancellationToken);
    }
}