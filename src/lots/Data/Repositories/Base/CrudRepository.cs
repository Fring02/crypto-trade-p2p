using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contexts;
using Data.Extensions;
using Domain.Models.Base;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Repositories.Base;

namespace Data.Repositories.Base;

public class CrudRepository<TEntity, TId> : ICrudRepository<TEntity, TId> where TEntity : BaseEntity<TId>, new() where TId : struct
{
    protected readonly AppDbContext Context;
    private readonly IMapper _mapper;
    public CrudRepository(AppDbContext context, IMapper mapper)
    {
        Context = context;
        _mapper = mapper;
    }

    public async Task<(int, ICollection<TProjection>)> GetAll<TProjection>(Expression<Func<TEntity, bool>>? filter = null, int page = 0, 
        int pageCount = 0, CancellationToken token = default) where TProjection : class
    {
        var set = Context.Set<TEntity>().AsNoTracking();
        if (filter != null) set = set.Where(filter);
        var data = await set.Paginate(page, pageCount).OrderByDescending(e => e.CreatedAt).ProjectTo<TProjection>(_mapper.ConfigurationProvider).ToListAsync(token);
        return (data.Count, data);
    }

    public async Task<TProjection?> FindOneAndProjectAsync<TProjection>(Expression<Func<TEntity, bool>> filter, CancellationToken token = default)
        where TProjection : class
    {
        return await Context.Set<TEntity>().AsNoTracking().Where(filter).ProjectTo<TProjection>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(token);
    }

    public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default)
    {
        return await Context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(filter, token);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = default) => await Context.Set<TEntity>().AnyAsync(filter, token);

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken token = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, token);
        await Context.SaveChangesAsync(token);
        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken token = default)
    {
        Context.Set<TEntity>().Update(entity);
        await Context.SaveChangesAsync(token);
        return entity;
    }

    public async Task DeleteAsync(TId id, CancellationToken token = default)
    {
        Context.Set<TEntity>().Remove(new TEntity { Id = id });
        await Context.SaveChangesAsync(token);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken token = default)
    {
        Context.Set<TEntity>().Remove(entity);
        await Context.SaveChangesAsync(token);
    }
}