namespace Application.Responses;

public record GetSessionByIdResponse
{
    public long Id { get; set; }
    public string StartedAt { get; set; }
    public string ExpiresAt { get; set; }
    public string BuyerEmail { get; set; }
    public string SellerEmail { get; set; }
    public double Price { get; set; }
    public string CurrencyType { get; set; }
}