namespace Application.Responses.Ethereum.Wallets;

public record EthereumWalletResponse(decimal Balance, string Address, bool IsFrozen);
public record EthereumWalletWithIdResponse(string Id, decimal Balance, string Address, bool IsFrozen) : EthereumWalletResponse(Balance, Address, IsFrozen);