using Application.Commands.Ethereum.Wallets.Sell;
using Application.Handlers.Ethereum.Base;
using Domain.Interfaces.Wallets;
using MongoDB.Bson;

namespace Application.Handlers.Ethereum.Wallets.Sell;

public class IncreaseAmountToSellHandler : EthereumP2PWalletBaseHandler<IncreaseAmountToSellCommand, decimal>
{
    public IncreaseAmountToSellHandler(IEthereumP2PWalletsRepository<ObjectId> repository) : base(repository)
    {
    }

    public override async Task<decimal> Handle(IncreaseAmountToSellCommand request, CancellationToken cancellationToken)
    {
        var id = ObjectId.Parse(request.WalletId);
        if (!await Repository.ExistsAsync(w => w.Id == id, cancellationToken))
            throw new ArgumentException($"Wallet with id {request.WalletId} is not found");
        var amountToSell = await Repository.FindOneAsync(w => w.Id == id, wallet => wallet.EthToSell, cancellationToken);
        amountToSell += request.Amount;
        await Repository.UpdateAmountToSellAsync(id, amountToSell, cancellationToken);
        return amountToSell;
    }
}