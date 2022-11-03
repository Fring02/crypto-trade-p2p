using Application.Commands.Ethereum.Wallets.Base;
using MediatR;

namespace Application.Commands.Ethereum.Wallets.Sell;

public record IncreaseAmountToSellCommand : P2PAmountBaseCommand, IRequest<decimal>;