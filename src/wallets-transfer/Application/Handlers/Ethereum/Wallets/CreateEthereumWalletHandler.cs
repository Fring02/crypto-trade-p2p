using Application.Commands.Ethereum.Wallets;
using Application.Handlers.Ethereum.Base;
using Application.Responses.Ethereum.Wallets;
using Domain.Interfaces.Wallets;
using Domain.Transfer;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using static BCrypt.Net.BCrypt;

namespace Application.Handlers.Ethereum.Wallets;

public class CreateEthereumWalletHandler : EthereumWalletBaseHandler<CreateEthereumWalletCommand, CreatedEthereumWalletResponse>
{
    private readonly IEthereumP2PWalletsRepository<ObjectId> _p2pWalletsRepository;
    private readonly EthereumAccountManager _accountManager;
    private readonly ILogger<CreateEthereumWalletHandler> _logger;

    public CreateEthereumWalletHandler(IEthereumWalletsRepository<ObjectId> repository, 
        IEthereumP2PWalletsRepository<ObjectId> p2PWalletsRepository, EthereumAccountManager accountManager, 
        ILogger<CreateEthereumWalletHandler> logger) : base(repository)
    {
        _p2pWalletsRepository = p2PWalletsRepository;
        _accountManager = accountManager;
        _logger = logger;
    }
    public override async Task<CreatedEthereumWalletResponse> Handle(CreateEthereumWalletCommand command, CancellationToken token)
    {
        if (await Repository.ExistsAsync(w => w.Email == command.Email, token) || await _p2pWalletsRepository.ExistsAsync(w => w.Email == command.Email, token))
        {
            _logger.LogWarning("Failed attempt to create wallet: wallet with email {command.Email} already exists", command.Email);
            throw new ArgumentException($"Wallet with email {command.Email} already exists");
        }
        var walletId = ObjectId.GenerateNewId(DateTime.Now);
        var passwordHash = HashPassword(command.Password);
        var createdWallets = await Task.WhenAll(CreateMainWallet(command, walletId, passwordHash, token), 
            CreateP2PWallet(walletId, passwordHash, command.Email, token));
        return createdWallets[0];
    }

    private async Task<CreatedEthereumWalletResponse> CreateMainWallet(CreateEthereumWalletCommand request, ObjectId walletId, string hash, CancellationToken token)
    {
        var keyStore = _accountManager.GenerateKeyStore(hash, out string privateKey);
        await Repository.CreateAsync(new() { Email = request.Email, Hash = hash, KeyStore = keyStore, Id = walletId }, token);
        _logger.LogInformation("Created main wallet: User = {request.Email}, wallet's encryption key = {hash}, Id = {walletId}", request.Email, hash, walletId);
        return new(keyStore.Address, privateKey, walletId.ToString());
    }
    private async Task<CreatedEthereumWalletResponse> CreateP2PWallet(ObjectId walletId, string hash, string email, CancellationToken token)
    {
        var keyStore = _accountManager.GenerateKeyStore(hash, out _);
        await _p2pWalletsRepository.CreateAsync(new() { Hash = hash, KeyStore = keyStore, Id = walletId, Email = email }, token);
        _logger.LogInformation("Created P2P wallet: User = {email}, wallet's encryption key = {hash}, Id = {walletId}", email, hash, walletId);
        return null;
    }
}