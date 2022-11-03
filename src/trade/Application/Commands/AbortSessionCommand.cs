using MediatR;
namespace Application.Commands;

public record AbortSessionCommand(long SessionId) : IRequest;