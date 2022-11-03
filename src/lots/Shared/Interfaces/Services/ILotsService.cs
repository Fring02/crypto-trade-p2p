using Domain.Models;
using Shared.Dtos;
using Shared.Interfaces.Services.Base;

namespace Shared.Interfaces.Services;

public interface ILotsService : ICrudService<LotCreateDto, LotUpdateDto, LotItemDto, LotViewDto, Lot, long>
{
    Task<(int, ICollection<LotItemDto>)> GetAllAsync(LotFilterDto? filter = null, int page = 0, int pageCount = 0, CancellationToken token = default);
    Task<LotViewDto?> GetByIdAsync(long id, string? accessToken = null, CancellationToken token = default);
}