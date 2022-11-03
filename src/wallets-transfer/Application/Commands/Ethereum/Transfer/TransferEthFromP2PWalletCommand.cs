using Domain.Models;
using MediatR;

namespace Application.Commands.Ethereum.Transfer;

public record TransferEthFromP2PWalletCommand(string WalletId, string RecipientId, decimal Amount) : IRequest<TransactionDetails>;