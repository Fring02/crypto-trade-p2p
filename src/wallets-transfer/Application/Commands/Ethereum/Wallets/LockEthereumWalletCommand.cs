using MediatR;

namespace Application.Commands.Ethereum.Wallets;

public record LockEthereumWalletCommand(string WalletId) : IRequest<bool>;