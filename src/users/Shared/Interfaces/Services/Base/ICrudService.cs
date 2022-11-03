using System.Linq.Expressions;
using Domain.Models.Base;
using Shared.Interfaces.Dtos;
namespace Shared.Interfaces.Services.Base;

public interface ICrudService<TEntity, in TId, in TUpdateDto, TCreateDto> : ICrudService<TEntity, TId, TUpdateDto> where TEntity : BaseEntity<TId> 
    where TId : struct where TCreateDto : ICreateDto<TId> where TUpdateDto : IUpdateDto<TId>
{
    Task<TCreateDto> CreateAsync(TCreateDto entity, CancellationToken token = default);
}
public interface ICrudService<TEntity, in TId, in TUpdateDto> where TEntity : BaseEntity<TId> 
    where TId : struct where TUpdateDto : IUpdateDto<TId>
{
    Task UpdateAsync(TUpdateDto entity, CancellationToken token = default);
    Task<TProjection?> GetAsync<TProjection>(Expression<Func<TEntity, bool>> filter, CancellationToken token = default) where TProjection : class;
    Task<(int, IReadOnlyCollection<TProjection>)> GetAllAsync<TProjection>(Expression<Func<TEntity, bool>>? filter = null, int page = 0, int pageCount = 0, CancellationToken token = default) 
        where TProjection : class;
    Task DeleteByIdAsync(TId id, CancellationToken token = default);
}