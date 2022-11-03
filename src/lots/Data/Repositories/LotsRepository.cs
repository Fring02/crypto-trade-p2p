using AutoMapper;
using Data.Contexts;
using Data.Repositories.Base;
using Domain.Models;
using Shared.Interfaces.Repositories;

namespace Data.Repositories;

public class LotsRepository : CrudRepository<Lot, long>, ILotsRepository
{
    public LotsRepository(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}