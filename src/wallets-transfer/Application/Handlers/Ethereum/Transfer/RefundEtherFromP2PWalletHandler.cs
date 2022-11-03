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

public class RefundEtherFromP2PWalletHandler : EthereumP2PWalletBaseHandler<RefundEthFromP2PWalletCommand, TransactionDetails>
{
    private readonly EthereumAccountManager _accountManager;
    private readonly IEthereumWalletsRepository<ObjectId> _platformWalletsRepository;
    private readonly ILogger<RefundEtherFromP2PWalletHandler> _logger;
    public RefundEtherFromP2PWalletHandler(EthereumAccountManager accountManager, 
        IEthereumP2PWalletsRepository<ObjectId> repository, 
        IEthereumWalletsRepository<ObjectId> platformWalletsRepository, ILogger<RefundEtherFromP2PWalletHandler> logger) : base(repository)
    {
        _accountManager = accountManager;
        _platformWalletsRepository = platformWalletsRepository;
        _logger = logger;
    }
    
    public override async Task<TransactionDetails> Handle(RefundEthFromP2PWalletCommand request, CancellationToken cancellationToken)
    {
        var userId = ObjectId.Parse(request.WalletId);
        var p2pWallet = await Repository.FindOneAsync(w => w.Id == userId, wallet => wallet, cancellationToken);
        if (p2pWallet is null)
        {
            _logger.LogWarning("P2P wallet with id {userId} is not found", userId);
            throw new EntityNotFoundException($"P2P wallet with id {userId} is not found");
        }
        if (p2pWallet.UnlockDate != null)
        {
            _logger.LogWarning("Transfer is forbidden: P2P wallet with id {userId} is locked", userId);
            throw new AccountLockedException(p2pWallet.UnlockDate!.Value);
        }
        var userWallet = await _platformWalletsRepository.FindOneAsync(w => w.Id == userId, wallet => wallet, cancellationToken);
        if (userWallet == null)
        {
            _logger.LogWarning("Main wallet with id {userId} is not found", userId);
            throw new EntityNotFoundException($"Wallet with id {userId} is not found");
        }
        if (userWallet.UnlockDate != null)
        {
            _logger.LogWarning("Transfer is forbidden: main wallet with id {userId} is locked", userId);
            throw new AccountLockedException(userWallet.UnlockDate!.Value);
        }
        var scryptService = new KeyStoreScryptService();
        var account = _accountManager.LoadAccountFromKeyStore(scryptService.SerializeKeyStoreToJson(p2pWallet.KeyStore), p2pWallet.Hash);
        _logger.LogWarning("Loaded account for refund: Address = {address}, Id = {id}", account.Address, userId);
        var receipt = await _accountManager.TransferAsync(userWallet.KeyStore.Address, request.Amount, account, cancellationToken);
        return receipt;
    }

}