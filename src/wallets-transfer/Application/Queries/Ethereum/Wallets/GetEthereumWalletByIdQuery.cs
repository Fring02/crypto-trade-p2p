using Application.Responses.Ethereum.Wallets;
using MediatR;

namespace Application.Queries.Ethereum.Wallets;

public record GetEthereumWalletByIdQuery(string Id) : IRequest<EthereumWalletResponse>;