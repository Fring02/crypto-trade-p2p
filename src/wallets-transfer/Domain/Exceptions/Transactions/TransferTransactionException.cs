namespace Domain.Exceptions.Transactions;

public class TransferTransactionException : Exception
{
    public DateTime ErrorDate { get; set; } = DateTime.Now;
    public TransferTransactionException(string message) : base(message){}
}