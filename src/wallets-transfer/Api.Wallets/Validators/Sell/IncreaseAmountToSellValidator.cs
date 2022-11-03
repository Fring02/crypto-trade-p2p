using Application.Commands.Ethereum.Wallets.Sell;
using FluentValidation;

namespace Wallets.Api.Validators.Sell;

public class IncreaseAmountToSellValidator : AbstractValidator<IncreaseAmountToSellCommand>
{
    public IncreaseAmountToSellValidator()
    {
        RuleFor(c => c.WalletId).NotEmpty().Must(IsParsable).WithMessage("Wallet id is invalid");
        RuleFor(c => c.Amount).GreaterThan(0).WithMessage("Amount to buy must be greater than 0");
    }
}