namespace Domain.Models;

public record WalletResponse
{
    public string Id { get; set; }
    public string Address { get; set; }
    public decimal Balance { get; set; }
    public override string ToString() => $"Id = {Id}, Address = {Address}, Balance = {Balance}";
}