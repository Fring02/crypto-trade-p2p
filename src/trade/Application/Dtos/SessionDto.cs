namespace Application.Dtos;

public record SessionDto
{
    public long Id { get; set; }
    public string StartedAt { get; set; }
    public string ExpiresAt { get; set; }
    public string BuyerId { get; set; }
    public string SellerId { get; set; }
    public double Price { get; set; }
    public string CurrencyType { get; set; }
}