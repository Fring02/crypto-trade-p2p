using MediatR;

namespace Application.Queries.Ethereum;

public record GetAmountToSellQuery : IRequest<decimal>{
    public string WalletId { get; set; }
}