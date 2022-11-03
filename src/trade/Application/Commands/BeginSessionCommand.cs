using Application.Responses;
using MediatR;

namespace Application.Commands;

public record BeginSessionCommand : IRequest<GetSessionByIdResponse>
{
    public string BuyerEmail { get; set; }
    public long BuyerId { get; set; }
    public string SellerEmail { get; set; }
    public long SellerId { get; set; }
    public string CurrencyType { get; set; }
    public double Price { get; set; }
    public long LotId { get; set; }
}