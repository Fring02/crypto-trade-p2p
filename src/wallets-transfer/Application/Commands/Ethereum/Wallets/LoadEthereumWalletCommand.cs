using Application.Responses.Ethereum.Wallets;
using MediatR;

namespace Application.Commands.Ethereum.Wallets;

public record LoadEthereumWalletCommand(string Email, string Password, string PrivateKey) : IRequest<CreatedEthereumWalletResponse>;