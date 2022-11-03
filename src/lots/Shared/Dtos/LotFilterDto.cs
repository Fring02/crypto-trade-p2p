namespace Shared.Dtos;

public record LotFilterDto
{
    public string? LotType { get; set; } = string.Empty;
    public double? MinPrice { get; set; }
    public double? MaxPrice { get; set; }
    public double? MinSupply { get; set; }
    public double? MaxSupply { get; set; }
    public double? MinLimit { get; set; }
    public double? MaxLimit { get; set; }
    public string? FiatType { get; set; } = string.Empty;
    public string? CryptoType { get; set; } = string.Empty;
}