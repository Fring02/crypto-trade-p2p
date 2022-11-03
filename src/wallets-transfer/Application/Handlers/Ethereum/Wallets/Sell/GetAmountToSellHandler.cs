using Application.Handlers.Ethereum.Base;
using Application.Queries.Ethereum;
using Domain.Interfaces.Wallets;
using MongoDB.Bson;

namespace Application.Handlers.Ethereum.Wallets.Sell;

public class GetAmountToSellHandler : EthereumP2PWalletBaseHandler<GetAmountToSellQuery, decimal>
{
    public GetAmountToSellHandler(IEthereumP2PWalletsRepository<ObjectId> repository) : base(repository)
    {
    }

    public override async Task<decimal> Handle(GetAmountToSellQuery request, CancellationToken cancellationToken)
    {
        var id = ObjectId.Parse(request.WalletId);
        if (!await Repository.ExistsAsync(w => w.Id == id, cancellationToken))
            throw new ArgumentException($"Wallet with id {request.WalletId} is not found");
        return await Repository.FindOneAsync(w => w.Id == id, wallet => wallet.EthToSell, cancellationToken);
    }
}