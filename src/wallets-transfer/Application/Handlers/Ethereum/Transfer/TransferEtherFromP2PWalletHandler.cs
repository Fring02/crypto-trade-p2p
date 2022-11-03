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

public class TransferEtherFromP2PWalletHandler : EthereumP2PWalletBaseHandler<TransferEthFromP2PWalletCommand, TransactionDetails>
{
    private readonly EthereumAccountManager _accountManager;
    private readonly ILogger<TransferEtherFromP2PWalletHandler> _logger;
    public TransferEtherFromP2PWalletHandler(EthereumAccountManager accountManager, 
        IEthereumP2PWalletsRepository<ObjectId> repository, ILogger<TransferEtherFromP2PWalletHandler> logger) : base(repository)
    {
        _accountManager = accountManager;
        _logger = logger;
    }
    public override async Task<TransactionDetails> Handle(TransferEthFromP2PWalletCommand request, CancellationToken cancellationToken)
    {
        ObjectId sellerWalletId = ObjectId.Parse(request.WalletId), recipientWalletId = ObjectId.Parse(request.RecipientId);
        var sellerWallet = await Repository.FindOneAsync(w => w.Id == sellerWalletId, wallet => wallet, cancellationToken);
        
        if (sellerWallet is null)
        {
            _logger.LogWarning("P2P wallet with id {sellerId} is not found", sellerWalletId);
            throw new EntityNotFoundException($"P2P wallet with id {sellerWalletId} is not found");
        }
        if (sellerWallet.UnlockDate != null)
        {
            _logger.LogWarning("Transfer is forbidden: P2P wallet {sellerId} of seller is locked", sellerWalletId);
            throw new AccountLockedException(sellerWallet.UnlockDate!.Value);
        }
        var recipientAddress = await Repository.FindOneAsync(w => w.Id == recipientWalletId, wallet => wallet.KeyStore.Address, cancellationToken);
        if (string.IsNullOrEmpty(recipientAddress))
        {
            _logger.LogWarning("Recipient address of wallet {id} is not found", recipientWalletId);
            throw new EntityNotFoundException($"Recipient P2P wallet with id {request.RecipientId} is not found");
        }
        var scryptService = new KeyStoreScryptService();
        var account = _accountManager.LoadAccountFromKeyStore(scryptService.SerializeKeyStoreToJson(sellerWallet.KeyStore), sellerWallet.Hash);
        _logger.LogWarning("Loaded account: Address = {address}, Id = {sellerId}, Recipient address = {recipient}", 
            account.Address, sellerWalletId, recipientAddress);
        return await _accountManager.TransferAsync(recipientAddress, request.Amount, account, cancellationToken);
    }
}