namespace Domain.Models.Wallets;

public record EthereumP2PWallet<TId> : EthereumWallet<TId>
{
    public decimal EthToBuy { get; set; }
    public decimal EthToSell { get; set; }
}