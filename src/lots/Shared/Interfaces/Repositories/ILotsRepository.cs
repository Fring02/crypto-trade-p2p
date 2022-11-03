using Domain.Models;
using Shared.Interfaces.Repositories.Base;

namespace Shared.Interfaces.Repositories;

public interface ILotsRepository : ICrudRepository<Lot, long>
{
}