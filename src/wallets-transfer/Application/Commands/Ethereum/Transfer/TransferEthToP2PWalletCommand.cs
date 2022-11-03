using Domain.Models;
using MediatR;

namespace Application.Commands.Ethereum.Transfer;

public record TransferEthToP2PWalletCommand(string WalletId, decimal Amount) : IRequest<TransactionDetails>;