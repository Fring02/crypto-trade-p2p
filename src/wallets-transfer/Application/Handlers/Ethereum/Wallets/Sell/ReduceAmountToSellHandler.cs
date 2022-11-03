using Application.Commands.Ethereum.Wallets.Sell;
using Application.Handlers.Ethereum.Base;
using Domain.Interfaces.Wallets;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Application.Handlers.Ethereum.Wallets.Sell;

public class ReduceAmountToSellHandler : EthereumP2PWalletBaseHandler<ReduceAmountToSellCommand, decimal>
{
    private readonly ILogger<ReduceAmountToSellHandler> _logger;
    public ReduceAmountToSellHandler(IEthereumP2PWalletsRepository<ObjectId> repository,
        ILogger<ReduceAmountToSellHandler> logger) : base(repository) => _logger = logger;

    public override async Task<decimal> Handle(ReduceAmountToSellCommand request, CancellationToken cancellationToken)
    {
        var id = ObjectId.Parse(request.WalletId);
        if (!await Repository.ExistsAsync(w => w.Id == id, cancellationToken))
            throw new ArgumentException($"Wallet with id {request.WalletId} is not found");
        var amountToSell = await Repository.FindOneAsync(w => w.Id == id, wallet => wallet.EthToSell, cancellationToken);
        if (amountToSell < request.Amount)
        {
            _logger.LogWarning("The amount to sell {amountToSell} exceeds the balance in P2P wallet {id}", amountToSell, id);
            throw new ArgumentException("The amount to sell exceeds the balance in P2P wallet");
        }
        amountToSell -= request.Amount;
        await Repository.UpdateAmountToSellAsync(id, amountToSell, cancellationToken);
        _logger.LogInformation("Reduce to sell amount to sell of {Id} => {amountToSell}", request.WalletId, amountToSell);
        return amountToSell;
    }
}