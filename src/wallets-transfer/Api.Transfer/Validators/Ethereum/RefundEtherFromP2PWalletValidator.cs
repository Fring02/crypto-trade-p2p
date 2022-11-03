using Application.Commands.Ethereum.Transfer;
using FluentValidation;

namespace Api.Transfer.Validators.Ethereum;

public class RefundEtherFromP2PWalletValidator : AbstractValidator<RefundEthFromP2PWalletCommand>
{
    public RefundEtherFromP2PWalletValidator()
    {
        RuleFor(c => c.WalletId).NotEmpty().Must(IsParsable).WithMessage("P2P wallet id is invalid");
        RuleFor(r => r.Amount).GreaterThan(0).WithMessage("Amount of ETH must be greater than 0");
    }
}