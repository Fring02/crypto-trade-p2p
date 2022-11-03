using Domain.Enums;
namespace Domain.Models;

public class SessionDetails
{
    public string BuyerEmail { get; set; }
    public string SellerEmail { get; set; }
    public CryptoCurrency CurrencyType { get; set; }
    public double Price { get; set; }
    public string? TransactionHash { get; set; }
    public DateTime? TransactionDate { get; set; }
}