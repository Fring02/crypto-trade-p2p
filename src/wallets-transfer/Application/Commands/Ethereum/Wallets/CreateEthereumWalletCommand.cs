using Application.Responses.Ethereum.Wallets;
using MediatR;

namespace Application.Commands.Ethereum.Wallets;
public record CreateEthereumWalletCommand(string Email, string Password) : IRequest<CreatedEthereumWalletResponse>;