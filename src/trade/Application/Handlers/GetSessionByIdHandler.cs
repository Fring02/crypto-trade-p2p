using Application.Queries;
using Application.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers;

public class GetSessionByIdHandler : IRequestHandler<GetSessionByIdQuery<long>, GetSessionByIdResponse?>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    public GetSessionByIdHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GetSessionByIdResponse?> Handle(GetSessionByIdQuery<long> request, CancellationToken cancellationToken) =>
        await _context.Sessions.AsNoTracking().Where(s => s.Id == request.Id)
            .ProjectTo<GetSessionByIdResponse>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
}