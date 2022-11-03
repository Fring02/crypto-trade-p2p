using Application.Commands.Ethereum.Wallets;
using Application.Handlers.Ethereum.Base;
using Application.Responses.Ethereum.Wallets;
using Domain.Interfaces.Wallets;
using Domain.Transfer;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Nethereum.Web3.Accounts;
using static BCrypt.Net.BCrypt;
namespace Application.Handlers.Ethereum.Wallets;

public class LoadEthereumWalletHandler : EthereumWalletBaseHandler<LoadEthereumWalletCommand, CreatedEthereumWalletResponse>
{
    private readonly IEthereumP2PWalletsRepository<ObjectId> _p2pWalletsRepository;
    private readonly EthereumAccountManager _accountManager;
    private readonly ILogger<LoadEthereumWalletHandler> _logger;
    
 public LoadEthereumWalletHandler(IEthereumWalletsRepository<ObjectId> repository,
     EthereumAccountManager accountManager, IEthereumP2PWalletsRepository<ObjectId> p2PWalletsRepository,
     ILogger<LoadEthereumWalletHandler> logger) : base(repository)
     {
         _accountManager = accountManager;
         _p2pWalletsRepository = p2PWalletsRepository;
         _logger = logger;
     }

    public override async Task<CreatedEthereumWalletResponse> Handle(LoadEthereumWalletCommand command, CancellationToken token)
    {
        if (await Repository.ExistsAsync(w => w.Email == command.Email, token) || await _p2pWalletsRepository.ExistsAsync(w => w.Email == command.Email, token))
        {
            _logger.LogWarning("Failed attempt to load wallet: wallet with email {command.Email} already exists", command.Email);
            throw new ArgumentException($"Wallet with email {command.Email} already exists");
        }
        var loadedAccount = new Account(command.PrivateKey, _accountManager.ChainId);
        _logger.LogInformation("Loaded account for {command.Email} with private key {loadedAccount.PrivateKey}, address {loadedAccount.Address}",
            command.Email, loadedAccount.PrivateKey, loadedAccount.Address);
        var walletId = ObjectId.GenerateNewId(DateTime.Now);
        var passwordHash = HashPassword(command.Password);
        var createdWallets = await Task.WhenAll(LoadMainWallet(loadedAccount, walletId, command.Email, passwordHash, token),
            CreateP2PWallet(walletId, passwordHash, command.Email, token));
        return createdWallets[0];
    }
    
    private async Task<CreatedEthereumWalletResponse> LoadMainWallet(Account loadedAccount, ObjectId walletId, string email, string hash, CancellationToken token)
    {
        var keyStore = _accountManager.GenerateKeyStoreFromKey(hash, loadedAccount.PrivateKey);
        await Repository.CreateAsync(new() { Email = email, Hash = hash, KeyStore = keyStore, Id = walletId }, token);
        _logger.LogInformation("Created main wallet: User = {email}, wallet's encryption key = {hash}, Id = {walletId}", email, hash, walletId);
        return new(keyStore.Address, loadedAccount.PrivateKey, walletId.ToString());
    }
    private async Task<CreatedEthereumWalletResponse> CreateP2PWallet(ObjectId walletId, string hash, string email, CancellationToken token)
    {
        var keyStore = _accountManager.GenerateKeyStore(hash, out _);
        await _p2pWalletsRepository.CreateAsync(new() { Hash = hash, KeyStore = keyStore, Id = walletId, Email = email }, token);
        _logger.LogInformation("Created P2P wallet: User = {email}, wallet's encryption key = {hash}, Id = {walletId}", email, hash, walletId);
        return null;
    }
}