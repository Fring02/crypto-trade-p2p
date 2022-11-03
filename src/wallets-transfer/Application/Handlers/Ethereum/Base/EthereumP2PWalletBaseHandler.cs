using Domain.Interfaces.Wallets;
using MediatR;
using MongoDB.Bson;

namespace Application.Handlers.Ethereum.Base;

public abstract class EthereumP2PWalletBaseHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    protected readonly IEthereumP2PWalletsRepository<ObjectId> Repository;
    protected EthereumP2PWalletBaseHandler(IEthereumP2PWalletsRepository<ObjectId> repository) => Repository = repository;
    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}