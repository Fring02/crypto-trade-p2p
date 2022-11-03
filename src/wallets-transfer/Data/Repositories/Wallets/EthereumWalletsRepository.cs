using Data.Configuration;
using Data.Repositories.Base;
using Domain.Interfaces.Wallets;
using Domain.Models.Wallets;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Data.Repositories.Wallets;

public class EthereumWalletsRepository : WalletsRepositoryBase<EthereumWallet<ObjectId>>, IEthereumWalletsRepository<ObjectId>
{
    public EthereumWalletsRepository(MongoSettings settings, IMongoClient client) : base(settings, client) {}

    protected override string CollectionName => DbSettings.EthereumWalletsCollection;
}