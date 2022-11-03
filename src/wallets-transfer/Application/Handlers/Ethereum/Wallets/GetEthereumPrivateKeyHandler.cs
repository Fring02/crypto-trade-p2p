using Application.Handlers.Ethereum.Base;
using Application.Queries.Ethereum;
using Domain.Exceptions.Common;
using Domain.Exceptions.Wallets;
using Domain.Interfaces.Wallets;
using Domain.Transfer;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Nethereum.KeyStore;

namespace Application.Handlers.Ethereum.Wallets;

public class GetEthereumPrivateKeyHandler : EthereumWalletBaseHandler<GetPrivateKeyQuery, string>
{
    private readonly EthereumAccountManager _accountManager;
    private readonly ILogger<GetEthereumPrivateKeyHandler> _logger;
    public GetEthereumPrivateKeyHandler(IEthereumWalletsRepository<ObjectId> repository, 
        EthereumAccountManager accountManager, ILogger<GetEthereumPrivateKeyHandler> logger) : base(repository)
    {
        _accountManager = accountManager;
        _logger = logger;
    }

    public override async Task<string> Handle(GetPrivateKeyQuery request, CancellationToken cancellationToken)
    {
        var id = ObjectId.Parse(request.Id);
        var wallet = await Repository.FindOneAsync(w => w.Email == request.Email, w => w, cancellationToken);
        if (wallet is null) throw new EntityNotFoundException($"Wallet with id {id} is not found");
        if (wallet.UnlockDate is not null) throw new AccountLockedException(wallet.UnlockDate.Value);
        var keyService = new KeyStoreScryptService();
        var account = _accountManager.LoadAccountFromKeyStore(keyService.SerializeKeyStoreToJson(wallet.KeyStore), wallet.Hash);
        _logger.LogInformation("Loaded account from key store: Address = {Address}", account.Address);
        return account.PrivateKey;
    }
}