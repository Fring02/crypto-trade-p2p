using Application.Handlers.Ethereum.Base;
using Application.Queries.Ethereum.Wallets;
using Application.Responses.Ethereum.Wallets;
using Domain.Exceptions.Common;
using Domain.Interfaces.Wallets;
using Domain.Transfer;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Nethereum.KeyStore;

namespace Application.Handlers.Ethereum.Wallets;

public class GetEthereumWalletByIdHandler : EthereumWalletBaseHandler<GetEthereumWalletByIdQuery, EthereumWalletResponse>
{
    private readonly EthereumAccountManager _accountManager;
    private readonly ILogger<GetEthereumWalletByIdHandler> _logger;
    public GetEthereumWalletByIdHandler(IEthereumWalletsRepository<ObjectId> repository, EthereumAccountManager accountManager, ILogger<GetEthereumWalletByIdHandler> logger)
        : base(repository)
    {
        _accountManager = accountManager;
        _logger = logger;
    }

    public override async Task<EthereumWalletResponse> Handle(GetEthereumWalletByIdQuery request, CancellationToken token)
    {
        var parsedId = ObjectId.Parse(request.Id);
        var wallet = await Repository.FindOneAsync(w => w.Id == parsedId, wallet => wallet, token);
        if (wallet == null || wallet.Id == ObjectId.Empty)
            throw new EntityNotFoundException($"Wallet with id {request.Id} is not found");
        _logger.LogInformation("Wallet with id {request.Id}, User = {wallet.Email}", request.Id, wallet.Email);
        var scryptService = new KeyStoreScryptService();
        var loadedAccount = _accountManager.LoadAccountFromKeyStore(scryptService.SerializeKeyStoreToJson(wallet.KeyStore), wallet.Hash);
        var balanceInEther = await _accountManager.GetAccountBalanceAsync(loadedAccount);
        _logger.LogInformation("Retrieved balance for wallet {request.Id} in ETH: {wallet.Balance}", request.Id, balanceInEther);
        return new EthereumWalletResponse(balanceInEther, loadedAccount.Address, wallet.UnlockDate != null);
    }

    
}