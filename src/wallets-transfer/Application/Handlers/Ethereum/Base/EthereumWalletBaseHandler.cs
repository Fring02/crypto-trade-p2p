using Domain.Interfaces.Wallets;
using MediatR;
using MongoDB.Bson;

namespace Application.Handlers.Ethereum.Base;

public abstract class EthereumWalletBaseHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    protected readonly IEthereumWalletsRepository<ObjectId> Repository;
    protected EthereumWalletBaseHandler(IEthereumWalletsRepository<ObjectId> repository) => Repository = repository;
    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}