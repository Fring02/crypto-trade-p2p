using Domain.Enums;
using Domain.Models.Base;
namespace Domain.Models;

public class Lot : BaseEntity<long>
{
    public string OwnerEmail { get; set; }
    public string OwnerWallet { get; set; }
    public LotType Type { get; set; }
    public double Price { get; set; }
    public double Supply { get; set; }
    public double MinLimit { get; set; }
    public double MaxLimit { get; set; }
    public FiatCurrency FiatType { get; set; }
    public CryptoCurrency CryptoType { get; set; }
    public bool IsActive { get; set; } = true;
    public long RequisiteId { get; set; }
}