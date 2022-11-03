using Application.Commands.Ethereum.Transfer;
using FluentValidation;

namespace Api.Transfer.Validators.Ethereum;

public class TransferEtherFromP2PWalletValidator : AbstractValidator<TransferEthFromP2PWalletCommand>
{
    public TransferEtherFromP2PWalletValidator()
    {
        RuleFor(c => c.WalletId).NotEmpty().Must(IsParsable).WithMessage("P2P wallet id is invalid");
        RuleFor(c => c.RecipientId).NotEmpty().Must(IsParsable).WithMessage("Recipient P2P wallet id is invalid");
        RuleFor(r => r.Amount).GreaterThan(0).WithMessage("Amount of ether must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Amount of ETH must not exceed more than 100 ETH");
    }
}