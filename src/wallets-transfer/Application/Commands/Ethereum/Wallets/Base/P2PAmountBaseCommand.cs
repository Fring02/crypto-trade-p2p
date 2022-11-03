namespace Application.Commands.Ethereum.Wallets.Base;

public record P2PAmountBaseCommand
{
    public string WalletId { get; set; }
    public decimal Amount { get; set; }
}