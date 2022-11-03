namespace Shared.Dtos;

public record LotItemDto
{
    public long Id { get; set; }
    public string OwnerWallet { get; set; }
    public string LotType { get; set; }
    public double Price { get; set; }
    public double Supply { get; set; }
    public double MinLimit { get; set; }
    public double MaxLimit { get; set; }
    public string FiatType { get; set; }
    public string CryptoType { get; set; }
    public string CreatedAt { get; set; }
}