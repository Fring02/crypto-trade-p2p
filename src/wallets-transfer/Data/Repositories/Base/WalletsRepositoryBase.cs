using System.Linq.Expressions;
using Data.Configuration;
using Domain.Interfaces.Base;
using Domain.Models.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
namespace Data.Repositories.Base;

public abstract class WalletsRepositoryBase<TWallet> : IWalletsRepository<TWallet, ObjectId> where TWallet : IWallet<ObjectId>
{
    protected readonly IMongoDatabase Database;
    protected readonly MongoSettings DbSettings;
    protected abstract string CollectionName { get; }
    protected IMongoCollection<TWallet> Wallets => Database.GetCollection<TWallet>(CollectionName);
    protected WalletsRepositoryBase(MongoSettings settings, IMongoClient client)
    {
        Database = client.GetDatabase(settings.DatabaseName);
        DbSettings = settings;
        if (Database.GetCollection<TWallet>(CollectionName) == null) Database.CreateCollection(CollectionName);
    }
    public async Task<TWallet> CreateAsync(TWallet wallet, CancellationToken token = default)
    {
        await Wallets.InsertOneAsync(wallet, new InsertOneOptions { BypassDocumentValidation = false }, token);
        return wallet;
    }

    public async Task<TProjection?> FindOneAsync<TProjection>(Expression<Func<TWallet, bool>> expr,
        Expression<Func<TWallet, TProjection>> projection, CancellationToken token = default) =>
        await Wallets.AsQueryable().Where(expr).Select(projection).FirstOrDefaultAsync(token);

    public async Task<bool> ExistsAsync(Expression<Func<TWallet, bool>> expr, CancellationToken token = default)
        => await Wallets.AsQueryable().AnyAsync(expr, token);

    public async Task<TWallet> UpdateAsync(TWallet wallet, CancellationToken token = default)
    {
        return await Wallets.FindOneAndReplaceAsync(Builders<TWallet>.Filter.Eq(w => w.Id, wallet.Id), wallet,
            options: new FindOneAndReplaceOptions<TWallet> { ReturnDocument = ReturnDocument.After },
            cancellationToken:token);
    }

    public async Task<bool> UpdateUnlockDate(ObjectId walletId, DateTime unlockDate, CancellationToken token = default)
    {
        var updateDefinition = Builders<TWallet>.Update.Set(w => w.UnlockDate, unlockDate);
        var result = await Wallets.UpdateOneAsync(w => w.Id == walletId, updateDefinition, cancellationToken: token);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}