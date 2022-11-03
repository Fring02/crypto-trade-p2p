using Application.Queries.Ethereum;
using FluentValidation;

namespace Wallets.Api.Validators.Buy;

public class GetAmountToBuyQueryValidator : AbstractValidator<GetAmountToBuyQuery>
{
    public GetAmountToBuyQueryValidator()
    {
        RuleFor(c => c.WalletId).NotEmpty().Must(IsParsable).WithMessage("Wallet id is invalid");
    }
}