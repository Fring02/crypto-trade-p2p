using Application.Commands.Ethereum.Wallets;
using Application.Handlers.Ethereum.Base;
using Domain.Exceptions.Common;
using Domain.Interfaces.Wallets;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Application.Handlers.Ethereum.Wallets;

public class LockEthereumWalletHandler : EthereumWalletBaseHandler<LockEthereumWalletCommand, bool>
{
    private readonly IEthereumP2PWalletsRepository<ObjectId> _p2PWalletsRepository;
    private readonly ILogger<LockEthereumWalletHandler> _logger;
    public LockEthereumWalletHandler(IEthereumWalletsRepository<ObjectId> repository, IEthereumP2PWalletsRepository<ObjectId> p2PWalletsRepository, ILogger<LockEthereumWalletHandler> logger) 
        : base(repository)
    {
        _p2PWalletsRepository = p2PWalletsRepository;
        _logger = logger;
    }

    public override async Task<bool> Handle(LockEthereumWalletCommand request, CancellationToken cancellationToken)
    {
        var walletId = ObjectId.Parse(request.WalletId);
        if (!await Repository.ExistsAsync(s => s.Id == walletId, cancellationToken))
            throw new EntityNotFoundException($"Wallet with id {request.WalletId} is not found");
        var unlockDate = DateTime.Now.AddHours(6) + TimeSpan.FromDays(30);
        _logger.LogInformation("Wallet lock: expected unlock date = {unlockDate}", unlockDate);
        
        var result = (await Task.WhenAll(Repository.UpdateUnlockDate(walletId, unlockDate, cancellationToken),
            _p2PWalletsRepository.UpdateUnlockDate(walletId, unlockDate, cancellationToken))).All(r => r);
        if (!result)
            throw new Exception("Error occured: failed to lock wallets");
        _logger.LogInformation("Wallets {walletId} are successfully locked till {unlockDate}", request.WalletId, unlockDate.ToString("f"));
        return result;
    }
}