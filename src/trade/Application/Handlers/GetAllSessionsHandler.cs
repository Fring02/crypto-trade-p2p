using Application.Dtos;
using Application.Queries;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contexts;
using Data.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Application.Handlers;

public class GetAllSessionsHandler : IRequestHandler<GetAllSessionsQuery, (int, ICollection<SessionDto>)>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    public GetAllSessionsHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<(int, ICollection<SessionDto>)> Handle(GetAllSessionsQuery request, CancellationToken cancellationToken)
    {
        var count = await _context.Sessions.CountAsync(cancellationToken);
        var data = await _context.Sessions.AsNoTracking().Paginate(request.Page, request.PageCount)
            .ProjectTo<SessionDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
        return (count, data);
    }
}