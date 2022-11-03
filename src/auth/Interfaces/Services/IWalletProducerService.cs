using Application.Commands.Ethereum.Wallets;
namespace AuthService.Interfaces.Services;

public interface IWalletProducerService
{
    Task PublishAsync<TWalletCommand>(TWalletCommand command) where TWalletCommand : CreateEthereumWalletCommand;
}