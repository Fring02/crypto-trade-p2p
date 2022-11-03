using Domain.Models.Base;
using Nethereum.KeyStore.Model;

namespace Domain.Models.Wallets;
public record EthereumWallet<TId> : IWallet<TId> {
    public TId Id { get; set; }
    public string Email { get; set; }
    public string Hash { get; set; }
    public KeyStore<ScryptParams> KeyStore { get; set; }
    public DateTime? UnlockDate { get; set; } = null;
}