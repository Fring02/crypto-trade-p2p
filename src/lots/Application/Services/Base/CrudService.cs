using System.Linq.Expressions;
using AutoMapper;
using Domain.Models.Base;
using Shared.Interfaces.Dtos;
using Shared.Interfaces.Repositories.Base;
using Shared.Interfaces.Services.Base;
namespace Application.Services.Base;

public abstract class CrudService<TCreateDto, TUpdateDto, TItemDto, TViewDto, TRepository, TEntity, TId> :
    ICrudService<TCreateDto, TUpdateDto, TItemDto, TViewDto, TEntity, TId> where TId : struct 
    where TEntity : BaseEntity<TId>, new() where TRepository : ICrudRepository<TEntity, TId>
    where TItemDto : class where TViewDto : class where TUpdateDto : IDto<TId>
{
    protected readonly TRepository Repository;
    protected readonly IMapper Mapper;
    protected CrudService(TRepository repository, IMapper mapper)
    {
        Repository = repository; Mapper = mapper;
    }

    public Task<(int, ICollection<TItemDto>)> GetAllAsync(int page = 0, int pageCount = 0, CancellationToken token = default) =>
        Repository.GetAll<TItemDto>(page: page, pageCount: pageCount, token: token);

    public virtual Task<TViewDto?> GetByIdAsync(TId id, CancellationToken token = default)
        => Repository.FindOneAndProjectAsync<TViewDto>(e => e.Id.Equals(id), token);

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default)
        => await Repository.ExistsAsync(filter, token);

    public virtual Task<TEntity> CreateAsync(TCreateDto entity, CancellationToken token = default) => 
        Repository.CreateAsync(Mapper.Map<TEntity>(entity), token);

    public abstract Task<TEntity> UpdateAsync(TUpdateDto entity, CancellationToken token = default);

    public virtual async Task DeleteAsync(TId id, CancellationToken token = default)
    { 
        if(!await Repository.ExistsAsync(e => e.Id.Equals(id), token)) throw new ArgumentException($"Entity with id {id} is not found");
        await Repository.DeleteAsync(id, token); 
    }
    public virtual async Task DeleteAsync(TEntity entity, CancellationToken token = default)
    {
        if(!await Repository.ExistsAsync(e => e.Id.Equals(entity.Id), token)) throw new ArgumentException($"Entity with id {entity.Id} is not found");
        await Repository.DeleteAsync(entity, token);
    }
}