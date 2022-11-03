using Application.Commands.Ethereum.Wallets.Buy;
using FluentValidation;

namespace Wallets.Api.Validators.Buy;

public class SetAmountToBuyValidator : AbstractValidator<SetEthAmountToBuyCommand>
{
    public SetAmountToBuyValidator()
    {
        RuleFor(c => c.WalletId).NotEmpty().Must(IsParsable).WithMessage("Wallet id is invalid");
        RuleFor(c => c.Amount).GreaterThan(0).WithMessage("Amount to buy must be greater than 0");
    }
}