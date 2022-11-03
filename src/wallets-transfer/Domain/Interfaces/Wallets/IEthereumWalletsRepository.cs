using Domain.Interfaces.Base;
using Domain.Models.Wallets;

namespace Domain.Interfaces.Wallets;

public interface IEthereumWalletsRepository<TId> : IWalletsRepository<EthereumWallet<TId>, TId>
{
}