using Application.Commands.Ethereum.Transfer;
using Application.Handlers.Ethereum.Base;
using Domain.Exceptions.Common;
using Domain.Exceptions.Wallets;
using Domain.Interfaces.Wallets;
using Domain.Models;
using Domain.Transfer;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Nethereum.KeyStore;

namespace Application.Handlers.Ethereum.Transfer;

public class TransferEtherToP2PWalletHandler : EthereumWalletBaseHandler<TransferEthToP2PWalletCommand, TransactionDetails>
{
    private readonly EthereumAccountManager _accountManager;
    private readonly IEthereumP2PWalletsRepository<ObjectId> _p2pWalletsRepository;
    private readonly ILogger<TransferEtherToP2PWalletHandler> _logger;
    public TransferEtherToP2PWalletHandler(EthereumAccountManager accountManager, 
        IEthereumWalletsRepository<ObjectId> repository, IEthereumP2PWalletsRepository<ObjectId> p2pWalletsRepository,
        ILogger<TransferEtherToP2PWalletHandler> logger) : base(repository)
    {
        _accountManager = accountManager;
        _p2pWalletsRepository = p2pWalletsRepository;
        _logger = logger;
    }
    public override async Task<TransactionDetails> Handle(TransferEthToP2PWalletCommand request, CancellationToken cancellationToken)
    {
        var walletId = ObjectId.Parse(request.WalletId);
        var userWallet = await Repository.FindOneAsync(w => w.Id == walletId, wallet => wallet, cancellationToken);
        if (userWallet is null)
        {
            _logger.LogWarning("Wallet not found by id {id}", walletId);
            throw new EntityNotFoundException($"Wallet with id {walletId} is not found");
        }
        if (userWallet.UnlockDate != null)
        {
            _logger.LogWarning("Transfer is forbidden: wallet {walletId} for user {email} is locked", walletId, userWallet.Email);
            throw new AccountLockedException(userWallet.UnlockDate!.Value);
        }
        var p2pWallet = await _p2pWalletsRepository.FindOneAsync(w => w.Id == walletId, wallet => wallet, cancellationToken);
        if (p2pWallet is null) throw new EntityNotFoundException($"P2P wallet with id {walletId} is not found");
        var scryptService = new KeyStoreScryptService();
        var account = _accountManager.LoadAccountFromKeyStore(scryptService.SerializeKeyStoreToJson(userWallet.KeyStore), userWallet.Hash);
        _logger.LogInformation("Loaded account for transfer: Address = {address}, amount to transfer = {} ETH", account.Address, request.Amount);
        return await _accountManager.TransferAsync(p2pWallet.KeyStore.Address, request.Amount, account, cancellationToken);
    }

}