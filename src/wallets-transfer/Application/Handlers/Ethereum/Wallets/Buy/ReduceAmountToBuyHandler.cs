using Application.Commands.Ethereum.Wallets.Buy;
using Application.Handlers.Ethereum.Base;
using Domain.Interfaces.Wallets;
using MongoDB.Bson;

namespace Application.Handlers.Ethereum.Wallets.Buy;

public class ReduceAmountToBuyHandler : EthereumP2PWalletBaseHandler<ReduceAmountToBuyCommand, decimal>
{
    public ReduceAmountToBuyHandler(IEthereumP2PWalletsRepository<ObjectId> repository) : base(repository)
    {
    }

    public override async Task<decimal> Handle(ReduceAmountToBuyCommand request, CancellationToken cancellationToken)
    {
        var id = ObjectId.Parse(request.WalletId);
        if (!await Repository.ExistsAsync(w => w.Id == id, cancellationToken))
            throw new ArgumentException($"Wallet with id {request.WalletId} is not found");
        var amountToBuy = await Repository.FindOneAsync(w => w.Id == id, wallet => wallet.EthToBuy, cancellationToken);
        if (amountToBuy < request.Amount)
            throw new ArgumentException("The amount to buy exceeds the balance in P2P wallet");
        amountToBuy -= request.Amount;
        await Repository.UpdateAmountToBuyAsync(id, amountToBuy, cancellationToken);
        return amountToBuy;
    }
}