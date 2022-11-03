using System.Linq.Expressions;
using Domain.Models.Base;

namespace Shared.Interfaces.Services.Base;

public interface ICrudService<TCreateDto, TUpdateDto, TItemDto, TViewDto, TEntity, TId> where TEntity : BaseEntity<TId>, new() where TId : struct
{
    Task<(int, ICollection<TItemDto>)> GetAllAsync(int page = 0, int pageCount = 0, CancellationToken token = default);
    Task<TViewDto?> GetByIdAsync(TId id, CancellationToken token = default);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default);
    Task<TEntity> CreateAsync(TCreateDto entity, CancellationToken token = default);
    Task<TEntity> UpdateAsync(TUpdateDto entity, CancellationToken token = default);
    Task DeleteAsync(TId id, CancellationToken token = default);
    Task DeleteAsync(TEntity entity, CancellationToken token = default);
}