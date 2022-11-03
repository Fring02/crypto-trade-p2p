using Application.Responses.Ethereum.Wallets;
using MediatR;

namespace Application.Queries.Ethereum.Wallets.P2P;

public record GetEthereumP2PWalletByIdQuery(string WalletId) : IRequest<EthereumP2PWalletResponse>;