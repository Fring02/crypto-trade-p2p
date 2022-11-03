using Application.Dtos;
using MediatR;

namespace Application.Queries;

public record GetAllSessionsQuery(int Page = 0, int PageCount = 10) : IRequest<(int, ICollection<SessionDto>)>;