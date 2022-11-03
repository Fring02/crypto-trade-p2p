using Application.Responses.Ethereum.Wallets;
using MediatR;

namespace Application.Queries.Ethereum.Wallets;

public record GetEthereumWalletByEmailQuery(string Email) : IRequest<EthereumWalletWithIdResponse>;