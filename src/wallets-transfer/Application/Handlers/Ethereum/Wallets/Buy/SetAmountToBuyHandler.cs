using Application.Commands.Ethereum.Wallets.Buy;
using Application.Handlers.Ethereum.Base;
using Domain.Interfaces.Wallets;
using MongoDB.Bson;

namespace Application.Handlers.Ethereum.Wallets.Buy;

public class SetAmountToBuyHandler : EthereumP2PWalletBaseHandler<SetEthAmountToBuyCommand, decimal>
{
    public SetAmountToBuyHandler(IEthereumP2PWalletsRepository<ObjectId> repository) : base(repository)
    {
    }

    public override async Task<decimal> Handle(SetEthAmountToBuyCommand request, CancellationToken cancellationToken)
    {
        var id = ObjectId.Parse(request.WalletId);
        if (!await Repository.ExistsAsync(w => w.Id == id, cancellationToken))
            throw new ArgumentException($"Wallet with id {request.WalletId} is not found");
        await Repository.UpdateAmountToBuyAsync(id, request.Amount, cancellationToken);
        return request.Amount;
    }
}