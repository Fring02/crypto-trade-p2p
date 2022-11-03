using Application.Services.Base;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contexts;
using Data.Extensions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos;
using Shared.Interfaces.Services;

namespace Application.Services;

public class RequisiteService : CrudBaseService<RequisiteDetails, long, RequisiteUpdateDto, RequisiteCreateDto>, IRequisiteService
{
    public RequisiteService(ApplicationContext context, IMapper mapper) : base(context, mapper)
    {
    }
    public override async Task<RequisiteCreateDto> CreateAsync(RequisiteCreateDto entity, CancellationToken token = default)
    {
        if (!await Context.Users.AnyAsync(u => u.Id == entity.UserId, token))
            throw new ArgumentException($"User with id {entity.UserId} is not found");
        return await base.CreateAsync(entity, token);
    }
    public override async Task<bool> ExistsAsync(RequisiteDetails entity, CancellationToken token = default)
        => await Context.RequisiteDetails.AnyAsync(r => r.PhoneNumber == entity.PhoneNumber &&
                                                  r.CreditCardNumber == entity.CreditCardNumber &&
                                                  r.BankName == entity.BankName, token);

    public async Task<(int, IReadOnlyCollection<RequisiteItemDto>)> GetByUserIdAsync(Guid userId, int page = 0, int pageCount = 0,
        CancellationToken token = default)
    {
        int count = await Context.RequisiteDetails.CountAsync(token);
        var data = await Context.RequisiteDetails.AsNoTracking().Where(r => r.UserId == userId).Paginate(page, pageCount)
            .ProjectTo<RequisiteItemDto>(Mapper.ConfigurationProvider).ToListAsync(token);
        return (count, data);
    } 
}