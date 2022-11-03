using Application.Handlers.Ethereum.Base;
using Application.Queries.Ethereum.Wallets.P2P;
using Application.Responses.Ethereum.Wallets;
using Domain.Exceptions.Common;
using Domain.Interfaces.Wallets;
using Domain.Transfer;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Nethereum.KeyStore;

namespace Application.Handlers.Ethereum.Wallets;

public class GetEthereumP2PWalletByEmailHandler : EthereumP2PWalletBaseHandler<GetEthereumP2PWalletByEmailQuery, EthereumP2PWalletWithIdResponse>
{
    private readonly EthereumAccountManager _accountManager;
    private readonly ILogger<GetEthereumP2PWalletByEmailHandler> _logger;
    public GetEthereumP2PWalletByEmailHandler(IEthereumP2PWalletsRepository<ObjectId> repository, EthereumAccountManager accountManager, ILogger<GetEthereumP2PWalletByEmailHandler> logger) 
        : base(repository)
    {
        _accountManager = accountManager;
        _logger = logger;
    }
    public override async Task<EthereumP2PWalletWithIdResponse> Handle(GetEthereumP2PWalletByEmailQuery request, CancellationToken cancellationToken)
    {
        var wallet = await Repository.FindOneAsync(w => w.Email == request.Email, wallet => wallet, cancellationToken);
        if (wallet == null || wallet.Id == ObjectId.Empty)
            throw new EntityNotFoundException($"P2P wallet with email {request.Email} is not found");
        var scryptService = new KeyStoreScryptService();
        var loadedAccount = _accountManager.LoadAccountFromKeyStore(scryptService.SerializeKeyStoreToJson(wallet.KeyStore), wallet.Hash);
        var balanceInEther = await _accountManager.GetAccountBalanceAsync(loadedAccount);
        _logger.LogInformation("Retrieved balance for wallet {request.Id}, User = {request.Email} in ETH: {balance}", wallet.Id, request.Email, balanceInEther);
        return new(wallet.Id.ToString(), loadedAccount.Address, balanceInEther, wallet.UnlockDate != null);
    }
}