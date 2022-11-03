namespace Domain.Models;

public record TransferMessage
{
    public string WalletId { get; set; }
    public string RecipientId { get; set; }
    public decimal Amount { get; set; }
}