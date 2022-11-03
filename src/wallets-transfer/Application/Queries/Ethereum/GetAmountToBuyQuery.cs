using MediatR;

namespace Application.Queries.Ethereum;

public record GetAmountToBuyQuery : IRequest<decimal>
{
    public string WalletId { get; set; }
} 