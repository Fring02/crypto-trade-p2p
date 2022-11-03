using Application.Commands.Ethereum.Transfer;
using FluentValidation;

namespace Api.Transfer.Validators.Ethereum;

public class TransferEtherToP2PWalletValidator : AbstractValidator<TransferEthToP2PWalletCommand>
{
    public TransferEtherToP2PWalletValidator()
    {
        RuleFor(c => c.WalletId).NotEmpty().Must(IsParsable).WithMessage("User wallet id is invalid");
        RuleFor(r => r.Amount).GreaterThan(0).WithMessage("Amount of ether must be greater than 0");
    }
}