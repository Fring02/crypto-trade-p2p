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

public class GetEthereumWalletByEmailHandler : EthereumWalletBaseHandler<GetEthereumWalletByEmailQuery, EthereumWalletWithIdResponse>
{
    private readonly EthereumAccountManager _accountManager;
    private readonly ILogger<GetEthereumWalletByEmailHandler> _logger;
    public GetEthereumWalletByEmailHandler(IEthereumWalletsRepository<ObjectId> repository, EthereumAccountManager accountManager, ILogger<GetEthereumWalletByEmailHandler> logger)
        : base(repository)
    {
        _accountManager = accountManager;
        _logger = logger;
    }
    public override async Task<EthereumWalletWithIdResponse> Handle(GetEthereumWalletByEmailQuery request, CancellationToken cancellationToken)
    {
        var wallet = await Repository.FindOneAsync(w => w.Email == request.Email, wallet => wallet, cancellationToken);
        if (wallet == null || wallet.Id == ObjectId.Empty) 
            throw new EntityNotFoundException($"Wallet with email {request.Email} is not found");
        var scryptService = new KeyStoreScryptService();
        var loadedAccount = _accountManager.LoadAccountFromKeyStore(scryptService.SerializeKeyStoreToJson(wallet.KeyStore), wallet.Hash);
        var balance = await _accountManager.GetAccountBalanceAsync(loadedAccount);
        _logger.LogInformation("Retrieved balance in ETH for wallet {request.Id}, User = {request.Email}", wallet.Id, request.Email);
        return new(wallet.Id.ToString(), balance, wallet.KeyStore.Address, wallet.UnlockDate != null);
    }
}