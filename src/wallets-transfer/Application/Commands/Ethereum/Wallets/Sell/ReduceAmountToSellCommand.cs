using Application.Commands.Ethereum.Wallets.Base;
using MediatR;

namespace Application.Commands.Ethereum.Wallets.Sell;

public record ReduceAmountToSellCommand : P2PAmountBaseCommand, IRequest<decimal>;