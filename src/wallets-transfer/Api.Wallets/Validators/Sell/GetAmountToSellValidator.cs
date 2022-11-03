using Application.Queries.Ethereum;
using FluentValidation;

namespace Wallets.Api.Validators.Sell;

public class GetAmountToSellValidator : AbstractValidator<GetAmountToSellQuery>
{
    public GetAmountToSellValidator()
    {
        RuleFor(c => c.WalletId).NotEmpty().Must(IsParsable).WithMessage("Wallet id is invalid");
    }
}