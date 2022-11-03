using System.Linq.Expressions;
using Domain.Models.Base;
namespace Shared.Interfaces.Repositories.Base;
public interface ICrudRepository<TEntity, in TId> where TId : struct where TEntity : BaseEntity<TId>, new()
{
    Task<(int, ICollection<TProjection>)> GetAll<TProjection>(Expression<Func<TEntity, bool>>? filter = null, 
        int page = 0, int pageCount = 0, CancellationToken token = default) where TProjection : class;
    Task<TProjection?> FindOneAndProjectAsync<TProjection>(Expression<Func<TEntity, bool>> filter, CancellationToken token = default) 
        where TProjection : class;
    Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default);
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken token = default);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken token = default);
    Task DeleteAsync(TId id, CancellationToken token = default);
    Task DeleteAsync(TEntity entity, CancellationToken token = default);
}