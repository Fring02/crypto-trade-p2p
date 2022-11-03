using Domain.Models;
using Shared.Dtos;
using Shared.Interfaces.Services.Base;

namespace Shared.Interfaces.Services;

public interface IRequisiteService : ICrudService<RequisiteDetails, long, RequisiteUpdateDto, RequisiteCreateDto>
{
    Task<(int, IReadOnlyCollection<RequisiteItemDto>)> GetByUserIdAsync(Guid userId, int page = 0, int pageCount = 0, CancellationToken token = default);
}