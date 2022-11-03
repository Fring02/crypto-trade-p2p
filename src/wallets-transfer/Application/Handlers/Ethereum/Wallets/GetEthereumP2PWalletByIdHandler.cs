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

public class GetEthereumP2PWalletByIdHandler : EthereumP2PWalletBaseHandler<GetEthereumP2PWalletByIdQuery, EthereumP2PWalletResponse>
{
    private readonly EthereumAccountManager _accountManager;
    private readonly ILogger<GetEthereumP2PWalletByIdHandler> _logger;
    public GetEthereumP2PWalletByIdHandler(IEthereumP2PWalletsRepository<ObjectId> repository, 
        EthereumAccountManager accountManager, ILogger<GetEthereumP2PWalletByIdHandler> logger) : base(repository)
    {
        _accountManager = accountManager;
        _logger = logger;
    }

    public override async Task<EthereumP2PWalletResponse> Handle(GetEthereumP2PWalletByIdQuery request, CancellationToken cancellationToken)
    {
        var parsedId = ObjectId.Parse(request.WalletId);
        var wallet = await Repository.FindOneAsync(w => w.Id == parsedId, wallet => wallet, cancellationToken);
        if (wallet == null || wallet.Id == ObjectId.Empty)
            throw new EntityNotFoundException($"P2P wallet with id {request.WalletId} is not found");
        var scryptService = new KeyStoreScryptService();
        var loadedAccount = _accountManager.LoadAccountFromKeyStore(scryptService.SerializeKeyStoreToJson(wallet.KeyStore), wallet.Hash);
        var balanceInEther = await _accountManager.GetAccountBalanceAsync(loadedAccount);
        _logger.LogInformation("Retrieved balance for wallet {request.Id}, User = {request.Email} in ETH: {balance}",
            request.WalletId, wallet.Email, balanceInEther);
        return new(loadedAccount.Address, balanceInEther, wallet.UnlockDate != null);
    }

    
}