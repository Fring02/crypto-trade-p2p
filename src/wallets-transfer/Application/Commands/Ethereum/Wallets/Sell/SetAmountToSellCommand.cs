using Application.Commands.Ethereum.Wallets.Base;
using MediatR;

namespace Application.Commands.Ethereum.Wallets.Sell;

public record SetAmountToSellCommand : P2PAmountBaseCommand, IRequest<decimal>;