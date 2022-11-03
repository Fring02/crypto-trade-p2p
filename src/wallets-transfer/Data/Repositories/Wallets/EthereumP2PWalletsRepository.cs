using Data.Configuration;
using Data.Repositories.Base;
using Domain.Interfaces.Wallets;
using Domain.Models.Wallets;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Data.Repositories.Wallets;

public class EthereumP2PWalletsRepository : WalletsRepositoryBase<EthereumP2PWallet<ObjectId>>, IEthereumP2PWalletsRepository<ObjectId>
{
    public EthereumP2PWalletsRepository(MongoSettings settings, IMongoClient client) : base(settings, client) {}

    public async Task UpdateAmountToBuyAsync(ObjectId walletId, decimal amount, CancellationToken token)
    {
        var updateDefinition = Builders<EthereumP2PWallet<ObjectId>>.Update.Set(w => w.EthToBuy, amount);
        await Wallets.FindOneAndUpdateAsync(Builders<EthereumP2PWallet<ObjectId>>.Filter.Eq(w => w.Id, walletId),
            updateDefinition, cancellationToken: token);
    }

    public async Task UpdateAmountToSellAsync(ObjectId walletId, decimal amount, CancellationToken token)
    {
        var updateDefinition = Builders<EthereumP2PWallet<ObjectId>>.Update.Set(w => w.EthToSell, amount);
        await Wallets.FindOneAndUpdateAsync(Builders<EthereumP2PWallet<ObjectId>>.Filter.Eq(w => w.Id, walletId),
            updateDefinition, cancellationToken: token);
    }

    protected override string CollectionName => DbSettings.EthereumP2PWalletsCollection;
}