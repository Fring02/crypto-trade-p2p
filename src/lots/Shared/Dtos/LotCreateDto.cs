namespace Shared.Dtos;
public record LotCreateDto
{
    public string OwnerEmail { get; set; }
    public string OwnerWallet { get; set; }
    public string LotType { get; set; }
    public double Price { get; set; }
    public double Supply { get; set; }
    public double MinLimit { get; set; }
    public double MaxLimit { get; set; }
    public string FiatType { get; set; }
    public string CryptoType { get; set; }
    public long RequisiteId { get; set; }
}