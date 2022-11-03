using Application.Responses;
using MediatR;

namespace Application.Queries;

public record GetSessionByIdQuery<TId>(TId Id) : IRequest<GetSessionByIdResponse?> where TId : struct;