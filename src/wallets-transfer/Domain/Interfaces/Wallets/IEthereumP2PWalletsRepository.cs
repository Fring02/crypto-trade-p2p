using Domain.Interfaces.Base;
using Domain.Models.Wallets;

namespace Domain.Interfaces.Wallets;

public interface IEthereumP2PWalletsRepository<TId> : IWalletsRepository<EthereumP2PWallet<TId>, TId>
{
    Task UpdateAmountToBuyAsync(TId walletId, decimal amount, CancellationToken token = default);
    Task UpdateAmountToSellAsync(TId walletId, decimal amount, CancellationToken token = default);
}