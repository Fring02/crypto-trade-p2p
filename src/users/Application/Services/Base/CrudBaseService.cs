using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contexts;
using Data.Extensions;
using Domain.Models.Base;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Dtos;
using Shared.Interfaces.Services.Base;
namespace Application.Services.Base;

public abstract class CrudBaseService<TEntity, TId, TUpdateDto, TCreateDto> : CrudBaseService<TEntity, TId, TUpdateDto>, 
    ICrudService<TEntity,TId, TUpdateDto, TCreateDto> 
    where TId : struct where TEntity : BaseEntity<TId> where TCreateDto : ICreateDto<TId> where TUpdateDto : IUpdateDto<TId>
{
    protected readonly ApplicationContext Context;
    protected readonly IMapper Mapper;
    protected CrudBaseService(ApplicationContext context, IMapper mapper) : base(context, mapper)
    {
        Context = context; Mapper = mapper;
    }

    public virtual async Task<TCreateDto> CreateAsync(TCreateDto entity, CancellationToken token = default)
    {
        var model = Mapper.Map<TEntity>(entity);
        if (await ExistsAsync(model, token)) throw new ArgumentException("Such entity already exists");
        await Context.Set<TEntity>().AddAsync(model, token);
        await Context.SaveAsync(token);
        entity.Id = model.Id;
        return entity;
    }
}

public abstract class CrudBaseService<TEntity, TId, TUpdateDto> : ICrudService<TEntity,TId, TUpdateDto> 
    where TId : struct where TEntity : BaseEntity<TId> where TUpdateDto : IUpdateDto<TId>
{
    protected readonly ApplicationContext Context;
    protected readonly IMapper Mapper;
    protected CrudBaseService(ApplicationContext context, IMapper mapper)
    {
        Context = context; Mapper = mapper;
    }

    public virtual async Task UpdateAsync(TUpdateDto entity, CancellationToken token = default)
    {
        var model = await Context.Set<TEntity>().FindAsync(new object?[] { entity.Id }, cancellationToken: token);
        if (model == null) throw new ArgumentException($"Entity with id {entity.Id} is not found");
        var entry = Context.Entry(model);
        var modelType = typeof(TEntity);
        foreach (var property in typeof(TUpdateDto).GetProperties())
        {
            var value = property.GetValue(entity);
            if(value != null && modelType.GetProperty(property.Name) != null) entry.Property(property.Name).CurrentValue = value;
        }
        Context.Set<TEntity>().Update(model);
        await Context.SaveChangesAsync(token);
    }

    public async Task<TProjection?> GetAsync<TProjection>(Expression<Func<TEntity, bool>> filter, CancellationToken token = default) where TProjection : class => 
        await Context.Set<TEntity>().AsNoTracking().Where(filter).ProjectTo<TProjection>(Mapper.ConfigurationProvider).FirstOrDefaultAsync(token);

    public async Task<(int, IReadOnlyCollection<TProjection>)> GetAllAsync<TProjection>(Expression<Func<TEntity, bool>>? filter, int page = 0, int pageCount = 0, CancellationToken token = default)
        where TProjection : class
    {
        var result = Context.Set<TEntity>().AsNoTracking();
        if (filter != null) result = result.Where(filter);
        var results = await result.Paginate(page, pageCount).ProjectTo<TProjection>(Mapper.ConfigurationProvider).ToListAsync(token);
        return (await Context.Set<TEntity>().CountAsync(token), results);
    }

    public async Task DeleteByIdAsync(TId id, CancellationToken token = default)
    {
        var entityToDelete = await Context.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken: token);
        if (entityToDelete is null) throw new ArgumentException("Entity with id " + id + " is not found");
        Context.Set<TEntity>().Remove(entityToDelete);
        await Context.SaveAsync(token);
    }
    public abstract Task<bool> ExistsAsync(TEntity entity, CancellationToken token = default);
}