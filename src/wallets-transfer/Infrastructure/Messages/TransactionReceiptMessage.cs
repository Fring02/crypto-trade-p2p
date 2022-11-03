namespace Infrastructure.Messages;

public record TransactionReceiptMessage
{
    public string Email { get; set; }
    public string TransactionHash { get; set; }
    public DateTime TransactionDate { get; set; }
}