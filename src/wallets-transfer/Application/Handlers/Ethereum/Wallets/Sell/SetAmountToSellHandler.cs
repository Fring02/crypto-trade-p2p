using Application.Commands.Ethereum.Wallets.Sell;
using Application.Handlers.Ethereum.Base;
using Domain.Interfaces.Wallets;
using MongoDB.Bson;

namespace Application.Handlers.Ethereum.Wallets.Sell;

public class SetAmountToSellHandler : EthereumP2PWalletBaseHandler<SetAmountToSellCommand, decimal>
{
    public SetAmountToSellHandler(IEthereumP2PWalletsRepository<ObjectId> repository) : base(repository)
    {
    }

    public override async Task<decimal> Handle(SetAmountToSellCommand request, CancellationToken cancellationToken)
    {
        var id = ObjectId.Parse(request.WalletId);
        if (!await Repository.ExistsAsync(w => w.Id == id, cancellationToken))
            throw new ArgumentException($"Wallet with id {request.WalletId} is not found");
        await Repository.UpdateAmountToSellAsync(id, request.Amount, cancellationToken);
        return request.Amount;
    }
}