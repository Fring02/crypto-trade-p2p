using Application.Commands.Ethereum.Wallets.Base;
using MediatR;

namespace Application.Commands.Ethereum.Wallets.Buy;

public record SetEthAmountToBuyCommand : P2PAmountBaseCommand, IRequest<decimal>;